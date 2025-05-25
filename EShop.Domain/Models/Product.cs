using System.ComponentModel.DataAnnotations;

namespace EShop.Domain.Models;

public class Product : BaseModel
{
    [Required]
    public required string Name { get; set; }
    
    public required string Description { get; set; }
    
    [Required]
    public decimal Price { get; set; }
    
    public required string SKU { get; set; }
    
    [Required]
    public int StockQuantity { get; set; }
    
    public bool IsAvailable { get; set; } = true;
    
    public string? ImageUrl { get; set; }
    
    public int CategoryId { get; set; }
    public required Category Category { get; set; }
    
    public int? RewardPointsEarned { get; set; }
    
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
} 