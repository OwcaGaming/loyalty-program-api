using EShop.Domain.Data;
using EShop.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace EShop.Domain.Repositories;

public class PointsTransactionRepository : Repository<PointsTransaction>, IPointsTransactionRepository
{
    private readonly IMemberRepository _memberRepository;

    public PointsTransactionRepository(IApplicationDbContext context, IMemberRepository memberRepository) 
        : base(context)
    {
        _memberRepository = memberRepository;
    }

    public async Task<IEnumerable<PointsTransaction>> GetByMemberIdAsync(int memberId)
    {
        return await _dbSet
            .Where(t => t.MemberId == memberId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<PointsTransaction> CreateTransactionAsync(int memberId, int points, string description, PointsTransactionType type)
    {
        var member = await _memberRepository.GetByIdAsync(memberId);
        if (member == null)
            throw new ArgumentException($"Member with ID {memberId} not found");

        var transaction = new PointsTransaction
        {
            Member = member,
            MemberId = memberId,
            Points = points,
            Description = description,
            Type = type
        };

        return await AddAsync(transaction);
    }
} 