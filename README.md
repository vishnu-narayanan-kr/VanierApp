# VanierApp

Create a new Database and add the following script
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
    ('diana123', 'Diana@123', 'S'),
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
    ('Diana', 'diana@vaniercollege.qc.ca', 3),
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

CREATE TABLE Courses (
    CourseID INT PRIMARY KEY IDENTITY(1,1),
    CourseName NVARCHAR(50) NOT NULL,
    CourseBlock NVARCHAR(10) NOT NULL,
	TeacherID INT,
	FOREIGN KEY (TeacherID) REFERENCES Teachers(TeacherID) ON DELETE CASCADE ON UPDATE CASCADE
);


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

Insert into StudentCourses (StudentID, CourseID)
VALUES
	('1', '1'),
	('1', '2'),
	('2', '1'),
	('3', '2');

