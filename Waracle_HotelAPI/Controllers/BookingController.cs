using Microsoft.AspNetCore.Mvc;

namespace Waracle_HotelAPI.Controllers
{
    public class BookingController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
