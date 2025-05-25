using EShop.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EShop.Application.Service
{
    public interface IRewardService
    {
        Task<List<Reward>> GetAllRewardsAsync();
        Task<List<Reward>> GetActiveRewardsAsync();
        Task<Reward?> GetRewardAsync(int id);
        Task<Reward> AddRewardAsync(Reward reward);
        Task<Reward> UpdateRewardAsync(Reward reward);
        Task DeleteRewardAsync(int id);
    }
} 