using System.ComponentModel.DataAnnotations;

namespace Waracle_HotelAPI.Models
{
    public class Hotel
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public ICollection<Room> Rooms { get; set; } = new List<Room>();
    }
}
