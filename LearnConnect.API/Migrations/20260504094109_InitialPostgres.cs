using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace LearnConnect.API.Migrations
{
    /// <inheritdoc />
    public partial class InitialPostgres : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Subjects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Category = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subjects", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    FirstName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Role = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SenderId = table.Column<int>(type: "integer", nullable: false),
                    ReceiverId = table.Column<int>(type: "integer", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    SentAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsRead = table.Column<bool>(type: "boolean", nullable: false),
                    ReadAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
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
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    Location = table.Column<string>(type: "text", nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ParentUserId = table.Column<int>(type: "integer", nullable: true),
                    StudentName = table.Column<string>(type: "text", nullable: true),
                    ExperiencePoints = table.Column<int>(type: "integer", nullable: false),
                    Level = table.Column<int>(type: "integer", nullable: false),
                    CurrentStreak = table.Column<int>(type: "integer", nullable: false),
                    LastLessonDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
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
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    Bio = table.Column<string>(type: "text", nullable: false),
                    HourlyRate = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    Location = table.Column<string>(type: "text", nullable: true),
                    Education = table.Column<string>(type: "text", nullable: true),
                    YearsOfExperience = table.Column<int>(type: "integer", nullable: false),
                    Languages = table.Column<string>(type: "text", nullable: true),
                    ProfileImageUrl = table.Column<string>(type: "text", nullable: true),
                    AverageRating = table.Column<double>(type: "double precision", nullable: false),
                    TotalLessons = table.Column<int>(type: "integer", nullable: false),
                    MemberSince = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ResponseTime = table.Column<string>(type: "text", nullable: true),
                    VerificationStatus = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    VerifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    VerificationNotes = table.Column<string>(type: "text", nullable: true),
                    CertificateUrl = table.Column<string>(type: "text", nullable: true)
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
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StudentId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Icon = table.Column<string>(type: "text", nullable: false),
                    AwardedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
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
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StudentId = table.Column<int>(type: "integer", nullable: false),
                    TeacherId = table.Column<int>(type: "integer", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
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
                name: "LessonPackages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StudentId = table.Column<int>(type: "integer", nullable: false),
                    TeacherId = table.Column<int>(type: "integer", nullable: false),
                    SubjectId = table.Column<int>(type: "integer", nullable: false),
                    TotalLessons = table.Column<int>(type: "integer", nullable: false),
                    RemainingLessons = table.Column<int>(type: "integer", nullable: false),
                    TotalPrice = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LessonPackages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LessonPackages_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LessonPackages_Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LessonPackages_Teachers_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "Teachers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ParentMeetingRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ParentId = table.Column<int>(type: "integer", nullable: false),
                    TeacherId = table.Column<int>(type: "integer", nullable: false),
                    StudentId = table.Column<int>(type: "integer", nullable: false),
                    RequestedDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DurationMinutes = table.Column<int>(type: "integer", nullable: false),
                    Reason = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
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
                name: "Payments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StudentId = table.Column<int>(type: "integer", nullable: false),
                    TeacherId = table.Column<int>(type: "integer", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Method = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    TransactionId = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ProcessedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payments_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Payments_Teachers_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "Teachers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Schedules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TeacherId = table.Column<int>(type: "integer", nullable: false),
                    DayOfWeek = table.Column<int>(type: "integer", nullable: false),
                    StartTime = table.Column<TimeSpan>(type: "interval", nullable: false),
                    EndTime = table.Column<TimeSpan>(type: "interval", nullable: false),
                    IsAvailable = table.Column<bool>(type: "boolean", nullable: false)
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
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TeacherId = table.Column<int>(type: "integer", nullable: false),
                    SubjectId = table.Column<int>(type: "integer", nullable: false)
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
                name: "Lessons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TeacherId = table.Column<int>(type: "integer", nullable: false),
                    StudentId = table.Column<int>(type: "integer", nullable: false),
                    SubjectId = table.Column<int>(type: "integer", nullable: true),
                    LessonPackageId = table.Column<int>(type: "integer", nullable: true),
                    ScheduledDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DurationMinutes = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    MeetingLink = table.Column<string>(type: "text", nullable: true),
                    Price = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lessons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Lessons_LessonPackages_LessonPackageId",
                        column: x => x.LessonPackageId,
                        principalTable: "LessonPackages",
                        principalColumn: "Id");
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
                name: "Reservations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StudentId = table.Column<int>(type: "integer", nullable: false),
                    TeacherId = table.Column<int>(type: "integer", nullable: false),
                    LessonId = table.Column<int>(type: "integer", nullable: true),
                    RequestedDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DurationMinutes = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Message = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ConfirmedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
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
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StudentId = table.Column<int>(type: "integer", nullable: false),
                    TeacherId = table.Column<int>(type: "integer", nullable: false),
                    LessonId = table.Column<int>(type: "integer", nullable: true),
                    Rating = table.Column<int>(type: "integer", nullable: false),
                    Comment = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
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
                    { 1, new DateTime(2026, 5, 4, 9, 41, 1, 934, DateTimeKind.Utc).AddTicks(6115), "admin@learnconnect.com", "Admin", true, "User", "$2a$11$NFxDUfE1eZFpUnRpj5Reb.6C8Af2zrzp8X6h67mYnIGPKLvrSGvQy", 2 },
                    { 2, new DateTime(2026, 5, 4, 9, 41, 2, 104, DateTimeKind.Utc).AddTicks(9400), "student@test.com", "Test", true, "Student", "$2a$11$8UGdpXz/OVcSUa1QFBTXkudSn90D4IM.CofnXYnkPG70kV/RUGJdG", 0 },
                    { 3, new DateTime(2026, 5, 4, 9, 41, 2, 449, DateTimeKind.Utc).AddTicks(7914), "sarah.johnson@learnconnect.com", "Sarah", true, "Johnson", "$2a$11$L8QByiYxWXfm7SZMyK48xONLWhbWumZlmAHEZvvqed0hNWLKg8WNi", 1 },
                    { 4, new DateTime(2026, 5, 4, 9, 41, 2, 607, DateTimeKind.Utc).AddTicks(2382), "michael.chen@learnconnect.com", "Michael", true, "Chen", "$2a$11$uK7RHyiuwlJnulMeL.Fa7u3TBasppNhM7krUNkxQdrr6ffyV9hCm6", 1 },
                    { 5, new DateTime(2026, 5, 4, 9, 41, 2, 797, DateTimeKind.Utc).AddTicks(7683), "emma.davis@learnconnect.com", "Emma", true, "Davis", "$2a$11$66MS6/Mg2AO.s4PTGDujGuvrYtc4luyxFfnW4kuSz0wz3xXrVnhQq", 1 },
                    { 6, new DateTime(2026, 5, 4, 9, 41, 2, 999, DateTimeKind.Utc).AddTicks(8090), "david.martinez@learnconnect.com", "David", true, "Martinez", "$2a$11$MhnK.8FuEPxw9qduat3XnOFstpre/VO2H3YxBo0Vtz49llN/8O/mm", 1 },
                    { 7, new DateTime(2026, 5, 4, 9, 41, 3, 161, DateTimeKind.Utc).AddTicks(9785), "lisa.anderson@learnconnect.com", "Lisa", true, "Anderson", "$2a$11$o3KmTIf3l3P9IGYauwHJHO6aIpJLMrl38MFPTgDK03FsrMvJeSc1.", 1 },
                    { 8, new DateTime(2026, 5, 4, 9, 41, 3, 320, DateTimeKind.Utc).AddTicks(1208), "james.wilson@learnconnect.com", "James", true, "Wilson", "$2a$11$aV7kSjKno8Oz1IVwVy5uUOM6UPh2ddG4ajyVWvBUsW/y3CiKa1hwu", 1 },
                    { 9, new DateTime(2026, 5, 4, 9, 41, 3, 482, DateTimeKind.Utc).AddTicks(6334), "sophia.garcia@learnconnect.com", "Sophia", true, "Garcia", "$2a$11$DP5Gf2Wv7ljkYuJHCPGuk.CX.FYu1RfqkOWtVD9y1T8h47z57EEC6", 1 },
                    { 10, new DateTime(2026, 5, 4, 9, 41, 3, 674, DateTimeKind.Utc).AddTicks(2136), "robert.brown@learnconnect.com", "Robert", true, "Brown", "$2a$11$hf2vG0X1XRoIjkzacyaBmuUimgR.8fmFj6teT3Vxwzwed7WrdYNei", 1 },
                    { 11, new DateTime(2026, 5, 4, 9, 41, 3, 861, DateTimeKind.Utc).AddTicks(2194), "olivia.taylor@learnconnect.com", "Olivia", true, "Taylor", "$2a$11$2eb/GcG62tMrho.QEHnhru7GuHrpHMUk/LedNrv4sGRwjUSDQkeNK", 1 },
                    { 12, new DateTime(2026, 5, 4, 9, 41, 4, 40, DateTimeKind.Utc).AddTicks(6207), "daniel.moore@learnconnect.com", "Daniel", true, "Moore", "$2a$11$Tu/tIJUs.i.ySLODxR0ejuZ8JsXZG6qiV8rmvQ0g8Adh9pMTU2nEW", 1 },
                    { 20, new DateTime(2026, 5, 4, 9, 41, 4, 217, DateTimeKind.Utc).AddTicks(2079), "alice.walker@learnconnect.com", "Alice", true, "Walker", "$2a$11$RWaZeyH9GjJYlEquA8xj4elDjOnyuAgeN/Pgg71BaccTWbajZw2JC", 1 },
                    { 21, new DateTime(2026, 5, 4, 9, 41, 4, 377, DateTimeKind.Utc).AddTicks(142), "robert.vance@learnconnect.com", "Robert", true, "Vance", "$2a$11$g5Ja1lciDEAvY4Vl8T8enO0yKzU81PgExktagT1KVu2d48vddTcEO", 1 },
                    { 22, new DateTime(2026, 5, 4, 9, 41, 4, 573, DateTimeKind.Utc).AddTicks(8164), "john.doe@learnconnect.com", "John", true, "Doe", "$2a$11$bYF2xaZLQ.aJlQQaoXuxNe70SNd5sfRfZaejf3oFOVKF6mt8APvh2", 1 },
                    { 23, new DateTime(2026, 5, 4, 9, 41, 4, 771, DateTimeKind.Utc).AddTicks(4629), "marie.curie@learnconnect.com", "Marie", true, "Curie", "$2a$11$pY8pOY6G93RVXeh5.fqpiu7fSLx.AxRh449zFzmEk6moyf.qf2Yk6", 1 },
                    { 24, new DateTime(2026, 5, 4, 9, 41, 4, 946, DateTimeKind.Utc).AddTicks(7661), "walter.white@learnconnect.com", "Walter", true, "White", "$2a$11$/rL6QzfGNzv0Ysw7Q5qiF.CjNsOdLD5E0ECjWe5NBy5iXfI.SPWlW", 1 },
                    { 25, new DateTime(2026, 5, 4, 9, 41, 5, 154, DateTimeKind.Utc).AddTicks(2010), "heisenberg@learnconnect.com", "Werner", true, "Heisenberg", "$2a$11$dq22.L6GqCRjAIz7HICpV.stymmSccWZZnN00Vqik.C3d7Gu1Fyj.", 1 },
                    { 26, new DateTime(2026, 5, 4, 9, 41, 5, 319, DateTimeKind.Utc).AddTicks(7413), "jane.goodall@learnconnect.com", "Jane", true, "Goodall", "$2a$11$IU1RqVqtC.WT2kCdq6YxsuW.12iep1lBQKO2/WntEVqpoDuYTf7.e", 1 },
                    { 27, new DateTime(2026, 5, 4, 9, 41, 5, 513, DateTimeKind.Utc).AddTicks(4481), "gregor.mendel@learnconnect.com", "Gregor", true, "Mendel", "$2a$11$jZrv14KTMyFb1Oyuhcff5.BYmRIKXZHRpe9fVJyWoD/mrdo1q2fQS", 1 },
                    { 28, new DateTime(2026, 5, 4, 9, 41, 5, 676, DateTimeKind.Utc).AddTicks(8907), "new.grad@learnconnect.com", "Emily", true, "Dickinson", "$2a$11$vdMdIGj/GRUtJTgxXVvStehKcvfh6ZQFkh9fLFX82UO7aVwcN6T5W", 1 },
                    { 29, new DateTime(2026, 5, 4, 9, 41, 5, 868, DateTimeKind.Utc).AddTicks(1185), "shakespeare@learnconnect.com", "William", true, "Shakespeare", "$2a$11$MGLVLDR5R90GfR6Ge8yDmOUxHRXZ9EtdfQCvPuec2LPcFKZsOTb22", 1 },
                    { 30, new DateTime(2026, 5, 4, 9, 41, 6, 70, DateTimeKind.Utc).AddTicks(5029), "carlos.ruiz@learnconnect.com", "Carlos", true, "Ruiz", "$2a$11$ZuSLTelyMurLyhyDIoAZxuMhBsdIYK.LEFcFBdaYJfpR//A8i4GU.", 1 },
                    { 31, new DateTime(2026, 5, 4, 9, 41, 6, 253, DateTimeKind.Utc).AddTicks(3693), "isabela.madrigal@learnconnect.com", "Isabela", true, "Madrigal", "$2a$11$UjRMw2B8eGRdzgVpVxNSwOr2T2tPdE.V3bfCU9kpWlt8/78vJZNfq", 1 },
                    { 32, new DateTime(2026, 5, 4, 9, 41, 6, 462, DateTimeKind.Utc).AddTicks(6117), "pierre.escargot@learnconnect.com", "Pierre", true, "Escargot", "$2a$11$xZg.qYWLl5fwYb94osUepOtAqNTCO4kOZ6AgWSYA7uqcJdJcp2ovu", 1 },
                    { 33, new DateTime(2026, 5, 4, 9, 41, 6, 629, DateTimeKind.Utc).AddTicks(9706), "chef.gusteau@learnconnect.com", "Auguste", true, "Gusteau", "$2a$11$1XeBIDOhwYDI77SL4Y6fZuypMTwqWnsH4MJKlN2CAchJ95Hzwc/Nq", 1 },
                    { 34, new DateTime(2026, 5, 4, 9, 41, 6, 844, DateTimeKind.Utc).AddTicks(6827), "script.kiddie@learnconnect.com", "Kevin", true, "Mitnick", "$2a$11$BeXxxlxiXoVJYI3UzTHll.m/Z/hhT78rHn85jVUg3xjXFV/kP9Ceq", 1 },
                    { 35, new DateTime(2026, 5, 4, 9, 41, 7, 21, DateTimeKind.Utc).AddTicks(9565), "dev.ops@learnconnect.com", "Linus", true, "Torvalds", "$2a$11$F97oTGGM0SWAmv41gI5bKusc0M4ck/.C/FuzwSGM8POUsoiTz1bIq", 1 },
                    { 36, new DateTime(2026, 5, 4, 9, 41, 7, 197, DateTimeKind.Utc).AddTicks(8362), "ai.researcher@learnconnect.com", "Ada", true, "Lovelace", "$2a$11$g41WAc4vI2C0VuN5HkER6.ZWMRi58touQ3h6U6QOKHeCWkRGhAgBe", 1 },
                    { 37, new DateTime(2026, 5, 4, 9, 41, 7, 371, DateTimeKind.Utc).AddTicks(9527), "time.traveler@learnconnect.com", "Marty", true, "McFly", "$2a$11$xnZSH4R4HsphBEfnSRy7GusLcuH6eEYoqcbc.e.rFwy.MQ1MdhNLa", 1 },
                    { 38, new DateTime(2026, 5, 4, 9, 41, 7, 536, DateTimeKind.Utc).AddTicks(3245), "museum.guide@learnconnect.com", "Indiana", true, "Jones", "$2a$11$WrN3sai/yV0kUCMplfw9euBLLMZ0Iq2C2xVECQJdCDRO4hsmoQutW", 1 },
                    { 39, new DateTime(2026, 5, 4, 9, 41, 7, 701, DateTimeKind.Utc).AddTicks(5957), "book.worm@learnconnect.com", "Hermione", true, "Granger", "$2a$11$wxq622IQ8qv3OzImy1rDLO.9dkeiRmAxa6TpF9VWjNvZLKEHnzjYm", 1 },
                    { 40, new DateTime(2026, 5, 4, 9, 41, 7, 907, DateTimeKind.Utc).AddTicks(1317), "published.author@learnconnect.com", "J.K.", true, "Rowling", "$2a$11$Lfcpwm793tvO4v9EgbCsrOoZmd1rqnEX9/DM7gQxMi444UpeF14Pm", 1 },
                    { 41, new DateTime(2026, 5, 4, 9, 41, 8, 152, DateTimeKind.Utc).AddTicks(3643), "street.performer@learnconnect.com", "Ed", true, "Sheeran", "$2a$11$E52icxQWgHx8qgrCONfdUOjXjU.9z5IBrVbiwcoww0BBY90odvUYa", 1 },
                    { 42, new DateTime(2026, 5, 4, 9, 41, 8, 345, DateTimeKind.Utc).AddTicks(3957), "concert.pianist@learnconnect.com", "Ludwig", true, "Beethoven", "$2a$11$UWxLZ4qquj7MMs9JcKDIyerf.1c7BAOcZk3R/7qP3L1V0tXlR53vu", 1 },
                    { 50, new DateTime(2026, 5, 4, 9, 41, 2, 275, DateTimeKind.Utc).AddTicks(2695), "parent@test.com", "Test", true, "Parent", "$2a$11$sdu92qHB371uXecuKU5U2uj3xXz21YNrq0eHcCJ7DVwtqtu/KbnQi", 3 }
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
                name: "IX_LessonPackages_StudentId",
                table: "LessonPackages",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_LessonPackages_SubjectId",
                table: "LessonPackages",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_LessonPackages_TeacherId",
                table: "LessonPackages",
                column: "TeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_Lessons_LessonPackageId",
                table: "Lessons",
                column: "LessonPackageId");

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
                name: "IX_Payments_StudentId",
                table: "Payments",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_TeacherId",
                table: "Payments",
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
                name: "Payments");

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
                name: "LessonPackages");

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
