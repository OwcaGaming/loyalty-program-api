using EShop.Application.Services;
using EShop.Domain.Models;
using EShop.Domain.Repositories;
using EShop.Tests.Helpers;
using Moq;
using Xunit;

namespace EShop.Tests.Services;

public class MemberServiceTests
{
    private readonly Mock<IRepository<Member>> _memberRepository;
    private readonly Mock<IRepository<PointsTransaction>> _pointsTransactionRepository;
    private readonly Mock<IRepository<Order>> _orderRepository;
    private readonly IMemberService _memberService;

    public MemberServiceTests()
    {
        _memberRepository = new Mock<IRepository<Member>>();
        _pointsTransactionRepository = new Mock<IRepository<PointsTransaction>>();
        _orderRepository = new Mock<IRepository<Order>>();
        _memberService = new MemberService(
            _memberRepository.Object,
            _pointsTransactionRepository.Object,
            _orderRepository.Object);
    }

    [Fact]
    public async Task GetByUserId_ShouldReturnMember_WhenMemberExists()
    {
        // Arrange
        var testMember = TestDataHelper.CreateTestMember();
        _memberRepository.Setup(r => r.FirstOrDefaultAsync(
            It.IsAny<System.Linq.Expressions.Expression<Func<Member, bool>>>()))
            .ReturnsAsync(testMember);

        // Act
        var result = await _memberService.GetByUserIdAsync(testMember.UserId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(testMember.Id, result.Id);
        Assert.Equal(testMember.UserId, result.UserId);
    }

    [Fact]
    public async Task AddPointsTransaction_ShouldUpdateMemberPoints()
    {
        // Arrange
        var testMember = TestDataHelper.CreateTestMember();
        var points = 100;
        var description = "Test points";

        _memberRepository.Setup(r => r.GetByIdAsync(testMember.Id))
            .ReturnsAsync(testMember);
        _pointsTransactionRepository.Setup(r => r.AddAsync(It.IsAny<PointsTransaction>()))
            .ReturnsAsync(TestDataHelper.CreateTestPointsTransaction(testMember.Id));

        // Act
        var result = await _memberService.AddPointsTransactionAsync(testMember.Id, points, description);

        // Assert
        Assert.True(result);
        _pointsTransactionRepository.Verify(r => r.AddAsync(
            It.Is<PointsTransaction>(t =>
                t.MemberId == testMember.Id &&
                t.Points == points &&
                t.Description == description)), Times.Once);
    }

    [Fact]
    public async Task CalculateMemberTier_ShouldReturnCorrectTier()
    {
        // Arrange
        var testMember = TestDataHelper.CreateTestMember();
        var orders = new List<Order>
        {
            TestDataHelper.CreateTestOrder(testMember.Id),
            TestDataHelper.CreateTestOrder(testMember.Id),
            TestDataHelper.CreateTestOrder(testMember.Id)
        };

        _orderRepository.Setup(r => r.GetAllAsync(
            It.IsAny<System.Linq.Expressions.Expression<Func<Order, bool>>>()))
            .ReturnsAsync(orders);

        // Act
        var tier = await _memberService.CalculateMemberTierAsync(testMember.Id);

        // Assert
        Assert.Equal(MemberTier.Silver, tier); // Assuming 3 orders of 99.99 each puts member in Silver tier
    }

    [Fact]
    public async Task GetTotalSpent_ShouldReturnCorrectAmount()
    {
        // Arrange
        var testMember = TestDataHelper.CreateTestMember();
        var orders = new List<Order>
        {
            TestDataHelper.CreateTestOrder(testMember.Id),
            TestDataHelper.CreateTestOrder(testMember.Id)
        };

        _orderRepository.Setup(r => r.GetAllAsync(
            It.IsAny<System.Linq.Expressions.Expression<Func<Order, bool>>>()))
            .ReturnsAsync(orders);

        // Act
        var totalSpent = await _memberService.GetTotalSpentAsync(testMember.Id);

        // Assert
        Assert.Equal(199.98m, totalSpent); // 2 orders of 99.99 each
    }

    [Fact]
    public async Task GetPointsTransactions_ShouldReturnTransactions()
    {
        // Arrange
        var testMember = TestDataHelper.CreateTestMember();
        var transactions = new List<PointsTransaction>
        {
            TestDataHelper.CreateTestPointsTransaction(testMember.Id),
            TestDataHelper.CreateTestPointsTransaction(testMember.Id)
        };

        _pointsTransactionRepository.Setup(r => r.GetAllAsync(
            It.IsAny<System.Linq.Expressions.Expression<Func<PointsTransaction, bool>>>()))
            .ReturnsAsync(transactions);

        // Act
        var result = await _memberService.GetPointsTransactionsAsync(testMember.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.All(result, t => Assert.Equal(testMember.Id, t.MemberId));
    }
} 