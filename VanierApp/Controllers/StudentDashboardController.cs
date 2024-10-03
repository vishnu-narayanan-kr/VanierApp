using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using VanierApp.Models;

namespace VanierApp.Controllers
{
    public class StudentDashboardController : Controller
    {
        public readonly string connectionString;
        public StudentDashboardController(IConfiguration configuration)
        {
            // Fetch the connection string from appsettings.json
            connectionString = configuration.GetConnectionString("DefaultConnection");

        }

        public string GetStudentIDFromUSername()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string userName = HttpContext.Session.GetString("Username");
                string sql = "SELECT Students.studentID from Users JOIN Students ON Users.Id = Students.UserID where Users.Username ='" + userName + "';";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string StudentID = reader["studentID"].ToString();
                            return StudentID;
                        }
                    }
                    return "";
                }
            }
        }

        public IActionResult Index(List<CourseViewModel> model)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("Username")))
            {
                return RedirectToAction("Index", "Login");
            }

            string StudentID = GetStudentIDFromUSername();

            model = new List<CourseViewModel>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string sql = "select Courses.CourseName , Courses.CourseBlock, Courses.CourseID from Courses JOIN StudentCourses ON Courses.CourseID = StudentCourses.CourseID where StudentCourses.StudentID = '"+ StudentID + "';";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                            

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string CourseName = reader["CourseName"].ToString();
                            string CourseBlock = reader["CourseBlock"].ToString();
                            string CourseID = reader["CourseID"].ToString();

                            CourseViewModel course = new CourseViewModel();
                                course.CourseName = CourseName;
                                course.CourseBlock = CourseBlock;
                                course.CourseID = CourseID; 
                            model.Add(course);
                        }

                    }
                }
            }

            return View(model);
        }
    }
}
 
