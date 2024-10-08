# VanierApp

## Project Overview
VanierApp is a web application designed to manage students, teachers, and courses at Vanier College. This app allows users to interact with course content, track enrollment, and manage user roles such as students, teachers, and administrators.

## Database Setup
To set up the VanierApp database, follow these steps:

1. **Create a new Database**: 
   Ensure that you have a new database created in your SQL Server instance to store the tables for this application.

2. **Run the following script** to create and populate the necessary tables:

```sql
-- Create the Users table
CREATE TABLE Users (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Username NVARCHAR(100) NOT NULL,
    Password NVARCHAR(255) NOT NULL,
    UserRole VARCHAR(1) NULL
);

-- Insert values into Users table
INSERT INTO Users (Username, Password, UserRole)
VALUES 
    ('admin123', 'Admin@123', 'A'),
    ('choo123', 'Choo@123', 'S'),
    ('daiana123', 'Daiana@123', 'S'),
    ('nicolas123', 'Nicolas@123', 'S'),
    ('jay123', 'Jay@123', 'T'),
    ('sylvie123', 'Sylvie@123', 'T');

-- Create the Students table with cascading update and delete
CREATE TABLE Students (
    StudentID INT PRIMARY KEY IDENTITY(1,1),
    StudentName NVARCHAR(25),
    StudentEmail NVARCHAR(30),
    UserID INT,
    FOREIGN KEY (UserID) REFERENCES Users(Id) ON DELETE CASCADE ON UPDATE CASCADE
);

-- Insert values into Students table
INSERT INTO Students (StudentName, StudentEmail, UserID)
VALUES 
    ('Choo', 'choo@vaniercollege.qc.ca', 2),
    ('Daiana', 'daiana@vaniercollege.qc.ca', 3),
    ('Nicolas', 'nicolas@vaniercollege.qc.ca', 4);

-- Create the Teachers table with cascading update and delete
CREATE TABLE Teachers (
    TeacherID INT PRIMARY KEY IDENTITY(1,1), 
    TeacherName NVARCHAR(25),
    TeacherEmail NVARCHAR(30),
    UserID INT,
    FOREIGN KEY (UserID) REFERENCES Users(Id) ON DELETE CASCADE ON UPDATE CASCADE
);

-- Insert values into Teachers table
INSERT INTO Teachers (TeacherName, TeacherEmail, UserID)
VALUES 
    ('Jay', 'jay@vaniercollege.qc.ca', 5),
    ('Sylvie', 'sylvie@vaniercollege.qc.ca', 6);

-- Create the Courses table
CREATE TABLE Courses (
    CourseID INT PRIMARY KEY IDENTITY(1,1),
    CourseName NVARCHAR(50) NOT NULL,
    CourseBlock NVARCHAR(10) NOT NULL,
    TeacherID INT,
    FOREIGN KEY (TeacherID) REFERENCES Teachers(TeacherID) ON DELETE CASCADE ON UPDATE CASCADE
);

-- Insert values into Courses table
INSERT INTO Courses (CourseName, CourseBlock)
VALUES 
    ('App Development 1', '4'),
    ('System Development', '4');

-- Create the StudentCourses table with a composite primary key and foreign key references
CREATE TABLE StudentCourses (
    StudentID INT NOT NULL,
    CourseID INT NOT NULL,
    PRIMARY KEY (StudentID, CourseID),
    FOREIGN KEY (StudentID) REFERENCES Students(StudentID),
    FOREIGN KEY (CourseID) REFERENCES Courses(CourseID)
);

-- Insert values into StudentCourses table
INSERT INTO StudentCourses (StudentID, CourseID)
VALUES
    (1, 1),
    (1, 2),
    (2, 1),
    (3, 2);

```

## New Queries to be added after Thursday's class ( 3 Oct 2024)

```sql
update Courses Set TeacherID = '1';

Create Table Grades (
	GradeID INT PRIMARY KEY IDENTITY(1,1),
	GradeCode NVARCHAR(2),
	GradeComments NVARCHAR (100),
	StudentID INT,
    CourseID INT,
	FOREIGN KEY (StudentID, CourseID) REFERENCES StudentCourses(StudentID, CourseID)
)

Insert into Grades (GradeCode, GradeComments, StudentID, CourseID) 
VALUES ('A', 'Excellent', 1, 1),
		('B', 'Good', 1, 2);
```
