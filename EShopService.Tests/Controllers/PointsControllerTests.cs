using EShop.Application.Service;
using EShop.Domain.Models;
using EShopService.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace EShopService.Tests.Controllers
{
    public class PointsControllerTests
    {
        private readonly Mock<IPointsService> _serviceMock;
        private readonly PointsController _controller;

        public PointsControllerTests()
        {
            _serviceMock = new Mock<IPointsService>();
            _controller = new PointsController(_serviceMock.Object);
        }

        [Fact]
        public async Task Earn_ReturnsOk()
        {
            var request = new EarnPointsRequest { MemberId = 1, Points = 10, Description = "test" };
            var result = await _controller.Earn(request);
            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(ok.Value);
            Assert.Equal("Points earned successfully", ((dynamic)ok.Value).message);
        }

        [Fact]
        public async Task Spend_ReturnsOk_WhenSuccess()
        {
            _serviceMock.Setup(s => s.SpendPointsAsync(1, 10, "test")).ReturnsAsync(true);
            var request = new SpendPointsRequest { MemberId = 1, Points = 10, Description = "test" };
            var result = await _controller.Spend(request);
            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(ok.Value);
            Assert.Equal("Points spent successfully", ((dynamic)ok.Value).message);
        }

        [Fact]
        public async Task Spend_ReturnsBadRequest_WhenFail()
        {
            _serviceMock.Setup(s => s.SpendPointsAsync(1, 10, "test")).ReturnsAsync(false);
            var request = new SpendPointsRequest { MemberId = 1, Points = 10, Description = "test" };
            var result = await _controller.Spend(request);
            var bad = Assert.IsType<BadRequestObjectResult>(result);
            Assert.NotNull(bad.Value);
            Assert.Equal("Insufficient points", ((dynamic)bad.Value).error);
        }

        [Fact]
        public async Task GetByMember_ReturnsOkWithTransactions()
        {
            _serviceMock.Setup(s => s.GetPointsTransactionsByMemberAsync(1)).ReturnsAsync(new List<PointsTransaction> { new PointsTransaction { Id = 1, Member = new Member { Id = 1, Name = "Test", Email = "test@email.com" }, Description = "desc" } });
            var result = await _controller.GetByMember(1);
            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.IsType<List<PointsTransaction>>(ok.Value);
        }

        [Fact]
        public async Task GetAll_ReturnsOkWithTransactions()
        {
            _serviceMock.Setup(s => s.GetAllPointsTransactionsAsync()).ReturnsAsync(new List<PointsTransaction> { new PointsTransaction { Id = 1, Member = new Member { Id = 1, Name = "Test", Email = "test@email.com" }, Description = "desc" } });
            var result = await _controller.GetAll();
            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.IsType<List<PointsTransaction>>(ok.Value);
        }
    }
} 