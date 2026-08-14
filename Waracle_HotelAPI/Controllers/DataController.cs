using Microsoft.AspNetCore.Mvc;
using Waracle_HotelAPI.Interfaces;

namespace Waracle_HotelAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DataController : ControllerBase
    {
        private readonly ISeedService seedService;
        private readonly ILogger<DataController> logger;

        public DataController(ISeedService seedService,ILogger<DataController> logger)
        {
            this.seedService = seedService;
            this.logger = logger;
        }

        [HttpDelete("ClearData")]
        public async Task<IActionResult> ClearData()
        {
            logger.LogInformation("Clear Data API Called");
            bool result = await seedService.ClearDatabase();
            if (!result) return BadRequest("Database Could not be cleared");
            return Ok("Database Succesfully Cleared");
        }

        [HttpPost("SeedData")]
        public async Task<IActionResult> PostData()
        {
            logger.LogInformation("Seed Data API Called");
            bool result = await seedService.SeedDatabase();
            if (!result) return BadRequest("Database Could not be seeded, it may not have been empty");
            return Ok("Database Succesfully Seeded");
        }



    }
}
