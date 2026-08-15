using Waracle_HotelAPI.ReturnModels;

namespace Waracle_HotelAPI.Interfaces
{
    public interface IHotelService
    {
        Task<HotelList> GetAllHotelInfo();
        Task<HotelList> GetHotelInfo(string HotelName);
    }
}