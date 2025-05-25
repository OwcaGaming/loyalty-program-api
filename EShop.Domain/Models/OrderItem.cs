using System.ComponentModel.DataAnnotations;

namespace EShop.Domain.Models;

public class OrderItem : BaseModel
{
    public int OrderId { get; set; }
    public required Order Order { get; set; }
    
    public int ProductId { get; set; }
    public required Product Product { get; set; }
    
    [Required]
    public int Quantity { get; set; }
    
    [Required]
    public decimal UnitPrice { get; set; }
    
    public decimal Subtotal => Quantity * UnitPrice;
    
    public int? PointsEarned { get; set; }
} 