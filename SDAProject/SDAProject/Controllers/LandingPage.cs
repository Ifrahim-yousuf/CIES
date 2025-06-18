using Microsoft.AspNetCore.Mvc;

namespace SDAProject.Controllers
{
    public class LandingPage : Controller
    {
        public IActionResult LandingPageView()
        {
            return View();
        }
        public IActionResult AboutUs()
        {
            return View();
        }
    }
}
