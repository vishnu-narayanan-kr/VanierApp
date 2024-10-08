using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
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

        public string GetStudentName()
        {
            string StudentID = GetStudentIDFromUSername();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string sql = "Select * FROM Students JOIN Users ON Users.Id = Students.UserID WHERE Students.StudentID= '" + StudentID + "'; ";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string studentName = reader["StudentName"].ToString();

                            if (!string.IsNullOrEmpty(studentName))
                            {
                                return studentName;
                            }
                        }
                        return "";
                    }

                }
            }

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
            string studentName = GetStudentName();
            HttpContext.Session.SetString("StudentName", studentName);

            model = new List<CourseViewModel>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string sql = "select Courses.CourseName , Courses.CourseBlock, Courses.CourseID from Courses JOIN StudentCourses ON Courses.CourseID = StudentCourses.CourseID where StudentCourses.StudentID = '" + StudentID + "';";
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

        public IActionResult CourseDetails(string CourseID)
        {
            string courseID = CourseID;
            string StudentID = GetStudentIDFromUSername();
            StudentGradeViewModel grade = new StudentGradeViewModel();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string sql = "Select Grades.GradeCode, Grades.GradeComments from Grades where StudentID = '" + StudentID + "' and CourseID = '" + CourseID + "';";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {

                    using (SqlDataReader reader = command.ExecuteReader())
                    {

                        while (reader.Read())
                        {
                            string GradeCode = reader["GradeCode"].ToString();
                            string GradeComments = reader["GradeComments"].ToString();
                            //string CourseID = reader["CourseID"].ToString();



                            grade.GradeCode = GradeCode;
                            grade.GradeComments = GradeComments;

                            if (string.IsNullOrEmpty(GradeCode))
                            {
                                grade.GradeCode = "Grade Yet to be Posted";

                            }
                            if (string.IsNullOrEmpty(GradeComments))
                            {
                                grade.GradeComments = "comments not available";

                            }
                            return View(grade);
                        }

                    }
                    grade.GradeCode = "Grade Yet to be Posted";
                    grade.GradeComments = "comments not available";
                }
            }
            return View(grade);
        }
    }
}

