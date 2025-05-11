using EShopDomain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EShop.Domain.Repositories
{
    public interface IRepository
    {
        // Member
        Task<Member> AddMemberAsync(Member member);
        Task<Member?> GetMemberAsync(int id);
        Task<List<Member>> GetAllMembersAsync();
        Task<Member> UpdateMemberAsync(Member member);
        Task DeleteMemberAsync(int id);

        // PointsTransaction
        Task<PointsTransaction> AddPointsTransactionAsync(PointsTransaction transaction);
        Task<List<PointsTransaction>> GetPointsTransactionsByMemberAsync(int memberId);
        Task<List<PointsTransaction>> GetAllPointsTransactionsAsync();

        // Reward
        Task<Reward> AddRewardAsync(Reward reward);
        Task<Reward?> GetRewardAsync(int id);
        Task<List<Reward>> GetAllRewardsAsync();
        Task<List<Reward>> GetActiveRewardsAsync();
        Task<Reward> UpdateRewardAsync(Reward reward);
        Task DeleteRewardAsync(int id);

        // Points operations
        Task EarnPointsAsync(int memberId, int points, string description);
        Task<bool> SpendPointsAsync(int memberId, int points, string description);
    }
}
