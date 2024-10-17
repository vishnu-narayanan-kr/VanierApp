using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using VanierApp.Models;

namespace VanierApp.Data
{
    public class StudentDAO
    {
        private readonly string _connectionString;

        public StudentDAO(string connectionString)
        {
            _connectionString = connectionString;
        }

        public string GetStudentName(string studentID)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string sql = "SELECT StudentName FROM Students WHERE StudentID = @StudentID;";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@StudentID", studentID);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return reader["StudentName"].ToString();
                        }
                    }
                }
            }

            return string.Empty;
        }

        public string GetStudentIDFromUsername(string userName)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string sql =
                    "SELECT Students.StudentID FROM Users JOIN Students ON Users.Id = Students.UserID WHERE Users.Username = @UserName;";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@UserName", userName);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return reader["StudentID"].ToString();
                        }
                    }
                }
            }

            return string.Empty;
        }

        public List<CourseViewModel> GetStudentCourses(string studentID)
        {
            List<CourseViewModel> courses = new List<CourseViewModel>();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string sql = "SELECT Courses.CourseName, Courses.CourseBlock, Courses.CourseID FROM Courses " +
                             "JOIN StudentCourses ON Courses.CourseID = StudentCourses.CourseID " +
                             "WHERE StudentCourses.StudentID = @StudentID;";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@StudentID", studentID);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            CourseViewModel course = new CourseViewModel
                            {
                                CourseName = reader["CourseName"].ToString(),
                                CourseBlock = reader["CourseBlock"].ToString(),
                                CourseID = reader["CourseID"].ToString()
                            };
                            courses.Add(course);
                        }
                    }
                }
            }

            return courses;
        }

        public StudentGradeViewModel GetStudentGrade(string studentID, string courseID)
        {
            StudentGradeViewModel grade = new StudentGradeViewModel();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string sql =
                    "SELECT Grades.GradeCode, Grades.GradeComments FROM Grades WHERE StudentID = @StudentID AND CourseID = @CourseID;";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@StudentID", studentID);
                    command.Parameters.AddWithValue("@CourseID", courseID);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            grade.GradeCode = reader["GradeCode"].ToString();
                            grade.GradeComments = reader["GradeComments"].ToString();
                        }
                    }
                }
            }

            grade.GradeCode ??= "Grade Yet to be Posted";
            grade.GradeComments ??= "Comments not available";
            return grade;
        }

        public List<TeacherGradeViewModel> GetTeacherGrade(string courseID)
        {
            List<TeacherGradeViewModel> StudentList = new List<TeacherGradeViewModel>();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string sql =
                    @"
                    SELECT Students.StudentID, Students.StudentName, Grades.GradeCode, Grades.GradeComments, CourseName
                    FROM StudentCourses
                    JOIN Students ON StudentCourses.StudentID = Students.StudentID
                    JOIN dbo.Courses C on C.CourseID = StudentCourses.CourseID
                    LEFT JOIN Grades ON Grades.StudentID = StudentCourses.StudentID AND Grades.CourseID = StudentCourses.CourseID
                    WHERE StudentCourses.CourseID = @courseID;";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@CourseID", courseID);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            TeacherGradeViewModel studentGrade = new TeacherGradeViewModel()
                            {
                                CourseName = reader["CourseName"].ToString(),
                                StudentID = reader["StudentID"].ToString(),
                                StudentName = reader["StudentName"].ToString(),
                                GradeCode = reader["GradeCode"] != DBNull.Value ? reader["GradeCode"].ToString() : "",
                                GradeComments = reader["GradeComments"] != DBNull.Value
                                    ? reader["GradeComments"].ToString()
                                    : "",
                            };
                            StudentList.Add(studentGrade);
                        }
                    }
                }
            }
            return StudentList;
        }
    }
}