namespace Waracle_HotelAPI.ReturnModels
{
    public class HotelDetails
    {
        public int HotelID { get; set; }
        public string Name { get; set; }
        public int TotalRooms { get; set; }
        public HotelDetails(int hotelId, string name, int totalRooms)
        {
            HotelID = hotelId; Name =name; TotalRooms=totalRooms;
        }
    }
}
