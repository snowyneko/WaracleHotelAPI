using Microsoft.AspNetCore.Mvc;
using Serilog.Core;
using Waracle_HotelAPI.Interfaces;
using Waracle_HotelAPI.ReturnModels;

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

        [HttpGet("Search")]
        public async Task<ActionResult<HotelList>> Search([FromQuery] string searchString)
        {
            logger.LogInformation($"Get Hotels API Called with argument {searchString}");
            try
            {
                HotelList results = await hotelService.GetHotelInfo(searchString);
                return results.Response switch
                {
                    ResponseType.OK => results,
                    ResponseType.NotFound => NotFound(results.Message),
                    ResponseType.Conflict => Conflict(results.Message),
                    ResponseType.Error => Problem(results.Message),
                    _ => BadRequest("Unknown Error")
                };
            }
            catch (Exception ex)
            {
                logger.LogError($"Could not Search Hotel Details {ex.Message} : {ex.StackTrace}");
                return Problem("An Error Occured Attempting to Search Hotels");
            }

        }

        [HttpGet("GetAll")]
        public async Task<ActionResult<HotelList>> GetAll()
        {
            logger.LogInformation("Get All Hotels API Called");
            try
            {
                HotelList results = await hotelService.GetAllHotelInfo();
                return results.Response switch
                {
                    ResponseType.OK => results,
                    ResponseType.NotFound => NotFound(results.Message),
                    ResponseType.Conflict => Conflict(results.Message),
                    ResponseType.Error => Problem(results.Message),
                    _ => BadRequest("Unknown Error")
                };
            }
            catch (Exception ex)
            {
                logger.LogError($"Could not Fetch Hotel Details {ex.Message} : {ex.StackTrace}");
                return Problem("An Error Occured Attempting to Fetch Hotels");
            }
        
        }
    }
}
