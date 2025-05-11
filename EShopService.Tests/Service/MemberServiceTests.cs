using EShop.Application.Service;
using EShopDomain.Models;
using EShop.Domain.Repositories;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace EShopService.Tests.Service
{
    public class MemberServiceTests
    {
        private readonly Mock<IRepository> _repoMock;
        private readonly MemberService _service;

        public MemberServiceTests()
        {
            _repoMock = new Mock<IRepository>();
            _service = new MemberService(_repoMock.Object);
        }

        [Fact]
        public async Task AddMemberAsync_CallsRepo()
        {
            var member = new Member { Id = 1 };
            _repoMock.Setup(r => r.AddMemberAsync(member)).ReturnsAsync(member);
            var result = await _service.AddMemberAsync(member);
            Assert.Equal(member, result);
            _repoMock.Verify(r => r.AddMemberAsync(member), Times.Once);
        }

        [Fact]
        public async Task GetMemberAsync_CallsRepo()
        {
            _repoMock.Setup(r => r.GetMemberAsync(1)).ReturnsAsync(new Member { Id = 1 });
            var result = await _service.GetMemberAsync(1);
            Assert.NotNull(result);
            _repoMock.Verify(r => r.GetMemberAsync(1), Times.Once);
        }

        [Fact]
        public async Task GetAllMembersAsync_CallsRepo()
        {
            _repoMock.Setup(r => r.GetAllMembersAsync()).ReturnsAsync(new List<Member>());
            var result = await _service.GetAllMembersAsync();
            Assert.NotNull(result);
            _repoMock.Verify(r => r.GetAllMembersAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateMemberAsync_CallsRepo()
        {
            var member = new Member { Id = 1 };
            _repoMock.Setup(r => r.UpdateMemberAsync(member)).ReturnsAsync(member);
            var result = await _service.UpdateMemberAsync(member);
            Assert.Equal(member, result);
            _repoMock.Verify(r => r.UpdateMemberAsync(member), Times.Once);
        }

        [Fact]
        public async Task DeleteMemberAsync_CallsRepo()
        {
            await _service.DeleteMemberAsync(1);
            _repoMock.Verify(r => r.DeleteMemberAsync(1), Times.Once);
        }
    }
} 