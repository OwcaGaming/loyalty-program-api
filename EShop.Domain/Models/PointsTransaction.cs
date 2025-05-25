using System.ComponentModel.DataAnnotations;

namespace EShop.Domain.Models;

public class PointsTransaction : BaseModel
{
    public int MemberId { get; set; }
    public required Member Member { get; set; }
    
    public int Points { get; set; }
    
    public required string Description { get; set; }
    
    public PointsTransactionType Type { get; set; }
    
    public DateTime TransactionDate { get; set; } = DateTime.UtcNow;
}

public enum PointsTransactionType
{
    Earn,
    Spend,
    Expire,
    Adjust
} 