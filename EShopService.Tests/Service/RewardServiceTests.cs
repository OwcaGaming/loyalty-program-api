using EShop.Application.Service;
using EShopDomain.Models;
using EShop.Domain.Repositories;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace EShopService.Tests.Service
{
    public class RewardServiceTests
    {
        private readonly Mock<IRepository> _repoMock;
        private readonly RewardService _service;

        public RewardServiceTests()
        {
            _repoMock = new Mock<IRepository>();
            _service = new RewardService(_repoMock.Object);
        }

        [Fact]
        public async Task AddRewardAsync_CallsRepo()
        {
            var reward = new Reward { Id = 1 };
            _repoMock.Setup(r => r.AddRewardAsync(reward)).ReturnsAsync(reward);
            var result = await _service.AddRewardAsync(reward);
            Assert.Equal(reward, result);
            _repoMock.Verify(r => r.AddRewardAsync(reward), Times.Once);
        }

        [Fact]
        public async Task GetRewardAsync_CallsRepo()
        {
            _repoMock.Setup(r => r.GetRewardAsync(1)).ReturnsAsync(new Reward { Id = 1 });
            var result = await _service.GetRewardAsync(1);
            Assert.NotNull(result);
            _repoMock.Verify(r => r.GetRewardAsync(1), Times.Once);
        }

        [Fact]
        public async Task GetAllRewardsAsync_CallsRepo()
        {
            _repoMock.Setup(r => r.GetAllRewardsAsync()).ReturnsAsync(new List<Reward>());
            var result = await _service.GetAllRewardsAsync();
            Assert.NotNull(result);
            _repoMock.Verify(r => r.GetAllRewardsAsync(), Times.Once);
        }

        [Fact]
        public async Task GetActiveRewardsAsync_CallsRepo()
        {
            _repoMock.Setup(r => r.GetActiveRewardsAsync()).ReturnsAsync(new List<Reward>());
            var result = await _service.GetActiveRewardsAsync();
            Assert.NotNull(result);
            _repoMock.Verify(r => r.GetActiveRewardsAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateRewardAsync_CallsRepo()
        {
            var reward = new Reward { Id = 1 };
            _repoMock.Setup(r => r.UpdateRewardAsync(reward)).ReturnsAsync(reward);
            var result = await _service.UpdateRewardAsync(reward);
            Assert.Equal(reward, result);
            _repoMock.Verify(r => r.UpdateRewardAsync(reward), Times.Once);
        }

        [Fact]
        public async Task DeleteRewardAsync_CallsRepo()
        {
            await _service.DeleteRewardAsync(1);
            _repoMock.Verify(r => r.DeleteRewardAsync(1), Times.Once);
        }
    }
} 