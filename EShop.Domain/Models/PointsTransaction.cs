namespace EShopDomain.Models
{
    public enum PointsTransactionType { Earn, Spend }
    public class PointsTransaction
    {
        public int Id { get; set; }
        public int MemberId { get; set; }
        public int Points { get; set; }
        public PointsTransactionType Type { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime Date { get; set; } = DateTime.UtcNow;
    }
} 