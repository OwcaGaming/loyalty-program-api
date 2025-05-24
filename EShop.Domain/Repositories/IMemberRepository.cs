using EShop.Domain.Models;

namespace EShop.Domain.Repositories;

public interface IMemberRepository : IRepository<Member>
{
    Task<Member?> GetByUserIdAsync(string userId);
    Task<Member?> GetByEmailAsync(string email);
    Task<Member?> GetWithOrderHistoryAsync(int memberId);
    Task<bool> UpdatePointsBalanceAsync(int memberId, int points);
    Task<bool> UpdateMemberTierAsync(int memberId, MemberTier tier);
    Task<IEnumerable<Member>> GetMembersByTierAsync(MemberTier tier);
    Task<IEnumerable<PointsTransaction>> GetPointsTransactionsAsync(int memberId);
} 