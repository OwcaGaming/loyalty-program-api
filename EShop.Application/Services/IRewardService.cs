using EShop.Domain.Models;

namespace EShop.Application.Services;

public interface IRewardService
{
    Task<List<Reward>> GetAllRewardsAsync();
    Task<List<Reward>> GetActiveRewardsAsync();
    Task<Reward?> GetRewardAsync(int id);
    Task<Reward?> AddRewardAsync(Reward reward);
    Task<Reward?> UpdateRewardAsync(Reward reward);
    Task<bool> DeleteRewardAsync(int id);
    Task<bool> RedeemRewardAsync(int memberId, int rewardId);
    Task<List<RewardRedemption>> GetMemberRedemptionsAsync(int memberId);
    Task<bool> UpdateStockQuantityAsync(int rewardId, int quantity);
    Task<List<Reward>> GetRewardsByPointsRangeAsync(int minPoints, int maxPoints);
} 