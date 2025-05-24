namespace EShop.Domain.Models;

public class Reward : BaseModel
{
    public string Name { get; set; }
    
    public string Description { get; set; }
    
    public int PointsCost { get; set; }
    
    public bool IsActive { get; set; }
    
    public DateTime? StartDate { get; set; }
    
    public DateTime? EndDate { get; set; }
    
    public int? StockQuantity { get; set; }
    
    public string? ImageUrl { get; set; }
    
    public RewardType Type { get; set; }
    
    public decimal? DiscountAmount { get; set; }
    
    public decimal? DiscountPercentage { get; set; }
}

public enum RewardType
{
    Discount,
    FreeProduct,
    Service,
    Experience
} 