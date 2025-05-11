using EShopDomain.Models;
using EShop.Domain.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EShop.Application.Service
{
    public class RewardService : IRewardService
    {
        private readonly IRepository _repository;
        public RewardService(IRepository repository)
        {
            _repository = repository;
        }
        public Task<Reward> AddRewardAsync(Reward reward) => _repository.AddRewardAsync(reward);
        public Task<Reward?> GetRewardAsync(int id) => _repository.GetRewardAsync(id);
        public Task<List<Reward>> GetAllRewardsAsync() => _repository.GetAllRewardsAsync();
        public Task<List<Reward>> GetActiveRewardsAsync() => _repository.GetActiveRewardsAsync();
        public Task<Reward> UpdateRewardAsync(Reward reward) => _repository.UpdateRewardAsync(reward);
        public Task DeleteRewardAsync(int id) => _repository.DeleteRewardAsync(id);
    }
} 