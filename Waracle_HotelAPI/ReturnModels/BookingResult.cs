using System.Net;
using Waracle_HotelAPI.Models;

namespace Waracle_HotelAPI.ReturnModels
{


    public class BookingResult
    {
        public string HotelName { get; set; } = "";
        public List<RoomBookingDetails> RoomDetails { get; set; } = new();
        public string BookingReference { get; set; } = "";
        public string Message { get; set; } = "";
        public bool Successfull { get; set; } =false;
    }

    public class RoomBookingDetails
    {
        public int RoomID { get; set; }
        public string RoomType { get; set; } = "";
        public RoomBookingDetails(int roomID,RoomType roomType)
        {
            RoomID = roomID; RoomType = roomType.ToString();
        }
    }
}
