using Microsoft.EntityFrameworkCore;
using Waracle_HotelAPI.Interfaces;
using Waracle_HotelAPI.Models;

namespace Waracle_HotelAPI.Services
{
    public class SeedService : ISeedService
    {
        private readonly BookingDBContext context;
        private readonly ILogger<SeedService> logger;

        public SeedService(BookingDBContext context, ILogger<SeedService> logger)
        {
            this.context = context;
            this.logger = logger;
        }

        public async Task<bool> ClearDatabase()
        {
            try
            {
                logger.LogWarning("Clearing Database Started");
                await context.Database.EnsureDeletedAsync();
                await context.Database.EnsureCreatedAsync();
                logger.LogWarning("Clearing Database Complete");
                return true;
            }
            catch (Exception ex)
            {
                logger.LogError($"Clear Database Failed - {ex.Message} : {ex.StackTrace}");
                return false;
            }
            return true;
        }

        public async Task<bool> SeedDatabase()
        {
            if (await context.Hotels.AnyAsync())
            {
                logger.LogWarning("Attempted to Seed Database, but Database was not empty");
                return false;
            }

            try
            {
                string[] HotelNames = File.ReadAllLines("HotelNames.txt");//we are assuming this is in the base directory
                foreach (var item in HotelNames) { context.Hotels.Add(new Hotel(item, 2, 2, 2)); }

                await context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                logger.LogError($"Seed Database Failed - {ex.Message} : {ex.StackTrace}");
                return false;
            }

        }
    }
}
