using EShop.Domain.Models;

namespace EShop.Tests.Helpers;

public static class TestDataHelper
{
    public static Member CreateTestMember()
    {
        return new Member
        {
            Id = 1,
            Name = "Test Member",
            Email = "test@example.com",
            UserId = "user-1",
            DateJoined = DateTime.UtcNow,
            Tier = MemberTier.Standard,
            CreatedAt = DateTime.UtcNow
        };
    }

    public static Product CreateTestProduct()
    {
        return new Product
        {
            Id = 1,
            Name = "Test Product",
            Description = "Test Description",
            Price = 99.99m,
            StockQuantity = 100,
            CategoryId = 1,
            CreatedAt = DateTime.UtcNow
        };
    }

    public static Order CreateTestOrder(int memberId)
    {
        return new Order
        {
            Id = 1,
            MemberId = memberId,
            Status = OrderStatus.Pending,
            PaymentStatus = PaymentStatus.Pending,
            TotalAmount = 99.99m,
            CreatedAt = DateTime.UtcNow,
            Items = new List<OrderItem>
            {
                new OrderItem
                {
                    Id = 1,
                    OrderId = 1,
                    ProductId = 1,
                    Quantity = 1,
                    UnitPrice = 99.99m
                }
            }
        };
    }

    public static Reward CreateTestReward()
    {
        return new Reward
        {
            Id = 1,
            Name = "Test Reward",
            Description = "Test Description",
            PointsCost = 100,
            IsActive = true,
            Type = RewardType.Discount,
            DiscountAmount = 10.00m,
            CreatedAt = DateTime.UtcNow
        };
    }

    public static PointsTransaction CreateTestPointsTransaction(int memberId)
    {
        return new PointsTransaction
        {
            Id = 1,
            MemberId = memberId,
            Points = 100,
            Description = "Test Transaction",
            Type = PointsTransactionType.Earn,
            TransactionDate = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };
    }
} 