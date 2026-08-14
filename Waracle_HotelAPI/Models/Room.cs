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

        public Room()
        {
            
        }
        public Room(RoomType type)
        {

            RoomType=type;
            Capacity = CapacityHelper(type);
        }

        static int CapacityHelper(RoomType type) => type switch { RoomType.Single => 1, RoomType.Double => 2, RoomType.Deluxe => 3, _ => 0 };
             

    }
}
