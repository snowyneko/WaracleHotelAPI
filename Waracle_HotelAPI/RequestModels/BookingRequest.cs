using Waracle_HotelAPI.Models;

namespace Waracle_HotelAPI.RequestModels
{
    public class BookingRequest
    {
        public int HotelID { get; set; }
        public string HotelName { get; set; }
        public string[] RequestedRooms { get; set; }
        public DateOnly Arrival { get; set; }
        public DateOnly Departure { get; set; }
    }
}
