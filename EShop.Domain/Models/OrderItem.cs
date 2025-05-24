using System.ComponentModel.DataAnnotations;

namespace EShop.Domain.Models;

public class OrderItem : BaseModel
{
    public int OrderId { get; set; }
    public Order Order { get; set; }
    
    public int ProductId { get; set; }
    public Product Product { get; set; }
    
    [Required]
    public int Quantity { get; set; }
    
    [Required]
    public decimal UnitPrice { get; set; }
    
    public decimal Subtotal => Quantity * UnitPrice;
    
    public int? PointsEarned { get; set; }
} 