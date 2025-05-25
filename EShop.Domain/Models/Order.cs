using System.ComponentModel.DataAnnotations;

namespace EShop.Domain.Models;

public class Order : BaseModel
{
    [Required]
    public required string OrderNumber { get; set; }
    
    public int MemberId { get; set; }
    public required Member Member { get; set; }
    
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    
    [Required]
    public decimal TotalAmount { get; set; }
    
    public decimal? DiscountAmount { get; set; }
    
    public int? PointsUsed { get; set; }
    
    public int? PointsEarned { get; set; }
    
    public required string ShippingAddress { get; set; }
    
    public required string BillingAddress { get; set; }
    
    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;
    
    public string? PaymentTransactionId { get; set; }
    
    public DateTime? PaidAt { get; set; }
    
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    
    public Invoice? Invoice { get; set; }
}

public enum OrderStatus
{
    Pending,
    Processing,
    Shipped,
    Delivered,
    Cancelled
}

public enum PaymentStatus
{
    Pending,
    Processing,
    Completed,
    Failed,
    Refunded
} 