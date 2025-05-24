using System.ComponentModel.DataAnnotations;
using EShop.Domain.Models;

namespace EShop.Domain.Models;

public class Member : BaseModel
{
    [Required]
    public string Name { get; set; }
    
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    
    public int PointsBalance { get; set; }
    
    public DateTime DateJoined { get; set; }
    
    public string? PhoneNumber { get; set; }
    
    public string? DefaultShippingAddress { get; set; }
    
    public string? DefaultBillingAddress { get; set; }
    
    public MemberTier Tier { get; set; }
    
    public string? UserId { get; set; }
    public User User { get; set; }
    
    public ICollection<Order> Orders { get; set; }
    public ICollection<PointsTransaction> PointsTransactions { get; set; }
}

public enum MemberTier
{
    Standard,
    Silver,
    Gold,
    Platinum
} 