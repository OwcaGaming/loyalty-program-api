using EShop.Application.Service;
using EShopDomain.Models;
using EShop.Domain.Repositories;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace EShopService.Tests.Service
{
    public class PointsServiceTests
    {
        private readonly Mock<IRepository> _repoMock;
        private readonly PointsService _service;

        public PointsServiceTests()
        {
            _repoMock = new Mock<IRepository>();
            _service = new PointsService(_repoMock.Object);
        }

        [Fact]
        public async Task EarnPointsAsync_CallsRepo()
        {
            await _service.EarnPointsAsync(1, 10, "desc");
            _repoMock.Verify(r => r.EarnPointsAsync(1, 10, "desc"), Times.Once);
        }

        [Fact]
        public async Task SpendPointsAsync_CallsRepoAndReturnsResult()
        {
            _repoMock.Setup(r => r.SpendPointsAsync(1, 10, "desc")).ReturnsAsync(true);
            var result = await _service.SpendPointsAsync(1, 10, "desc");
            Assert.True(result);
            _repoMock.Verify(r => r.SpendPointsAsync(1, 10, "desc"), Times.Once);
        }

        [Fact]
        public async Task GetPointsTransactionsByMemberAsync_CallsRepo()
        {
            _repoMock.Setup(r => r.GetPointsTransactionsByMemberAsync(1)).ReturnsAsync(new List<PointsTransaction>());
            var result = await _service.GetPointsTransactionsByMemberAsync(1);
            Assert.NotNull(result);
            _repoMock.Verify(r => r.GetPointsTransactionsByMemberAsync(1), Times.Once);
        }

        [Fact]
        public async Task GetAllPointsTransactionsAsync_CallsRepo()
        {
            _repoMock.Setup(r => r.GetAllPointsTransactionsAsync()).ReturnsAsync(new List<PointsTransaction>());
            var result = await _service.GetAllPointsTransactionsAsync();
            Assert.NotNull(result);
            _repoMock.Verify(r => r.GetAllPointsTransactionsAsync(), Times.Once);
        }
    }
} 