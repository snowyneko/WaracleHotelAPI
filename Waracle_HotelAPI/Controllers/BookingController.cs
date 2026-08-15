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


        [HttpGet("Get")]
        public async Task<ActionResult<BookingDetails>> Get([FromQuery] string Reference)
        {
            logger.LogInformation($"Get Booking Called");
            try
            {
                return await bookingService.FindBooking(Reference);
            }
            catch (Exception ex)
            {
                logger.LogError($"Error Thrown Searching for Booking {Reference} {ex.Message} : {ex.StackTrace}");
                return BadRequest("An Error Occured Attempting to Locate your Booking");
            }
        }

        [HttpPost("FindAvailableRooms")]
        public async Task<ActionResult<List<BookingSet>>> FindAvailableRooms([FromBody] BookingEnquiry enquiry)
        {
            logger.LogInformation($"find Available Rooms Called");
            try
            {
                return await bookingService.CheckForAvailableBookings(enquiry);
            }
            catch(Exception ex)
            {
                logger.LogError($"Error Thrown Searching for Rooms {ex.Message} : {ex.StackTrace}");
                return BadRequest("An Error Occured Attempting to Find Available Rooms");
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
                if(request.RequestedRooms.Count()==0) return BadRequest("You must request at least one room");
                return await bookingService.CreateBooking(request);
            }
            catch (Exception ex)
            {
                logger.LogError($"Error Thrown Creating Booking {ex.Message} : {ex.StackTrace}");
                return BadRequest("An Error Occured Attempting to Create a Booking");
            }
        }
      
    }
}
