using Microsoft.EntityFrameworkCore;
using LearnConnect.API.Models;

namespace LearnConnect.API.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Student> Students { get; set; }
    public DbSet<Teacher> Teachers { get; set; }
    public DbSet<Subject> Subjects { get; set; }
    public DbSet<TeacherSubject> TeacherSubjects { get; set; }
    public DbSet<Lesson> Lessons { get; set; }
    public DbSet<Reservation> Reservations { get; set; }
    public DbSet<Schedule> Schedules { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<LessonNotebook> LessonNotebooks { get; set; }
    public DbSet<ParentMeetingRequest> ParentMeetingRequests { get; set; }
    public DbSet<StudentBadge> StudentBadges { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<LessonPackage> LessonPackages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
            
            entity.HasOne(e => e.Student)
                .WithOne(s => s.User)
                .HasForeignKey<Student>(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasOne(e => e.Teacher)
                .WithOne(t => t.User)
                .HasForeignKey<Teacher>(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Teacher configuration
        modelBuilder.Entity<Teacher>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.HourlyRate).HasColumnType("decimal(18,2)");
            entity.Property(e => e.VerificationStatus)
                  .HasConversion<string>()
                  .HasMaxLength(20);
            // IsVerified is a computed property, not stored
            entity.Ignore(e => e.IsVerified);
        });

        // Lesson configuration
        modelBuilder.Entity<Lesson>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
            
            entity.HasOne(e => e.Teacher)
                .WithMany(t => t.Lessons)
                .HasForeignKey(e => e.TeacherId)
                .OnDelete(DeleteBehavior.Restrict);
                
            entity.HasOne(e => e.Student)
                .WithMany()
                .HasForeignKey(e => e.StudentId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Reservation configuration
        modelBuilder.Entity<Reservation>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.HasOne(e => e.Student)
                .WithMany(s => s.Reservations)
                .HasForeignKey(e => e.StudentId)
                .OnDelete(DeleteBehavior.Restrict);
                
            entity.HasOne(e => e.Teacher)
                .WithMany()
                .HasForeignKey(e => e.TeacherId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Review configuration
        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.HasOne(e => e.Student)
                .WithMany(s => s.Reviews)
                .HasForeignKey(e => e.StudentId)
                .OnDelete(DeleteBehavior.Restrict);
                
            entity.HasOne(e => e.Teacher)
                .WithMany(t => t.Reviews)
                .HasForeignKey(e => e.TeacherId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // TeacherSubject configuration
        modelBuilder.Entity<TeacherSubject>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.HasOne(e => e.Teacher)
                .WithMany(t => t.TeacherSubjects)
                .HasForeignKey(e => e.TeacherId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasOne(e => e.Subject)
                .WithMany(s => s.TeacherSubjects)
                .HasForeignKey(e => e.SubjectId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Message configuration
        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.HasOne(e => e.Sender)
                .WithMany()
                .HasForeignKey(e => e.SenderId)
                .OnDelete(DeleteBehavior.Restrict);
                
            entity.HasOne(e => e.Receiver)
                .WithMany()
                .HasForeignKey(e => e.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // LessonNotebook configuration
        modelBuilder.Entity<LessonNotebook>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Student)
                .WithMany()
                .HasForeignKey(e => e.StudentId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.Teacher)
                .WithMany()
                .HasForeignKey(e => e.TeacherId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // ParentMeetingRequest configuration
        modelBuilder.Entity<ParentMeetingRequest>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Parent)
                .WithMany()
                .HasForeignKey(e => e.ParentId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.Teacher)
                .WithMany()
                .HasForeignKey(e => e.TeacherId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.Student)
                .WithMany()
                .HasForeignKey(e => e.StudentId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // StudentBadge configuration
        modelBuilder.Entity<StudentBadge>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Student)
                .WithMany(s => s.Badges)
                .HasForeignKey(e => e.StudentId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Payment configuration
        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");
            
            entity.HasOne(e => e.Student)
                .WithMany()
                .HasForeignKey(e => e.StudentId)
                .OnDelete(DeleteBehavior.Restrict);
                
            entity.HasOne(e => e.Teacher)
                .WithMany()
                .HasForeignKey(e => e.TeacherId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // LessonPackage configuration
        modelBuilder.Entity<LessonPackage>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TotalPrice).HasColumnType("decimal(18,2)");
            
            entity.HasOne(e => e.Student)
                .WithMany()
                .HasForeignKey(e => e.StudentId)
                .OnDelete(DeleteBehavior.Restrict);
                
            entity.HasOne(e => e.Teacher)
                .WithMany()
                .HasForeignKey(e => e.TeacherId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Seed initial data
        SeedData(modelBuilder);
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        // Seed Subjects
        modelBuilder.Entity<Subject>().HasData(
            new Subject { Id = 1, Name = "Mathematics", Category = "Science" },
            new Subject { Id = 2, Name = "Physics", Category = "Science" },
            new Subject { Id = 3, Name = "Chemistry", Category = "Science" },
            new Subject { Id = 4, Name = "Biology", Category = "Science" },
            new Subject { Id = 5, Name = "English", Category = "Languages" },
            new Subject { Id = 6, Name = "Spanish", Category = "Languages" },
            new Subject { Id = 7, Name = "French", Category = "Languages" },
            new Subject { Id = 8, Name = "Computer Science", Category = "Technology" },
            new Subject { Id = 9, Name = "Programming", Category = "Technology" },
            new Subject { Id = 10, Name = "History", Category = "Humanities" },
            new Subject { Id = 11, Name = "Literature", Category = "Humanities" },
            new Subject { Id = 12, Name = "Music", Category = "Arts" },
            new Subject { Id = 13, Name = "Piano", Category = "Arts" }
        );

        // Seed Admin User
        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = 1,
                Email = "admin@learnconnect.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                FirstName = "Admin",
                LastName = "User",
                Role = UserRole.Admin,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            },
            // Test Student Account
            new User
            {
                Id = 2,
                Email = "student@test.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Student123!"),
                FirstName = "Test",
                LastName = "Student",
                Role = UserRole.Student,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            },
            // Test Parent Account
            new User
            {
                Id = 50,
                Email = "parent@test.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Parent123!"),
                FirstName = "Test",
                LastName = "Parent",
                Role = UserRole.Parent,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            },
            // Teacher Users
            new User { Id = 3, Email = "sarah.johnson@learnconnect.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Teacher123!"), FirstName = "Sarah", LastName = "Johnson", Role = UserRole.Teacher, CreatedAt = DateTime.UtcNow, IsActive = true },
            new User { Id = 4, Email = "michael.chen@learnconnect.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Teacher123!"), FirstName = "Michael", LastName = "Chen", Role = UserRole.Teacher, CreatedAt = DateTime.UtcNow, IsActive = true },
            new User { Id = 5, Email = "emma.davis@learnconnect.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Teacher123!"), FirstName = "Emma", LastName = "Davis", Role = UserRole.Teacher, CreatedAt = DateTime.UtcNow, IsActive = true },
            new User { Id = 6, Email = "david.martinez@learnconnect.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Teacher123!"), FirstName = "David", LastName = "Martinez", Role = UserRole.Teacher, CreatedAt = DateTime.UtcNow, IsActive = true },
            new User { Id = 7, Email = "lisa.anderson@learnconnect.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Teacher123!"), FirstName = "Lisa", LastName = "Anderson", Role = UserRole.Teacher, CreatedAt = DateTime.UtcNow, IsActive = true },
            new User { Id = 8, Email = "james.wilson@learnconnect.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Teacher123!"), FirstName = "James", LastName = "Wilson", Role = UserRole.Teacher, CreatedAt = DateTime.UtcNow, IsActive = true },
            new User { Id = 9, Email = "sophia.garcia@learnconnect.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Teacher123!"), FirstName = "Sophia", LastName = "Garcia", Role = UserRole.Teacher, CreatedAt = DateTime.UtcNow, IsActive = true },
            new User { Id = 10, Email = "robert.brown@learnconnect.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Teacher123!"), FirstName = "Robert", LastName = "Brown", Role = UserRole.Teacher, CreatedAt = DateTime.UtcNow, IsActive = true },
            new User { Id = 11, Email = "olivia.taylor@learnconnect.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Teacher123!"), FirstName = "Olivia", LastName = "Taylor", Role = UserRole.Teacher, CreatedAt = DateTime.UtcNow, IsActive = true },
            new User { Id = 12, Email = "daniel.moore@learnconnect.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Teacher123!"), FirstName = "Daniel", LastName = "Moore", Role = UserRole.Teacher, CreatedAt = DateTime.UtcNow, IsActive = true }
        );

        // Seed Student Profile
        modelBuilder.Entity<Student>().HasData(
            new Student
            {
                Id = 1,
                UserId = 2,
                PhoneNumber = "+1234567890",
                Location = "New York, USA"
            }
        );

        // Seed Teacher Profiles
        modelBuilder.Entity<Teacher>().HasData(
            new Teacher
            {
                Id = 1,
                UserId = 3,
                Bio = "Experienced mathematics teacher with 10+ years of teaching experience. Specialized in calculus and algebra.",
                HourlyRate = 45.00m,
                PhoneNumber = "+1234567891",
                Location = "Boston, USA",
                Education = "PhD in Mathematics, MIT",
                YearsOfExperience = 10,
                Languages = "English,Spanish",
                AverageRating = 4.8,
                TotalLessons = 250,
                MemberSince = new DateTime(2020, 1, 15),
                ResponseTime = "Within 2 hours",
                VerificationStatus = TeacherVerificationStatus.Verified,
                VerifiedAt = new DateTime(2020, 1, 20)
            },
            new Teacher
            {
                Id = 2,
                UserId = 4,
                Bio = "Computer Science expert specializing in Python, Java, and web development. Former software engineer at Google.",
                HourlyRate = 60.00m,
                PhoneNumber = "+1234567892",
                Location = "San Francisco, USA",
                Education = "MS in Computer Science, Stanford University",
                YearsOfExperience = 8,
                Languages = "English,Mandarin",
                AverageRating = 4.9,
                TotalLessons = 180,
                MemberSince = new DateTime(2021, 3, 20),
                ResponseTime = "Within 1 hour",
                VerificationStatus = TeacherVerificationStatus.Verified,
                VerifiedAt = new DateTime(2021, 3, 25)
            },
            new Teacher
            {
                Id = 3,
                UserId = 5,
                Bio = "Native English speaker with TEFL certification. Helping students improve their English communication skills.",
                HourlyRate = 35.00m,
                PhoneNumber = "+1234567893",
                Location = "London, UK",
                Education = "BA in English Literature, Oxford University",
                YearsOfExperience = 6,
                Languages = "English,French",
                AverageRating = 4.7,
                TotalLessons = 320,
                MemberSince = new DateTime(2019, 6, 10),
                ResponseTime = "Within 3 hours",
                VerificationStatus = TeacherVerificationStatus.Verified,
                VerifiedAt = new DateTime(2019, 6, 15)
            },
            new Teacher
            {
                Id = 4,
                UserId = 6,
                Bio = "Physics enthusiast with a passion for making complex concepts simple. Specialized in mechanics and electromagnetism.",
                HourlyRate = 50.00m,
                PhoneNumber = "+1234567894",
                Location = "Chicago, USA",
                Education = "PhD in Physics, Caltech",
                YearsOfExperience = 12,
                Languages = "English,Spanish,Portuguese",
                AverageRating = 4.9,
                TotalLessons = 290,
                MemberSince = new DateTime(2018, 9, 5),
                ResponseTime = "Within 2 hours",
                VerificationStatus = TeacherVerificationStatus.Verified,
                VerifiedAt = new DateTime(2018, 9, 10)
            },
            new Teacher
            {
                Id = 5,
                UserId = 7,
                Bio = "Professional pianist and music teacher. Teaching piano, music theory, and composition for all levels.",
                HourlyRate = 40.00m,
                PhoneNumber = "+1234567895",
                Location = "Vienna, Austria",
                Education = "Master of Music, Juilliard School",
                YearsOfExperience = 15,
                Languages = "English,German,Italian",
                AverageRating = 5.0,
                TotalLessons = 450,
                MemberSince = new DateTime(2017, 2, 14),
                ResponseTime = "Within 4 hours",
                VerificationStatus = TeacherVerificationStatus.Verified,
                VerifiedAt = new DateTime(2017, 2, 20)
            },
            new Teacher
            {
                Id = 6,
                UserId = 8,
                Bio = "Chemistry teacher with expertise in organic and inorganic chemistry. Making chemistry fun and understandable!",
                HourlyRate = 42.00m,
                PhoneNumber = "+1234567896",
                Location = "Toronto, Canada",
                Education = "PhD in Chemistry, University of Toronto",
                YearsOfExperience = 9,
                Languages = "English,French",
                AverageRating = 4.6,
                TotalLessons = 210,
                MemberSince = new DateTime(2020, 5, 22),
                ResponseTime = "Within 2 hours",
                VerificationStatus = TeacherVerificationStatus.Verified,
                VerifiedAt = new DateTime(2020, 5, 28)
            },
            new Teacher
            {
                Id = 7,
                UserId = 9,
                Bio = "Native Spanish speaker offering conversational Spanish lessons. Learn Spanish the natural way!",
                HourlyRate = 30.00m,
                PhoneNumber = "+1234567897",
                Location = "Madrid, Spain",
                Education = "BA in Spanish Linguistics, Universidad Complutense",
                YearsOfExperience = 5,
                Languages = "Spanish,English,Catalan",
                AverageRating = 4.8,
                TotalLessons = 380,
                MemberSince = new DateTime(2021, 8, 30),
                ResponseTime = "Within 1 hour",
                VerificationStatus = TeacherVerificationStatus.Verified,
                VerifiedAt = new DateTime(2021, 9, 5)
            },
            new Teacher
            {
                Id = 8,
                UserId = 10,
                Bio = "History professor specializing in World War II and American history. Bringing history to life through engaging storytelling.",
                HourlyRate = 38.00m,
                PhoneNumber = "+1234567898",
                Location = "Washington DC, USA",
                Education = "PhD in History, Harvard University",
                YearsOfExperience = 20,
                Languages = "English",
                AverageRating = 4.7,
                TotalLessons = 340,
                MemberSince = new DateTime(2016, 11, 8),
                ResponseTime = "Within 5 hours",
                VerificationStatus = TeacherVerificationStatus.Verified,
                VerifiedAt = new DateTime(2016, 11, 15)
            },
            new Teacher
            {
                Id = 9,
                UserId = 11,
                Bio = "Biology teacher with a focus on molecular biology and genetics. PhD researcher turned educator.",
                HourlyRate = 48.00m,
                PhoneNumber = "+1234567899",
                Location = "Cambridge, UK",
                Education = "PhD in Molecular Biology, Cambridge University",
                YearsOfExperience = 7,
                Languages = "English,French",
                AverageRating = 4.9,
                TotalLessons = 195,
                MemberSince = new DateTime(2021, 1, 12),
                ResponseTime = "Within 3 hours",
                VerificationStatus = TeacherVerificationStatus.Verified,
                VerifiedAt = new DateTime(2021, 1, 18)
            },
            new Teacher
            {
                Id = 10,
                UserId = 12,
                Bio = "French language expert offering lessons from beginner to advanced. Certified DELF/DALF examiner.",
                HourlyRate = 36.00m,
                PhoneNumber = "+1234567800",
                Location = "Paris, France",
                Education = "MA in French Literature, Sorbonne University",
                YearsOfExperience = 11,
                Languages = "French,English,Spanish",
                AverageRating = 4.8,
                TotalLessons = 420,
                MemberSince = new DateTime(2019, 4, 25),
                ResponseTime = "Within 2 hours",
                VerificationStatus = TeacherVerificationStatus.Verified,
                VerifiedAt = new DateTime(2019, 4, 30)
            }
        );

        // Seed Teacher-Subject relationships
        modelBuilder.Entity<TeacherSubject>().HasData(
            // Existing relationships
            new TeacherSubject { Id = 1, TeacherId = 1, SubjectId = 1 },
            new TeacherSubject { Id = 2, TeacherId = 2, SubjectId = 8 },
            new TeacherSubject { Id = 3, TeacherId = 2, SubjectId = 9 },
            new TeacherSubject { Id = 4, TeacherId = 3, SubjectId = 5 },
            new TeacherSubject { Id = 5, TeacherId = 3, SubjectId = 11 },
            new TeacherSubject { Id = 6, TeacherId = 4, SubjectId = 2 },
            new TeacherSubject { Id = 7, TeacherId = 5, SubjectId = 12 },
            new TeacherSubject { Id = 8, TeacherId = 5, SubjectId = 13 },
            new TeacherSubject { Id = 9, TeacherId = 6, SubjectId = 3 },
            new TeacherSubject { Id = 10, TeacherId = 7, SubjectId = 6 },
            new TeacherSubject { Id = 11, TeacherId = 8, SubjectId = 10 },
            new TeacherSubject { Id = 12, TeacherId = 9, SubjectId = 4 },
            new TeacherSubject { Id = 13, TeacherId = 10, SubjectId = 7 },
            new TeacherSubject { Id = 14, TeacherId = 10, SubjectId = 11 },

            // New relationships
            new TeacherSubject { Id = 20, TeacherId = 20, SubjectId = 1 }, // Math - Low
            new TeacherSubject { Id = 21, TeacherId = 21, SubjectId = 1 }, // Math - High
            new TeacherSubject { Id = 22, TeacherId = 22, SubjectId = 2 }, // Physics - Low
            new TeacherSubject { Id = 23, TeacherId = 23, SubjectId = 2 }, // Physics - High
            new TeacherSubject { Id = 24, TeacherId = 24, SubjectId = 3 }, // Chemistry - Low
            new TeacherSubject { Id = 25, TeacherId = 25, SubjectId = 3 }, // Chemistry - High
            new TeacherSubject { Id = 26, TeacherId = 26, SubjectId = 4 }, // Biology - Low
            new TeacherSubject { Id = 27, TeacherId = 27, SubjectId = 4 }, // Biology - Medium
            new TeacherSubject { Id = 28, TeacherId = 28, SubjectId = 5 }, // English - Low
            new TeacherSubject { Id = 29, TeacherId = 29, SubjectId = 5 }, // English - High
            new TeacherSubject { Id = 30, TeacherId = 30, SubjectId = 6 }, // Spanish - Medium
            new TeacherSubject { Id = 31, TeacherId = 31, SubjectId = 6 }, // Spanish - High
            new TeacherSubject { Id = 32, TeacherId = 32, SubjectId = 7 }, // French - Low
            new TeacherSubject { Id = 33, TeacherId = 33, SubjectId = 7 }, // French - High
            new TeacherSubject { Id = 34, TeacherId = 34, SubjectId = 8 }, // CS - Low
            new TeacherSubject { Id = 35, TeacherId = 34, SubjectId = 9 }, // Prog - Low
            new TeacherSubject { Id = 36, TeacherId = 35, SubjectId = 8 }, // CS - Medium
            new TeacherSubject { Id = 37, TeacherId = 35, SubjectId = 9 }, // Prog - Medium
            new TeacherSubject { Id = 38, TeacherId = 36, SubjectId = 8 }, // CS - High
            new TeacherSubject { Id = 39, TeacherId = 36, SubjectId = 9 }, // Prog - High
            new TeacherSubject { Id = 40, TeacherId = 37, SubjectId = 10 }, // History - Low
            new TeacherSubject { Id = 41, TeacherId = 38, SubjectId = 10 }, // History - Medium
            new TeacherSubject { Id = 42, TeacherId = 39, SubjectId = 11 }, // Lit - Low
            new TeacherSubject { Id = 43, TeacherId = 40, SubjectId = 11 }, // Lit - High
            new TeacherSubject { Id = 44, TeacherId = 41, SubjectId = 12 }, // Music - Low
            new TeacherSubject { Id = 45, TeacherId = 41, SubjectId = 13 }, // Piano - Low
            new TeacherSubject { Id = 46, TeacherId = 42, SubjectId = 12 }, // Music - High
            new TeacherSubject { Id = 47, TeacherId = 42, SubjectId = 13 }  // Piano - High
        );

        // Add new Users for the new teachers
        modelBuilder.Entity<User>().HasData(
            new User { Id = 20, Email = "alice.walker@learnconnect.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Teacher123!"), FirstName = "Alice", LastName = "Walker", Role = UserRole.Teacher, CreatedAt = DateTime.UtcNow, IsActive = true },
            new User { Id = 21, Email = "robert.vance@learnconnect.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Teacher123!"), FirstName = "Robert", LastName = "Vance", Role = UserRole.Teacher, CreatedAt = DateTime.UtcNow, IsActive = true },
            new User { Id = 22, Email = "john.doe@learnconnect.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Teacher123!"), FirstName = "John", LastName = "Doe", Role = UserRole.Teacher, CreatedAt = DateTime.UtcNow, IsActive = true },
            new User { Id = 23, Email = "marie.curie@learnconnect.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Teacher123!"), FirstName = "Marie", LastName = "Curie", Role = UserRole.Teacher, CreatedAt = DateTime.UtcNow, IsActive = true },
            new User { Id = 24, Email = "walter.white@learnconnect.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Teacher123!"), FirstName = "Walter", LastName = "White", Role = UserRole.Teacher, CreatedAt = DateTime.UtcNow, IsActive = true },
            new User { Id = 25, Email = "heisenberg@learnconnect.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Teacher123!"), FirstName = "Werner", LastName = "Heisenberg", Role = UserRole.Teacher, CreatedAt = DateTime.UtcNow, IsActive = true },
            new User { Id = 26, Email = "jane.goodall@learnconnect.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Teacher123!"), FirstName = "Jane", LastName = "Goodall", Role = UserRole.Teacher, CreatedAt = DateTime.UtcNow, IsActive = true },
            new User { Id = 27, Email = "gregor.mendel@learnconnect.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Teacher123!"), FirstName = "Gregor", LastName = "Mendel", Role = UserRole.Teacher, CreatedAt = DateTime.UtcNow, IsActive = true },
            new User { Id = 28, Email = "new.grad@learnconnect.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Teacher123!"), FirstName = "Emily", LastName = "Dickinson", Role = UserRole.Teacher, CreatedAt = DateTime.UtcNow, IsActive = true },
            new User { Id = 29, Email = "shakespeare@learnconnect.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Teacher123!"), FirstName = "William", LastName = "Shakespeare", Role = UserRole.Teacher, CreatedAt = DateTime.UtcNow, IsActive = true },
            new User { Id = 30, Email = "carlos.ruiz@learnconnect.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Teacher123!"), FirstName = "Carlos", LastName = "Ruiz", Role = UserRole.Teacher, CreatedAt = DateTime.UtcNow, IsActive = true },
            new User { Id = 31, Email = "isabela.madrigal@learnconnect.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Teacher123!"), FirstName = "Isabela", LastName = "Madrigal", Role = UserRole.Teacher, CreatedAt = DateTime.UtcNow, IsActive = true },
            new User { Id = 32, Email = "pierre.escargot@learnconnect.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Teacher123!"), FirstName = "Pierre", LastName = "Escargot", Role = UserRole.Teacher, CreatedAt = DateTime.UtcNow, IsActive = true },
            new User { Id = 33, Email = "chef.gusteau@learnconnect.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Teacher123!"), FirstName = "Auguste", LastName = "Gusteau", Role = UserRole.Teacher, CreatedAt = DateTime.UtcNow, IsActive = true },
            new User { Id = 34, Email = "script.kiddie@learnconnect.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Teacher123!"), FirstName = "Kevin", LastName = "Mitnick", Role = UserRole.Teacher, CreatedAt = DateTime.UtcNow, IsActive = true },
            new User { Id = 35, Email = "dev.ops@learnconnect.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Teacher123!"), FirstName = "Linus", LastName = "Torvalds", Role = UserRole.Teacher, CreatedAt = DateTime.UtcNow, IsActive = true },
            new User { Id = 36, Email = "ai.researcher@learnconnect.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Teacher123!"), FirstName = "Ada", LastName = "Lovelace", Role = UserRole.Teacher, CreatedAt = DateTime.UtcNow, IsActive = true },
            new User { Id = 37, Email = "time.traveler@learnconnect.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Teacher123!"), FirstName = "Marty", LastName = "McFly", Role = UserRole.Teacher, CreatedAt = DateTime.UtcNow, IsActive = true },
            new User { Id = 38, Email = "museum.guide@learnconnect.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Teacher123!"), FirstName = "Indiana", LastName = "Jones", Role = UserRole.Teacher, CreatedAt = DateTime.UtcNow, IsActive = true },
            new User { Id = 39, Email = "book.worm@learnconnect.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Teacher123!"), FirstName = "Hermione", LastName = "Granger", Role = UserRole.Teacher, CreatedAt = DateTime.UtcNow, IsActive = true },
            new User { Id = 40, Email = "published.author@learnconnect.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Teacher123!"), FirstName = "J.K.", LastName = "Rowling", Role = UserRole.Teacher, CreatedAt = DateTime.UtcNow, IsActive = true },
            new User { Id = 41, Email = "street.performer@learnconnect.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Teacher123!"), FirstName = "Ed", LastName = "Sheeran", Role = UserRole.Teacher, CreatedAt = DateTime.UtcNow, IsActive = true },
            new User { Id = 42, Email = "concert.pianist@learnconnect.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Teacher123!"), FirstName = "Ludwig", LastName = "Beethoven", Role = UserRole.Teacher, CreatedAt = DateTime.UtcNow, IsActive = true }
        );

        // Add new Teacher profiles
        modelBuilder.Entity<Teacher>().HasData(
            new Teacher { Id = 20, UserId = 20, Bio = "Math enthusiast helping students love numbers.", HourlyRate = 25.00m, Location = "Online", Education = "BS Math", YearsOfExperience = 2, Languages = "English", AverageRating = 4.5, TotalLessons = 50, MemberSince = new DateTime(2023, 8, 1), ResponseTime = "Within 1 hour", VerificationStatus = TeacherVerificationStatus.Verified, VerifiedAt = new DateTime(2023, 8, 5) },
            new Teacher { Id = 21, UserId = 21, Bio = "Advanced mathematics for serious students.", HourlyRate = 75.00m, Location = "New York", Education = "PhD Math", YearsOfExperience = 20, Languages = "English", AverageRating = 5.0, TotalLessons = 500, MemberSince = new DateTime(2021, 1, 1), ResponseTime = "Within 12 hours", VerificationStatus = TeacherVerificationStatus.Verified, VerifiedAt = new DateTime(2021, 1, 10) },
            new Teacher { Id = 22, UserId = 22, Bio = "Physics made simple and fun.", HourlyRate = 28.00m, Location = "Online", Education = "BS Physics", YearsOfExperience = 3, Languages = "English", AverageRating = 4.6, TotalLessons = 80, MemberSince = new DateTime(2023, 6, 1), ResponseTime = "Within 2 hours", VerificationStatus = TeacherVerificationStatus.Verified, VerifiedAt = new DateTime(2023, 6, 8) },
            new Teacher { Id = 23, UserId = 23, Bio = "Expert physics tutoring for university level.", HourlyRate = 70.00m, Location = "Paris", Education = "PhD Physics", YearsOfExperience = 15, Languages = "English,French", AverageRating = 4.9, TotalLessons = 300, MemberSince = new DateTime(2023, 1, 1), ResponseTime = "Within 6 hours", VerificationStatus = TeacherVerificationStatus.Verified, VerifiedAt = new DateTime(2023, 1, 10) },
            new Teacher { Id = 24, UserId = 24, Bio = "High school chemistry support.", HourlyRate = 30.00m, Location = "Online", Education = "BS Chemistry", YearsOfExperience = 4, Languages = "English", AverageRating = 4.7, TotalLessons = 120, MemberSince = new DateTime(2024, 1, 1), ResponseTime = "Within 3 hours", VerificationStatus = TeacherVerificationStatus.Verified, VerifiedAt = new DateTime(2024, 1, 8) },
            new Teacher { Id = 25, UserId = 25, Bio = "Advanced organic chemistry and lab prep.", HourlyRate = 80.00m, Location = "Berlin", Education = "PhD Chemistry", YearsOfExperience = 25, Languages = "English,German", AverageRating = 5.0, TotalLessons = 600, MemberSince = new DateTime(2020, 1, 1), ResponseTime = "Within 24 hours", VerificationStatus = TeacherVerificationStatus.Verified, VerifiedAt = new DateTime(2020, 1, 15) },
            new Teacher { Id = 26, UserId = 26, Bio = "Biology basics for everyone.", HourlyRate = 22.00m, Location = "Online", Education = "BS Biology", YearsOfExperience = 1, Languages = "English", AverageRating = 4.4, TotalLessons = 30, MemberSince = new DateTime(2024, 3, 1), ResponseTime = "Within 1 hour", VerificationStatus = TeacherVerificationStatus.Verified, VerifiedAt = new DateTime(2024, 3, 5) },
            new Teacher { Id = 27, UserId = 27, Bio = "Genetics and evolutionary biology expert.", HourlyRate = 45.00m, Location = "Vienna", Education = "MS Biology", YearsOfExperience = 8, Languages = "English,German", AverageRating = 4.8, TotalLessons = 200, MemberSince = new DateTime(2022, 1, 1), ResponseTime = "Within 4 hours", VerificationStatus = TeacherVerificationStatus.Verified, VerifiedAt = new DateTime(2022, 1, 10) },
            new Teacher { Id = 28, UserId = 28, Bio = "English conversation and grammar.", HourlyRate = 20.00m, Location = "Online", Education = "BA English", YearsOfExperience = 1, Languages = "English", AverageRating = 4.3, TotalLessons = 40, MemberSince = new DateTime(2024, 2, 1), ResponseTime = "Within 1 hour", VerificationStatus = TeacherVerificationStatus.Verified, VerifiedAt = new DateTime(2024, 2, 8) },
            new Teacher { Id = 29, UserId = 29, Bio = "Literature analysis and creative writing.", HourlyRate = 65.00m, Location = "London", Education = "MFA Writing", YearsOfExperience = 12, Languages = "English", AverageRating = 4.9, TotalLessons = 350, MemberSince = new DateTime(2022, 1, 1), ResponseTime = "Within 5 hours", VerificationStatus = TeacherVerificationStatus.Verified, VerifiedAt = new DateTime(2022, 1, 12) },
            new Teacher { Id = 30, UserId = 30, Bio = "Spanish for travel and business.", HourlyRate = 40.00m, Location = "Barcelona", Education = "BA Spanish", YearsOfExperience = 6, Languages = "Spanish,English", AverageRating = 4.7, TotalLessons = 150, MemberSince = new DateTime(2022, 1, 1), ResponseTime = "Within 2 hours", VerificationStatus = TeacherVerificationStatus.Verified, VerifiedAt = new DateTime(2022, 1, 15) },
            new Teacher { Id = 31, UserId = 31, Bio = "Native Spanish speaker, advanced levels.", HourlyRate = 60.00m, Location = "Madrid", Education = "MA Linguistics", YearsOfExperience = 10, Languages = "Spanish,English", AverageRating = 4.9, TotalLessons = 280, MemberSince = new DateTime(2023, 1, 1), ResponseTime = "Within 3 hours", VerificationStatus = TeacherVerificationStatus.Verified, VerifiedAt = new DateTime(2023, 1, 10) },
            new Teacher { Id = 32, UserId = 32, Bio = "Learn French basics quickly.", HourlyRate = 25.00m, Location = "Online", Education = "BA French", YearsOfExperience = 2, Languages = "French,English", AverageRating = 4.5, TotalLessons = 60, MemberSince = new DateTime(2023, 7, 1), ResponseTime = "Within 1 hour", VerificationStatus = TeacherVerificationStatus.Verified, VerifiedAt = new DateTime(2023, 7, 8) },
            new Teacher { Id = 33, UserId = 33, Bio = "Master French cuisine and language.", HourlyRate = 70.00m, Location = "Paris", Education = "Culinary Arts", YearsOfExperience = 15, Languages = "French,English", AverageRating = 5.0, TotalLessons = 400, MemberSince = new DateTime(2021, 1, 1), ResponseTime = "Within 8 hours", VerificationStatus = TeacherVerificationStatus.Verified, VerifiedAt = new DateTime(2021, 1, 12) },
            new Teacher { Id = 34, UserId = 34, Bio = "Intro to coding and cybersecurity.", HourlyRate = 25.00m, Location = "Online", Education = "Self-taught", YearsOfExperience = 3, Languages = "English", AverageRating = 4.6, TotalLessons = 90, MemberSince = new DateTime(2024, 1, 1), ResponseTime = "Within 2 hours", VerificationStatus = TeacherVerificationStatus.Verified, VerifiedAt = new DateTime(2024, 1, 10) },
            new Teacher { Id = 35, UserId = 35, Bio = "DevOps, Linux, and System Admin.", HourlyRate = 55.00m, Location = "Helsinki", Education = "MS CS", YearsOfExperience = 10, Languages = "English,Finnish", AverageRating = 4.8, TotalLessons = 220, MemberSince = new DateTime(2023, 1, 1), ResponseTime = "Within 4 hours", VerificationStatus = TeacherVerificationStatus.Verified, VerifiedAt = new DateTime(2023, 1, 15) },
            new Teacher { Id = 36, UserId = 36, Bio = "Artificial Intelligence and Machine Learning.", HourlyRate = 90.00m, Location = "San Francisco", Education = "PhD CS", YearsOfExperience = 12, Languages = "English", AverageRating = 5.0, TotalLessons = 180, MemberSince = new DateTime(2022, 1, 1), ResponseTime = "Within 12 hours", VerificationStatus = TeacherVerificationStatus.Verified, VerifiedAt = new DateTime(2022, 1, 18) },
            new Teacher { Id = 37, UserId = 37, Bio = "History through the ages.", HourlyRate = 20.00m, Location = "Online", Education = "BA History", YearsOfExperience = 2, Languages = "English", AverageRating = 4.4, TotalLessons = 45, MemberSince = new DateTime(2023, 9, 1), ResponseTime = "Within 1 hour", VerificationStatus = TeacherVerificationStatus.Verified, VerifiedAt = new DateTime(2023, 9, 5) },
            new Teacher { Id = 38, UserId = 38, Bio = "Archaeology and ancient civilizations.", HourlyRate = 40.00m, Location = "Cairo", Education = "PhD Archaeology", YearsOfExperience = 15, Languages = "English", AverageRating = 4.8, TotalLessons = 300, MemberSince = new DateTime(2022, 1, 1), ResponseTime = "Within 6 hours", VerificationStatus = TeacherVerificationStatus.Verified, VerifiedAt = new DateTime(2022, 1, 20) },
            new Teacher { Id = 39, UserId = 39, Bio = "Reading comprehension and book clubs.", HourlyRate = 22.00m, Location = "Online", Education = "BA Lit", YearsOfExperience = 3, Languages = "English", AverageRating = 4.7, TotalLessons = 100, MemberSince = new DateTime(2024, 1, 1), ResponseTime = "Within 2 hours", VerificationStatus = TeacherVerificationStatus.Verified, VerifiedAt = new DateTime(2024, 1, 8) },
            new Teacher { Id = 40, UserId = 40, Bio = "Creative writing masterclass.", HourlyRate = 65.00m, Location = "Edinburgh", Education = "BA Classics", YearsOfExperience = 20, Languages = "English", AverageRating = 4.9, TotalLessons = 500, MemberSince = new DateTime(2020, 1, 1), ResponseTime = "Within 24 hours", VerificationStatus = TeacherVerificationStatus.Verified, VerifiedAt = new DateTime(2020, 1, 15) },
            new Teacher { Id = 41, UserId = 41, Bio = "Guitar and pop music basics.", HourlyRate = 25.00m, Location = "London", Education = "Self-taught", YearsOfExperience = 5, Languages = "English", AverageRating = 4.8, TotalLessons = 150, MemberSince = new DateTime(2022, 1, 1), ResponseTime = "Within 2 hours", VerificationStatus = TeacherVerificationStatus.Verified, VerifiedAt = new DateTime(2022, 1, 12) },
            new Teacher { Id = 42, UserId = 42, Bio = "Classical piano and composition.", HourlyRate = 85.00m, Location = "Vienna", Education = "Conservatory", YearsOfExperience = 30, Languages = "German,English", AverageRating = 5.0, TotalLessons = 800, MemberSince = new DateTime(2018, 1, 1), ResponseTime = "Within 12 hours", VerificationStatus = TeacherVerificationStatus.Verified, VerifiedAt = new DateTime(2018, 1, 20) }
        );
    }

}
