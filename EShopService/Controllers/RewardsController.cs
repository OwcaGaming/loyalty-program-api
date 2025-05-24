using EShop.Application.Services;
using EShop.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EShopService.Controllers;

public class RewardsController : BaseApiController
{
    private readonly IRewardService _rewardService;
    private readonly IMemberService _memberService;

    public RewardsController(IRewardService rewardService, IMemberService memberService)
    {
        _rewardService = rewardService;
        _memberService = memberService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetRewards()
    {
        var rewards = await _rewardService.GetActiveRewardsAsync();
        return HandleResult(rewards);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetReward(int id)
    {
        var reward = await _rewardService.GetByIdAsync(id);
        return HandleResult(reward);
    }

    [HttpGet("by-type/{type}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetRewardsByType(RewardType type)
    {
        var rewards = await _rewardService.GetRewardsByTypeAsync(type);
        return HandleResult(rewards);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateReward(Reward reward)
    {
        var result = await _rewardService.CreateAsync(reward);
        return HandleResult(result);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateReward(int id, Reward reward)
    {
        if (id != reward.Id)
            return BadRequest();

        var result = await _rewardService.UpdateAsync(reward);
        return HandleResult(result);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteReward(int id)
    {
        await _rewardService.DeleteAsync(id);
        return NoContent();
    }

    [HttpPost("{id}/redeem")]
    public async Task<IActionResult> RedeemReward(int id)
    {
        var userId = GetUserId();
        var member = await _memberService.GetByUserIdAsync(userId);
        if (member == null)
            return NotFound();

        var result = await _rewardService.RedeemRewardAsync(id, member.Id);
        if (!result)
            return BadRequest("Failed to redeem reward");

        return NoContent();
    }

    [HttpGet("my-redeemed")]
    public async Task<IActionResult> GetMyRedeemedRewards()
    {
        var userId = GetUserId();
        var member = await _memberService.GetByUserIdAsync(userId);
        if (member == null)
            return NotFound();

        var rewards = await _rewardService.GetRedeemedRewardsAsync(member.Id);
        return HandleResult(rewards);
    }

    [HttpGet("available")]
    public async Task<IActionResult> GetAvailableRewards()
    {
        var userId = GetUserId();
        var member = await _memberService.GetByUserIdAsync(userId);
        if (member == null)
            return NotFound();

        var rewards = await _rewardService.GetAvailableRewardsForMemberAsync(member.Id);
        return HandleResult(rewards);
    }

    [HttpGet("popular")]
    [AllowAnonymous]
    public async Task<IActionResult> GetPopularRewards([FromQuery] int count = 5)
    {
        var rewards = await _rewardService.GetPopularRewardsAsync(count);
        return HandleResult(rewards);
    }

    [HttpPut("{id}/stock")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateStock(int id, [FromBody] int quantity)
    {
        var result = await _rewardService.UpdateStockQuantityAsync(id, quantity);
        if (!result)
            return NotFound();

        return NoContent();
    }
} 