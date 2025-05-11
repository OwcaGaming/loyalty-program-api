using EShopDomain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EShop.Application.Service
{
    public interface IRewardService
    {
        Task<Reward> AddRewardAsync(Reward reward);
        Task<Reward?> GetRewardAsync(int id);
        Task<List<Reward>> GetAllRewardsAsync();
        Task<List<Reward>> GetActiveRewardsAsync();
        Task<Reward> UpdateRewardAsync(Reward reward);
        Task DeleteRewardAsync(int id);
    }
} 