using EShop.Domain.Models;

namespace EShop.Application.Services;

public interface IMemberService : IService<Member>
{
    Task<Member?> GetByUserIdAsync(string userId);
    Task<Member?> GetByEmailAsync(string email);
    Task<Member?> GetWithOrderHistoryAsync(int memberId);
    Task<bool> UpdatePointsBalanceAsync(int memberId, int points);
    Task<bool> AddPointsTransactionAsync(int memberId, int points, string description);
    Task<bool> UpdateMemberTierAsync(int memberId, MemberTier tier);
    Task<IEnumerable<Member>> GetMembersByTierAsync(MemberTier tier);
    Task<IEnumerable<PointsTransaction>> GetPointsTransactionsAsync(int memberId);
    Task<MemberTier> CalculateMemberTierAsync(int memberId);
    Task<decimal> GetTotalSpentAsync(int memberId);
    Task<int> GetTotalPointsEarnedAsync(int memberId);
} 