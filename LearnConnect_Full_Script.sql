-- Create Database
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'LearnConnectDB')
BEGIN
    CREATE DATABASE LearnConnectDB;
END
GO

USE LearnConnectDB;
GO

-- Drop foreign keys first to avoid errors when dropping tables
DECLARE @sql NVARCHAR(MAX) = N'';
SELECT @sql += N'ALTER TABLE ' + QUOTENAME(SCHEMA_NAME(schema_id)) + N'.' + QUOTENAME(OBJECT_NAME(parent_object_id)) + 
               N' DROP CONSTRAINT ' + QUOTENAME(name) + N';' + CHAR(13)
FROM sys.foreign_keys;
EXEC sp_executesql @sql;
GO

-- Drop tables if they exist
IF OBJECT_ID('dbo.Messages', 'U') IS NOT NULL DROP TABLE dbo.Messages;
IF OBJECT_ID('dbo.Reviews', 'U') IS NOT NULL DROP TABLE dbo.Reviews;
IF OBJECT_ID('dbo.Reservations', 'U') IS NOT NULL DROP TABLE dbo.Reservations;
IF OBJECT_ID('dbo.Lessons', 'U') IS NOT NULL DROP TABLE dbo.Lessons;
IF OBJECT_ID('dbo.Schedules', 'U') IS NOT NULL DROP TABLE dbo.Schedules;
IF OBJECT_ID('dbo.TeacherSubjects', 'U') IS NOT NULL DROP TABLE dbo.TeacherSubjects;
IF OBJECT_ID('dbo.Subjects', 'U') IS NOT NULL DROP TABLE dbo.Subjects;
IF OBJECT_ID('dbo.Teachers', 'U') IS NOT NULL DROP TABLE dbo.Teachers;
IF OBJECT_ID('dbo.Students', 'U') IS NOT NULL DROP TABLE dbo.Students;
IF OBJECT_ID('dbo.Users', 'U') IS NOT NULL DROP TABLE dbo.Users;
GO

-- 1. Users Table
CREATE TABLE Users (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Email NVARCHAR(255) NOT NULL,
    PasswordHash NVARCHAR(MAX) NOT NULL,
    FirstName NVARCHAR(100) NOT NULL,
    LastName NVARCHAR(100) NOT NULL,
    Role INT NOT NULL, -- 0:Student, 1:Teacher, 2:Admin, 3:Parent
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    IsActive BIT NOT NULL DEFAULT 1
);
CREATE UNIQUE INDEX IX_Users_Email ON Users(Email);
GO

-- 2. Students Table
CREATE TABLE Students (
    Id INT PRIMARY KEY IDENTITY(1,1),
    UserId INT NOT NULL,
    PhoneNumber NVARCHAR(MAX) NULL,
    Location NVARCHAR(MAX) NULL,
    DateOfBirth DATETIME2 NULL,
    CONSTRAINT FK_Students_Users FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE
);
CREATE UNIQUE INDEX IX_Students_UserId ON Students(UserId);
GO

-- 3. Teachers Table
CREATE TABLE Teachers (
    Id INT PRIMARY KEY IDENTITY(1,1),
    UserId INT NOT NULL,
    Bio NVARCHAR(MAX) NOT NULL DEFAULT '',
    HourlyRate DECIMAL(18,2) NOT NULL,
    PhoneNumber NVARCHAR(MAX) NULL,
    Location NVARCHAR(MAX) NULL,
    Education NVARCHAR(MAX) NULL,
    YearsOfExperience INT NOT NULL,
    Languages NVARCHAR(MAX) NULL,
    ProfileImageUrl NVARCHAR(MAX) NULL,
    AverageRating FLOAT NOT NULL DEFAULT 0,
    TotalLessons INT NOT NULL DEFAULT 0,
    MemberSince DATETIME2 NULL,
    ResponseTime NVARCHAR(MAX) DEFAULT 'Within 2 hours',
    CONSTRAINT FK_Teachers_Users FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE
);
CREATE UNIQUE INDEX IX_Teachers_UserId ON Teachers(UserId);
GO

-- 4. Subjects Table
CREATE TABLE Subjects (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(MAX) NOT NULL DEFAULT '',
    Description NVARCHAR(MAX) NULL,
    Category NVARCHAR(MAX) NULL
);
GO

-- 5. TeacherSubjects Table
CREATE TABLE TeacherSubjects (
    Id INT PRIMARY KEY IDENTITY(1,1),
    TeacherId INT NOT NULL,
    SubjectId INT NOT NULL,
    CONSTRAINT FK_TeacherSubjects_Teachers FOREIGN KEY (TeacherId) REFERENCES Teachers(Id) ON DELETE CASCADE,
    CONSTRAINT FK_TeacherSubjects_Subjects FOREIGN KEY (SubjectId) REFERENCES Subjects(Id) ON DELETE CASCADE
);
CREATE INDEX IX_TeacherSubjects_TeacherId ON TeacherSubjects(TeacherId);
CREATE INDEX IX_TeacherSubjects_SubjectId ON TeacherSubjects(SubjectId);
GO

-- 6. Lessons Table
CREATE TABLE Lessons (
    Id INT PRIMARY KEY IDENTITY(1,1),
    TeacherId INT NOT NULL,
    StudentId INT NOT NULL,
    SubjectId INT NULL,
    ScheduledDateTime DATETIME2 NOT NULL,
    DurationMinutes INT NOT NULL DEFAULT 60,
    Status INT NOT NULL DEFAULT 0, -- 0:Scheduled, 1:InProgress, 2:Completed, 3:Cancelled
    Notes NVARCHAR(MAX) NULL,
    MeetingLink NVARCHAR(MAX) NULL,
    Price DECIMAL(18,2) NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CompletedAt DATETIME2 NULL,
    CONSTRAINT FK_Lessons_Teachers FOREIGN KEY (TeacherId) REFERENCES Teachers(Id) ON DELETE NO ACTION, -- Restrict
    CONSTRAINT FK_Lessons_Students FOREIGN KEY (StudentId) REFERENCES Students(Id) ON DELETE NO ACTION, -- Restrict
    CONSTRAINT FK_Lessons_Subjects FOREIGN KEY (SubjectId) REFERENCES Subjects(Id)
);
CREATE INDEX IX_Lessons_TeacherId ON Lessons(TeacherId);
CREATE INDEX IX_Lessons_StudentId ON Lessons(StudentId);
GO

-- 7. Reservations Table
CREATE TABLE Reservations (
    Id INT PRIMARY KEY IDENTITY(1,1),
    StudentId INT NOT NULL,
    TeacherId INT NOT NULL,
    LessonId INT NULL,
    RequestedDateTime DATETIME2 NOT NULL,
    DurationMinutes INT NOT NULL DEFAULT 60,
    Status INT NOT NULL DEFAULT 0, -- 0:Pending, 1:Confirmed, 2:Cancelled, 3:Completed
    Message NVARCHAR(MAX) NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    ConfirmedAt DATETIME2 NULL,
    CONSTRAINT FK_Reservations_Students FOREIGN KEY (StudentId) REFERENCES Students(Id) ON DELETE NO ACTION, -- Restrict
    CONSTRAINT FK_Reservations_Teachers FOREIGN KEY (TeacherId) REFERENCES Teachers(Id) ON DELETE NO ACTION, -- Restrict
    CONSTRAINT FK_Reservations_Lessons FOREIGN KEY (LessonId) REFERENCES Lessons(Id)
);
CREATE INDEX IX_Reservations_StudentId ON Reservations(StudentId);
CREATE INDEX IX_Reservations_TeacherId ON Reservations(TeacherId);
GO

-- 8. Reviews Table
CREATE TABLE Reviews (
    Id INT PRIMARY KEY IDENTITY(1,1),
    StudentId INT NOT NULL,
    TeacherId INT NOT NULL,
    LessonId INT NULL,
    Rating INT NOT NULL,
    Comment NVARCHAR(MAX) NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT FK_Reviews_Students FOREIGN KEY (StudentId) REFERENCES Students(Id) ON DELETE NO ACTION, -- Restrict
    CONSTRAINT FK_Reviews_Teachers FOREIGN KEY (TeacherId) REFERENCES Teachers(Id) ON DELETE NO ACTION, -- Restrict
    CONSTRAINT FK_Reviews_Lessons FOREIGN KEY (LessonId) REFERENCES Lessons(Id)
);
CREATE INDEX IX_Reviews_StudentId ON Reviews(StudentId);
CREATE INDEX IX_Reviews_TeacherId ON Reviews(TeacherId);
GO

-- 9. Messages Table
CREATE TABLE Messages (
    Id INT PRIMARY KEY IDENTITY(1,1),
    SenderId INT NOT NULL,
    ReceiverId INT NOT NULL,
    Content NVARCHAR(MAX) NOT NULL DEFAULT '',
    SentAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    IsRead BIT NOT NULL DEFAULT 0,
    ReadAt DATETIME2 NULL,
    CONSTRAINT FK_Messages_Sender FOREIGN KEY (SenderId) REFERENCES Users(Id) ON DELETE NO ACTION, -- Restrict
    CONSTRAINT FK_Messages_Receiver FOREIGN KEY (ReceiverId) REFERENCES Users(Id) ON DELETE NO ACTION -- Restrict
);
CREATE INDEX IX_Messages_SenderId ON Messages(SenderId);
CREATE INDEX IX_Messages_ReceiverId ON Messages(ReceiverId);
GO

-- 10. Schedules Table
CREATE TABLE Schedules (
    Id INT PRIMARY KEY IDENTITY(1,1),
    TeacherId INT NOT NULL,
    DayOfWeek INT NOT NULL,
    StartTime TIME NOT NULL,
    EndTime TIME NOT NULL,
    IsAvailable BIT NOT NULL DEFAULT 1,
    CONSTRAINT FK_Schedules_Teachers FOREIGN KEY (TeacherId) REFERENCES Teachers(Id) ON DELETE CASCADE
);
CREATE INDEX IX_Schedules_TeacherId ON Schedules(TeacherId);
GO

-- =============================================
-- SEED DATA
-- =============================================
SET IDENTITY_INSERT Users ON;
GO

INSERT INTO Users (Id, Email, PasswordHash, FirstName, LastName, Role, CreatedAt, IsActive) VALUES
(1, 'admin@learnconnect.com', '$2a$11$Z5...', 'Admin', 'User', 2, GETUTCDATE(), 1),
(2, 'student@test.com', '$2a$11$Z5...', 'Test', 'Student', 0, GETUTCDATE(), 1),
(3, 'sarah.johnson@learnconnect.com', '$2a$11$Z5...', 'Sarah', 'Johnson', 1, GETUTCDATE(), 1),
(4, 'michael.chen@learnconnect.com', '$2a$11$Z5...', 'Michael', 'Chen', 1, GETUTCDATE(), 1),
(5, 'emma.davis@learnconnect.com', '$2a$11$Z5...', 'Emma', 'Davis', 1, GETUTCDATE(), 1),
(6, 'david.martinez@learnconnect.com', '$2a$11$Z5...', 'David', 'Martinez', 1, GETUTCDATE(), 1),
(7, 'lisa.anderson@learnconnect.com', '$2a$11$Z5...', 'Lisa', 'Anderson', 1, GETUTCDATE(), 1),
(8, 'james.wilson@learnconnect.com', '$2a$11$Z5...', 'James', 'Wilson', 1, GETUTCDATE(), 1),
(9, 'sophia.garcia@learnconnect.com', '$2a$11$Z5...', 'Sophia', 'Garcia', 1, GETUTCDATE(), 1),
(10, 'robert.brown@learnconnect.com', '$2a$11$Z5...', 'Robert', 'Brown', 1, GETUTCDATE(), 1),
(11, 'olivia.taylor@learnconnect.com', '$2a$11$Z5...', 'Olivia', 'Taylor', 1, GETUTCDATE(), 1),
(12, 'daniel.moore@learnconnect.com', '$2a$11$Z5...', 'Daniel', 'Moore', 1, GETUTCDATE(), 1),
(20, 'alice.walker@learnconnect.com', '$2a$11$Z5...', 'Alice', 'Walker', 1, GETUTCDATE(), 1),
(21, 'robert.vance@learnconnect.com', '$2a$11$Z5...', 'Robert', 'Vance', 1, GETUTCDATE(), 1),
(22, 'john.doe@learnconnect.com', '$2a$11$Z5...', 'John', 'Doe', 1, GETUTCDATE(), 1),
(23, 'marie.curie@learnconnect.com', '$2a$11$Z5...', 'Marie', 'Curie', 1, GETUTCDATE(), 1),
(24, 'walter.white@learnconnect.com', '$2a$11$Z5...', 'Walter', 'White', 1, GETUTCDATE(), 1),
(25, 'heisenberg@learnconnect.com', '$2a$11$Z5...', 'Werner', 'Heisenberg', 1, GETUTCDATE(), 1),
(26, 'jane.goodall@learnconnect.com', '$2a$11$Z5...', 'Jane', 'Goodall', 1, GETUTCDATE(), 1),
(27, 'gregor.mendel@learnconnect.com', '$2a$11$Z5...', 'Gregor', 'Mendel', 1, GETUTCDATE(), 1),
(28, 'new.grad@learnconnect.com', '$2a$11$Z5...', 'Emily', 'Dickinson', 1, GETUTCDATE(), 1),
(29, 'shakespeare@learnconnect.com', '$2a$11$Z5...', 'William', 'Shakespeare', 1, GETUTCDATE(), 1),
(30, 'carlos.ruiz@learnconnect.com', '$2a$11$Z5...', 'Carlos', 'Ruiz', 1, GETUTCDATE(), 1),
(31, 'isabela.madrigal@learnconnect.com', '$2a$11$Z5...', 'Isabela', 'Madrigal', 1, GETUTCDATE(), 1),
(32, 'pierre.escargot@learnconnect.com', '$2a$11$Z5...', 'Pierre', 'Escargot', 1, GETUTCDATE(), 1),
(33, 'chef.gusteau@learnconnect.com', '$2a$11$Z5...', 'Auguste', 'Gusteau', 1, GETUTCDATE(), 1),
(34, 'script.kiddie@learnconnect.com', '$2a$11$Z5...', 'Kevin', 'Mitnick', 1, GETUTCDATE(), 1),
(35, 'dev.ops@learnconnect.com', '$2a$11$Z5...', 'Linus', 'Torvalds', 1, GETUTCDATE(), 1),
(36, 'ai.researcher@learnconnect.com', '$2a$11$Z5...', 'Ada', 'Lovelace', 1, GETUTCDATE(), 1),
(37, 'time.traveler@learnconnect.com', '$2a$11$Z5...', 'Marty', 'McFly', 1, GETUTCDATE(), 1),
(38, 'museum.guide@learnconnect.com', '$2a$11$Z5...', 'Indiana', 'Jones', 1, GETUTCDATE(), 1),
(39, 'book.worm@learnconnect.com', '$2a$11$Z5...', 'Hermione', 'Granger', 1, GETUTCDATE(), 1),
(40, 'published.author@learnconnect.com', '$2a$11$Z5...', 'J.K.', 'Rowling', 1, GETUTCDATE(), 1),
(41, 'street.performer@learnconnect.com', '$2a$11$Z5...', 'Ed', 'Sheeran', 1, GETUTCDATE(), 1),
(42, 'concert.pianist@learnconnect.com', '$2a$11$Z5...', 'Ludwig', 'Beethoven', 1, GETUTCDATE(), 1),
(50, 'parent@test.com', '$2a$11$Z5...', 'Test', 'Parent', 3, GETUTCDATE(), 1);

SET IDENTITY_INSERT Users OFF;
GO

SET IDENTITY_INSERT Subjects ON;
GO

INSERT INTO Subjects (Id, Name, Category) VALUES
(1, 'Mathematics', 'Science'),
(2, 'Physics', 'Science'),
(3, 'Chemistry', 'Science'),
(4, 'Biology', 'Science'),
(5, 'English', 'Languages'),
(6, 'Spanish', 'Languages'),
(7, 'French', 'Languages'),
(8, 'Computer Science', 'Technology'),
(9, 'Programming', 'Technology'),
(10, 'History', 'Humanities'),
(11, 'Literature', 'Humanities'),
(12, 'Music', 'Arts'),
(13, 'Piano', 'Arts');

SET IDENTITY_INSERT Subjects OFF;
GO

SET IDENTITY_INSERT Students ON;
GO

INSERT INTO Students (Id, UserId, PhoneNumber, Location) VALUES
(1, 2, '+1234567890', 'New York, USA');

SET IDENTITY_INSERT Students OFF;
GO

SET IDENTITY_INSERT Teachers ON;
GO

INSERT INTO Teachers (Id, UserId, Bio, HourlyRate, PhoneNumber, Location, Education, YearsOfExperience, Languages, AverageRating, TotalLessons, MemberSince, ResponseTime) VALUES
(1, 3, 'Experienced mathematics teacher with 10+ years of teaching experience. Specialized in calculus and algebra.', 45.00, '+1234567891', 'Boston, USA', 'PhD in Mathematics, MIT', 10, 'English,Spanish', 4.8, 250, '2020-01-15', 'Within 2 hours'),
(2, 4, 'Computer Science expert specializing in Python, Java, and web development. Former software engineer at Google.', 60.00, '+1234567892', 'San Francisco, USA', 'MS in Computer Science, Stanford University', 8, 'English,Mandarin', 4.9, 180, '2021-03-20', 'Within 1 hour'),
(3, 5, 'Native English speaker with TEFL certification. Helping students improve their English communication skills.', 35.00, '+1234567893', 'London, UK', 'BA in English Literature, Oxford University', 6, 'English,French', 4.7, 320, '2019-06-10', 'Within 3 hours'),
(4, 6, 'Physics enthusiast with a passion for making complex concepts simple. Specialized in mechanics and electromagnetism.', 50.00, '+1234567894', 'Chicago, USA', 'PhD in Physics, Caltech', 12, 'English,Spanish,Portuguese', 4.9, 290, '2018-09-05', 'Within 2 hours'),
(5, 7, 'Professional pianist and music teacher. Teaching piano, music theory, and composition for all levels.', 40.00, '+1234567895', 'Vienna, Austria', 'Master of Music, Juilliard School', 15, 'English,German,Italian', 5.0, 450, '2017-02-14', 'Within 4 hours'),
(6, 8, 'Chemistry teacher with expertise in organic and inorganic chemistry. Making chemistry fun and understandable!', 42.00, '+1234567896', 'Toronto, Canada', 'PhD in Chemistry, University of Toronto', 9, 'English,French', 4.6, 210, '2020-05-22', 'Within 2 hours'),
(7, 9, 'Native Spanish speaker offering conversational Spanish lessons. Learn Spanish the natural way!', 30.00, '+1234567897', 'Madrid, Spain', 'BA in Spanish Linguistics, Universidad Complutense', 5, 'Spanish,English,Catalan', 4.8, 380, '2021-08-30', 'Within 1 hour'),
(8, 10, 'History professor specializing in World War II and American history. Bringing history to life through engaging storytelling.', 38.00, '+1234567898', 'Washington DC, USA', 'PhD in History, Harvard University', 20, 'English', 4.7, 340, '2016-11-08', 'Within 5 hours'),
(9, 11, 'Biology teacher with a focus on molecular biology and genetics. PhD researcher turned educator.', 48.00, '+1234567899', 'Cambridge, UK', 'PhD in Molecular Biology, Cambridge University', 7, 'English,French', 4.9, 195, '2021-01-12', 'Within 3 hours'),
(10, 12, 'French language expert offering lessons from beginner to advanced. Certified DELF/DALF examiner.', 36.00, '+1234567800', 'Paris, France', 'MA in French Literature, Sorbonne University', 11, 'French,English,Spanish', 4.8, 420, '2019-04-25', 'Within 2 hours'),
(20, 20, 'Math enthusiast helping students love numbers.', 25.00, NULL, 'Online', 'BS Math', 2, 'English', 4.5, 50, DATEADD(month, -6, GETUTCDATE()), 'Within 1 hour'),
(21, 21, 'Advanced mathematics for serious students.', 75.00, NULL, 'New York', 'PhD Math', 20, 'English', 5.0, 500, DATEADD(year, -5, GETUTCDATE()), 'Within 12 hours'),
(22, 22, 'Physics made simple and fun.', 28.00, NULL, 'Online', 'BS Physics', 3, 'English', 4.6, 80, DATEADD(month, -8, GETUTCDATE()), 'Within 2 hours'),
(23, 23, 'Expert physics tutoring for university level.', 70.00, NULL, 'Paris', 'PhD Physics', 15, 'English,French', 4.9, 300, DATEADD(year, -3, GETUTCDATE()), 'Within 6 hours'),
(24, 24, 'High school chemistry support.', 30.00, NULL, 'Online', 'BS Chemistry', 4, 'English', 4.7, 120, DATEADD(year, -1, GETUTCDATE()), 'Within 3 hours'),
(25, 25, 'Advanced organic chemistry and lab prep.', 80.00, NULL, 'Berlin', 'PhD Chemistry', 25, 'English,German', 5.0, 600, DATEADD(year, -6, GETUTCDATE()), 'Within 24 hours'),
(26, 26, 'Biology basics for everyone.', 22.00, NULL, 'Online', 'BS Biology', 1, 'English', 4.4, 30, DATEADD(month, -3, GETUTCDATE()), 'Within 1 hour'),
(27, 27, 'Genetics and evolutionary biology expert.', 45.00, NULL, 'Vienna', 'MS Biology', 8, 'English,German', 4.8, 200, DATEADD(year, -2, GETUTCDATE()), 'Within 4 hours'),
(28, 28, 'English conversation and grammar.', 20.00, NULL, 'Online', 'BA English', 1, 'English', 4.3, 40, DATEADD(month, -4, GETUTCDATE()), 'Within 1 hour'),
(29, 29, 'Literature analysis and creative writing.', 65.00, NULL, 'London', 'MFA Writing', 12, 'English', 4.9, 350, DATEADD(year, -4, GETUTCDATE()), 'Within 5 hours'),
(30, 30, 'Spanish for travel and business.', 40.00, NULL, 'Barcelona', 'BA Spanish', 6, 'Spanish,English', 4.7, 150, DATEADD(year, -2, GETUTCDATE()), 'Within 2 hours'),
(31, 31, 'Native Spanish speaker, advanced levels.', 60.00, NULL, 'Madrid', 'MA Linguistics', 10, 'Spanish,English', 4.9, 280, DATEADD(year, -3, GETUTCDATE()), 'Within 3 hours'),
(32, 32, 'Learn French basics quickly.', 25.00, NULL, 'Online', 'BA French', 2, 'French,English', 4.5, 60, DATEADD(month, -7, GETUTCDATE()), 'Within 1 hour'),
(33, 33, 'Master French cuisine and language.', 70.00, NULL, 'Paris', 'Culinary Arts', 15, 'French,English', 5.0, 400, DATEADD(year, -5, GETUTCDATE()), 'Within 8 hours'),
(34, 34, 'Intro to coding and cybersecurity.', 25.00, NULL, 'Online', 'Self-taught', 3, 'English', 4.6, 90, DATEADD(year, -1, GETUTCDATE()), 'Within 2 hours'),
(35, 35, 'DevOps, Linux, and System Admin.', 55.00, NULL, 'Helsinki', 'MS CS', 10, 'English,Finnish', 4.8, 220, DATEADD(year, -3, GETUTCDATE()), 'Within 4 hours'),
(36, 36, 'Artificial Intelligence and Machine Learning.', 90.00, NULL, 'San Francisco', 'PhD CS', 12, 'English', 5.0, 180, DATEADD(year, -2, GETUTCDATE()), 'Within 12 hours'),
(37, 37, 'History through the ages.', 20.00, NULL, 'Online', 'BA History', 2, 'English', 4.4, 45, DATEADD(month, -5, GETUTCDATE()), 'Within 1 hour'),
(38, 38, 'Archaeology and ancient civilizations.', 40.00, NULL, 'Cairo', 'PhD Archaeology', 15, 'English', 4.8, 300, DATEADD(year, -4, GETUTCDATE()), 'Within 6 hours'),
(39, 39, 'Reading comprehension and book clubs.', 22.00, NULL, 'Online', 'BA Lit', 3, 'English', 4.7, 100, DATEADD(year, -1, GETUTCDATE()), 'Within 2 hours'),
(40, 40, 'Creative writing masterclass.', 65.00, NULL, 'Edinburgh', 'BA Classics', 20, 'English', 4.9, 500, DATEADD(year, -6, GETUTCDATE()), 'Within 24 hours'),
(41, 41, 'Guitar and pop music basics.', 25.00, NULL, 'London', 'Self-taught', 5, 'English', 4.8, 150, DATEADD(year, -2, GETUTCDATE()), 'Within 2 hours'),
(42, 42, 'Classical piano and composition.', 85.00, NULL, 'Vienna', 'Conservatory', 30, 'German,English', 5.0, 800, DATEADD(year, -8, GETUTCDATE()), 'Within 12 hours');

SET IDENTITY_INSERT Teachers OFF;
GO

SET IDENTITY_INSERT TeacherSubjects ON;
GO

INSERT INTO TeacherSubjects (Id, TeacherId, SubjectId) VALUES
(1, 1, 1),
(2, 2, 8),
(3, 2, 9),
(4, 3, 5),
(5, 3, 11),
(6, 4, 2),
(7, 5, 12),
(8, 5, 13),
(9, 6, 3),
(10, 7, 6),
(11, 8, 10),
(12, 9, 4),
(13, 10, 7),
(14, 10, 11),
(20, 20, 1),
(21, 21, 1),
(22, 22, 2),
(23, 23, 2),
(24, 24, 3),
(25, 25, 3),
(26, 26, 4),
(27, 27, 4),
(28, 28, 5),
(29, 29, 5),
(30, 30, 6),
(31, 31, 6),
(32, 32, 7),
(33, 33, 7),
(34, 34, 8),
(35, 34, 9),
(36, 35, 8),
(37, 35, 9),
(38, 36, 8),
(39, 36, 9),
(40, 37, 10),
(41, 38, 10),
(42, 39, 11),
(43, 40, 11),
(44, 41, 12),
(45, 41, 13),
(46, 42, 12),
(47, 42, 13);

SET IDENTITY_INSERT TeacherSubjects OFF;
GO
