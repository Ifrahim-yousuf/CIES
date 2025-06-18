using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using SDAProject.Models;
using System.Data;
using System.Threading.Tasks;

namespace SDAProject.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IConfiguration _configuration;

        public DashboardController(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        public async Task<IActionResult> EmployeeDetails(Guid id)
        {
            var employee = new EmployessDetailModel();

            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("GetEmployeeDetailsByDeviceID", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@DeviceID", id);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            employee.DeviceID = reader.GetGuid(reader.GetOrdinal("DeviceID"));
                            employee.EmpiName = reader["EmpiName"].ToString();
                            employee.Address = reader["Address"].ToString();
                            employee.ContactNo = reader["ContactNo"].ToString();
                            employee.DepartmentName = reader["DepartmentName"].ToString();
                            employee.ManagerName = reader["ManagerName"].ToString();
                            employee.DesktopName = reader["DesktopName"].ToString();
                            employee.OSVersion = reader["OSVersion"].ToString();
                            employee.RamSize = reader["RamSize"].ToString();
                            employee.MacAddress = reader["MacAddress"].ToString();
                            employee.ViolationCount = Convert.ToInt32(reader["ViolationCount"]);
                            employee.Score = Convert.ToInt32(reader["Score"]);
                            employee.KeylogData = reader["KeylogData"].ToString();
                        }
                    }
                }
            }

            return View(employee);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(Guid deviceId)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    SqlCommand cmd = new SqlCommand("DeleteEmployee", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@DeviceID", deviceId);

                    con.Open();
                    cmd.ExecuteNonQuery();
                }

                // ✅ Set a success message
                TempData["SuccessMessage"] = "Employee deleted successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error deleting employee.";
                // You could log ex.Message here
            }

            return RedirectToAction("DashBoardView");
        }


        public async Task<IActionResult> DashBoardView()
        {
            var employeeList = new List<DashBoardModel>();

            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                using (var command = new SqlCommand("GetAllEmployees", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    await connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            employeeList.Add(new DashBoardModel
                            {
                                EmpiName = reader["EmpiName"].ToString(),
                                DepartmentID = Convert.ToInt32(reader["DepartmentID"]),
                                ManagerID = Convert.ToInt32(reader["ManagerID"]),
                                DeviceID = Guid.Parse(reader["DeviceID"].ToString())
                            });
                        }
                    }
                }
            }

            return View(employeeList);
        }
    }
}
