using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Waracle_HotelAPI.Interfaces;
using Waracle_HotelAPI.Models;
using Waracle_HotelAPI.RequestModels;
using Waracle_HotelAPI.ReturnModels;
using Waracle_HotelAPI.Services;

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


        [HttpGet]
        public async Task<ActionResult<Booking>> Get([FromQuery] string Reference)
        {
            throw new NotImplementedException();
        }

        [HttpPost("FindAvailableRooms")]
        public async Task<ActionResult<List<BookingSet>>> FindAvailableRooms([FromBody] BookingEnquiry enquiry)
        {
            return await bookingService.CheckForAvailableBookings(enquiry);
        }

        [HttpPost]
        public async Task<ActionResult<BookingResult>> BookRooms([FromBody] BookingRequest request)
        {
            throw new NotImplementedException();
        }
      
    }
}
