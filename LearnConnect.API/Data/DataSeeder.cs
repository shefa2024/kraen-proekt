using LearnConnect.API.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace LearnConnect.API.Data
{
    public static class DataSeeder
    {
        public static void SeedData(ApplicationDbContext context)
        {
            if (context.Subjects.Any())
            {
                return;   // DB has been seeded
            }

            using var transaction = context.Database.BeginTransaction();
            try
            {
                // Note: when inserting records with specific IDs that we need to reference, we must enable identity insert for SQL server if it was SQL Server.
                // But for postgres we can just insert them or reset sequences. To keep it simple, we use the ApplicationDbContext to save these entities.
                
                // We'll create objects and use AddRange instead of raw SQL to be provider agnostic.
                
                var subjects = new[]
                {
                    new Subject { Name = "Mathematics", Category = "Science" },
                    new Subject { Name = "Physics", Category = "Science" },
                    new Subject { Name = "Chemistry", Category = "Science" },
                    new Subject { Name = "Biology", Category = "Science" },
                    new Subject { Name = "English", Category = "Languages" },
                    new Subject { Name = "Spanish", Category = "Languages" },
                    new Subject { Name = "French", Category = "Languages" },
                    new Subject { Name = "Computer Science", Category = "Technology" },
                    new Subject { Name = "Programming", Category = "Technology" },
                    new Subject { Name = "History", Category = "Humanities" },
                    new Subject { Name = "Literature", Category = "Humanities" },
                    new Subject { Name = "Music", Category = "Arts" },
                    new Subject { Name = "Piano", Category = "Arts" }
                };

                context.Subjects.AddRange(subjects);
                context.SaveChanges();

                var users = new[]
                {
                    new User { Email = "admin@learnconnect.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"), FirstName = "Admin", LastName = "User", Role = UserRole.Admin, CreatedAt = DateTime.UtcNow, IsActive = true },
                    new User { Email = "student@test.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Student123!"), FirstName = "Test", LastName = "Student", Role = UserRole.Student, CreatedAt = DateTime.UtcNow, IsActive = true },
                    new User { Email = "parent@test.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Parent123!"), FirstName = "Test", LastName = "Parent", Role = UserRole.Parent, CreatedAt = DateTime.UtcNow, IsActive = true },
                    new User { Email = "sarah.johnson@learnconnect.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Teacher123!"), FirstName = "Sarah", LastName = "Johnson", Role = UserRole.Teacher, CreatedAt = DateTime.UtcNow, IsActive = true },
                    new User { Email = "michael.chen@learnconnect.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Teacher123!"), FirstName = "Michael", LastName = "Chen", Role = UserRole.Teacher, CreatedAt = DateTime.UtcNow, IsActive = true },
                    new User { Email = "emma.davis@learnconnect.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Teacher123!"), FirstName = "Emma", LastName = "Davis", Role = UserRole.Teacher, CreatedAt = DateTime.UtcNow, IsActive = true },
                    new User { Email = "david.martinez@learnconnect.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Teacher123!"), FirstName = "David", LastName = "Martinez", Role = UserRole.Teacher, CreatedAt = DateTime.UtcNow, IsActive = true },
                    new User { Email = "lisa.anderson@learnconnect.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Teacher123!"), FirstName = "Lisa", LastName = "Anderson", Role = UserRole.Teacher, CreatedAt = DateTime.UtcNow, IsActive = true },
                    new User { Email = "james.wilson@learnconnect.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Teacher123!"), FirstName = "James", LastName = "Wilson", Role = UserRole.Teacher, CreatedAt = DateTime.UtcNow, IsActive = true },
                    new User { Email = "sophia.garcia@learnconnect.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Teacher123!"), FirstName = "Sophia", LastName = "Garcia", Role = UserRole.Teacher, CreatedAt = DateTime.UtcNow, IsActive = true },
                    new User { Email = "robert.brown@learnconnect.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Teacher123!"), FirstName = "Robert", LastName = "Brown", Role = UserRole.Teacher, CreatedAt = DateTime.UtcNow, IsActive = true },
                    new User { Email = "olivia.taylor@learnconnect.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Teacher123!"), FirstName = "Olivia", LastName = "Taylor", Role = UserRole.Teacher, CreatedAt = DateTime.UtcNow, IsActive = true },
                    new User { Email = "daniel.moore@learnconnect.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Teacher123!"), FirstName = "Daniel", LastName = "Moore", Role = UserRole.Teacher, CreatedAt = DateTime.UtcNow, IsActive = true }
                };

                context.Users.AddRange(users);
                context.SaveChanges();

                var student = new Student
                {
                    UserId = users.First(u => u.Email == "student@test.com").Id,
                    PhoneNumber = "+1234567890",
                    Location = "New York, USA"
                };

                context.Students.Add(student);
                context.SaveChanges();

                var teachers = new[]
                {
                    new Teacher { UserId = users.First(u => u.Email == "sarah.johnson@learnconnect.com").Id, Bio = "Experienced mathematics teacher with 10+ years of teaching experience. Specialized in calculus and algebra.", HourlyRate = 45.00m, PhoneNumber = "+1234567891", Location = "Boston, USA", Education = "PhD in Mathematics, MIT", YearsOfExperience = 10, Languages = "English,Spanish", AverageRating = 4.8, TotalLessons = 250, MemberSince = new DateTime(2020, 1, 15, 0, 0, 0, DateTimeKind.Utc), ResponseTime = "Within 2 hours", VerificationStatus = TeacherVerificationStatus.Verified, VerifiedAt = new DateTime(2020, 1, 20, 0, 0, 0, DateTimeKind.Utc) },
                    new Teacher { UserId = users.First(u => u.Email == "michael.chen@learnconnect.com").Id, Bio = "Computer Science expert specializing in Python, Java, and web development. Former software engineer at Google.", HourlyRate = 60.00m, PhoneNumber = "+1234567892", Location = "San Francisco, USA", Education = "MS in Computer Science, Stanford University", YearsOfExperience = 8, Languages = "English,Mandarin", AverageRating = 4.9, TotalLessons = 180, MemberSince = new DateTime(2021, 3, 20, 0, 0, 0, DateTimeKind.Utc), ResponseTime = "Within 1 hour", VerificationStatus = TeacherVerificationStatus.Verified, VerifiedAt = new DateTime(2021, 3, 25, 0, 0, 0, DateTimeKind.Utc) },
                    new Teacher { UserId = users.First(u => u.Email == "emma.davis@learnconnect.com").Id, Bio = "Native English speaker with TEFL certification. Helping students improve their English communication skills.", HourlyRate = 35.00m, PhoneNumber = "+1234567893", Location = "London, UK", Education = "BA in English Literature, Oxford University", YearsOfExperience = 6, Languages = "English,French", AverageRating = 4.7, TotalLessons = 320, MemberSince = new DateTime(2019, 6, 10, 0, 0, 0, DateTimeKind.Utc), ResponseTime = "Within 3 hours", VerificationStatus = TeacherVerificationStatus.Verified, VerifiedAt = new DateTime(2019, 6, 15, 0, 0, 0, DateTimeKind.Utc) },
                    new Teacher { UserId = users.First(u => u.Email == "david.martinez@learnconnect.com").Id, Bio = "Physics enthusiast with a passion for making complex concepts simple. Specialized in mechanics and electromagnetism.", HourlyRate = 50.00m, PhoneNumber = "+1234567894", Location = "Chicago, USA", Education = "PhD in Physics, Caltech", YearsOfExperience = 12, Languages = "English,Spanish,Portuguese", AverageRating = 4.9, TotalLessons = 290, MemberSince = new DateTime(2018, 9, 5, 0, 0, 0, DateTimeKind.Utc), ResponseTime = "Within 2 hours", VerificationStatus = TeacherVerificationStatus.Verified, VerifiedAt = new DateTime(2018, 9, 10, 0, 0, 0, DateTimeKind.Utc) },
                    new Teacher { UserId = users.First(u => u.Email == "lisa.anderson@learnconnect.com").Id, Bio = "Professional pianist and music teacher. Teaching piano, music theory, and composition for all levels.", HourlyRate = 40.00m, PhoneNumber = "+1234567895", Location = "Vienna, Austria", Education = "Master of Music, Juilliard School", YearsOfExperience = 15, Languages = "English,German,Italian", AverageRating = 5.0, TotalLessons = 450, MemberSince = new DateTime(2017, 2, 14, 0, 0, 0, DateTimeKind.Utc), ResponseTime = "Within 4 hours", VerificationStatus = TeacherVerificationStatus.Verified, VerifiedAt = new DateTime(2017, 2, 20, 0, 0, 0, DateTimeKind.Utc) },
                    new Teacher { UserId = users.First(u => u.Email == "james.wilson@learnconnect.com").Id, Bio = "Chemistry teacher with expertise in organic and inorganic chemistry. Making chemistry fun and understandable!", HourlyRate = 42.00m, PhoneNumber = "+1234567896", Location = "Toronto, Canada", Education = "PhD in Chemistry, University of Toronto", YearsOfExperience = 9, Languages = "English,French", AverageRating = 4.6, TotalLessons = 210, MemberSince = new DateTime(2020, 5, 22, 0, 0, 0, DateTimeKind.Utc), ResponseTime = "Within 2 hours", VerificationStatus = TeacherVerificationStatus.Verified, VerifiedAt = new DateTime(2020, 5, 28, 0, 0, 0, DateTimeKind.Utc) },
                    new Teacher { UserId = users.First(u => u.Email == "sophia.garcia@learnconnect.com").Id, Bio = "Native Spanish speaker offering conversational Spanish lessons. Learn Spanish the natural way!", HourlyRate = 30.00m, PhoneNumber = "+1234567897", Location = "Madrid, Spain", Education = "BA in Spanish Linguistics, Universidad Complutense", YearsOfExperience = 5, Languages = "Spanish,English,Catalan", AverageRating = 4.8, TotalLessons = 380, MemberSince = new DateTime(2021, 8, 30, 0, 0, 0, DateTimeKind.Utc), ResponseTime = "Within 1 hour", VerificationStatus = TeacherVerificationStatus.Verified, VerifiedAt = new DateTime(2021, 9, 5, 0, 0, 0, DateTimeKind.Utc) },
                    new Teacher { UserId = users.First(u => u.Email == "robert.brown@learnconnect.com").Id, Bio = "History professor specializing in World War II and American history. Bringing history to life through engaging storytelling.", HourlyRate = 38.00m, PhoneNumber = "+1234567898", Location = "Washington DC, USA", Education = "PhD in History, Harvard University", YearsOfExperience = 20, Languages = "English", AverageRating = 4.7, TotalLessons = 340, MemberSince = new DateTime(2016, 11, 8, 0, 0, 0, DateTimeKind.Utc), ResponseTime = "Within 5 hours", VerificationStatus = TeacherVerificationStatus.Verified, VerifiedAt = new DateTime(2016, 11, 15, 0, 0, 0, DateTimeKind.Utc) },
                    new Teacher { UserId = users.First(u => u.Email == "olivia.taylor@learnconnect.com").Id, Bio = "Biology teacher with a focus on molecular biology and genetics. PhD researcher turned educator.", HourlyRate = 48.00m, PhoneNumber = "+1234567899", Location = "Cambridge, UK", Education = "PhD in Molecular Biology, Cambridge University", YearsOfExperience = 7, Languages = "English,French", AverageRating = 4.9, TotalLessons = 195, MemberSince = new DateTime(2021, 1, 12, 0, 0, 0, DateTimeKind.Utc), ResponseTime = "Within 3 hours", VerificationStatus = TeacherVerificationStatus.Verified, VerifiedAt = new DateTime(2021, 1, 18, 0, 0, 0, DateTimeKind.Utc) },
                    new Teacher { UserId = users.First(u => u.Email == "daniel.moore@learnconnect.com").Id, Bio = "French language expert offering lessons from beginner to advanced. Certified DELF/DALF examiner.", HourlyRate = 36.00m, PhoneNumber = "+1234567800", Location = "Paris, France", Education = "MA in French Literature, Sorbonne University", YearsOfExperience = 11, Languages = "French,English,Spanish", AverageRating = 4.8, TotalLessons = 420, MemberSince = new DateTime(2019, 4, 25, 0, 0, 0, DateTimeKind.Utc), ResponseTime = "Within 2 hours", VerificationStatus = TeacherVerificationStatus.Verified, VerifiedAt = new DateTime(2019, 4, 30, 0, 0, 0, DateTimeKind.Utc) }
                };

                context.Teachers.AddRange(teachers);
                context.SaveChanges();

                var teacherSubjects = new[]
                {
                    new TeacherSubject { TeacherId = teachers[0].Id, SubjectId = subjects.First(s => s.Name == "Mathematics").Id },
                    new TeacherSubject { TeacherId = teachers[1].Id, SubjectId = subjects.First(s => s.Name == "Computer Science").Id },
                    new TeacherSubject { TeacherId = teachers[1].Id, SubjectId = subjects.First(s => s.Name == "Programming").Id },
                    new TeacherSubject { TeacherId = teachers[2].Id, SubjectId = subjects.First(s => s.Name == "English").Id },
                    new TeacherSubject { TeacherId = teachers[2].Id, SubjectId = subjects.First(s => s.Name == "Literature").Id },
                    new TeacherSubject { TeacherId = teachers[3].Id, SubjectId = subjects.First(s => s.Name == "Physics").Id },
                    new TeacherSubject { TeacherId = teachers[4].Id, SubjectId = subjects.First(s => s.Name == "Music").Id },
                    new TeacherSubject { TeacherId = teachers[4].Id, SubjectId = subjects.First(s => s.Name == "Piano").Id },
                    new TeacherSubject { TeacherId = teachers[5].Id, SubjectId = subjects.First(s => s.Name == "Chemistry").Id },
                    new TeacherSubject { TeacherId = teachers[6].Id, SubjectId = subjects.First(s => s.Name == "Spanish").Id },
                    new TeacherSubject { TeacherId = teachers[7].Id, SubjectId = subjects.First(s => s.Name == "History").Id },
                    new TeacherSubject { TeacherId = teachers[8].Id, SubjectId = subjects.First(s => s.Name == "Biology").Id },
                    new TeacherSubject { TeacherId = teachers[9].Id, SubjectId = subjects.First(s => s.Name == "French").Id },
                    new TeacherSubject { TeacherId = teachers[9].Id, SubjectId = subjects.First(s => s.Name == "Literature").Id }
                };

                context.TeacherSubjects.AddRange(teacherSubjects);
                context.SaveChanges();

                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Console.WriteLine($"[DB] Error seeding data: {ex.Message}");
            }
        }
    }
}
