using Waracle_HotelAPI.Models;

namespace Waracle_HotelAPI.ReturnModels
{
    public class BookingOptions
    {
        public ResponseType Response { get; set; } = ResponseType.OK;
        public string Message { get; set; } = "";
        public List<BookingSet> Options { get; set; } = new();

    }
    public class BookingSet
    {
        public List<string> RoomSet { get; set; } = new();
        internal int TotalCapacity { get; set; } = 0;

    }
}
