using EShop.Infrastructure.Data;
using EShop.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace EShop.Domain.Repositories;

public class MemberRepository : Repository<Member>, IMemberRepository
{
    public MemberRepository(IApplicationDbContext context) : base(context)
    {
    }

    public async Task<Member?> GetByUserIdAsync(string userId)
    {
        return await _dbSet
            .Include(m => m.User)
            .FirstOrDefaultAsync(m => m.UserId == userId);
    }

    public async Task<Member?> GetByEmailAsync(string email)
    {
        return await _dbSet
            .Include(m => m.User)
            .FirstOrDefaultAsync(m => m.Email == email);
    }

    public async Task<Member?> GetWithOrderHistoryAsync(int memberId)
    {
        return await _dbSet
            .Include(m => m.Orders)
                .ThenInclude(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
            .Include(m => m.PointsTransactions)
            .FirstOrDefaultAsync(m => m.Id == memberId);
    }

    public async Task<bool> UpdatePointsBalanceAsync(int memberId, int points)
    {
        var member = await _dbSet.FindAsync(memberId);
        if (member == null)
            return false;

        member.PointsBalance = points;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateMemberTierAsync(int memberId, MemberTier tier)
    {
        var member = await _dbSet.FindAsync(memberId);
        if (member == null)
            return false;

        member.Tier = tier;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<Member>> GetMembersByTierAsync(MemberTier tier)
    {
        return await _dbSet
            .Where(m => m.Tier == tier)
            .ToListAsync();
    }

    public async Task<IEnumerable<PointsTransaction>> GetPointsTransactionsAsync(int memberId)
    {
        var member = await _dbSet
            .Include(m => m.PointsTransactions)
            .FirstOrDefaultAsync(m => m.Id == memberId);

        return member?.PointsTransactions ?? Enumerable.Empty<PointsTransaction>();
    }
} 