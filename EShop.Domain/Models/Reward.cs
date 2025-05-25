using System.ComponentModel.DataAnnotations;

namespace EShop.Domain.Models;

public enum RewardType
{
    Discount,
    FreeProduct,
    Service,
    Experience
}

public class Reward : BaseModel
{
    [Required]
    public required string Name { get; set; }
    
    public required string Description { get; set; }
    
    [Required]
    public decimal DiscountAmount { get; set; }
    
    public int PointsCost { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    public DateTime? ExpiryDate { get; set; }
    
    public int? UsageLimit { get; set; }
    
    public int UsageCount { get; set; }
    
    public int? StockQuantity { get; set; }
    
    public string? ImageUrl { get; set; }
    
    public RewardType Type { get; set; }
    
    public decimal? DiscountPercentage { get; set; }

    public ICollection<RewardRedemption> Redemptions { get; set; } = new List<RewardRedemption>();
} 