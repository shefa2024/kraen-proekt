using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LearnConnect.API.Data;
using LearnConnect.API.Models;
using LearnConnect.API.DTOs;
using System.Security.Claims;
using Stripe;

namespace LearnConnect.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class PaymentsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private const string StripeSecretKey = "sk_test_51TDp6CRtJFgXdvuMEfEG9gUaCtUlkoGAGrYxGFqFA0nyYlG1ulWZIasMrIDflHlovkxFnjULrkUacUtPGteoDzCD00lgub17P5";

    public PaymentsController(ApplicationDbContext context)
    {
        _context = context;
        StripeConfiguration.ApiKey = StripeSecretKey;
    }

    [HttpPost("create-payment-intent")]
    public async Task<IActionResult> CreatePaymentIntent([FromBody] PaymentRequestDto request)
    {
        var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdStr, out int userId)) return Unauthorized();

        var student = await _context.Students.Include(s => s.User).FirstOrDefaultAsync(s => s.UserId == userId);
        if (student == null) return BadRequest(new { message = "Only students can make payments." });

        var teacher = await _context.Teachers.FindAsync(request.TeacherId);
        if (teacher == null) return NotFound(new { message = "Teacher not found." });

        if (request.Amount <= 0) return BadRequest(new { message = "Invalid payment amount." });

        try
        {
            var options = new PaymentIntentCreateOptions
            {
                Amount = (long)(request.Amount * 100), // Stripe uses cents
                Currency = "usd",
                AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                {
                    Enabled = true,
                },
                Metadata = new Dictionary<string, string>
                {
                    { "StudentId", student.Id.ToString() },
                    { "TeacherId", teacher.Id.ToString() }
                }
            };

            var service = new PaymentIntentService();
            var paymentIntent = await service.CreateAsync(options);

            // Create a pending payment record in our DB
            var payment = new Payment
            {
                StudentId = student.Id,
                TeacherId = teacher.Id,
                Amount = request.Amount,
                Method = Models.PaymentMethod.Card, // Default for now
                Status = PaymentStatus.Pending,
                TransactionId = paymentIntent.Id,
                ProcessedAt = DateTime.UtcNow
            };

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                clientSecret = paymentIntent.ClientSecret,
                paymentId = payment.Id
            });
        }
        catch (StripeException ex)
        {
            return BadRequest(new { message = ex.StripeError.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An internal server error occurred while processing the payment.", details = ex.Message });
        }
    }

    [HttpPost("confirm/{paymentId}")]
    public async Task<IActionResult> ConfirmPayment(int paymentId)
    {
        var payment = await _context.Payments.FindAsync(paymentId);
        if (payment == null) return NotFound();

        payment.Status = PaymentStatus.Completed;
        payment.ProcessedAt = DateTime.UtcNow;
        
        await _context.SaveChangesAsync();
        return Ok(new { message = "Payment confirmed in database." });
    }
}
