using EShop.Application.Services;
using EShop.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EShopService.Controllers;

public class OrdersController : BaseApiController
{
    private readonly IOrderService _orderService;
    private readonly IMemberService _memberService;

    public OrdersController(IOrderService orderService, IMemberService memberService)
    {
        _orderService = orderService;
        _memberService = memberService;
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetOrders()
    {
        var orders = await _orderService.GetAllAsync();
        return HandleResult(orders);
    }

    [HttpGet("my")]
    public async Task<IActionResult> GetMyOrders()
    {
        var userId = GetUserId();
        var member = await _memberService.GetByUserIdAsync(userId);
        if (member == null)
            return NotFound();

        var orders = await _orderService.GetOrdersByMemberAsync(member.Id);
        return HandleResult(orders);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrder(int id)
    {
        var order = await _orderService.GetOrderWithDetailsAsync(id);
        if (order == null)
            return NotFound();

        // Verify the order belongs to the current user or user is admin
        if (!User.IsInRole("Admin"))
        {
            var userId = GetUserId();
            var member = await _memberService.GetByUserIdAsync(userId);
            if (member == null || order.MemberId != member.Id)
                return Forbid();
        }

        return HandleResult(order);
    }

    public class CreateOrderRequest
    {
        public List<OrderItemRequest> Items { get; set; }
        public int? PointsToUse { get; set; }
    }

    public class OrderItemRequest
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder(CreateOrderRequest request)
    {
        var userId = GetUserId();
        var member = await _memberService.GetByUserIdAsync(userId);
        if (member == null)
            return NotFound();

        var orderItems = request.Items.Select(i => (i.ProductId, i.Quantity)).ToList();
        var order = await _orderService.CreateOrderAsync(member.Id, orderItems, request.PointsToUse);
        
        return HandleResult(order);
    }

    [HttpPost("{id}/cancel")]
    public async Task<IActionResult> CancelOrder(int id)
    {
        var order = await _orderService.GetOrderWithDetailsAsync(id);
        if (order == null)
            return NotFound();

        // Verify the order belongs to the current user or user is admin
        if (!User.IsInRole("Admin"))
        {
            var userId = GetUserId();
            var member = await _memberService.GetByUserIdAsync(userId);
            if (member == null || order.MemberId != member.Id)
                return Forbid();
        }

        var result = await _orderService.CancelOrderAsync(id);
        if (!result)
            return BadRequest("Order cannot be cancelled");

        return NoContent();
    }

    [HttpPost("{id}/process-payment")]
    public async Task<IActionResult> ProcessPayment(int id, [FromBody] string paymentTransactionId)
    {
        var order = await _orderService.GetOrderWithDetailsAsync(id);
        if (order == null)
            return NotFound();

        // Verify the order belongs to the current user or user is admin
        if (!User.IsInRole("Admin"))
        {
            var userId = GetUserId();
            var member = await _memberService.GetByUserIdAsync(userId);
            if (member == null || order.MemberId != member.Id)
                return Forbid();
        }

        var result = await _orderService.ProcessPaymentAsync(id, paymentTransactionId);
        if (!result)
            return BadRequest("Payment processing failed");

        return NoContent();
    }

    [HttpPut("{id}/status")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] OrderStatus status)
    {
        var result = await _orderService.UpdateOrderStatusAsync(id, status);
        if (!result)
            return NotFound();

        return NoContent();
    }

    [HttpGet("calculate-total")]
    public async Task<IActionResult> CalculateOrderTotal([FromQuery] List<OrderItemRequest> items)
    {
        var orderItems = items.Select(i => (i.ProductId, i.Quantity)).ToList();
        var total = await _orderService.CalculateOrderTotalAsync(orderItems);
        var pointsToEarn = await _orderService.CalculatePointsEarnedAsync(total);

        return Ok(new { total, pointsToEarn });
    }
} 