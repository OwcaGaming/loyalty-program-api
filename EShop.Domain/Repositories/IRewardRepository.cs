using EShop.Domain.Models;

namespace EShop.Domain.Repositories;

public interface IRewardRepository : IRepository<Reward>
{
    Task<List<Reward>> GetActiveRewardsAsync();
    Task<List<Reward>> GetRewardsByPointsRangeAsync(int minPoints, int maxPoints);
    Task<List<RewardRedemption>> GetMemberRedemptionsAsync(int memberId);
    Task<RewardRedemption> AddRedemptionAsync(RewardRedemption redemption);
} 