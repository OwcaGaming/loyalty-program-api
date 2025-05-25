using System;

namespace EShop.Domain.Models;

public class RewardRedemption
{
    public int Id { get; set; }
    public int MemberId { get; set; }
    public int RewardId { get; set; }
    public DateTime RedeemedAt { get; set; }
    public int PointsCost { get; set; }

    // Navigation properties
    public Member Member { get; set; } = null!;
    public Reward Reward { get; set; } = null!;
} 