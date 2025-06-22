using EShop.Application.Service;
using EShop.Domain.Models;
using EShop.Domain.Repositories;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace EShopService.Tests.Service
{
    public class PointsServiceTests
    {
        private readonly Mock<IMemberRepository> _memberRepoMock;
        private readonly Mock<IPointsTransactionRepository> _transactionRepoMock;
        private readonly PointsService _service;

        public PointsServiceTests()
        {
            _memberRepoMock = new Mock<IMemberRepository>();
            _transactionRepoMock = new Mock<IPointsTransactionRepository>();
            _service = new PointsService(_memberRepoMock.Object, _transactionRepoMock.Object);
        }

        [Fact]
        public async Task EarnPointsAsync_CallsRepo()
        {
            var member = new Member { Id = 1, Name = "Test", Email = "test@email.com", PointsBalance = 0 };
            _memberRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(member);
            _memberRepoMock.Setup(r => r.UpdateAsync(member)).ReturnsAsync(member);
            _transactionRepoMock.Setup(r => r.CreateTransactionAsync(1, 10, "desc", PointsTransactionType.Earn))
                .ReturnsAsync(new PointsTransaction { Id = 1, Member = member, MemberId = 1, Points = 10, Description = "desc", Type = PointsTransactionType.Earn });
            await _service.EarnPointsAsync(1, 10, "desc");
            _transactionRepoMock.Verify(r => r.CreateTransactionAsync(1, 10, "desc", PointsTransactionType.Earn), Times.Once);
        }

        [Fact]
        public async Task SpendPointsAsync_CallsRepoAndReturnsResult()
        {
            var member = new Member { Id = 1, Name = "Test", Email = "test@email.com", PointsBalance = 20 };
            _memberRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(member);
            _memberRepoMock.Setup(r => r.UpdateAsync(member)).ReturnsAsync(member);
            _transactionRepoMock.Setup(r => r.CreateTransactionAsync(1, 10, "desc", PointsTransactionType.Spend))
                .ReturnsAsync(new PointsTransaction { Id = 2, Member = member, MemberId = 1, Points = 10, Description = "desc", Type = PointsTransactionType.Spend });
            var result = await _service.SpendPointsAsync(1, 10, "desc");
            Assert.True(result);
            _transactionRepoMock.Verify(r => r.CreateTransactionAsync(1, 10, "desc", PointsTransactionType.Spend), Times.Once);
        }

        [Fact]
        public async Task GetPointsTransactionsByMemberAsync_CallsRepo()
        {
            _transactionRepoMock.Setup(r => r.GetByMemberIdAsync(1)).ReturnsAsync(new List<PointsTransaction>());
            var result = await _service.GetPointsTransactionsByMemberAsync(1);
            Assert.NotNull(result);
            _transactionRepoMock.Verify(r => r.GetByMemberIdAsync(1), Times.Once);
        }

        [Fact]
        public async Task GetAllPointsTransactionsAsync_CallsRepo()
        {
            _transactionRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<PointsTransaction>());
            var result = await _service.GetAllPointsTransactionsAsync();
            Assert.NotNull(result);
            _transactionRepoMock.Verify(r => r.GetAllAsync(), Times.Once);
        }
    }
} 