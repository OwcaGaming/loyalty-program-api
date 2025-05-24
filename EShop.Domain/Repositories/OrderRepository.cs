using EShop.Infrastructure.Data;
using EShop.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace EShop.Domain.Repositories;

public class OrderRepository : Repository<Order>, IOrderRepository
{
    public OrderRepository(IApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Order>> GetOrdersByMemberAsync(int memberId)
    {
        return await _dbSet
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .Include(o => o.Invoice)
            .Where(o => o.MemberId == memberId)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();
    }

    public async Task<Order?> GetOrderWithDetailsAsync(int orderId)
    {
        return await _dbSet
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .Include(o => o.Member)
            .Include(o => o.Invoice)
            .FirstOrDefaultAsync(o => o.Id == orderId);
    }

    public async Task<IEnumerable<Order>> GetOrdersByStatusAsync(OrderStatus status)
    {
        return await _dbSet
            .Include(o => o.OrderItems)
            .Include(o => o.Member)
            .Where(o => o.Status == status)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();
    }

    public async Task<bool> UpdateOrderStatusAsync(int orderId, OrderStatus status)
    {
        var order = await _dbSet.FindAsync(orderId);
        if (order == null)
            return false;

        order.Status = status;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdatePaymentStatusAsync(int orderId, PaymentStatus status)
    {
        var order = await _dbSet.FindAsync(orderId);
        if (order == null)
            return false;

        order.PaymentStatus = status;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<decimal> GetTotalOrderAmountByMemberAsync(int memberId)
    {
        return await _dbSet
            .Where(o => o.MemberId == memberId && o.PaymentStatus == PaymentStatus.Completed)
            .SumAsync(o => o.TotalAmount);
    }
} 