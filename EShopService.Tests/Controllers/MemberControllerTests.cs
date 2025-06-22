using EShop.Application.Services;
using EShop.Domain.Models;
using EShopService.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace EShopService.Tests.Controllers
{
    public class MemberControllerTests
    {
        private readonly Mock<IMemberService> _serviceMock;
        private readonly MembersController _controller;

        public MemberControllerTests()
        {
            _serviceMock = new Mock<IMemberService>();
            _controller = new MembersController(_serviceMock.Object);
        }

        [Fact]
        public async Task GetMembers_ReturnsOkWithMembers()
        {
            _serviceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<Member> { new Member { Id = 1, Name = "Test", Email = "test@email.com" } });
            var result = await _controller.GetMembers();
            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.IsType<List<Member>>(ok.Value);
        }

        [Fact]
        public async Task GetMember_ReturnsOk_WhenFound()
        {
            _serviceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(new Member { Id = 1, Name = "Test", Email = "test@email.com" });
            var result = await _controller.GetMember(1);
            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.IsType<Member>(ok.Value);
        }

        [Fact]
        public async Task GetMember_ReturnsNotFound_WhenNotFound()
        {
            _serviceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync((Member?)null);
            var result = await _controller.GetMember(1);
            Assert.IsType<NotFoundResult>(result);
        }

        /*
        [Fact]
        public async Task Post_ReturnsCreatedAtAction()
        {
            var member = new Member { Id = 1 };
            _serviceMock.Setup(s => s.AddMemberAsync(member)).ReturnsAsync(member);
            var result = await _controller.Post(member);
            var created = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(member, created.Value);
        }

        [Fact]
        public async Task Put_ReturnsBadRequest_WhenIdMismatch()
        {
            var member = new Member { Id = 2 };
            var result = await _controller.Put(1, member);
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task Put_ReturnsOk_WhenUpdated()
        {
            var member = new Member { Id = 1 };
            _serviceMock.Setup(s => s.UpdateMemberAsync(member)).ReturnsAsync(member);
            var result = await _controller.Put(1, member);
            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(member, ok.Value);
        }

        [Fact]
        public async Task Delete_ReturnsNoContent()
        {
            var result = await _controller.Delete(1);
            Assert.IsType<NoContentResult>(result);
        }
        */
    }
} 