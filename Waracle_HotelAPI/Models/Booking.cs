using System.ComponentModel.DataAnnotations;

namespace Waracle_HotelAPI.Models
{
    public class Booking
    {
        [Key]
        public int Id { get; set; }
        public string Reference { get; set; } = "ToBeAssigned";

        public DateTime BookingDate { get; set; } = DateTime.UtcNow;
        public DateOnly ArrivalDate { get; set; }
        public DateOnly DepartureDate { get; set; }
        public ICollection<Room> Rooms { get; set; } = new List<Room>();
    }
}
