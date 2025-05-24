using EShop.Application.Services;
using EShop.Domain.Models;
using EShop.Domain.Repositories;
using EShop.Tests.Helpers;
using Moq;
using Xunit;

namespace EShop.Tests.Services;

public class OrderServiceTests
{
    private readonly Mock<IRepository<Order>> _orderRepository;
    private readonly Mock<IRepository<Product>> _productRepository;
    private readonly Mock<IMemberService> _memberService;
    private readonly IOrderService _orderService;

    public OrderServiceTests()
    {
        _orderRepository = new Mock<IRepository<Order>>();
        _productRepository = new Mock<IRepository<Product>>();
        _memberService = new Mock<IMemberService>();
        _orderService = new OrderService(
            _orderRepository.Object,
            _productRepository.Object,
            _memberService.Object);
    }

    [Fact]
    public async Task CreateOrder_ShouldCreateOrderAndUpdateStock()
    {
        // Arrange
        var testMember = TestDataHelper.CreateTestMember();
        var testProduct = TestDataHelper.CreateTestProduct();
        var orderItems = new List<(int productId, int quantity)>
        {
            (testProduct.Id, 1)
        };

        _productRepository.Setup(r => r.GetByIdAsync(testProduct.Id))
            .ReturnsAsync(testProduct);
        _orderRepository.Setup(r => r.AddAsync(It.IsAny<Order>()))
            .ReturnsAsync(TestDataHelper.CreateTestOrder(testMember.Id));

        // Act
        var result = await _orderService.CreateOrderAsync(testMember.Id, orderItems);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(testMember.Id, result.MemberId);
        Assert.Equal(OrderStatus.Pending, result.Status);
        Assert.Equal(PaymentStatus.Pending, result.PaymentStatus);
        _productRepository.Verify(r => r.UpdateAsync(It.Is<Product>(p => 
            p.Id == testProduct.Id && 
            p.StockQuantity == testProduct.StockQuantity - 1)), Times.Once);
    }

    [Fact]
    public async Task CreateOrder_WithPoints_ShouldApplyDiscount()
    {
        // Arrange
        var testMember = TestDataHelper.CreateTestMember();
        var testProduct = TestDataHelper.CreateTestProduct();
        var orderItems = new List<(int productId, int quantity)>
        {
            (testProduct.Id, 1)
        };
        var pointsToUse = 1000; // $10 worth of points

        _productRepository.Setup(r => r.GetByIdAsync(testProduct.Id))
            .ReturnsAsync(testProduct);
        _memberService.Setup(r => r.GetAvailablePointsAsync(testMember.Id))
            .ReturnsAsync(pointsToUse);
        _orderRepository.Setup(r => r.AddAsync(It.IsAny<Order>()))
            .ReturnsAsync(TestDataHelper.CreateTestOrder(testMember.Id));

        // Act
        var result = await _orderService.CreateOrderAsync(testMember.Id, orderItems, pointsToUse);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(testMember.Id, result.MemberId);
        Assert.Equal(89.99m, result.TotalAmount); // 99.99 - 10.00 (points discount)
        _memberService.Verify(s => s.AddPointsTransactionAsync(
            testMember.Id,
            -pointsToUse,
            It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task CancelOrder_ShouldUpdateOrderAndRestoreStock()
    {
        // Arrange
        var testMember = TestDataHelper.CreateTestMember();
        var testOrder = TestDataHelper.CreateTestOrder(testMember.Id);
        var testProduct = TestDataHelper.CreateTestProduct();

        _orderRepository.Setup(r => r.GetByIdAsync(testOrder.Id))
            .ReturnsAsync(testOrder);
        _productRepository.Setup(r => r.GetByIdAsync(testProduct.Id))
            .ReturnsAsync(testProduct);

        // Act
        var result = await _orderService.CancelOrderAsync(testOrder.Id);

        // Assert
        Assert.True(result);
        _orderRepository.Verify(r => r.UpdateAsync(It.Is<Order>(o => 
            o.Id == testOrder.Id && 
            o.Status == OrderStatus.Cancelled)), Times.Once);
        _productRepository.Verify(r => r.UpdateAsync(It.Is<Product>(p => 
            p.Id == testProduct.Id && 
            p.StockQuantity == testProduct.StockQuantity + 1)), Times.Once);
    }

    [Fact]
    public async Task ProcessPayment_ShouldUpdateOrderAndAwardPoints()
    {
        // Arrange
        var testMember = TestDataHelper.CreateTestMember();
        var testOrder = TestDataHelper.CreateTestOrder(testMember.Id);
        var paymentTransactionId = "test-transaction";

        _orderRepository.Setup(r => r.GetByIdAsync(testOrder.Id))
            .ReturnsAsync(testOrder);

        // Act
        var result = await _orderService.ProcessPaymentAsync(testOrder.Id, paymentTransactionId);

        // Assert
        Assert.True(result);
        _orderRepository.Verify(r => r.UpdateAsync(It.Is<Order>(o => 
            o.Id == testOrder.Id && 
            o.PaymentStatus == PaymentStatus.Paid)), Times.Once);
        _memberService.Verify(s => s.AddPointsTransactionAsync(
            testMember.Id,
            It.Is<int>(points => points == (int)testOrder.TotalAmount),
            It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task CalculateOrderTotal_ShouldReturnCorrectAmount()
    {
        // Arrange
        var testProduct = TestDataHelper.CreateTestProduct();
        var orderItems = new List<(int productId, int quantity)>
        {
            (testProduct.Id, 2)
        };

        _productRepository.Setup(r => r.GetByIdAsync(testProduct.Id))
            .ReturnsAsync(testProduct);

        // Act
        var total = await _orderService.CalculateOrderTotalAsync(orderItems);

        // Assert
        Assert.Equal(199.98m, total); // 2 * 99.99
    }
} 