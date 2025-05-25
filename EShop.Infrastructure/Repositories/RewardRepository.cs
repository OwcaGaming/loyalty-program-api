using EShop.Domain.Models;
using EShop.Domain.Repositories;
using EShop.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EShop.Infrastructure.Repositories;

public class RewardRepository : Repository<Reward>, IRewardRepository
{
    private new readonly ApplicationDbContext _context;

    public RewardRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<List<Reward>> GetActiveRewardsAsync()
    {
        return await _context.Rewards
            .Where(r => r.IsActive && r.StockQuantity > 0)
            .OrderBy(r => r.PointsCost)
            .ToListAsync();
    }

    public async Task<List<Reward>> GetRewardsByPointsRangeAsync(int minPoints, int maxPoints)
    {
        return await _context.Rewards
            .Where(r => r.IsActive && 
                       r.StockQuantity > 0 && 
                       r.PointsCost >= minPoints && 
                       r.PointsCost <= maxPoints)
            .OrderBy(r => r.PointsCost)
            .ToListAsync();
    }

    public async Task<List<RewardRedemption>> GetMemberRedemptionsAsync(int memberId)
    {
        return await _context.RewardRedemptions
            .Include(r => r.Reward)
            .Where(r => r.MemberId == memberId)
            .OrderByDescending(r => r.RedeemedAt)
            .ToListAsync();
    }

    public async Task<RewardRedemption> AddRedemptionAsync(RewardRedemption redemption)
    {
        _context.RewardRedemptions.Add(redemption);
        await _context.SaveChangesAsync();
        return redemption;
    }
} 