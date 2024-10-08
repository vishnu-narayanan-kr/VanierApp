using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Reflection;
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

            model = new List<CourseViewModel>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string sql = "select Courses.CourseName , Courses.CourseBlock, Courses.CourseID from Courses where TeacherID = '" + teacherID + "';";

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

        public IActionResult CourseDetails(string courseID)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("Username")))
            {
                return RedirectToAction("Login", "Account");
            }

            List<TeacherGradeViewModel> StudentList = new List<TeacherGradeViewModel>();

            try
            {
                string teacherID = GetTeacherIDFromUSername();
                
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Fetch students enrolled in the course along with their grades
                    string sql = @"
                SELECT Students.StudentID, Students.StudentName, Grades.GradeCode, Grades.GradeComments
                FROM StudentCourses
                JOIN Students ON StudentCourses.StudentID = Students.StudentID
                LEFT JOIN Grades ON Grades.StudentID = StudentCourses.StudentID AND Grades.CourseID = StudentCourses.CourseID
                WHERE StudentCourses.CourseID = @CourseID";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@CourseID", courseID);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                // Load saved grades (if any) for each student
                                TeacherGradeViewModel studentGrade = new TeacherGradeViewModel
                                {
                                    StudentID = reader["StudentID"].ToString(),
                                    StudentName = reader["StudentName"].ToString(),
                                    GradeCode = reader["GradeCode"] != DBNull.Value ? reader["GradeCode"].ToString() : "",  // Load saved GradeCode
                                    GradeComments = reader["GradeComments"] != DBNull.Value ? reader["GradeComments"].ToString() : ""  // Load saved comments
                                };

                                StudentList.Add(studentGrade);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            ViewBag.CourseID = courseID;  // Pass the CourseID to the view
            return View(StudentList);
        }

        [HttpPost]
        public IActionResult SaveGrades(List<TeacherGradeViewModel> StudentList, string CourseID)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    foreach (var student in StudentList)
                    {
                        // Check if the grade already exists for the student
                        string checkQuery = "SELECT COUNT(*) FROM Grades WHERE StudentID = @StudentID AND CourseID = @CourseID";
                        using (SqlCommand checkCommand = new SqlCommand(checkQuery, connection))
                        {
                            checkCommand.Parameters.AddWithValue("@StudentID", student.StudentID);
                            checkCommand.Parameters.AddWithValue("@CourseID", CourseID);

                            int gradeExists = (int)checkCommand.ExecuteScalar();

                            if (gradeExists > 0)
                            {
                                // Update the existing grade
                                string updateQuery = "UPDATE Grades SET GradeCode = @GradeCode, GradeComments = @GradeComments WHERE StudentID = @StudentID AND CourseID = @CourseID";
                                using (SqlCommand updateCommand = new SqlCommand(updateQuery, connection))
                                {
                                    updateCommand.Parameters.AddWithValue("@GradeCode", student.GradeCode ?? (object)DBNull.Value);
                                    updateCommand.Parameters.AddWithValue("@GradeComments", student.GradeComments ?? (object)DBNull.Value);
                                    updateCommand.Parameters.AddWithValue("@StudentID", student.StudentID);
                                    updateCommand.Parameters.AddWithValue("@CourseID", CourseID);

                                    updateCommand.ExecuteNonQuery();
                                }
                            }
                            else
                            {
                                // Insert new grade if it doesn't exist
                                string insertQuery = "INSERT INTO Grades (StudentID, CourseID, GradeCode, GradeComments) VALUES (@StudentID, @CourseID, @GradeCode, @GradeComments)";
                                using (SqlCommand insertCommand = new SqlCommand(insertQuery, connection))
                                {
                                    insertCommand.Parameters.AddWithValue("@StudentID", student.StudentID);
                                    insertCommand.Parameters.AddWithValue("@CourseID", CourseID);
                                    insertCommand.Parameters.AddWithValue("@GradeCode", student.GradeCode ?? (object)DBNull.Value);
                                    insertCommand.Parameters.AddWithValue("@GradeComments", student.GradeComments ?? (object)DBNull.Value);

                                    insertCommand.ExecuteNonQuery();
                                }
                            }
                        }
                    }
                }

                // Set success message
                TempData["Message"] = "Grades saved successfully!";
                TempData["MessageType"] = "success";  // Bootstrap alert type for success
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                // Set error message
                TempData["Message"] = "There was an error saving the grades.";
                TempData["MessageType"] = "danger";  // Bootstrap alert type for errors
            }

            //return View(StudentList);

            // Return to the same CourseDetails view
            return RedirectToAction("CourseDetails", new { courseID = CourseID });
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
    }


}
