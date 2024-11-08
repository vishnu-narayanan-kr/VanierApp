using Microsoft.AspNetCore.Mvc;
using VanierApp.Data;
using System.Linq;
using VanierApp.Models;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

namespace VanierApp.Controllers
{
    public class AdminDashboardController : Controller
    {
        private readonly AppDbContext _context;

        public AdminDashboardController(AppDbContext context)
        {
            _context = context;
        }

        // Dashboard Index method
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
                    StudentEmail = _context.Students.FirstOrDefault(s => s.UserID == u.Id) != null ? _context.Students.FirstOrDefault(s => s.UserID == u.Id).StudentEmail : ""
                }).ToList();

            var teachers = _context.Users
                .Where(u => u.UserRole == "T")
                .Select(u => new Teacher
                {
                    TeacherID = u.Id,
                    TeacherName = u.Username,
                    TeacherEmail = _context.Teachers.FirstOrDefault(t => t.UserID == u.Id) != null ? _context.Teachers.FirstOrDefault(t => t.UserID == u.Id).TeacherEmail : ""
                }).ToList();

            var courses = _context.Courses
                .Select(c => new Course
                {
                    CourseID = c.CourseID,
                    CourseName = c.CourseName,
                    CourseBlock = c.CourseBlock,
                    TeacherID = c.TeacherID,
                }).ToList();

            var dashboardData = new DashboardViewModel
            {
                TotalStudents = totalStudents,
                TotalTeachers = totalTeachers,
                TotalCourses = totalCourses,
                TotalEnrollments = totalEnrollments,
                Students = students,
                Teachers = teachers,
                Courses = courses
            };

            return View(dashboardData);
        }

        // Student operations
        [HttpPost]
        public IActionResult CreateStudent(string StudentName, string StudentEmail, string Password)
        {
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(Password);

            var user = new User
            {
                Username = StudentName,
                Password = hashedPassword,
                UserRole = "S"
            };
            _context.Users.Add(user);
            _context.SaveChanges();

            var student = new Student
            {
                StudentName = StudentName,
                StudentEmail = StudentEmail,
                UserID = user.Id
            };
            _context.Students.Add(student);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult EditStudent(int StudentID, string StudentName, string StudentEmail)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == StudentID && u.UserRole == "S");
            if (user != null)
            {
                user.Username = StudentName;
                var student = _context.Students.FirstOrDefault(s => s.UserID == user.Id);
                if (student != null)
                {
                    student.StudentEmail = StudentEmail;
                    _context.SaveChanges();
                }
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult DeleteStudent(int id)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == id && u.UserRole == "S");
            if (user != null)
            {
                var student = _context.Students.FirstOrDefault(s => s.UserID == user.Id);
                if (student != null)
                {
                    _context.Students.Remove(student);
                }
                _context.Users.Remove(user);
                _context.SaveChanges();
            }

            return Json(new { success = true });
        }

        // Teacher operations
        [HttpPost]
        public IActionResult CreateTeacher(string TeacherName, string TeacherEmail, string Password)
        {
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(Password);

            var user = new User
            {
                Username = TeacherName,
                Password = hashedPassword,
                UserRole = "T"
            };
            _context.Users.Add(user);
            _context.SaveChanges();

            var teacher = new Teacher
            {
                TeacherName = TeacherName,
                TeacherEmail = TeacherEmail,
                UserID = user.Id
            };
            _context.Teachers.Add(teacher);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult EditTeacher(int TeacherID, string TeacherName, string TeacherEmail)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == TeacherID && u.UserRole == "T");
            if (user != null)
            {
                user.Username = TeacherName;
                var teacher = _context.Teachers.FirstOrDefault(t => t.UserID == user.Id);
                if (teacher != null)
                {
                    teacher.TeacherEmail = TeacherEmail;
                    _context.SaveChanges();
                }
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult DeleteTeacher(int id)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == id && u.UserRole == "T");
            if (user != null)
            {
                var teacher = _context.Teachers.FirstOrDefault(t => t.UserID == user.Id);
                if (teacher != null)
                {
                    _context.Teachers.Remove(teacher);
                }
                _context.Users.Remove(user);
                _context.SaveChanges();
            }

            return Json(new { success = true });
        }
        // Create a new course
        [HttpPost]
        public IActionResult CreateCourse(string CourseName, int TeacherID, string CourseBlock)
        {
            var teacher = _context.Teachers.FirstOrDefault(t => t.UserID == TeacherID);
            if (teacher == null)
            {
                ModelState.AddModelError("TeacherID", "The specified teacher does not exist." + TeacherID);
                return BadRequest(ModelState);
            }

            var course = new Course
            {
                CourseName = CourseName,
                TeacherID = teacher.TeacherID,
                CourseBlock = CourseBlock
            };
            _context.Courses.Add(course);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        // Edit an existing course
        [HttpPost]
        public IActionResult EditCourse(int CourseID, string CourseName, int TeacherID, string CourseBlock)
        {
            var teacher = _context.Teachers.FirstOrDefault(t => t.UserID == TeacherID);
            if (teacher == null)
            {
                ModelState.AddModelError("TeacherID", "The specified teacher does not exist.");
                return BadRequest(ModelState);
            }

            var course = _context.Courses.FirstOrDefault(c => c.CourseID == CourseID);
            if (course != null)
            {
                course.CourseName = CourseName;
                course.TeacherID = teacher.TeacherID;
                course.CourseBlock = CourseBlock;
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }
        [HttpPost]
        public IActionResult DeleteCourse(int CourseID)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var studentCourses = _context.StudentCourses.Where(sc => sc.CourseID == CourseID).ToList();
                    _context.StudentCourses.RemoveRange(studentCourses);
                    _context.SaveChanges();

                    var course = _context.Courses.FirstOrDefault(c => c.CourseID == CourseID);
                    if (course != null)
                    {
                        _context.Courses.Remove(course);
                        _context.SaveChanges();
                    }

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Console.WriteLine(ex.Message);
                    return BadRequest("An error occurred while deleting the course.");
                }
            }

            return RedirectToAction("Index");
        }

        // Enroll a student in a course
        [HttpPost]
        public IActionResult EnrollStudent(int StudentID, int CourseID)
        {
            var enrollmentExists = _context.StudentCourses.Any(sc => sc.StudentID == StudentID && sc.CourseID == CourseID);
            if (enrollmentExists)
            {
                ModelState.AddModelError("Enrollment", "The student is already enrolled in this course.");
                return BadRequest(ModelState);
            }

            var studentCourse = new StudentCourse
            {
                StudentID = StudentID,
                CourseID = CourseID
            };
            _context.StudentCourses.Add(studentCourse);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

    }
}
