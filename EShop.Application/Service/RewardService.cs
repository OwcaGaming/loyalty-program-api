using EShop.Domain.Models;
using EShop.Domain.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EShop.Application.Service
{
    public class RewardService : IRewardService
    {
        private readonly IRepository<Reward> _repository;

        public RewardService(IRepository<Reward> repository)
        {
            _repository = repository;
        }

        public async Task<Reward> AddRewardAsync(Reward reward)
        {
            return await _repository.AddAsync(reward);
        }

        public async Task<Reward?> GetRewardAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<List<Reward>> GetAllRewardsAsync()
        {
            var rewards = await _repository.GetAllAsync();
            return rewards.ToList();
        }

        public async Task<List<Reward>> GetActiveRewardsAsync()
        {
            var rewards = await _repository.FindAsync(r => r.IsActive);
            return rewards.ToList();
        }

        public async Task<Reward> UpdateRewardAsync(Reward reward)
        {
            return await _repository.UpdateAsync(reward);
        }

        public async Task DeleteRewardAsync(int id)
        {
            var reward = await _repository.GetByIdAsync(id);
            if (reward != null)
            {
                await _repository.DeleteAsync(reward);
            }
        }
    }
} 