using Waracle_HotelAPI.RequestModels;
using Waracle_HotelAPI.ReturnModels;

namespace Waracle_HotelAPI.Interfaces
{
    public interface IBookingService
    {
        Task<BookingOptions> CheckForAvailableBookings(BookingEnquiry request);
        Task<BookingResult> CreateBooking(BookingRequest request);
        Task<BookingDetails> FindBooking(string Reference);
    }
}