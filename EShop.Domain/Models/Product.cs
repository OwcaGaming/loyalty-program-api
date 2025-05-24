using System.ComponentModel.DataAnnotations;

namespace EShop.Domain.Models;

public class Product : BaseModel
{
    [Required]
    public string Name { get; set; }
    
    public string Description { get; set; }
    
    [Required]
    public decimal Price { get; set; }
    
    public string SKU { get; set; }
    
    [Required]
    public int StockQuantity { get; set; }
    
    public bool IsAvailable { get; set; }
    
    public string ImageUrl { get; set; }
    
    public int CategoryId { get; set; }
    public Category Category { get; set; }
    
    public int? RewardPointsEarned { get; set; }
    
    public ICollection<OrderItem> OrderItems { get; set; }
} 