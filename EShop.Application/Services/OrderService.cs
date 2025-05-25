using EShop.Domain.Models;
using EShop.Domain.Repositories;

namespace EShop.Application.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;
    private readonly IMemberRepository _memberRepository;
    private readonly IUnitOfWork _unitOfWork;
    private const decimal POINTS_CONVERSION_RATE = 0.01m; // 1 point = $0.01 discount
    private const decimal POINTS_EARNING_RATE = 1; // Earn 1 point per $1 spent

    public OrderService(
        IOrderRepository orderRepository,
        IProductRepository productRepository,
        IMemberRepository memberRepository,
        IUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
        _memberRepository = memberRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Order?> GetByIdAsync(int id)
    {
        return await _orderRepository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<Order>> GetAllAsync()
    {
        return await _orderRepository.GetAllAsync();
    }

    public async Task<Order> CreateAsync(Order order)
    {
        return await _orderRepository.AddAsync(order);
    }

    public async Task<Order> UpdateAsync(Order order)
    {
        return await _orderRepository.UpdateAsync(order);
    }

    public async Task DeleteAsync(int id)
    {
        var order = await _orderRepository.GetByIdAsync(id);
        if (order != null)
        {
            await _orderRepository.DeleteAsync(order);
        }
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _orderRepository.ExistsAsync(o => o.Id == id);
    }

    public async Task<Order?> CreateOrderAsync(int memberId, IEnumerable<(int ProductId, int Quantity)> orderItems, int? pointsToUse = null)
    {
        var member = await _memberRepository.GetByIdAsync(memberId);
        if (member == null)
            return null;

        // Validate stock availability
        foreach (var (productId, quantity) in orderItems)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null || product.StockQuantity < quantity)
                return null;
        }

        try
        {
            await _unitOfWork.BeginTransactionAsync();

            var order = new Order
            {
                MemberId = memberId,
                Member = member,
                OrderNumber = GenerateOrderNumber(),
                Status = OrderStatus.Pending,
                PaymentStatus = PaymentStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                OrderItems = new List<OrderItem>(),
                ShippingAddress = member.DefaultShippingAddress ?? "Not provided",
                BillingAddress = member.DefaultBillingAddress ?? "Not provided"
            };

            decimal subtotal = 0;
            foreach (var (productId, quantity) in orderItems)
            {
                var product = await _productRepository.GetByIdAsync(productId);
                if (product == null) continue;

                var orderItem = new OrderItem
                {
                    Order = order,
                    Product = product,
                    ProductId = productId,
                    Quantity = quantity,
                    UnitPrice = product.Price,
                    PointsEarned = (int)(product.Price * quantity * POINTS_EARNING_RATE)
                };

                order.OrderItems.Add(orderItem);
                subtotal += orderItem.UnitPrice * orderItem.Quantity;

                // Update stock
                await _productRepository.UpdateStockQuantityAsync(productId, product.StockQuantity - quantity);
            }

            order.TotalAmount = subtotal;

            // Apply points discount if requested
            if (pointsToUse.HasValue && pointsToUse.Value > 0 && pointsToUse.Value <= member.PointsBalance)
            {
                decimal pointsDiscount = pointsToUse.Value * POINTS_CONVERSION_RATE;
                order.DiscountAmount = Math.Min(pointsDiscount, order.TotalAmount);
                order.PointsUsed = pointsToUse.Value;
                order.TotalAmount -= order.DiscountAmount.Value;

                // Deduct points from member's balance
                await _memberRepository.UpdatePointsBalanceAsync(memberId, member.PointsBalance - pointsToUse.Value);
            }

            // Calculate points to be earned
            order.PointsEarned = CalculatePointsEarned(order.TotalAmount);

            var createdOrder = await _orderRepository.AddAsync(order);
            await _unitOfWork.CommitTransactionAsync();
            return createdOrder;
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }

    public async Task<Order?> GetOrderWithDetailsAsync(int orderId)
    {
        return await _orderRepository.GetOrderWithDetailsAsync(orderId);
    }

    public async Task<IEnumerable<Order>> GetOrdersByMemberAsync(int memberId)
    {
        return await _orderRepository.GetOrdersByMemberAsync(memberId);
    }

    public async Task<bool> UpdateOrderStatusAsync(int orderId, OrderStatus status)
    {
        var order = await _orderRepository.GetOrderWithDetailsAsync(orderId);
        if (order == null)
            return false;

        var success = await _orderRepository.UpdateOrderStatusAsync(orderId, status);

        if (success && status == OrderStatus.Delivered)
        {
            // Add earned points to member's balance
            var member = await _memberRepository.GetByIdAsync(order.MemberId);
            if (member != null && order.PointsEarned.HasValue)
            {
                await _memberRepository.UpdatePointsBalanceAsync(
                    order.MemberId,
                    member.PointsBalance + order.PointsEarned.Value
                );
            }
        }

        return success;
    }

    public async Task<bool> ProcessPaymentAsync(int orderId, string paymentTransactionId)
    {
        var order = await _orderRepository.GetByIdAsync(orderId);
        if (order == null)
            return false;

        order.PaymentTransactionId = paymentTransactionId;
        order.PaymentStatus = PaymentStatus.Completed;
        order.PaidAt = DateTime.UtcNow;
        
        await _orderRepository.UpdateAsync(order);
        return true;
    }

    public async Task<bool> CancelOrderAsync(int orderId)
    {
        var order = await _orderRepository.GetOrderWithDetailsAsync(orderId);
        if (order == null || order.Status == OrderStatus.Delivered)
            return false;

        try
        {
            await _unitOfWork.BeginTransactionAsync();

            // Restore stock
            foreach (var orderItem in order.OrderItems)
            {
                var product = await _productRepository.GetByIdAsync(orderItem.ProductId);
                if (product != null)
                {
                    await _productRepository.UpdateStockQuantityAsync(
                        orderItem.ProductId,
                        product.StockQuantity + orderItem.Quantity
                    );
                }
            }

            // Restore points if they were used
            if (order.PointsUsed.HasValue && order.PointsUsed.Value > 0)
            {
                var member = await _memberRepository.GetByIdAsync(order.MemberId);
                if (member != null)
                {
                    await _memberRepository.UpdatePointsBalanceAsync(
                        order.MemberId,
                        member.PointsBalance + order.PointsUsed.Value
                    );
                }
            }

            var success = await _orderRepository.UpdateOrderStatusAsync(orderId, OrderStatus.Cancelled);
            await _unitOfWork.CommitTransactionAsync();
            return success;
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }

    public async Task<decimal> CalculateOrderTotalAsync(IEnumerable<(int ProductId, int Quantity)> orderItems)
    {
        decimal total = 0;
        foreach (var (productId, quantity) in orderItems)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            if (product != null)
            {
                total += product.Price * quantity;
            }
        }
        return total;
    }

    public int CalculatePointsEarned(decimal orderTotal)
    {
        return (int)(orderTotal * POINTS_EARNING_RATE);
    }

    private string GenerateOrderNumber()
    {
        return $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N").Substring(0, 8)}";
    }
} 