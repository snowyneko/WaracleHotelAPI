using Waracle_HotelAPI.Models;

namespace Waracle_HotelAPI.RequestModels
{
    public class BookingEnquiry
    {
        public int HotelID { get; set; } = -1;
        public string HotelName { get; set; } = "";
        public int NoOfPeople { get; set; }
        public DateOnly Arrival { get; set; }
        public DateOnly Departure { get; set; }
    }
}
