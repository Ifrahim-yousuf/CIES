using Microsoft.AspNetCore.Mvc;

namespace SDAProject.Controllers
{
    public class Registration : Controller
    {
        public IActionResult RegistrationView()
        {
            return View();
        }
    }
}
