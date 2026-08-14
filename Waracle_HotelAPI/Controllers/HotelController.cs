using Microsoft.AspNetCore.Mvc;

namespace Waracle_HotelAPI.Controllers
{
    public class HotelController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
