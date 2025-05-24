using System.ComponentModel.DataAnnotations;

namespace EShop.Domain.Models;

public class Order : BaseModel
{
    [Required]
    public string OrderNumber { get; set; }
    
    public int MemberId { get; set; }
    public Member Member { get; set; }
    
    public OrderStatus Status { get; set; }
    
    [Required]
    public decimal TotalAmount { get; set; }
    
    public decimal? DiscountAmount { get; set; }
    
    public int? PointsUsed { get; set; }
    
    public int? PointsEarned { get; set; }
    
    public string ShippingAddress { get; set; }
    
    public string BillingAddress { get; set; }
    
    public PaymentStatus PaymentStatus { get; set; }
    
    public string? PaymentTransactionId { get; set; }
    
    public DateTime? PaidAt { get; set; }
    
    public ICollection<OrderItem> OrderItems { get; set; }
    
    public Invoice Invoice { get; set; }
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