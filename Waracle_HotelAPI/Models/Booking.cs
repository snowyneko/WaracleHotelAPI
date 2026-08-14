using System.ComponentModel.DataAnnotations;

namespace Waracle_HotelAPI.Models
{
    public class Booking
    {
        [Key]
        public int Id { get; set; } 
        public string Reference { get; set; }
        public int RoomId { get; set; }
        public DateTime BookingDate { get; set; }
        public DateTime ArrivalDate { get; set; }
        public DateTime DepartureDate { get; set; }
    }
}
