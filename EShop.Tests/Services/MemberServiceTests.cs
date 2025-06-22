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

    [Fact]
    public async Task CreateAsync_ShouldCreateMember_WhenValid()
    {
        // Arrange
        var newMember = new Member { Name = "New Member", Email = "new@example.com" };
        _memberRepository.Setup(r => r.GetByEmailAsync(newMember.Email)).ReturnsAsync((Member?)null);
        _memberRepository.Setup(r => r.AddAsync(It.IsAny<Member>())).ReturnsAsync((Member m) => m);

        // Act
        var result = await _memberService.CreateAsync(newMember);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(MemberTier.Standard, result.Tier);
        Assert.Equal(0, result.PointsBalance);
        Assert.Equal(newMember.Email, result.Email);
    }

    [Fact]
    public async Task CreateAsync_ShouldThrow_WhenDuplicateEmail()
    {
        // Arrange
        var newMember = new Member { Name = "New Member", Email = "test@example.com" };
        _memberRepository.Setup(r => r.GetByEmailAsync(newMember.Email)).ReturnsAsync(TestDataHelper.CreateTestMember());

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _memberService.CreateAsync(newMember));
    }

    [Fact]
    public async Task CreateAsync_ShouldThrow_WhenMissingFields()
    {
        // Arrange
        var memberNoName = new Member { Email = "a@b.com" };
        var memberNoEmail = new Member { Name = "No Email" };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _memberService.CreateAsync(memberNoName));
        await Assert.ThrowsAsync<ArgumentException>(() => _memberService.CreateAsync(memberNoEmail));
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateMember_WhenValid()
    {
        // Arrange
        var existing = TestDataHelper.CreateTestMember();
        var updated = new Member { Id = existing.Id, Name = "Updated Name", Email = existing.Email };
        _memberRepository.Setup(r => r.GetByIdAsync(existing.Id)).ReturnsAsync(existing);
        _memberRepository.Setup(r => r.GetByEmailAsync(updated.Email)).ReturnsAsync(existing);
        _memberRepository.Setup(r => r.UpdateAsync(It.IsAny<Member>())).ReturnsAsync((Member m) => m);

        // Act
        var result = await _memberService.UpdateAsync(updated);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Updated Name", result.Name);
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrow_WhenNotFound()
    {
        // Arrange
        var updated = new Member { Id = 999, Name = "Name", Email = "email@x.com" };
        _memberRepository.Setup(r => r.GetByIdAsync(updated.Id)).ReturnsAsync((Member?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _memberService.UpdateAsync(updated));
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrow_WhenDuplicateEmail()
    {
        // Arrange
        var existing = TestDataHelper.CreateTestMember();
        var another = new Member { Id = 2, Name = "Other", Email = "other@example.com" };
        var updated = new Member { Id = existing.Id, Name = "Name", Email = another.Email };
        _memberRepository.Setup(r => r.GetByIdAsync(existing.Id)).ReturnsAsync(existing);
        _memberRepository.Setup(r => r.GetByEmailAsync(another.Email)).ReturnsAsync(another);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _memberService.UpdateAsync(updated));
    }

    [Fact]
    public async Task DeleteAsync_ShouldDelete_WhenExists()
    {
        // Arrange
        var existing = TestDataHelper.CreateTestMember();
        _memberRepository.Setup(r => r.GetByIdAsync(existing.Id)).ReturnsAsync(existing);
        _memberRepository.Setup(r => r.DeleteAsync(existing)).Returns(Task.CompletedTask);
        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.Setup(u => u.BeginTransactionAsync()).Returns(Task.CompletedTask);
        unitOfWork.Setup(u => u.CommitTransactionAsync()).Returns(Task.CompletedTask);
        unitOfWork.Setup(u => u.RollbackTransactionAsync()).Returns(Task.CompletedTask);
        var service = new MemberService(_memberRepository.Object, _orderRepository.Object, _pointsTransactionRepository.Object, unitOfWork.Object);

        // Act
        await service.DeleteAsync(existing.Id);

        // Assert
        _memberRepository.Verify(r => r.DeleteAsync(existing), Times.Once);
        unitOfWork.Verify(u => u.CommitTransactionAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ShouldNotThrow_WhenNotFound()
    {
        // Arrange
        _memberRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Member?)null);
        var unitOfWork = new Mock<IUnitOfWork>();
        var service = new MemberService(_memberRepository.Object, _orderRepository.Object, _pointsTransactionRepository.Object, unitOfWork.Object);

        // Act & Assert
        await service.DeleteAsync(999); // Should not throw
    }
} 