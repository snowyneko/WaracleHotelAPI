using Waracle_HotelAPI.RequestModels;
using Waracle_HotelAPI.ReturnModels;

namespace Waracle_HotelAPI.Services
{
    public interface IBookingService
    {
        Task<List<BookingSet>?> CheckForAvailableBookings(BookingEnquiry request);
    }
}