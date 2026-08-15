using System.Text;

namespace Waracle_HotelAPI.ReturnModels
{
    public class BookingDetails
    {
        public string BookingReference { get; set; } = "";
        public string Message { get; set; } = "";
        public string HotelName { get; set; } = "";
        public List<string> RoomTypes { get; set; } = new();
        public DateOnly ArrivalDate { get; set; } = DateOnly.MinValue;
        public DateOnly DepartureDate { get; set; } = DateOnly.MinValue;
    }
}
