using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace LearnConnect.API.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Subjects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subjects", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Role = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SenderId = table.Column<int>(type: "int", nullable: false),
                    ReceiverId = table.Column<int>(type: "int", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SentAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    ReadAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Messages_Users_ReceiverId",
                        column: x => x.ReceiverId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Messages_Users_SenderId",
                        column: x => x.SenderId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Students",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ParentUserId = table.Column<int>(type: "int", nullable: true),
                    StudentName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExperiencePoints = table.Column<int>(type: "int", nullable: false),
                    Level = table.Column<int>(type: "int", nullable: false),
                    CurrentStreak = table.Column<int>(type: "int", nullable: false),
                    LastLessonDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Students", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Students_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Teachers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Bio = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HourlyRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Education = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    YearsOfExperience = table.Column<int>(type: "int", nullable: false),
                    Languages = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProfileImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AverageRating = table.Column<double>(type: "float", nullable: false),
                    TotalLessons = table.Column<int>(type: "int", nullable: false),
                    MemberSince = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ResponseTime = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VerificationStatus = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    VerifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    VerificationNotes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CertificateUrl = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teachers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Teachers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudentBadges",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Icon = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AwardedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentBadges", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentBadges_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LessonNotebooks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    TeacherId = table.Column<int>(type: "int", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastUpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LessonNotebooks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LessonNotebooks_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LessonNotebooks_Teachers_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "Teachers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Lessons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TeacherId = table.Column<int>(type: "int", nullable: false),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    SubjectId = table.Column<int>(type: "int", nullable: true),
                    ScheduledDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DurationMinutes = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MeetingLink = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lessons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Lessons_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Lessons_Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subjects",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Lessons_Teachers_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "Teachers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ParentMeetingRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ParentId = table.Column<int>(type: "int", nullable: false),
                    TeacherId = table.Column<int>(type: "int", nullable: false),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    RequestedDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DurationMinutes = table.Column<int>(type: "int", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParentMeetingRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ParentMeetingRequests_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ParentMeetingRequests_Teachers_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "Teachers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ParentMeetingRequests_Users_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Schedules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TeacherId = table.Column<int>(type: "int", nullable: false),
                    DayOfWeek = table.Column<int>(type: "int", nullable: false),
                    StartTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    EndTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    IsAvailable = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Schedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Schedules_Teachers_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "Teachers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TeacherSubjects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TeacherId = table.Column<int>(type: "int", nullable: false),
                    SubjectId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeacherSubjects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TeacherSubjects_Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TeacherSubjects_Teachers_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "Teachers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reservations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    TeacherId = table.Column<int>(type: "int", nullable: false),
                    LessonId = table.Column<int>(type: "int", nullable: true),
                    RequestedDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DurationMinutes = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ConfirmedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reservations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reservations_Lessons_LessonId",
                        column: x => x.LessonId,
                        principalTable: "Lessons",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Reservations_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Reservations_Teachers_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "Teachers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Reviews",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    TeacherId = table.Column<int>(type: "int", nullable: false),
                    LessonId = table.Column<int>(type: "int", nullable: true),
                    Rating = table.Column<int>(type: "int", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reviews_Lessons_LessonId",
                        column: x => x.LessonId,
                        principalTable: "Lessons",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Reviews_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Reviews_Teachers_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "Teachers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Subjects",
                columns: new[] { "Id", "Category", "Description", "Name" },
                values: new object[,]
                {
                    { 1, "Science", null, "Mathematics" },
                    { 2, "Science", null, "Physics" },
                    { 3, "Science", null, "Chemistry" },
                    { 4, "Science", null, "Biology" },
                    { 5, "Languages", null, "English" },
                    { 6, "Languages", null, "Spanish" },
                    { 7, "Languages", null, "French" },
                    { 8, "Technology", null, "Computer Science" },
                    { 9, "Technology", null, "Programming" },
                    { 10, "Humanities", null, "History" },
                    { 11, "Humanities", null, "Literature" },
                    { 12, "Arts", null, "Music" },
                    { 13, "Arts", null, "Piano" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Email", "FirstName", "IsActive", "LastName", "PasswordHash", "Role" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 2, 26, 8, 41, 39, 721, DateTimeKind.Utc).AddTicks(5393), "admin@learnconnect.com", "Admin", true, "User", "$2a$11$o05GVdsdThXmsIcK26bvHe0rVj.UFyI0KuuRLajSvoaaMyrLoqwGC", 2 },
                    { 2, new DateTime(2026, 2, 26, 8, 41, 39, 934, DateTimeKind.Utc).AddTicks(484), "student@test.com", "Test", true, "Student", "$2a$11$RYCxzT9nDuGRFydE8Rrcj.TJY5QsZODcTCTybzShhPhVP2/PKBxLK", 0 },
                    { 3, new DateTime(2026, 2, 26, 8, 41, 40, 384, DateTimeKind.Utc).AddTicks(2275), "sarah.johnson@learnconnect.com", "Sarah", true, "Johnson", "$2a$11$rwLPymy5/nLX6GIqYbewGOSIHn4chqUeW3Ub8/l5sGXcM9mXSSmy6", 1 },
                    { 4, new DateTime(2026, 2, 26, 8, 41, 40, 623, DateTimeKind.Utc).AddTicks(7364), "michael.chen@learnconnect.com", "Michael", true, "Chen", "$2a$11$ybu3WyaClQC5oTR4Sl5VPOEkuW1vsCxIR.LE3kK33.8QTfO0rCSW6", 1 },
                    { 5, new DateTime(2026, 2, 26, 8, 41, 40, 923, DateTimeKind.Utc).AddTicks(1336), "emma.davis@learnconnect.com", "Emma", true, "Davis", "$2a$11$IPU5xF5j/hlwrqcxlY4cvOs0XrkL3ROfKjrX7xsBwJ35arUKgl/GW", 1 },
                    { 6, new DateTime(2026, 2, 26, 8, 41, 41, 157, DateTimeKind.Utc).AddTicks(3448), "david.martinez@learnconnect.com", "David", true, "Martinez", "$2a$11$wI9M.DwjaZuXaGFE7vUz/On1mN.LBehQZaLRsjZhEO4uOXXl35/am", 1 },
                    { 7, new DateTime(2026, 2, 26, 8, 41, 41, 451, DateTimeKind.Utc).AddTicks(1181), "lisa.anderson@learnconnect.com", "Lisa", true, "Anderson", "$2a$11$2Cfh.p4pNMXwyhFP6EPsDe6nKkINBw9DWFAf.ValsYzBycFjI7RxS", 1 },
                    { 8, new DateTime(2026, 2, 26, 8, 41, 41, 691, DateTimeKind.Utc).AddTicks(8244), "james.wilson@learnconnect.com", "James", true, "Wilson", "$2a$11$BQZ82pp/..yLITKaHVHgAeEayO6uGk0hMlG5K3b41fSlomsVl7Cm6", 1 },
                    { 9, new DateTime(2026, 2, 26, 8, 41, 41, 929, DateTimeKind.Utc).AddTicks(1397), "sophia.garcia@learnconnect.com", "Sophia", true, "Garcia", "$2a$11$2tUvzRh6smu73bfDmaC4..d5ZcdFk3QvNm1FVzKHw00GVk9yP89zK", 1 },
                    { 10, new DateTime(2026, 2, 26, 8, 41, 42, 89, DateTimeKind.Utc).AddTicks(5014), "robert.brown@learnconnect.com", "Robert", true, "Brown", "$2a$11$nKdbnMdB2iUzf4W9th4foel/puyYs1IjIIQLBN7rQn32cDbjm3AsC", 1 },
                    { 11, new DateTime(2026, 2, 26, 8, 41, 42, 280, DateTimeKind.Utc).AddTicks(2871), "olivia.taylor@learnconnect.com", "Olivia", true, "Taylor", "$2a$11$t3fkQqvHNtKVEAy6eesleOWSA1rOhHqGfWGQy2M3Uf9fSaApN3saO", 1 },
                    { 12, new DateTime(2026, 2, 26, 8, 41, 42, 443, DateTimeKind.Utc).AddTicks(7453), "daniel.moore@learnconnect.com", "Daniel", true, "Moore", "$2a$11$aZw3zZ628E7UL0su0OZDEO3tQX6L0.QV2DMY9t54ZMfrR8hRDfcxm", 1 },
                    { 20, new DateTime(2026, 2, 26, 8, 41, 42, 630, DateTimeKind.Utc).AddTicks(4226), "alice.walker@learnconnect.com", "Alice", true, "Walker", "$2a$11$oZViqyN8hO0HhDEKPz65XORrPyxWiPZyU4alUDa2rCzLXQ1QNmYrO", 1 },
                    { 21, new DateTime(2026, 2, 26, 8, 41, 42, 791, DateTimeKind.Utc).AddTicks(7481), "robert.vance@learnconnect.com", "Robert", true, "Vance", "$2a$11$lWQoDoeLAkNKZEhgkXI4FuJoD3RKX2lj3R4gnYaIajgZU77whDPou", 1 },
                    { 22, new DateTime(2026, 2, 26, 8, 41, 42, 975, DateTimeKind.Utc).AddTicks(5542), "john.doe@learnconnect.com", "John", true, "Doe", "$2a$11$cMykedgIuxyVlHkEWr1GPeY6cy9x9gSbGWIb0M30oyfCZ4Vt4HU46", 1 },
                    { 23, new DateTime(2026, 2, 26, 8, 41, 43, 138, DateTimeKind.Utc).AddTicks(9834), "marie.curie@learnconnect.com", "Marie", true, "Curie", "$2a$11$rc.aXpJkaBeIfFgwPZgv9OeYQrjNs3X3feAfTFoJAAyuBb96r69I.", 1 },
                    { 24, new DateTime(2026, 2, 26, 8, 41, 43, 366, DateTimeKind.Utc).AddTicks(8933), "walter.white@learnconnect.com", "Walter", true, "White", "$2a$11$UGLYTLUT6ksWbgqZFiLcHOW4rYFErrDMV5sXfO/zZ9hXaG1Swl1hC", 1 },
                    { 25, new DateTime(2026, 2, 26, 8, 41, 43, 525, DateTimeKind.Utc).AddTicks(6720), "heisenberg@learnconnect.com", "Werner", true, "Heisenberg", "$2a$11$wmJS9ni6r.O2XSVkNkvLNeS/3laRcf0928aGFG.AvGyLGn.EnpdS6", 1 },
                    { 26, new DateTime(2026, 2, 26, 8, 41, 43, 790, DateTimeKind.Utc).AddTicks(1953), "jane.goodall@learnconnect.com", "Jane", true, "Goodall", "$2a$11$p5FL4Lqn8aivwe44ALwRc.SAdJGhdvWiwaHtObcxde4y0N4VJayRm", 1 },
                    { 27, new DateTime(2026, 2, 26, 8, 41, 43, 980, DateTimeKind.Utc).AddTicks(9971), "gregor.mendel@learnconnect.com", "Gregor", true, "Mendel", "$2a$11$XEVczyR8X9b6ymKX3TTBWOk0s0dLDH5Yj1azZCBbHHpxWYwJ8.A7S", 1 },
                    { 28, new DateTime(2026, 2, 26, 8, 41, 44, 292, DateTimeKind.Utc).AddTicks(9777), "new.grad@learnconnect.com", "Emily", true, "Dickinson", "$2a$11$VAZDCP7O6xBuAdeV0.u2OORRcJSzFz7jjFzSs0z1mK28H9RlZUz4q", 1 },
                    { 29, new DateTime(2026, 2, 26, 8, 41, 44, 502, DateTimeKind.Utc).AddTicks(7092), "shakespeare@learnconnect.com", "William", true, "Shakespeare", "$2a$11$pTo.xI9ax30l7Dx74QN/o.1FWu5FPMUw.Sj8RXkxJ0sbaijDEEUAK", 1 },
                    { 30, new DateTime(2026, 2, 26, 8, 41, 44, 663, DateTimeKind.Utc).AddTicks(1082), "carlos.ruiz@learnconnect.com", "Carlos", true, "Ruiz", "$2a$11$40g0va.uWMuWfaiqQr4pkuY73xk3h3PqSCIRe1ikk6DdsVJn7/5HC", 1 },
                    { 31, new DateTime(2026, 2, 26, 8, 41, 44, 841, DateTimeKind.Utc).AddTicks(6071), "isabela.madrigal@learnconnect.com", "Isabela", true, "Madrigal", "$2a$11$5yydyXC7ssx7takPDmVjYuBse2EahTgB1KQ.s18JUj5cDko4l0uBi", 1 },
                    { 32, new DateTime(2026, 2, 26, 8, 41, 45, 10, DateTimeKind.Utc).AddTicks(7250), "pierre.escargot@learnconnect.com", "Pierre", true, "Escargot", "$2a$11$ANm.ARSJg6AurkLKJC63H.N6oeR3FBHhmw3ASP8hau3hWA898XZcC", 1 },
                    { 33, new DateTime(2026, 2, 26, 8, 41, 45, 175, DateTimeKind.Utc).AddTicks(3863), "chef.gusteau@learnconnect.com", "Auguste", true, "Gusteau", "$2a$11$VupLELbXd1l.AvoRYlPwde80.dWGiaSNVTdg3k0Ac6WtLhuwqvn0C", 1 },
                    { 34, new DateTime(2026, 2, 26, 8, 41, 45, 428, DateTimeKind.Utc).AddTicks(8845), "script.kiddie@learnconnect.com", "Kevin", true, "Mitnick", "$2a$11$eowmstJOr11ETd7FlUkPt.LdKDsbypmqiDWRvUIybgOeK1YyHWPzu", 1 },
                    { 35, new DateTime(2026, 2, 26, 8, 41, 45, 723, DateTimeKind.Utc).AddTicks(8854), "dev.ops@learnconnect.com", "Linus", true, "Torvalds", "$2a$11$hZl5dRdewj67uvpVkrUjGuP3qfUkIIkHVgeoxHFKjJlOJ29FDlGfa", 1 },
                    { 36, new DateTime(2026, 2, 26, 8, 41, 45, 961, DateTimeKind.Utc).AddTicks(1650), "ai.researcher@learnconnect.com", "Ada", true, "Lovelace", "$2a$11$0Xb3dgsv0jA2UL9DSii7xeiVRw.vBzfMbZetM5icZCpkX9V/Curve", 1 },
                    { 37, new DateTime(2026, 2, 26, 8, 41, 46, 160, DateTimeKind.Utc).AddTicks(9166), "time.traveler@learnconnect.com", "Marty", true, "McFly", "$2a$11$d.rDmU7o1Rh3FVLW7C9CGeOGFfKl38jBAHrSgdaWhzndzD2VUKdYm", 1 },
                    { 38, new DateTime(2026, 2, 26, 8, 41, 46, 365, DateTimeKind.Utc).AddTicks(346), "museum.guide@learnconnect.com", "Indiana", true, "Jones", "$2a$11$uVhvmTmMMT8qoPW39BRcW.Ugdkl9QyHP67l8t0TksJgZtkxYQ74fG", 1 },
                    { 39, new DateTime(2026, 2, 26, 8, 41, 46, 575, DateTimeKind.Utc).AddTicks(4972), "book.worm@learnconnect.com", "Hermione", true, "Granger", "$2a$11$ee6KlPgnoK.uS9GGsWY2Hez01rbGsvwA7KTvnywqkbmHugT3hKJEG", 1 },
                    { 40, new DateTime(2026, 2, 26, 8, 41, 46, 852, DateTimeKind.Utc).AddTicks(2628), "published.author@learnconnect.com", "J.K.", true, "Rowling", "$2a$11$FujJ..O.vYlxIHA0P7LMEuVpDkJxYv/cW4ypjUN7ga7MubPSUtG/q", 1 },
                    { 41, new DateTime(2026, 2, 26, 8, 41, 47, 79, DateTimeKind.Utc).AddTicks(3074), "street.performer@learnconnect.com", "Ed", true, "Sheeran", "$2a$11$620ggWZ7RzSLEnGeGA41Wep9oOdpb/LPXUo1Sdk/Uc56btXp5V00K", 1 },
                    { 42, new DateTime(2026, 2, 26, 8, 41, 47, 402, DateTimeKind.Utc).AddTicks(3728), "concert.pianist@learnconnect.com", "Ludwig", true, "Beethoven", "$2a$11$RmH1CeUY.Aus.aY1YEo1k.rw7Gwn0GKaXvYgQY9MYp50sC1AiVAE.", 1 },
                    { 50, new DateTime(2026, 2, 26, 8, 41, 40, 177, DateTimeKind.Utc).AddTicks(6108), "parent@test.com", "Test", true, "Parent", "$2a$11$Sky1vCjyAJqdAa77x406HuEPk/yCmDXkLJFItRd8dLbZ1yThx/Gey", 3 }
                });

            migrationBuilder.InsertData(
                table: "Students",
                columns: new[] { "Id", "CurrentStreak", "DateOfBirth", "ExperiencePoints", "LastLessonDate", "Level", "Location", "ParentUserId", "PhoneNumber", "StudentName", "UserId" },
                values: new object[] { 1, 0, null, 0, null, 1, "New York, USA", null, "+1234567890", null, 2 });

            migrationBuilder.InsertData(
                table: "Teachers",
                columns: new[] { "Id", "AverageRating", "Bio", "CertificateUrl", "Education", "HourlyRate", "Languages", "Location", "MemberSince", "PhoneNumber", "ProfileImageUrl", "ResponseTime", "TotalLessons", "UserId", "VerificationNotes", "VerificationStatus", "VerifiedAt", "YearsOfExperience" },
                values: new object[,]
                {
                    { 1, 4.7999999999999998, "Experienced mathematics teacher with 10+ years of teaching experience. Specialized in calculus and algebra.", null, "PhD in Mathematics, MIT", 45.00m, "English,Spanish", "Boston, USA", new DateTime(2020, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "+1234567891", null, "Within 2 hours", 250, 3, null, "Verified", new DateTime(2020, 1, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), 10 },
                    { 2, 4.9000000000000004, "Computer Science expert specializing in Python, Java, and web development. Former software engineer at Google.", null, "MS in Computer Science, Stanford University", 60.00m, "English,Mandarin", "San Francisco, USA", new DateTime(2021, 3, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "+1234567892", null, "Within 1 hour", 180, 4, null, "Verified", new DateTime(2021, 3, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), 8 },
                    { 3, 4.7000000000000002, "Native English speaker with TEFL certification. Helping students improve their English communication skills.", null, "BA in English Literature, Oxford University", 35.00m, "English,French", "London, UK", new DateTime(2019, 6, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "+1234567893", null, "Within 3 hours", 320, 5, null, "Verified", new DateTime(2019, 6, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), 6 },
                    { 4, 4.9000000000000004, "Physics enthusiast with a passion for making complex concepts simple. Specialized in mechanics and electromagnetism.", null, "PhD in Physics, Caltech", 50.00m, "English,Spanish,Portuguese", "Chicago, USA", new DateTime(2018, 9, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "+1234567894", null, "Within 2 hours", 290, 6, null, "Verified", new DateTime(2018, 9, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), 12 },
                    { 5, 5.0, "Professional pianist and music teacher. Teaching piano, music theory, and composition for all levels.", null, "Master of Music, Juilliard School", 40.00m, "English,German,Italian", "Vienna, Austria", new DateTime(2017, 2, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), "+1234567895", null, "Within 4 hours", 450, 7, null, "Verified", new DateTime(2017, 2, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), 15 },
                    { 6, 4.5999999999999996, "Chemistry teacher with expertise in organic and inorganic chemistry. Making chemistry fun and understandable!", null, "PhD in Chemistry, University of Toronto", 42.00m, "English,French", "Toronto, Canada", new DateTime(2020, 5, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "+1234567896", null, "Within 2 hours", 210, 8, null, "Verified", new DateTime(2020, 5, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), 9 },
                    { 7, 4.7999999999999998, "Native Spanish speaker offering conversational Spanish lessons. Learn Spanish the natural way!", null, "BA in Spanish Linguistics, Universidad Complutense", 30.00m, "Spanish,English,Catalan", "Madrid, Spain", new DateTime(2021, 8, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), "+1234567897", null, "Within 1 hour", 380, 9, null, "Verified", new DateTime(2021, 9, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), 5 },
                    { 8, 4.7000000000000002, "History professor specializing in World War II and American history. Bringing history to life through engaging storytelling.", null, "PhD in History, Harvard University", 38.00m, "English", "Washington DC, USA", new DateTime(2016, 11, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), "+1234567898", null, "Within 5 hours", 340, 10, null, "Verified", new DateTime(2016, 11, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), 20 },
                    { 9, 4.9000000000000004, "Biology teacher with a focus on molecular biology and genetics. PhD researcher turned educator.", null, "PhD in Molecular Biology, Cambridge University", 48.00m, "English,French", "Cambridge, UK", new DateTime(2021, 1, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "+1234567899", null, "Within 3 hours", 195, 11, null, "Verified", new DateTime(2021, 1, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), 7 },
                    { 10, 4.7999999999999998, "French language expert offering lessons from beginner to advanced. Certified DELF/DALF examiner.", null, "MA in French Literature, Sorbonne University", 36.00m, "French,English,Spanish", "Paris, France", new DateTime(2019, 4, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), "+1234567800", null, "Within 2 hours", 420, 12, null, "Verified", new DateTime(2019, 4, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), 11 },
                    { 20, 4.5, "Math enthusiast helping students love numbers.", null, "BS Math", 25.00m, "English", "Online", new DateTime(2023, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Within 1 hour", 50, 20, null, "Verified", new DateTime(2023, 8, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), 2 },
                    { 21, 5.0, "Advanced mathematics for serious students.", null, "PhD Math", 75.00m, "English", "New York", new DateTime(2021, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Within 12 hours", 500, 21, null, "Verified", new DateTime(2021, 1, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), 20 },
                    { 22, 4.5999999999999996, "Physics made simple and fun.", null, "BS Physics", 28.00m, "English", "Online", new DateTime(2023, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Within 2 hours", 80, 22, null, "Verified", new DateTime(2023, 6, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), 3 },
                    { 23, 4.9000000000000004, "Expert physics tutoring for university level.", null, "PhD Physics", 70.00m, "English,French", "Paris", new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Within 6 hours", 300, 23, null, "Verified", new DateTime(2023, 1, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), 15 },
                    { 24, 4.7000000000000002, "High school chemistry support.", null, "BS Chemistry", 30.00m, "English", "Online", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Within 3 hours", 120, 24, null, "Verified", new DateTime(2024, 1, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), 4 },
                    { 25, 5.0, "Advanced organic chemistry and lab prep.", null, "PhD Chemistry", 80.00m, "English,German", "Berlin", new DateTime(2020, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Within 24 hours", 600, 25, null, "Verified", new DateTime(2020, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), 25 },
                    { 26, 4.4000000000000004, "Biology basics for everyone.", null, "BS Biology", 22.00m, "English", "Online", new DateTime(2024, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Within 1 hour", 30, 26, null, "Verified", new DateTime(2024, 3, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 27, 4.7999999999999998, "Genetics and evolutionary biology expert.", null, "MS Biology", 45.00m, "English,German", "Vienna", new DateTime(2022, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Within 4 hours", 200, 27, null, "Verified", new DateTime(2022, 1, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), 8 },
                    { 28, 4.2999999999999998, "English conversation and grammar.", null, "BA English", 20.00m, "English", "Online", new DateTime(2024, 2, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Within 1 hour", 40, 28, null, "Verified", new DateTime(2024, 2, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 29, 4.9000000000000004, "Literature analysis and creative writing.", null, "MFA Writing", 65.00m, "English", "London", new DateTime(2022, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Within 5 hours", 350, 29, null, "Verified", new DateTime(2022, 1, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), 12 },
                    { 30, 4.7000000000000002, "Spanish for travel and business.", null, "BA Spanish", 40.00m, "Spanish,English", "Barcelona", new DateTime(2022, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Within 2 hours", 150, 30, null, "Verified", new DateTime(2022, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), 6 },
                    { 31, 4.9000000000000004, "Native Spanish speaker, advanced levels.", null, "MA Linguistics", 60.00m, "Spanish,English", "Madrid", new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Within 3 hours", 280, 31, null, "Verified", new DateTime(2023, 1, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), 10 },
                    { 32, 4.5, "Learn French basics quickly.", null, "BA French", 25.00m, "French,English", "Online", new DateTime(2023, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Within 1 hour", 60, 32, null, "Verified", new DateTime(2023, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), 2 },
                    { 33, 5.0, "Master French cuisine and language.", null, "Culinary Arts", 70.00m, "French,English", "Paris", new DateTime(2021, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Within 8 hours", 400, 33, null, "Verified", new DateTime(2021, 1, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), 15 },
                    { 34, 4.5999999999999996, "Intro to coding and cybersecurity.", null, "Self-taught", 25.00m, "English", "Online", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Within 2 hours", 90, 34, null, "Verified", new DateTime(2024, 1, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), 3 },
                    { 35, 4.7999999999999998, "DevOps, Linux, and System Admin.", null, "MS CS", 55.00m, "English,Finnish", "Helsinki", new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Within 4 hours", 220, 35, null, "Verified", new DateTime(2023, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), 10 },
                    { 36, 5.0, "Artificial Intelligence and Machine Learning.", null, "PhD CS", 90.00m, "English", "San Francisco", new DateTime(2022, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Within 12 hours", 180, 36, null, "Verified", new DateTime(2022, 1, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), 12 },
                    { 37, 4.4000000000000004, "History through the ages.", null, "BA History", 20.00m, "English", "Online", new DateTime(2023, 9, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Within 1 hour", 45, 37, null, "Verified", new DateTime(2023, 9, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), 2 },
                    { 38, 4.7999999999999998, "Archaeology and ancient civilizations.", null, "PhD Archaeology", 40.00m, "English", "Cairo", new DateTime(2022, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Within 6 hours", 300, 38, null, "Verified", new DateTime(2022, 1, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), 15 },
                    { 39, 4.7000000000000002, "Reading comprehension and book clubs.", null, "BA Lit", 22.00m, "English", "Online", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Within 2 hours", 100, 39, null, "Verified", new DateTime(2024, 1, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), 3 },
                    { 40, 4.9000000000000004, "Creative writing masterclass.", null, "BA Classics", 65.00m, "English", "Edinburgh", new DateTime(2020, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Within 24 hours", 500, 40, null, "Verified", new DateTime(2020, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), 20 },
                    { 41, 4.7999999999999998, "Guitar and pop music basics.", null, "Self-taught", 25.00m, "English", "London", new DateTime(2022, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Within 2 hours", 150, 41, null, "Verified", new DateTime(2022, 1, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), 5 },
                    { 42, 5.0, "Classical piano and composition.", null, "Conservatory", 85.00m, "German,English", "Vienna", new DateTime(2018, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Within 12 hours", 800, 42, null, "Verified", new DateTime(2018, 1, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), 30 }
                });

            migrationBuilder.InsertData(
                table: "TeacherSubjects",
                columns: new[] { "Id", "SubjectId", "TeacherId" },
                values: new object[,]
                {
                    { 1, 1, 1 },
                    { 2, 8, 2 },
                    { 3, 9, 2 },
                    { 4, 5, 3 },
                    { 5, 11, 3 },
                    { 6, 2, 4 },
                    { 7, 12, 5 },
                    { 8, 13, 5 },
                    { 9, 3, 6 },
                    { 10, 6, 7 },
                    { 11, 10, 8 },
                    { 12, 4, 9 },
                    { 13, 7, 10 },
                    { 14, 11, 10 },
                    { 20, 1, 20 },
                    { 21, 1, 21 },
                    { 22, 2, 22 },
                    { 23, 2, 23 },
                    { 24, 3, 24 },
                    { 25, 3, 25 },
                    { 26, 4, 26 },
                    { 27, 4, 27 },
                    { 28, 5, 28 },
                    { 29, 5, 29 },
                    { 30, 6, 30 },
                    { 31, 6, 31 },
                    { 32, 7, 32 },
                    { 33, 7, 33 },
                    { 34, 8, 34 },
                    { 35, 9, 34 },
                    { 36, 8, 35 },
                    { 37, 9, 35 },
                    { 38, 8, 36 },
                    { 39, 9, 36 },
                    { 40, 10, 37 },
                    { 41, 10, 38 },
                    { 42, 11, 39 },
                    { 43, 11, 40 },
                    { 44, 12, 41 },
                    { 45, 13, 41 },
                    { 46, 12, 42 },
                    { 47, 13, 42 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_LessonNotebooks_StudentId",
                table: "LessonNotebooks",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_LessonNotebooks_TeacherId",
                table: "LessonNotebooks",
                column: "TeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_Lessons_StudentId",
                table: "Lessons",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_Lessons_SubjectId",
                table: "Lessons",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Lessons_TeacherId",
                table: "Lessons",
                column: "TeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_ReceiverId",
                table: "Messages",
                column: "ReceiverId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_SenderId",
                table: "Messages",
                column: "SenderId");

            migrationBuilder.CreateIndex(
                name: "IX_ParentMeetingRequests_ParentId",
                table: "ParentMeetingRequests",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_ParentMeetingRequests_StudentId",
                table: "ParentMeetingRequests",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_ParentMeetingRequests_TeacherId",
                table: "ParentMeetingRequests",
                column: "TeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_LessonId",
                table: "Reservations",
                column: "LessonId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_StudentId",
                table: "Reservations",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_TeacherId",
                table: "Reservations",
                column: "TeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_LessonId",
                table: "Reviews",
                column: "LessonId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_StudentId",
                table: "Reviews",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_TeacherId",
                table: "Reviews",
                column: "TeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_Schedules_TeacherId",
                table: "Schedules",
                column: "TeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentBadges_StudentId",
                table: "StudentBadges",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_Students_UserId",
                table: "Students",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Teachers_UserId",
                table: "Teachers",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TeacherSubjects_SubjectId",
                table: "TeacherSubjects",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherSubjects_TeacherId",
                table: "TeacherSubjects",
                column: "TeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LessonNotebooks");

            migrationBuilder.DropTable(
                name: "Messages");

            migrationBuilder.DropTable(
                name: "ParentMeetingRequests");

            migrationBuilder.DropTable(
                name: "Reservations");

            migrationBuilder.DropTable(
                name: "Reviews");

            migrationBuilder.DropTable(
                name: "Schedules");

            migrationBuilder.DropTable(
                name: "StudentBadges");

            migrationBuilder.DropTable(
                name: "TeacherSubjects");

            migrationBuilder.DropTable(
                name: "Lessons");

            migrationBuilder.DropTable(
                name: "Students");

            migrationBuilder.DropTable(
                name: "Subjects");

            migrationBuilder.DropTable(
                name: "Teachers");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
