using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using VanierApp.Models;
using Microsoft.Data.SqlClient;
using System.Diagnostics.Eventing.Reader;
using System.Reflection;
using Microsoft.Extensions.Configuration;

namespace VanierApp.Controllers
{
    public class LoginController : Controller
    {

        public readonly string connectionString;
        public LoginController(IConfiguration configuration)
        {
            // Fetch the connection string from appsettings.json
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        private readonly ILogger<HomeController> _logger;

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return View("Index");
        }
        public IActionResult LoginMethod(LoginViewModel model)
        {
             //private readonly string connectionString = _configuration.GetConnectionString("DefaultConnectionString");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string sql = "SELECT * FROM Users WHERE Username = @Username and Password =@Password";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    //model.Password =model.Password.Trim();


                    command.Parameters.AddWithValue("@Username", model.Username);
                    command.Parameters.AddWithValue("@Password", model.Password);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string userRole = reader["UserRole"].ToString();

                            HttpContext.Session.SetString("Username", model.Username);
                            HttpContext.Session.SetString("UserRole", userRole);
                            HttpContext.Session.SetString("IsAuthenticated", "Y");

                         

                            if (userRole.Equals("S"))
                                return RedirectToAction("Index", "StudentDashboard");
                            else
                                return RedirectToAction("Index", "TeacherDashboard");

                        }
                        else
                        {
                            model.ErrorMessage = "Your username/password is incorrect";
                            return View("Index", model);
                        }
                    

                    }
                }
            }

            //return View(model);
            return View("Index", model);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
