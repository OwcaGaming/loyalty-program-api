using EShop.Application.Services;
using EShop.Domain.Models;
using EShop.Domain.Repositories;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace EShopService.Tests.Service
{
    public class MemberServiceTests
    {
        private readonly Mock<IMemberRepository> _memberRepoMock;
        private readonly Mock<IOrderRepository> _orderRepoMock;
        private readonly Mock<IPointsTransactionRepository> _pointsTransactionRepoMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly MemberService _service;

        public MemberServiceTests()
        {
            _memberRepoMock = new Mock<IMemberRepository>();
            _orderRepoMock = new Mock<IOrderRepository>();
            _pointsTransactionRepoMock = new Mock<IPointsTransactionRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _service = new MemberService(_memberRepoMock.Object, _orderRepoMock.Object, _pointsTransactionRepoMock.Object, _unitOfWorkMock.Object);
        }

        [Fact]
        public async Task CreateAsync_CallsRepo()
        {
            var member = new Member { Id = 1, Name = "Test", Email = "test@email.com" };
            _memberRepoMock.Setup(r => r.GetByEmailAsync(member.Email)).ReturnsAsync((Member?)null);
            _memberRepoMock.Setup(r => r.AddAsync(member)).ReturnsAsync(member);
            var result = await _service.CreateAsync(member);
            Assert.Equal(member, result);
            _memberRepoMock.Verify(r => r.AddAsync(member), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_CallsRepo()
        {
            _memberRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Member { Id = 1, Name = "Test", Email = "test@email.com" });
            var result = await _service.GetByIdAsync(1);
            Assert.NotNull(result);
            _memberRepoMock.Verify(r => r.GetByIdAsync(1), Times.Once);
        }

        [Fact]
        public async Task GetAllAsync_CallsRepo()
        {
            _memberRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Member>());
            var result = await _service.GetAllAsync();
            Assert.NotNull(result);
            _memberRepoMock.Verify(r => r.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_CallsRepo()
        {
            var member = new Member { Id = 1, Name = "Test", Email = "test@email.com" };
            _memberRepoMock.Setup(r => r.GetByIdAsync(member.Id)).ReturnsAsync(member);
            _memberRepoMock.Setup(r => r.GetByEmailAsync(member.Email)).ReturnsAsync((Member?)null);
            _memberRepoMock.Setup(r => r.UpdateAsync(member)).ReturnsAsync(member);
            var result = await _service.UpdateAsync(member);
            Assert.Equal(member, result);
            _memberRepoMock.Verify(r => r.UpdateAsync(member), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_CallsRepo()
        {
            var member = new Member { Id = 1, Name = "Test", Email = "test@email.com" };
            _memberRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(member);
            _unitOfWorkMock.Setup(u => u.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(u => u.CommitTransactionAsync()).Returns(Task.CompletedTask);
            await _service.DeleteAsync(1);
            _memberRepoMock.Verify(r => r.DeleteAsync(member), Times.Once);
        }
    }
} 