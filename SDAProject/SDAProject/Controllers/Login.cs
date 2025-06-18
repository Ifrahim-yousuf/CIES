using Microsoft.AspNetCore.Mvc;

namespace SDAProject.Controllers
{
    public class Login : Controller
    {
        public IActionResult LoginView()
        {
            return View();
        }
    }
}
