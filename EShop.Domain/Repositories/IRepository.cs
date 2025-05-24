using EShop.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace EShop.Domain.Repositories
{
    public interface IRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        Task<T> AddAsync(T entity);
        Task<T> UpdateAsync(T entity);
        Task DeleteAsync(T entity);
        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
        Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null);
    }

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
