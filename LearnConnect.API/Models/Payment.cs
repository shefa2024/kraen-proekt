namespace LearnConnect.API.Models;

public enum PaymentMethod
{
    Card,
    PayPal,
    Apple
}

public enum PaymentStatus
{
    Pending,
    Completed,
    Failed,
    Refunded
}

public class Payment
{
    public int Id { get; set; }
    
    public int StudentId { get; set; }
    public Student Student { get; set; } = null!;
    
    public int TeacherId { get; set; }
    public Teacher Teacher { get; set; } = null!;
    
    public decimal Amount { get; set; }
    
    public PaymentMethod Method { get; set; }
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
    
    public string? TransactionId { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ProcessedAt { get; set; }
}
