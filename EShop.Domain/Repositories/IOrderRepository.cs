using EShop.Domain.Models;

namespace EShop.Domain.Repositories;

public interface IOrderRepository : IRepository<Order>
{
    Task<IEnumerable<Order>> GetOrdersByMemberAsync(int memberId);
    Task<Order?> GetOrderWithDetailsAsync(int orderId);
    Task<IEnumerable<Order>> GetOrdersByStatusAsync(OrderStatus status);
    Task<bool> UpdateOrderStatusAsync(int orderId, OrderStatus status);
    Task<bool> UpdatePaymentStatusAsync(int orderId, PaymentStatus status);
    Task<decimal> GetTotalOrderAmountByMemberAsync(int memberId);
} 