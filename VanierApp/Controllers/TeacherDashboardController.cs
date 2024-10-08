using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using VanierApp.Models;

namespace VanierApp.Controllers
{
    public class TeacherDashboardController : Controller
    {
        public readonly string connectionString;
        public TeacherDashboardController(IConfiguration configuration)
        {
            // Fetch the connection string from appsettings.json
            connectionString = configuration.GetConnectionString("DefaultConnection");

        }
        public IActionResult Index(List<CourseViewModel> model)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("Username")))
            {
                return RedirectToAction("Index", "Login");
            }

            string teacherID = GetTeacherIDFromUSername();
            string teacherName = GetTeacherName();
            HttpContext.Session.SetString("TeacherName", teacherName);



            model = new List<CourseViewModel>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string sql = "select Courses.CourseName , Courses.CourseBlock from Courses where TeacherID = '" + teacherID + "';";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string CourseName = reader["CourseName"].ToString();
                            string CourseBlock = reader["CourseBlock"].ToString();

                            CourseViewModel course = new CourseViewModel();
                            course.CourseName = CourseName;
                            course.CourseBlock = CourseBlock;
                            model.Add(course);
                        }

                    }
                }
            }
            return View(model);
        }

        public string GetTeacherIDFromUSername()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string userName = HttpContext.Session.GetString("Username");
                string sql = "SELECT Teachers.TeacherID from Users JOIN Teachers ON Users.Id = Teachers.UserID where Users.Username ='" + userName + "';";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string teacherID = reader["TeacherID"].ToString();
                            return teacherID;
                        }
                    }
                    return "";
                }
            }
        }

        public string GetTeacherName()
        {

            string TeacherID = GetTeacherIDFromUSername();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string sql = "Select * FROM Teachers JOIN Users ON Users.Id = Teachers.UserID WHERE Teachers.TeacherID= '" + TeacherID + "'; ";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {


                    using (SqlDataReader reader = command.ExecuteReader())
                    {

                        while (reader.Read())
                        {
                            string teacherName = reader["TeacherName"].ToString();
                            //string studentEmail = reader["StudentEmail"].ToString();

                            //model.StudentName = studentName;
                            //model.StudentEmail = studentEmail;

                            if (!string.IsNullOrEmpty(teacherName))
                            {
                                //HttpContext.Session.SetString("StudentName", studentName);
                                //ViewBag.StudentName = studentName;
                                return teacherName;
                            }


                        }
                        return "";

                    }

                }
            }

        }

    }


}
