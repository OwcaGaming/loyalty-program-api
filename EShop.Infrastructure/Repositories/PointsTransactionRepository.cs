using EShop.Domain.Models;
using EShop.Domain.Repositories;
using EShop.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EShop.Infrastructure.Repositories;

public class PointsTransactionRepository : Repository<PointsTransaction>, IPointsTransactionRepository
{
    private new readonly ApplicationDbContext _context;

    public PointsTransactionRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<IEnumerable<PointsTransaction>> GetByMemberIdAsync(int memberId)
    {
        return await _context.PointsTransactions
            .Where(pt => pt.MemberId == memberId)
            .OrderByDescending(pt => pt.CreatedAt)
            .ToListAsync();
    }

    public async Task<PointsTransaction> CreateTransactionAsync(int memberId, int points, string description, PointsTransactionType type)
    {
        var member = await _context.Members.FindAsync(memberId);
        if (member == null)
            throw new ArgumentException("Member not found", nameof(memberId));

        var transaction = new PointsTransaction
        {
            MemberId = memberId,
            Member = member,
            Points = points,
            Description = description,
            Type = type,
            CreatedAt = DateTime.UtcNow
        };

        _context.PointsTransactions.Add(transaction);
        await _context.SaveChangesAsync();
        return transaction;
    }
} 