using Microsoft.AspNetCore.Mvc;
using VanierApp.Data;
using System.Linq;
using VanierApp.Models;

namespace VanierApp.Controllers
{
    public class AdminDashboardController : Controller
    {
        private readonly AppDbContext _context;

        public AdminDashboardController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var totalStudents = _context.Users.Count(u => u.UserRole == "S");
            var totalTeachers = _context.Users.Count(u => u.UserRole == "T");
            var totalCourses = _context.Courses.Count();
            var totalEnrollments = _context.StudentCourses.Count();

            var students = _context.Users
                .Where(u => u.UserRole == "S")
                .Select(u => new Student
                {
                    StudentID = u.Id,
                    StudentName = u.Username,
                    StudentEmail = _context.Students.FirstOrDefault(s => s.UserID == u.Id).StudentEmail
                }).ToList();

            var teachers = _context.Users
                .Where(u => u.UserRole == "T")
                .Select(u => new Teacher
                {
                    TeacherID = u.Id,
                    TeacherName = u.Username,
                    TeacherEmail = _context.Teachers.FirstOrDefault(t => t.UserID == u.Id).TeacherEmail
                }).ToList();

            var course = _context.Courses
                .Select(c => new Course
                {
                    CourseID = c.CourseID,
                    CourseName = c.CourseName,
                    CourseBlock = c.CourseBlock,
                    TeacherName = _context.Teachers.FirstOrDefault(t => t.TeacherID == c.TeacherID).TeacherName,
                }).ToList();

            var dashboardData = new DashboardViewModel
            {
                TotalStudents = totalStudents,
                TotalTeachers = totalTeachers,
                TotalCourses = totalCourses,
                TotalEnrollments = totalEnrollments,
                Students = students,
                Teachers = teachers,
                Courses = course
            };

            return View(dashboardData);
        }
    }
}
