using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using VanierApp.Models;
using VanierApp.Data;

namespace VanierApp.Controllers
{
    public class StudentDashboardController : Controller
    {
        private readonly StudentDAO _studentDAO;

        public StudentDashboardController(IConfiguration configuration)
        {
            // Initialize the DAO with the connection string
            string connectionString = configuration.GetConnectionString("DefaultConnection");
            _studentDAO = new StudentDAO(connectionString);
        }

        public IActionResult Index(List<CourseViewModel> model)
        {
            string username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Index", "Login");
            }

            string studentID = _studentDAO.GetStudentIDFromUsername(username);
            string studentName = _studentDAO.GetStudentName(studentID);
            HttpContext.Session.SetString("StudentName", studentName);

            model = _studentDAO.GetStudentCourses(studentID);

            return View(model);
        }

        public IActionResult CourseDetails(string courseID)
        {
            string studentID = _studentDAO.GetStudentIDFromUsername(HttpContext.Session.GetString("Username"));
            StudentGradeViewModel grade = _studentDAO.GetStudentGrade(studentID, courseID);

            return View(grade);
        }
    }
}
