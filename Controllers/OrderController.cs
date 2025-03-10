using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Controllers
{
    public class OrderController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public IActionResult Order()
        {
            return View();
        }
    }
}
