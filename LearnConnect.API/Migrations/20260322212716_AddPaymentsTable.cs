using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LearnConnect.API.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    TeacherId = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Method = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    TransactionId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProcessedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
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

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 3, 22, 21, 26, 59, 532, DateTimeKind.Utc).AddTicks(2553), "$2a$11$0eMDP.67Qv5QelJtomEOpOuR6SXvlVdHyDBMfnOLNxkUFx0WMA0Ka" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 3, 22, 21, 26, 59, 898, DateTimeKind.Utc).AddTicks(9363), "$2a$11$KyPhg159sn2A3iPtggsRqOG4Ytpdxc8WE8a4JAhAK0dE/4klxC/4y" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 3, 22, 21, 27, 0, 743, DateTimeKind.Utc).AddTicks(6929), "$2a$11$GP1sPUdeLXqZkLPb8082AO39mZi8sMEAJX/qn.pI5ayIwg9qFKlS." });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 3, 22, 21, 27, 1, 217, DateTimeKind.Utc).AddTicks(7509), "$2a$11$x4cKDkJHwrqxIChmeECrkeLLUQskLMHZpvrAmyGOdV9/qxfppdrsK" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 3, 22, 21, 27, 1, 661, DateTimeKind.Utc).AddTicks(2594), "$2a$11$lHfugm66S8BEXzrNjvKoH.EZDI6IQMtmOw1geVGUvU7234JvsvRBC" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 3, 22, 21, 27, 2, 58, DateTimeKind.Utc).AddTicks(8218), "$2a$11$eYiO6hYOfichkAsgVApPNe4bofElaSQNXbM8zNZoFYMpBG4Ge/VhO" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 3, 22, 21, 27, 3, 257, DateTimeKind.Utc).AddTicks(8342), "$2a$11$OEIb/B8SgnZ4sqBDXnLf9.WiMqjuuDxBxQ8k9AoMqSt.GqDU.WDuS" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 3, 22, 21, 27, 3, 645, DateTimeKind.Utc).AddTicks(1894), "$2a$11$dQMwzKK8jSDeKBe9DDJ5hunPcA.YY4XY/E6PUlalBJU4WwquW2M6m" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 3, 22, 21, 27, 3, 990, DateTimeKind.Utc).AddTicks(6281), "$2a$11$f2DihjqX.avDL.8.Sa9.d.dkJh6YtM.MCoiHgEoUls7AplJUl6UPe" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 3, 22, 21, 27, 4, 317, DateTimeKind.Utc).AddTicks(7288), "$2a$11$lOdCDBuVT3ZBXTc.y/JbPuF9YrYYYwdW.SLesaosiVDISLBkY/TaO" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 3, 22, 21, 27, 4, 647, DateTimeKind.Utc).AddTicks(3631), "$2a$11$dIzoounLVjLmk6u3mRM2heErYzZ0RfByDDfFTy9aZZZAYu62XhUk2" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 3, 22, 21, 27, 4, 968, DateTimeKind.Utc).AddTicks(6585), "$2a$11$7VacmbYiwfPYV1Ims0umpue5wj2f/v9UG6LfwisI1v0gI6KVf4pyq" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 20,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 3, 22, 21, 27, 5, 190, DateTimeKind.Utc).AddTicks(7564), "$2a$11$UIFAwQK9vtD/em02ykdOI.319TsZdep/orrwvek/hfypBpHiDjztq" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 21,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 3, 22, 21, 27, 5, 683, DateTimeKind.Utc).AddTicks(1436), "$2a$11$FtfIrl6p5N7AMuYGO9b25OZhr24QwXqnG6rE93u.flv81XTxje63e" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 22,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 3, 22, 21, 27, 6, 96, DateTimeKind.Utc).AddTicks(4894), "$2a$11$X/vESYSw1CIex8gKRJ2Hj.kJoqjO165WHO64mEs6c9bwSJb996LKO" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 23,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 3, 22, 21, 27, 6, 505, DateTimeKind.Utc).AddTicks(5278), "$2a$11$tHYe1L0bE6yE5sFJ1BtwW.mdFSjVfbQw2ZkcqpO1.3N4.hqMA9idS" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 24,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 3, 22, 21, 27, 6, 808, DateTimeKind.Utc).AddTicks(4807), "$2a$11$hmM38RDEqFgnIMaZvqnkvutPUjwn2J/AMyjR71WojHMoXnK88NlHC" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 25,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 3, 22, 21, 27, 7, 88, DateTimeKind.Utc).AddTicks(7667), "$2a$11$Abhrhsgp/KoMIXQV6.5QkOS1NFl1DFsf34cErUJ5Cin7n2FDLkr9e" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 26,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 3, 22, 21, 27, 7, 289, DateTimeKind.Utc).AddTicks(488), "$2a$11$aFaM7qf0N49ggUw8dWDVK.MMSEPW7ppANFWfVw27rbyxuFTlssbgC" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 27,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 3, 22, 21, 27, 7, 608, DateTimeKind.Utc).AddTicks(9638), "$2a$11$JSOvFRzNqK7RrAIMbAV.rObWGoHZ21n8hQ0w7nXa3eGyATl2E857u" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 28,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 3, 22, 21, 27, 7, 941, DateTimeKind.Utc).AddTicks(8894), "$2a$11$xUB4VhuRChuW150Ri4rutOHRRMm/nE6dcR49jJG/j691mRtot1tdG" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 29,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 3, 22, 21, 27, 9, 19, DateTimeKind.Utc).AddTicks(2272), "$2a$11$1Q1wAgCXg43QMQxkmdtCButZIWGg6vCa0JaotAT8JL0lEr./TO7c6" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 30,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 3, 22, 21, 27, 9, 518, DateTimeKind.Utc).AddTicks(859), "$2a$11$IgZOpwL7DQU77MWhQcEQQ.5ZwQmbfAtyxvKCbPSsiMCfLxNAwDVHi" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 31,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 3, 22, 21, 27, 10, 91, DateTimeKind.Utc).AddTicks(6082), "$2a$11$YaqEOOBsEDIc4xYzg/MYeOz7zDP8EXWmA2vFmZGkpB64kot0K6Z2y" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 32,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 3, 22, 21, 27, 10, 271, DateTimeKind.Utc).AddTicks(4622), "$2a$11$rUBpSRa8O9bndbJ6trc97OpNaP4ThoOVel.FUtCIbQvjizC8hm5u2" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 33,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 3, 22, 21, 27, 10, 691, DateTimeKind.Utc).AddTicks(9795), "$2a$11$t91VH8OsQJYzpJ4PGRiGQO26WXSehYBLwgN3lAs6f6u9LQ4JapPCS" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 34,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 3, 22, 21, 27, 11, 8, DateTimeKind.Utc).AddTicks(35), "$2a$11$xfDZJAUzWNVmddTMm3ZyZ.R8WNsnnXb56z52eIqeR0wLcGmk13UyC" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 35,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 3, 22, 21, 27, 11, 302, DateTimeKind.Utc).AddTicks(6056), "$2a$11$C7AiS/jcxSr/GtcFjsbKw.UFVWr4c/UwERN.8AxDTHm3PvEMXQppe" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 36,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 3, 22, 21, 27, 11, 673, DateTimeKind.Utc).AddTicks(9548), "$2a$11$grOueZDfjYoZA3VXhU4WT.7KIpAjVWyvb/ff9M4YKc9IMMeXWgFNq" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 37,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 3, 22, 21, 27, 12, 61, DateTimeKind.Utc).AddTicks(3418), "$2a$11$7S8vqZsj70FBPjyPKdWj1.0uubkJ3hOaKi1OCelEnrjT4jfeXngim" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 38,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 3, 22, 21, 27, 12, 377, DateTimeKind.Utc).AddTicks(9519), "$2a$11$BiOcINEuBChcpol3l2DDoeLqjB7wjTTH40Vw4Qbt7F1LZmMAg1an2" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 39,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 3, 22, 21, 27, 12, 685, DateTimeKind.Utc).AddTicks(7086), "$2a$11$rIvLwv.Bax9w5T8VkXCOcunN9Mh5geIOvNs3pY0Dq22Z8SOkctVVO" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 40,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 3, 22, 21, 27, 12, 997, DateTimeKind.Utc).AddTicks(8653), "$2a$11$Hc1SaDnVkIYVeRHgCh.QaOIiDaSMem2vssYGEE07TJypS7z7ZBdYy" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 41,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 3, 22, 21, 27, 13, 280, DateTimeKind.Utc).AddTicks(9618), "$2a$11$qlWTsv9CDJflh8FgWsgTLekhrmvxfBuczpY0GrbwqKIUbnWh0SDNW" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 42,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 3, 22, 21, 27, 13, 714, DateTimeKind.Utc).AddTicks(8029), "$2a$11$X8g1/zpp8uVVrxmkDx0YnuyfxNXVw2PE8EAD2dXDVb5YCjlPuNpvK" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 50,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 3, 22, 21, 27, 0, 228, DateTimeKind.Utc).AddTicks(6249), "$2a$11$v/L3pPAcSAlcJiI3DHl89elY0Qr5qIuxHs1XH5xLnu9ySSTIVq1Uu" });

            migrationBuilder.CreateIndex(
                name: "IX_Payments_StudentId",
                table: "Payments",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_TeacherId",
                table: "Payments",
                column: "TeacherId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 2, 26, 8, 41, 39, 721, DateTimeKind.Utc).AddTicks(5393), "$2a$11$o05GVdsdThXmsIcK26bvHe0rVj.UFyI0KuuRLajSvoaaMyrLoqwGC" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 2, 26, 8, 41, 39, 934, DateTimeKind.Utc).AddTicks(484), "$2a$11$RYCxzT9nDuGRFydE8Rrcj.TJY5QsZODcTCTybzShhPhVP2/PKBxLK" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 2, 26, 8, 41, 40, 384, DateTimeKind.Utc).AddTicks(2275), "$2a$11$rwLPymy5/nLX6GIqYbewGOSIHn4chqUeW3Ub8/l5sGXcM9mXSSmy6" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 2, 26, 8, 41, 40, 623, DateTimeKind.Utc).AddTicks(7364), "$2a$11$ybu3WyaClQC5oTR4Sl5VPOEkuW1vsCxIR.LE3kK33.8QTfO0rCSW6" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 2, 26, 8, 41, 40, 923, DateTimeKind.Utc).AddTicks(1336), "$2a$11$IPU5xF5j/hlwrqcxlY4cvOs0XrkL3ROfKjrX7xsBwJ35arUKgl/GW" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 2, 26, 8, 41, 41, 157, DateTimeKind.Utc).AddTicks(3448), "$2a$11$wI9M.DwjaZuXaGFE7vUz/On1mN.LBehQZaLRsjZhEO4uOXXl35/am" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 2, 26, 8, 41, 41, 451, DateTimeKind.Utc).AddTicks(1181), "$2a$11$2Cfh.p4pNMXwyhFP6EPsDe6nKkINBw9DWFAf.ValsYzBycFjI7RxS" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 2, 26, 8, 41, 41, 691, DateTimeKind.Utc).AddTicks(8244), "$2a$11$BQZ82pp/..yLITKaHVHgAeEayO6uGk0hMlG5K3b41fSlomsVl7Cm6" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 2, 26, 8, 41, 41, 929, DateTimeKind.Utc).AddTicks(1397), "$2a$11$2tUvzRh6smu73bfDmaC4..d5ZcdFk3QvNm1FVzKHw00GVk9yP89zK" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 2, 26, 8, 41, 42, 89, DateTimeKind.Utc).AddTicks(5014), "$2a$11$nKdbnMdB2iUzf4W9th4foel/puyYs1IjIIQLBN7rQn32cDbjm3AsC" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 2, 26, 8, 41, 42, 280, DateTimeKind.Utc).AddTicks(2871), "$2a$11$t3fkQqvHNtKVEAy6eesleOWSA1rOhHqGfWGQy2M3Uf9fSaApN3saO" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 2, 26, 8, 41, 42, 443, DateTimeKind.Utc).AddTicks(7453), "$2a$11$aZw3zZ628E7UL0su0OZDEO3tQX6L0.QV2DMY9t54ZMfrR8hRDfcxm" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 20,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 2, 26, 8, 41, 42, 630, DateTimeKind.Utc).AddTicks(4226), "$2a$11$oZViqyN8hO0HhDEKPz65XORrPyxWiPZyU4alUDa2rCzLXQ1QNmYrO" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 21,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 2, 26, 8, 41, 42, 791, DateTimeKind.Utc).AddTicks(7481), "$2a$11$lWQoDoeLAkNKZEhgkXI4FuJoD3RKX2lj3R4gnYaIajgZU77whDPou" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 22,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 2, 26, 8, 41, 42, 975, DateTimeKind.Utc).AddTicks(5542), "$2a$11$cMykedgIuxyVlHkEWr1GPeY6cy9x9gSbGWIb0M30oyfCZ4Vt4HU46" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 23,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 2, 26, 8, 41, 43, 138, DateTimeKind.Utc).AddTicks(9834), "$2a$11$rc.aXpJkaBeIfFgwPZgv9OeYQrjNs3X3feAfTFoJAAyuBb96r69I." });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 24,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 2, 26, 8, 41, 43, 366, DateTimeKind.Utc).AddTicks(8933), "$2a$11$UGLYTLUT6ksWbgqZFiLcHOW4rYFErrDMV5sXfO/zZ9hXaG1Swl1hC" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 25,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 2, 26, 8, 41, 43, 525, DateTimeKind.Utc).AddTicks(6720), "$2a$11$wmJS9ni6r.O2XSVkNkvLNeS/3laRcf0928aGFG.AvGyLGn.EnpdS6" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 26,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 2, 26, 8, 41, 43, 790, DateTimeKind.Utc).AddTicks(1953), "$2a$11$p5FL4Lqn8aivwe44ALwRc.SAdJGhdvWiwaHtObcxde4y0N4VJayRm" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 27,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 2, 26, 8, 41, 43, 980, DateTimeKind.Utc).AddTicks(9971), "$2a$11$XEVczyR8X9b6ymKX3TTBWOk0s0dLDH5Yj1azZCBbHHpxWYwJ8.A7S" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 28,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 2, 26, 8, 41, 44, 292, DateTimeKind.Utc).AddTicks(9777), "$2a$11$VAZDCP7O6xBuAdeV0.u2OORRcJSzFz7jjFzSs0z1mK28H9RlZUz4q" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 29,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 2, 26, 8, 41, 44, 502, DateTimeKind.Utc).AddTicks(7092), "$2a$11$pTo.xI9ax30l7Dx74QN/o.1FWu5FPMUw.Sj8RXkxJ0sbaijDEEUAK" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 30,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 2, 26, 8, 41, 44, 663, DateTimeKind.Utc).AddTicks(1082), "$2a$11$40g0va.uWMuWfaiqQr4pkuY73xk3h3PqSCIRe1ikk6DdsVJn7/5HC" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 31,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 2, 26, 8, 41, 44, 841, DateTimeKind.Utc).AddTicks(6071), "$2a$11$5yydyXC7ssx7takPDmVjYuBse2EahTgB1KQ.s18JUj5cDko4l0uBi" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 32,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 2, 26, 8, 41, 45, 10, DateTimeKind.Utc).AddTicks(7250), "$2a$11$ANm.ARSJg6AurkLKJC63H.N6oeR3FBHhmw3ASP8hau3hWA898XZcC" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 33,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 2, 26, 8, 41, 45, 175, DateTimeKind.Utc).AddTicks(3863), "$2a$11$VupLELbXd1l.AvoRYlPwde80.dWGiaSNVTdg3k0Ac6WtLhuwqvn0C" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 34,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 2, 26, 8, 41, 45, 428, DateTimeKind.Utc).AddTicks(8845), "$2a$11$eowmstJOr11ETd7FlUkPt.LdKDsbypmqiDWRvUIybgOeK1YyHWPzu" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 35,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 2, 26, 8, 41, 45, 723, DateTimeKind.Utc).AddTicks(8854), "$2a$11$hZl5dRdewj67uvpVkrUjGuP3qfUkIIkHVgeoxHFKjJlOJ29FDlGfa" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 36,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 2, 26, 8, 41, 45, 961, DateTimeKind.Utc).AddTicks(1650), "$2a$11$0Xb3dgsv0jA2UL9DSii7xeiVRw.vBzfMbZetM5icZCpkX9V/Curve" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 37,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 2, 26, 8, 41, 46, 160, DateTimeKind.Utc).AddTicks(9166), "$2a$11$d.rDmU7o1Rh3FVLW7C9CGeOGFfKl38jBAHrSgdaWhzndzD2VUKdYm" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 38,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 2, 26, 8, 41, 46, 365, DateTimeKind.Utc).AddTicks(346), "$2a$11$uVhvmTmMMT8qoPW39BRcW.Ugdkl9QyHP67l8t0TksJgZtkxYQ74fG" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 39,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 2, 26, 8, 41, 46, 575, DateTimeKind.Utc).AddTicks(4972), "$2a$11$ee6KlPgnoK.uS9GGsWY2Hez01rbGsvwA7KTvnywqkbmHugT3hKJEG" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 40,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 2, 26, 8, 41, 46, 852, DateTimeKind.Utc).AddTicks(2628), "$2a$11$FujJ..O.vYlxIHA0P7LMEuVpDkJxYv/cW4ypjUN7ga7MubPSUtG/q" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 41,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 2, 26, 8, 41, 47, 79, DateTimeKind.Utc).AddTicks(3074), "$2a$11$620ggWZ7RzSLEnGeGA41Wep9oOdpb/LPXUo1Sdk/Uc56btXp5V00K" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 42,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 2, 26, 8, 41, 47, 402, DateTimeKind.Utc).AddTicks(3728), "$2a$11$RmH1CeUY.Aus.aY1YEo1k.rw7Gwn0GKaXvYgQY9MYp50sC1AiVAE." });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 50,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 2, 26, 8, 41, 40, 177, DateTimeKind.Utc).AddTicks(6108), "$2a$11$Sky1vCjyAJqdAa77x406HuEPk/yCmDXkLJFItRd8dLbZ1yThx/Gey" });
        }
    }
}
