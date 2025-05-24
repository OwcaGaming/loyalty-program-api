namespace EShop.Domain.Models;

public class PointsTransaction : BaseModel
{
    public int MemberId { get; set; }
    public Member Member { get; set; }
    
    public int Points { get; set; }
    
    public string Description { get; set; }
    
    public PointsTransactionType Type { get; set; }
    
    public DateTime TransactionDate { get; set; }
}

public enum PointsTransactionType
{
    Earn,
    Spend,
    Expire,
    Adjust
} 