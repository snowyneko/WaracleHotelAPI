using Microsoft.EntityFrameworkCore;
using Waracle_HotelAPI.Interfaces;
using Waracle_HotelAPI.Models;
using Waracle_HotelAPI.ReturnModels;

namespace Waracle_HotelAPI.Services
{
    public class HotelService : IHotelService
    {
        private readonly BookingDBContext context;
        private readonly ILogger<HotelService> logger;

        public HotelService(BookingDBContext context, ILogger<HotelService> logger)
        {
            this.context = context;
            this.logger = logger;
        }
        public async Task<HotelList> GetHotelInfo(string HotelName)
        {
            HotelList results = new();
            try
            {
                var Hotels = await context.Hotels.Select(x => new HotelDetails(x.Id, x.Name, x.Rooms.Count)).ToListAsync();
                //We dont have a massive list of hotels, there are ways to make this more efficent later if needed
                //Namely by Caching the hotel list somewhere

                foreach (HotelDetails hotel in Hotels)
                {
                    int AllowedDistance = 1 + (HotelName.Length / 4);
                    if (LevenshteinDistance(hotel.Name, HotelName) <= AllowedDistance) results.HotelDetails.Add(hotel);

                }
                return results;
            }
            catch (Exception ex)
            {
                logger.LogError($"Unable to get Hotel Details for search {HotelName} - {ex.Message} : {ex.StackTrace}");
                results.Message = "Unable to get Hotel Details";
                results.Response = ResponseType.Error;
                return results;
            }

        }

        public async Task<HotelList> GetAllHotelInfo()
        {
            HotelList results = new();
            try
            {
                results.HotelDetails = await context.Hotels.Select(x => new HotelDetails(x.Id, x.Name, x.Rooms.Count)).ToListAsync();
                return results;
            }
            catch (Exception ex)
            {
                logger.LogError($"Unable to get All Hotel Details - {ex.Message} : {ex.StackTrace}");
                results.Message = "Unable to get Hotel Details";
                results.Response = ResponseType.Error;
                return results;
            }
        }

        //Ive written my own before but just got Gemini to throw this quickly together its a bonus anyway
        private static int LevenshteinDistance(ReadOnlySpan<char> source, ReadOnlySpan<char> target)
        {
            if (source.IsEmpty) return target.Length;
            if (target.IsEmpty) return source.Length;

            // Ensure 'target' is the shorter string to minimize array/stack allocation size
            if (source.Length < target.Length)
            {
                ReadOnlySpan<char> temp = source;
                source = target;
                target = temp;
            }

            int targetLength = target.Length;

            // Use stack memory for standard strings <= 256 chars; fallback to heap for massive text
            Span<int> costs = targetLength <= 256
                ? stackalloc int[targetLength + 1]
                : new int[targetLength + 1];

            // Initialize first row
            for (int j = 0; j <= targetLength; j++)
            {
                costs[j] = j;
            }

            for (int i = 1; i <= source.Length; i++)
            {
                int lastDiagonal = costs[0];
                costs[0] = i;

                for (int j = 1; j <= targetLength; j++)
                {
                    int currentDiagonal = costs[j];
                    int substitutionCost = source[i - 1] == target[j - 1] ? 0 : 1;

                    costs[j] = Math.Min(
                        Math.Min(costs[j] + 1, costs[j - 1] + 1), // Insertion / Deletion
                        lastDiagonal + substitutionCost           // Substitution
                    );

                    lastDiagonal = currentDiagonal;
                }
            }

            return costs[targetLength];
        }

    }
}
