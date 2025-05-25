using System.ComponentModel.DataAnnotations;
using EShop.Domain.Models;

namespace EShop.Domain.Models;

public class Member : BaseModel
{
    [Required]
    public required string Name { get; set; }
    
    [Required]
    [EmailAddress]
    public required string Email { get; set; }
    
    public int PointsBalance { get; set; }
    
    public DateTime DateJoined { get; set; } = DateTime.UtcNow;
    
    public string? PhoneNumber { get; set; }
    
    public string? DefaultShippingAddress { get; set; }
    
    public string? DefaultBillingAddress { get; set; }
    
    public MemberTier Tier { get; set; } = MemberTier.Standard;
    
    public string? UserId { get; set; }
    public User? User { get; set; }
    
    public ICollection<Order> Orders { get; set; } = new List<Order>();
    public ICollection<PointsTransaction> PointsTransactions { get; set; } = new List<PointsTransaction>();
    public ICollection<RewardRedemption> RewardRedemptions { get; set; } = new List<RewardRedemption>();
}

public enum MemberTier
{
    Standard,
    Silver,
    Gold,
    Platinum
} 