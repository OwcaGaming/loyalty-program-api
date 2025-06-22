using EShop.Application.Services;
using EShop.Domain.Models;
using EShop.Domain.Repositories;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace EShopService.Tests.Service
{
    public class RewardServiceTests
    {
        private readonly Mock<IRewardRepository> _rewardRepoMock;
        private readonly Mock<IMemberRepository> _memberRepoMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly RewardService _service;

        public RewardServiceTests()
        {
            _rewardRepoMock = new Mock<IRewardRepository>();
            _memberRepoMock = new Mock<IMemberRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _service = new RewardService(_rewardRepoMock.Object, _memberRepoMock.Object, _unitOfWorkMock.Object);
        }

        [Fact]
        public async Task AddRewardAsync_CallsRepo()
        {
            var reward = new Reward { Id = 1, Name = "Reward1", Description = "Desc", PointsCost = 10, StockQuantity = 1 };
            _rewardRepoMock.Setup(r => r.AddAsync(reward)).ReturnsAsync(reward);
            var result = await _service.AddRewardAsync(reward);
            Assert.Equal(reward, result);
            _rewardRepoMock.Verify(r => r.AddAsync(reward), Times.Once);
        }

        [Fact]
        public async Task GetRewardAsync_CallsRepo()
        {
            _rewardRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Reward { Id = 1, Name = "Reward1", Description = "Desc", PointsCost = 10, StockQuantity = 1 });
            var result = await _service.GetRewardAsync(1);
            Assert.NotNull(result);
            _rewardRepoMock.Verify(r => r.GetByIdAsync(1), Times.Once);
        }

        [Fact]
        public async Task GetAllRewardsAsync_CallsRepo()
        {
            _rewardRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Reward>());
            var result = await _service.GetAllRewardsAsync();
            Assert.NotNull(result);
            _rewardRepoMock.Verify(r => r.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetActiveRewardsAsync_CallsRepo()
        {
            _rewardRepoMock.Setup(r => r.GetActiveRewardsAsync()).ReturnsAsync(new List<Reward>());
            var result = await _service.GetActiveRewardsAsync();
            Assert.NotNull(result);
            _rewardRepoMock.Verify(r => r.GetActiveRewardsAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateRewardAsync_CallsRepo()
        {
            var reward = new Reward { Id = 1, Name = "Reward1", Description = "Desc", PointsCost = 10, StockQuantity = 1 };
            _rewardRepoMock.Setup(r => r.GetByIdAsync(reward.Id)).ReturnsAsync(reward);
            _rewardRepoMock.Setup(r => r.UpdateAsync(reward)).ReturnsAsync(reward);
            var result = await _service.UpdateRewardAsync(reward);
            Assert.Equal(reward, result);
            _rewardRepoMock.Verify(r => r.UpdateAsync(reward), Times.Once);
        }

        [Fact]
        public async Task DeleteRewardAsync_CallsRepo()
        {
            var reward = new Reward { Id = 1, Name = "Reward1", Description = "Desc", PointsCost = 10, StockQuantity = 1 };
            _rewardRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(reward);
            _rewardRepoMock.Setup(r => r.DeleteAsync(reward)).Returns(Task.CompletedTask);
            var result = await _service.DeleteRewardAsync(1);
            Assert.True(result);
            _rewardRepoMock.Verify(r => r.DeleteAsync(reward), Times.Once);
        }
    }
} 