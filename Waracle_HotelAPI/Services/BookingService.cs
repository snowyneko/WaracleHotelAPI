using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using System.Net;
using Waracle_HotelAPI.Interfaces;
using Waracle_HotelAPI.Models;
using Waracle_HotelAPI.RequestModels;
using Waracle_HotelAPI.ReturnModels;

namespace Waracle_HotelAPI.Services
{
    public class BookingService : IBookingService
    {
        private readonly BookingDBContext context;
        private readonly ILogger<HotelService> logger;
        private readonly IHotelService hotelService;

        public BookingService(BookingDBContext context, ILogger<HotelService> logger, IHotelService hotelService)
        {
            this.context = context;
            this.logger = logger;
            this.hotelService = hotelService;
        }

        public async Task<BookingDetails> FindBooking(string Reference)
        {
            BookingDetails bookingDetails = new BookingDetails();
            Booking? booking = await context.Bookings.AsNoTracking().Where(x => x.Reference == Reference).Include(b => b.Rooms).FirstOrDefaultAsync();
            if (booking is null)
            {
                bookingDetails.Message = "Booking with Reference not found";
                bookingDetails.Response = ResponseType.NotFound;
                return bookingDetails;
            }
            if (booking.Rooms.Count == 0)
            {
                logger.LogError($"Booking {Reference} Contains no Rooms, Those Rooms may have been deleted.");
                bookingDetails.Message = "There is an issue with this Booking, please contact our helpdesk";
                bookingDetails.Response = ResponseType.Error;
                return bookingDetails;
            }
            Hotel? hotel = await context.Hotels.AsNoTracking().Where(h => h.Id == booking.Rooms.First().HotelId).FirstOrDefaultAsync();
            if (hotel is null)
            {
                logger.LogError($"Booking {Reference} Does not Reference an available Hotel, it may have been deleted.");
                bookingDetails.Message = "There is an issue with this Booking, please contact our helpdesk";
                bookingDetails.Response = ResponseType.Error;
                return bookingDetails;
            }
            bookingDetails.HotelName = hotel.Name;
            bookingDetails.Message = "Booking Details Retreived.";
            bookingDetails.BookingReference = booking.Reference;
            bookingDetails.ArrivalDate = booking.ArrivalDate;
            bookingDetails.DepartureDate = booking.DepartureDate;
            foreach (Room room in booking.Rooms) { bookingDetails.RoomTypes.Add(room.RoomType.ToString()); }
            return bookingDetails;


        }

        public async Task<BookingOptions> CheckForAvailableBookings(BookingEnquiry request)
        {

            BookingOptions bookingOptions = new BookingOptions();
            // 1. Fetch hotel and free rooms from DB
            var query = context.Hotels.AsNoTracking();

            if (string.IsNullOrEmpty(request.HotelName)) { query = query.Where(x => x.Id == request.HotelID); }
            else { query = query.Where(x => x.Name == request.HotelName); }

            Hotel? hotel = await query
                .Include(x => x.Rooms)
                    .ThenInclude(r => r.Bookings.Where(b => b.ArrivalDate < request.Departure && b.DepartureDate > request.Arrival))
                .FirstOrDefaultAsync();

            if (hotel is null)
            {
                logger.LogError($"Unknown Hotel Searched for {request.HotelName} | {request.HotelID}");
                bookingOptions.Message = "Hotel not Found";
                bookingOptions.Response = ResponseType.NotFound;
                return bookingOptions;
            }

            List<Room> freeRooms = hotel.Rooms
                .Where(x => x.Bookings.Count == 0)
                .OrderBy(x => x.Capacity)
                .ToList();

            // 2. Delegate algorithm to pure helper method
            bookingOptions.Options = FindRoomCombinations(freeRooms, request.NoOfPeople);
            return bookingOptions;
        }

        public async Task<BookingResult> CreateBooking(BookingRequest request)
        {
            using var transaction = await context.Database.BeginTransactionAsync();
            var query = context.Hotels.AsNoTracking();
            BookingResult bookingResult = new BookingResult();
            //Lets fine the hotel as we need the ID and they might have supplied the hotel name
            if (string.IsNullOrEmpty(request.HotelName)) { query = query.Where(x => x.Id == request.HotelID); }
            else { query = query.Where(x => x.Name == request.HotelName); }
            Hotel? hotel = await query.FirstOrDefaultAsync();
            if (hotel is null)
            {
                logger.LogWarning($"The user Attempted to book a room at an unknown hotel {request.HotelName} : {request.HotelID}");
                bookingResult.Message = "Requested Hotel not found";
                bookingResult.Response = ResponseType.NotFound;
                return bookingResult;
            }
            bookingResult.HotelName = hotel.Name;

            //We must apply an update lock against the hotel rooms to stop any other query reading those rows. This is critical because we arent actually
            //changing any of these rows to create a booking. as a result since the first step of creating a booking is reading the room list, we should
            //prevent another instance from accessing them until we are done.
            List<Room> rooms = await context.Rooms.FromSqlInterpolated($"SELECT * FROM Rooms WITH (UPDLOCK, HOLDLOCK) WHERE HotelId = {hotel.Id}")
            .Include(r => r.Bookings.Where(b => b.ArrivalDate < request.Departure && b.DepartureDate > request.Arrival))
            .ToListAsync();
            //Identify only rooms with no bookings across the booking period
            List<Room> freeRooms = rooms
              .Where(x => x.Bookings.Count == 0)
              .OrderBy(x => x.Capacity)
              .ToList();
            //Lets see if we can find rooms to match our requirements
            bool[] Assigned = new bool[freeRooms.Count()];
            List<Room> RoomsToBook = new();
            foreach (string type in request.RequestedRooms)
            {
                for (int i = 0; i < freeRooms.Count; i++)
                {
                    if (TypeHelper(type) == freeRooms[i].RoomType && Assigned[i] is false)
                    {
                        RoomsToBook.Add(freeRooms[i]);
                        Assigned[i] = true;//so we cant use the same room type twice
                        break;
                    }
                }
            }
            if (RoomsToBook.Count() != request.RequestedRooms.Count())
            {
                logger.LogWarning($"User attempted to book unavailable rooms at {hotel.Name}");
                bookingResult.Message = "Requested Rooms are not Available";
                bookingResult.Response = ResponseType.Conflict;
                return bookingResult;
            }
            //Ok Create the new booking
            Booking booking = new Booking() { ArrivalDate = request.Arrival, DepartureDate = request.Departure };

            foreach (Room room in RoomsToBook)
            {
                room.Bookings.Add(booking);
                bookingResult.RoomDetails.Add(new RoomBookingDetails(room.Id, room.RoomType));
            }

            await context.SaveChangesAsync();
            booking.Reference = $"{Guid.NewGuid().ToString().Substring(0, 4)}-{10000 + booking.Id}";
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            bookingResult.BookingReference = booking.Reference;
            bookingResult.Message = "Rooms Succesfully Booked";

            return bookingResult;
        }

        private RoomType TypeHelper(string roomType)
        {
            if (Enum.TryParse<RoomType>(roomType, ignoreCase: true, out var enumType))
            {
                if (Enum.IsDefined(enumType)) return enumType;
            }
            throw new InvalidOperationException($"Unknown Room Type Requested");
        }





        //This is actually not a trivial problem to solve without heavy brute forcing every combination. im sure there are better solutions than this
        //and if i spent more time im sure id come up with one
        private List<BookingSet> FindRoomCombinations(List<Room> freeRooms, int targetPeople)
        {
            List<BookingSet> sets = new();

            for (int i = 0; i < freeRooms.Count; i++)
            {
                int capacity = freeRooms[i].Capacity;

                // Single room option
                if (capacity >= targetPeople)
                {
                    BookingSet singleSet = new();
                    singleSet.RoomSet.Add(freeRooms[i].RoomType.ToString());
                    sets.Add(singleSet);
                    break;
                }

                for (int offset = 1; offset < freeRooms.Count - i; offset++)
                {
                    BookingSet set = new();
                    set.RoomSet.Add(freeRooms[i].RoomType.ToString());
                    capacity = freeRooms[i].Capacity;

                    for (int j = i + offset; j < freeRooms.Count; j++)
                    {
                        capacity += freeRooms[j].Capacity;
                        set.RoomSet.Add(freeRooms[j].RoomType.ToString());

                        if (capacity >= targetPeople)
                        {
                            //We are checking if the most recent room can handle everyone by itself because otherwise.
                            //for like 2 people we suggest a single and a double which is a little silly.
                            if (freeRooms[j].Capacity < targetPeople) sets.Add(set);
                            break;
                        }
                    }
                }
            }

            // Deduplicate combinations before returning
            return sets
                .DistinctBy(s => string.Join(",", s.RoomSet.OrderBy(x => x)))
                .ToList();
        }






    }
}
