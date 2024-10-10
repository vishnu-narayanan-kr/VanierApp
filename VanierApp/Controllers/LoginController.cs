using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using VanierApp.Models;
using Microsoft.Data.SqlClient;
using System.Diagnostics.Eventing.Reader;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using VanierApp.Data;

namespace VanierApp.Controllers
{
    public class LoginController : Controller
    {
        private readonly AppDbContext _context;

        public LoginController(AppDbContext context)
        {
            _context = context;
        }
        private void UpdateLogRegistry(string msg) 
        {
            string logEntry = "date: " + DateTime.Now + ", " + msg;
            string dir = Directory.GetCurrentDirectory();
            string path = Path.Combine(dir, "Output\\Log.txt");
            using (StreamWriter sw = new StreamWriter(path, true))
            {
                sw.WriteLine(logEntry);
            }
        }
        public readonly string connectionString;
        //public LoginController(IConfiguration configuration)
        //{
        //    // Fetch the connection string from appsettings.json
        //    connectionString = configuration.GetConnectionString("DefaultConnection");
        //}

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
        //public IActionResult LoginMethod(LoginViewModel model)
        //{
        //     //private readonly string connectionString = _configuration.GetConnectionString("DefaultConnectionString");
        //    using (SqlConnection connection = new SqlConnection(connectionString))
        //    {
        //        connection.Open();
        //        string sql = "SELECT * FROM Users WHERE Username = @Username and Password =@Password";
        //        using (SqlCommand command = new SqlCommand(sql, connection))
        //        {
        //            //model.Password =model.Password.Trim();


        //            command.Parameters.AddWithValue("@Username", model.Username);
        //            command.Parameters.AddWithValue("@Password", model.Password);

        //            using (SqlDataReader reader = command.ExecuteReader())
        //            {
        //                if (reader.Read())
        //                {
        //                    UpdateLogRegistry("userName: " + model.Username +
        //                        ", result: Successful login");
        //                    string userRole = reader["UserRole"].ToString();

        //                    HttpContext.Session.SetString("Username", model.Username);
        //                    HttpContext.Session.SetString("UserRole", userRole);
        //                    HttpContext.Session.SetString("IsAuthenticated", "Y");



        //                    if (userRole.Equals("S"))
        //                        return RedirectToAction("Index", "StudentDashboard");
        //                    else
        //                        return RedirectToAction("Index", "TeacherDashboard");

        //                }
        //                else
        //                {
        //                    UpdateLogRegistry("userName: " + model.Username +
        //                        ", result: Unsuccessful login");
        //                    model.ErrorMessage = "Your username/password is incorrect";
        //                    return View("Index", model);
        //                }


        //            }
        //        }
        //    }

        //    //return View(model);
        //    return View("Index", model);
        //}

        public IActionResult LoginMethod(LoginViewModel model)
        {
            var user = _context.Users
                .FirstOrDefault(u => u.Username == model.Username && u.Password == model.Password);

            if (user != null)
            {
                //UpdateLogRegistry("userName: " + model.Username + ", result: Successful login");
                HttpContext.Session.SetString("Username", model.Username);
                HttpContext.Session.SetString("UserRole", user.UserRole);
                HttpContext.Session.SetString("IsAuthenticated", "Y");

                if (user.UserRole == "S")
                    return RedirectToAction("Index", "StudentDashboard");
                else
                    return RedirectToAction("Index", "TeacherDashboard");
            }
            else
            {
                //UpdateLogRegistry("userName: " + model.Username + ", result: Unsuccessful login");
                model.ErrorMessage = "Your username/password is incorrect";
                return View("Index", model);
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
