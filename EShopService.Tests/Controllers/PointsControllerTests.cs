using EShop.Application.Service;
using EShopDomain.Models;
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
            var result = await _controller.Earn(1, 10, "test");
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task Spend_ReturnsOk_WhenSuccess()
        {
            _serviceMock.Setup(s => s.SpendPointsAsync(1, 10, "test")).ReturnsAsync(true);
            var result = await _controller.Spend(1, 10, "test");
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task Spend_ReturnsBadRequest_WhenFail()
        {
            _serviceMock.Setup(s => s.SpendPointsAsync(1, 10, "test")).ReturnsAsync(false);
            var result = await _controller.Spend(1, 10, "test");
            var bad = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Insufficient points", bad.Value);
        }

        [Fact]
        public async Task GetByMember_ReturnsOkWithTransactions()
        {
            _serviceMock.Setup(s => s.GetPointsTransactionsByMemberAsync(1)).ReturnsAsync(new List<PointsTransaction> { new PointsTransaction { Id = 1 } });
            var result = await _controller.GetByMember(1);
            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.IsType<List<PointsTransaction>>(ok.Value);
        }

        [Fact]
        public async Task GetAll_ReturnsOkWithTransactions()
        {
            _serviceMock.Setup(s => s.GetAllPointsTransactionsAsync()).ReturnsAsync(new List<PointsTransaction> { new PointsTransaction { Id = 1 } });
            var result = await _controller.GetAll();
            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.IsType<List<PointsTransaction>>(ok.Value);
        }
    }
} 