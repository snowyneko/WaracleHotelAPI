using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Waracle_HotelAPI.Interfaces;
using Waracle_HotelAPI.Models;
using Waracle_HotelAPI.RequestModels;
using Waracle_HotelAPI.ReturnModels;

namespace Waracle_HotelAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService bookingService;
        private readonly ILogger<BookingController> logger;

        public BookingController(IBookingService bookingService, ILogger<BookingController> logger)
        {
            this.bookingService = bookingService;
            this.logger = logger;
        }


        [HttpGet("Search")]
        public async Task<ActionResult<BookingDetails>> Search([FromQuery] string Reference)
        {
            logger.LogInformation($"Get Booking Called");
            try
            {
                BookingDetails details = await bookingService.FindBooking(Reference);
                return details.Response switch
                {
                    ResponseType.OK => details,
                    ResponseType.NotFound => NotFound(details.Message),
                    ResponseType.Conflict => Conflict(details.Message),
                    ResponseType.Error => Problem(details.Message),
                    _ => BadRequest("Unknown Error")
                };
            }
            catch (Exception ex)
            {
                logger.LogError($"Error Thrown Searching for Booking {Reference} {ex.Message} : {ex.StackTrace}");
                return Problem("An Error Occured Attempting to Locate your Booking");
            }
        }

        [HttpPost("FindAvailableRooms")]
        public async Task<ActionResult<BookingOptions>> FindAvailableRooms([FromBody] BookingEnquiry enquiry)
        {
            if(enquiry.NoOfPeople<1) return BadRequest("There must be at least one guest.");
            logger.LogInformation($"find Available Rooms Called");
            try
            {
                
                BookingOptions options = await bookingService.CheckForAvailableBookings(enquiry);
                return options.Response switch
                {
                    ResponseType.OK => options,
                    ResponseType.NotFound => NotFound(options.Message),
                    ResponseType.Conflict => Conflict(options.Message),
                    ResponseType.Error => Problem(options.Message),
                    _ => BadRequest("Unknown Error")
                };
            }
            catch(Exception ex)
            {
                logger.LogError($"Error Thrown Searching for Rooms {ex.Message} : {ex.StackTrace}");
                return Problem("An Error Occured Attempting to Find Available Rooms");
            }
        }

        [HttpPost("CreateBooking")]
        public async Task<ActionResult<BookingResult>> CreateBooking([FromBody] BookingRequest request)
        {
            logger.LogInformation($"Create Booking Called");
            try
            {
                if(request.Arrival < DateOnly.FromDateTime(DateTime.UtcNow)) return BadRequest("Your arrival day cannot be in the past");
                if (request.Departure <=request.Arrival) return BadRequest("Your departure day must be after your arrival day.");
                if(request.RequestedRooms is null ||  request.RequestedRooms.Count()==0) return BadRequest("You must request at least one room");
                BookingResult result = await bookingService.CreateBooking(request);
                return result.Response switch
                {
                    ResponseType.OK => result,
                    ResponseType.NotFound => NotFound(result.Message),
                    ResponseType.Conflict => Conflict(result.Message),
                    ResponseType.Error => BadRequest(result.Message),
                    _ => Problem("Unknown Error")
                };
            }
            catch (Exception ex)
            {
                logger.LogError($"Error Thrown Creating Booking {ex.Message} : {ex.StackTrace}");
                return Problem("An Error Occured Attempting to Create a Booking");
            }
        }
      
    }
}
