using System.ComponentModel.DataAnnotations;

namespace Waracle_HotelAPI.Models
{
    public class Hotel
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public ICollection<Room> Rooms { get; set; } = new List<Room>();

        public Hotel()
        {
            
        }
        public Hotel(string name,int single,int dbl,int deluxe)
        {
            Name = name;
            for (int i = 0; i < single; i++) { Rooms.Add(new Room(RoomType.Single)); }
            for (int i = 0; i < dbl; i++) { Rooms.Add(new Room(RoomType.Double)); }
            for (int i = 0; i < deluxe; i++) { Rooms.Add(new Room(RoomType.Deluxe)); }

        }
    }
}
