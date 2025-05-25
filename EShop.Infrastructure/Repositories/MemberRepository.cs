using EShop.Domain.Models;
using EShop.Domain.Repositories;
using EShop.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace EShop.Infrastructure.Repositories;

public class MemberRepository : Repository<Member>, IMemberRepository
{
    private new readonly ApplicationDbContext _context;

    public MemberRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<Member?> GetByUserIdAsync(string userId)
    {
        return await _context.Members
            .FirstOrDefaultAsync(m => m.UserId == userId);
    }

    public async Task<Member?> GetByEmailAsync(string email)
    {
        return await _context.Members
            .FirstOrDefaultAsync(m => m.Email == email);
    }

    public async Task<Member?> GetWithOrderHistoryAsync(int memberId)
    {
        return await _context.Members
            .Include(m => m.Orders)
            .FirstOrDefaultAsync(m => m.Id == memberId);
    }

    public async Task<bool> UpdatePointsBalanceAsync(int memberId, int points)
    {
        var member = await _context.Members.FindAsync(memberId);
        if (member == null)
            return false;

        member.PointsBalance = points;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateMemberTierAsync(int memberId, MemberTier tier)
    {
        var member = await _context.Members.FindAsync(memberId);
        if (member == null)
            return false;

        member.Tier = tier;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<Member>> GetMembersByTierAsync(MemberTier tier)
    {
        return await _context.Members
            .Where(m => m.Tier == tier)
            .ToListAsync();
    }

    public async Task<IEnumerable<PointsTransaction>> GetPointsTransactionsAsync(int memberId)
    {
        return await _context.PointsTransactions
            .Where(pt => pt.MemberId == memberId)
            .OrderByDescending(pt => pt.CreatedAt)
            .ToListAsync();
    }
} 