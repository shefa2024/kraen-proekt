using Xunit;
using Moq;
using Microsoft.EntityFrameworkCore;
using LearnConnect.API.Controllers;
using LearnConnect.API.Data;
using LearnConnect.API.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace LearnConnect.Tests;

public class ReservationsTests
{
    private ApplicationDbContext GetInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new ApplicationDbContext(options);
    }

    [Fact]
    public async Task CreateReservation_ReturnsBadRequest_WhenStudentNotFound()
    {
        // Arrange
        var context = GetInMemoryContext();
        var controller = new ReservationsController(context);

        // Mock User Identity
        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.NameIdentifier, "1"),
            new Claim(ClaimTypes.Role, "Student")
        }));
        controller.ControllerContext = new ControllerContext()
        {
            HttpContext = new DefaultHttpContext() { User = user }
        };

        var request = new CreateReservationRequest
        {
            TeacherId = 1,
            RequestedDateTime = DateTime.UtcNow.AddDays(1),
            DurationMinutes = 60
        };

        // Act
        var result = await controller.CreateReservation(request);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }
}
