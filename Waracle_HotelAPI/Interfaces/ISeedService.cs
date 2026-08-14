namespace Waracle_HotelAPI.Interfaces
{
    public interface ISeedService
    {
        Task<bool> ClearDatabase();
        Task<bool> SeedDatabase();
    }
}