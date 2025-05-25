using EShop.Domain.Models;
using EShop.Domain.Repositories;
using EShop.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EShop.Infrastructure.Repositories;

public class OrderRepository : Repository<Order>, IOrderRepository
{
    private new readonly ApplicationDbContext _context;

    public OrderRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public override async Task<Order?> GetByIdAsync(int id)
    {
        return await _context.Orders
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    public override async Task<IEnumerable<Order>> GetAllAsync()
    {
        return await _context.Orders
            .Include(o => o.OrderItems)
            .ToListAsync();
    }

    public async Task<IEnumerable<Order>> GetOrdersByMemberAsync(int memberId)
    {
        return await _context.Orders
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .Where(o => o.MemberId == memberId)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();
    }

    public async Task<Order?> GetOrderWithDetailsAsync(int orderId)
    {
        return await _context.Orders
            .Include(o => o.Member)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                    .ThenInclude(p => p.Category)
            .FirstOrDefaultAsync(o => o.Id == orderId);
    }

    public async Task<IEnumerable<Order>> GetOrdersByMemberIdAsync(int memberId)
    {
        return await GetOrdersByMemberAsync(memberId);
    }

    public async Task<bool> UpdateOrderStatusAsync(int orderId, OrderStatus status)
    {
        var order = await _context.Orders.FindAsync(orderId);
        if (order == null)
            return false;

        order.Status = status;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<decimal> GetTotalOrderAmountByMemberAsync(int memberId)
    {
        return await _context.Orders
            .Where(o => o.MemberId == memberId && o.Status == OrderStatus.Delivered)
            .SumAsync(o => o.TotalAmount);
    }

    public async Task<IEnumerable<Order>> GetOrdersByStatusAsync(OrderStatus status)
    {
        return await _context.Orders
            .Include(o => o.Member)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .Where(o => o.Status == status)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();
    }

    public async Task<bool> UpdatePaymentStatusAsync(int orderId, PaymentStatus status)
    {
        var order = await _context.Orders.FindAsync(orderId);
        if (order == null)
            return false;

        order.PaymentStatus = status;
        if (status == PaymentStatus.Completed)
        {
            order.PaidAt = DateTime.UtcNow;
        }
        await _context.SaveChangesAsync();
        return true;
    }

    public override async Task<Order> AddAsync(Order entity)
    {
        await _context.Orders.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public override async Task<Order> UpdateAsync(Order entity)
    {
        entity.UpdatedAt = DateTime.UtcNow;
        _context.Entry(entity).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return entity;
    }

    public override async Task DeleteAsync(Order entity)
    {
        _context.Orders.Remove(entity);
        await _context.SaveChangesAsync();
    }
} 