using System.ComponentModel.DataAnnotations;

namespace Waracle_HotelAPI.Models
{
    public enum RoomType { Single, Double, Deluxe }
    public class Room
    {
        public int Id { get; set; }
        public int HotelId { get; set; }
        public RoomType RoomType { get; set; }
        public int Capacity { get; set; }
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    }
}
