using Microsoft.AspNetCore.Mvc;

namespace Waracle_HotelAPI.Controllers
{
    public class DataController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
