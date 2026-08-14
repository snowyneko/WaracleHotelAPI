using Microsoft.EntityFrameworkCore;
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

        public async Task<List<BookingSet>?> CheckForAvailableBookings(BookingEnquiry request)
        {
            // 1. Fetch hotel and free rooms from DB
            var query = context.Hotels.AsNoTracking();

            if (string.IsNullOrEmpty(request.HotelName))
            {
                query = query.Where(x => x.Id == request.HotelID);
            }
            else
            {
                query = query.Where(x => x.Name == request.HotelName);
            }

            Hotel? hotel = await query
                .Include(x => x.Rooms)
                    .ThenInclude(r => r.Bookings.Where(b => b.ArrivalDate < request.Departure && b.DepartureDate > request.Arrival))
                .FirstOrDefaultAsync();

            if (hotel is null)
            {
                return null;
            }

            List<Room> freeRooms = hotel.Rooms
                .Where(x => x.Bookings.Count == 0)
                .OrderBy(x => x.Capacity)
                .ToList();

            // 2. Delegate algorithm to pure helper method
            return FindRoomCombinations(freeRooms, request.NoOfPeople);
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
                           if(freeRooms[j].Capacity< targetPeople) sets.Add(set);
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
