using EShop.Application.Services;
using EShop.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EShopService.Controllers;

public class MembersController : BaseApiController
{
    private readonly IMemberService _memberService;

    public MembersController(IMemberService memberService)
    {
        _memberService = memberService;
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetMembers()
    {
        var members = await _memberService.GetAllAsync();
        return HandleResult(members);
    }

    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        var userId = GetUserId();
        var member = await _memberService.GetByUserIdAsync(userId);
        return HandleResult(member);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetMember(int id)
    {
        var member = await _memberService.GetByIdAsync(id);
        return HandleResult(member);
    }

    [HttpGet("by-tier/{tier}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetMembersByTier(MemberTier tier)
    {
        var members = await _memberService.GetMembersByTierAsync(tier);
        return HandleResult(members);
    }

    [HttpGet("points-history")]
    public async Task<IActionResult> GetPointsHistory()
    {
        var userId = GetUserId();
        var member = await _memberService.GetByUserIdAsync(userId);
        if (member == null)
            return NotFound();

        var transactions = await _memberService.GetPointsTransactionsAsync(member.Id);
        return HandleResult(transactions);
    }

    [HttpGet("{id}/points-history")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetMemberPointsHistory(int id)
    {
        var transactions = await _memberService.GetPointsTransactionsAsync(id);
        return HandleResult(transactions);
    }

    [HttpPost("points-transaction")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AddPointsTransaction(int memberId, int points, string description)
    {
        var result = await _memberService.AddPointsTransactionAsync(memberId, points, description);
        if (!result)
            return BadRequest("Failed to add points transaction");

        return NoContent();
    }

    [HttpGet("calculate-tier")]
    public async Task<IActionResult> CalculateMemberTier()
    {
        var userId = GetUserId();
        var member = await _memberService.GetByUserIdAsync(userId);
        if (member == null)
            return NotFound();

        var tier = await _memberService.CalculateMemberTierAsync(member.Id);
        if (tier != member.Tier)
        {
            await _memberService.UpdateMemberTierAsync(member.Id, tier);
        }

        return Ok(new { tier });
    }

    [HttpGet("total-spent")]
    public async Task<IActionResult> GetTotalSpent()
    {
        var userId = GetUserId();
        var member = await _memberService.GetByUserIdAsync(userId);
        if (member == null)
            return NotFound();

        var totalSpent = await _memberService.GetTotalSpentAsync(member.Id);
        var totalPoints = await _memberService.GetTotalPointsEarnedAsync(member.Id);

        return Ok(new { totalSpent, totalPoints });
    }

    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile(Member updatedMember)
    {
        var userId = GetUserId();
        var member = await _memberService.GetByUserIdAsync(userId);
        if (member == null)
            return NotFound();

        // Ensure we're only updating allowed fields
        member.PhoneNumber = updatedMember.PhoneNumber;
        member.DefaultShippingAddress = updatedMember.DefaultShippingAddress;
        member.DefaultBillingAddress = updatedMember.DefaultBillingAddress;

        var result = await _memberService.UpdateAsync(member);
        return HandleResult(result);
    }
} 