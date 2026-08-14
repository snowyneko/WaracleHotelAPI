using Waracle_HotelAPI.ReturnModels;

namespace Waracle_HotelAPI.Interfaces
{
    public interface IHotelService
    {
        Task<List<HotelDetails>> GetAllHotelInfo();
        Task<List<HotelDetails>> GetHotelInfo(string HotelName);
    }
}