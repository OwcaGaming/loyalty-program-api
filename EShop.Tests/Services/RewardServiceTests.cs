using EShop.Application.Services;
using EShop.Domain.Models;
using EShop.Domain.Repositories;
using EShop.Tests.Helpers;
using Moq;
using Xunit;

namespace EShop.Tests.Services;

public class RewardServiceTests
{
    private readonly Mock<IRepository<Reward>> _rewardRepository;
    private readonly Mock<IMemberService> _memberService;
    private readonly IRewardService _rewardService;

    public RewardServiceTests()
    {
        _rewardRepository = new Mock<IRepository<Reward>>();
        _memberService = new Mock<IMemberService>();
        _rewardService = new RewardService(
            _rewardRepository.Object,
            _memberService.Object);
    }

    [Fact]
    public async Task GetActiveRewards_ShouldReturnOnlyActiveRewards()
    {
        // Arrange
        var activeReward = TestDataHelper.CreateTestReward();
        var inactiveReward = TestDataHelper.CreateTestReward();
        inactiveReward.IsActive = false;

        var rewards = new List<Reward> { activeReward, inactiveReward };
        _rewardRepository.Setup(r => r.GetAllAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Reward, bool>>>()))
            .ReturnsAsync(rewards.Where(r => r.IsActive));

        // Act
        var result = await _rewardService.GetActiveRewardsAsync();

        // Assert
        Assert.Single(result);
        Assert.True(result.First().IsActive);
    }

    [Fact]
    public async Task RedeemReward_ShouldSucceed_WhenMemberHasEnoughPoints()
    {
        // Arrange
        var testMember = TestDataHelper.CreateTestMember();
        var testReward = TestDataHelper.CreateTestReward();
        var availablePoints = testReward.PointsCost + 100;

        _rewardRepository.Setup(r => r.GetByIdAsync(testReward.Id))
            .ReturnsAsync(testReward);
        _memberService.Setup(s => s.GetAvailablePointsAsync(testMember.Id))
            .ReturnsAsync(availablePoints);

        // Act
        var result = await _rewardService.RedeemRewardAsync(testReward.Id, testMember.Id);

        // Assert
        Assert.True(result);
        _memberService.Verify(s => s.AddPointsTransactionAsync(
            testMember.Id,
            -testReward.PointsCost,
            It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task RedeemReward_ShouldFail_WhenMemberDoesNotHaveEnoughPoints()
    {
        // Arrange
        var testMember = TestDataHelper.CreateTestMember();
        var testReward = TestDataHelper.CreateTestReward();
        var availablePoints = testReward.PointsCost - 1;

        _rewardRepository.Setup(r => r.GetByIdAsync(testReward.Id))
            .ReturnsAsync(testReward);
        _memberService.Setup(s => s.GetAvailablePointsAsync(testMember.Id))
            .ReturnsAsync(availablePoints);

        // Act
        var result = await _rewardService.RedeemRewardAsync(testReward.Id, testMember.Id);

        // Assert
        Assert.False(result);
        _memberService.Verify(s => s.AddPointsTransactionAsync(
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task RedeemReward_ShouldFail_WhenRewardIsOutOfStock()
    {
        // Arrange
        var testMember = TestDataHelper.CreateTestMember();
        var testReward = TestDataHelper.CreateTestReward();
        testReward.StockQuantity = 0;

        _rewardRepository.Setup(r => r.GetByIdAsync(testReward.Id))
            .ReturnsAsync(testReward);

        // Act
        var result = await _rewardService.RedeemRewardAsync(testReward.Id, testMember.Id);

        // Assert
        Assert.False(result);
        _memberService.Verify(s => s.AddPointsTransactionAsync(
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task GetAvailableRewardsForMember_ShouldFilterByPointsAndStock()
    {
        // Arrange
        var testMember = TestDataHelper.CreateTestMember();
        var availablePoints = 150;

        var reward1 = TestDataHelper.CreateTestReward(); // 100 points
        var reward2 = TestDataHelper.CreateTestReward();
        reward2.PointsCost = 200; // Too expensive
        var reward3 = TestDataHelper.CreateTestReward();
        reward3.StockQuantity = 0; // Out of stock

        var rewards = new List<Reward> { reward1, reward2, reward3 };

        _rewardRepository.Setup(r => r.GetAllAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Reward, bool>>>()))
            .ReturnsAsync(rewards.Where(r => r.IsActive));
        _memberService.Setup(s => s.GetAvailablePointsAsync(testMember.Id))
            .ReturnsAsync(availablePoints);

        // Act
        var result = await _rewardService.GetAvailableRewardsForMemberAsync(testMember.Id);

        // Assert
        Assert.Single(result);
        Assert.Equal(reward1.Id, result.First().Id);
    }

    [Fact]
    public async Task UpdateStockQuantity_ShouldUpdateStock()
    {
        // Arrange
        var testReward = TestDataHelper.CreateTestReward();
        var newQuantity = 50;

        _rewardRepository.Setup(r => r.GetByIdAsync(testReward.Id))
            .ReturnsAsync(testReward);

        // Act
        var result = await _rewardService.UpdateStockQuantityAsync(testReward.Id, newQuantity);

        // Assert
        Assert.True(result);
        _rewardRepository.Verify(r => r.UpdateAsync(It.Is<Reward>(r => 
            r.Id == testReward.Id && 
            r.StockQuantity == newQuantity)), Times.Once);
    }
} 