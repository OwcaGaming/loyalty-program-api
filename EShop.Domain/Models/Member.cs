namespace EShopDomain.Models
{
    public class Member
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int PointsBalance { get; set; } = 0;
        public DateTime DateJoined { get; set; } = DateTime.UtcNow;
    }
} 