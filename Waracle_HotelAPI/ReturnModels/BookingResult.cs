using System.Net;

namespace Waracle_HotelAPI.ReturnModels
{
    public class BookingResult
    {
        
    }

    public class BookingDetails
    {
        public string HotelName { get; set; } = "";
        public List<RoomBookingDetails> RoomDetails { get; set; } = new();
    }

    public class RoomBookingDetails
    {
        public int RoomID { get; set; }
        public string RoomType { get; set; } = "";
        public string BookingReference { get; set; } = "";
    }
}
