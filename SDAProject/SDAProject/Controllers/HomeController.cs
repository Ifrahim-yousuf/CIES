using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using SDAProject.Models;
using System.Data;
using System.Diagnostics;
using SDAProject.Services;


namespace SDAProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;
        private readonly ActivityLoggerService _activityLogger;

        public HomeController(
            IConfiguration configuration,
            ILogger<HomeController> logger,
            ActivityLoggerService activityLogger)
        {
            _configuration = configuration;
            _logger = logger;
            _activityLogger = activityLogger;
        }

        public IActionResult LandingPageView()
        {
            return View();
        }
        public IActionResult LoginView()
        {
            return View();
        }

        public IActionResult RegistrationView()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegistrationModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                    {
                        using (var command = new SqlCommand("InsertEmployess", connection))
                        {
                            command.CommandType = CommandType.StoredProcedure;

                            command.Parameters.AddWithValue("@DeviceID", model.DeviceID);
                            command.Parameters.AddWithValue("@EmpiName", model.EmpiName);
                            command.Parameters.AddWithValue("@Address", model.Address);
                            command.Parameters.AddWithValue("@ContactNo", model.ContactNo);
                            command.Parameters.AddWithValue("@DepartName", model.DepartName); // Make sure this param matches the SP

                            connection.Open();
                            int result = await command.ExecuteNonQueryAsync();

                            if (result > 0)
                            {
                                ModelState.Clear(); 
                                ViewBag.Success = true;
                                return View("RegistrationView",new RegistrationModel()); 
                            }
                            else
                            {
                                ViewBag.Error = "Registration failed!";
                                return View("RegistrationView",model); 
                            }

                        }
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.Error = "? Error: " + ex.Message;
                }
            }
            else
            {
                ViewBag.Error = "? Invalid input. Please correct the form.";
            }

            return View("RegistrationView", model);

        }


        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    var command = new SqlCommand("SELECT * FROM Users WHERE Name = @Username AND Password = @Password", connection);
                    command.Parameters.AddWithValue("@Username", model.Username);
                    command.Parameters.AddWithValue("@Password", model.Password);

                    await connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            int userId = Convert.ToInt32(reader["UserID"]);

                            // Use the service for logging
                            await _activityLogger.LogUserActivityAsync(userId, 1);

                            TempData["UserID"] = userId;

                            return RedirectToAction("DashBoardView", "Dashboard");
                        }
                        else
                        {
                            ViewBag.LoginError = "Invalid username or password.";
                            return View("LoginView", model);
                        }
                    }
                }
            }

            ViewBag.LoginError = "Please enter valid login credentials.";
            return View("LoginView", model);
        }



        public async Task<IActionResult> Logout()
        {
            try
            {
                if (TempData["UserID"] != null)
                {
                    int userId = Convert.ToInt32(TempData["UserID"]);
                    await _activityLogger.LogUserActivityAsync(userId, 0); // 0 = Logout
                }

                TempData.Clear();
                return RedirectToAction("LandingPageView", "LandingPage");
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Error during logout.";
                return RedirectToAction("DashBoardView", "Dashboard");
            }
        }


    }
}
