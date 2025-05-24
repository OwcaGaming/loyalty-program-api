using EShop.Domain.Models;

namespace EShop.Application.Services;

public interface IOrderService : IService<Order>
{
    Task<Order?> CreateOrderAsync(int memberId, IEnumerable<(int ProductId, int Quantity)> orderItems, int? pointsToUse = null);
    Task<Order?> GetOrderWithDetailsAsync(int orderId);
    Task<IEnumerable<Order>> GetOrdersByMemberAsync(int memberId);
    Task<bool> UpdateOrderStatusAsync(int orderId, OrderStatus status);
    Task<bool> ProcessPaymentAsync(int orderId, string paymentTransactionId);
    Task<bool> CancelOrderAsync(int orderId);
    Task<decimal> CalculateOrderTotalAsync(IEnumerable<(int ProductId, int Quantity)> orderItems);
    Task<int> CalculatePointsEarnedAsync(decimal orderTotal);
} 