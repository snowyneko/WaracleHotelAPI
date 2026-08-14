using Microsoft.AspNetCore.Mvc;
using Serilog.Core;
using Waracle_HotelAPI.Interfaces;
using Waracle_HotelAPI.ReturnModels;
using Waracle_HotelAPI.Services;

namespace Waracle_HotelAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HotelController : ControllerBase
    {
        private readonly IHotelService hotelService;
        private readonly ILogger<HotelController> logger;

        public HotelController(IHotelService hotelService,ILogger<HotelController> logger)
        {
            this.hotelService = hotelService;
            this.logger = logger;
        }

        [HttpGet("Get")]
        public async Task<ActionResult<List<HotelDetails>>> Get([FromQuery] string searchString)
        {
            logger.LogInformation($"Get Hotels API Called with argument {searchString}");
            try
            {
                List<HotelDetails> results = await hotelService.GetHotelInfo(searchString);
                return Ok(results);
            }
            catch (Exception ex)
            {
                logger.LogError($"Could not Search Hotel Details {ex.Message} : {ex.StackTrace}");
                return BadRequest("An Error Occured Attempting to Search Hotels");
            }

        }

        [HttpGet("GetAll")]
        public async Task<ActionResult<List<HotelDetails>>> GetAll()
        {
            logger.LogInformation("Get All Hotels API Called");
            try
            {
                List<HotelDetails> results = await hotelService.GetAllHotelInfo();
                return Ok(results);
            }
            catch (Exception ex)
            {
                logger.LogError($"Could not Fetch Hotel Details {ex.Message} : {ex.StackTrace}");
                return BadRequest("An Error Occured Attempting to Fetch Hotels");
            }
        
        }
    }
}
