using EShop.Application.Service;
using EShop.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EShopService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RewardsController : ControllerBase
{
    private readonly IRewardService _rewardService;

    public RewardsController(IRewardService rewardService)
    {
        _rewardService = rewardService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<List<Reward>>> GetAll()
    {
        var rewards = await _rewardService.GetAllRewardsAsync();
        return Ok(rewards);
    }

    [HttpGet("active")]
    [AllowAnonymous]
    public async Task<ActionResult<List<Reward>>> GetActive()
    {
        var rewards = await _rewardService.GetActiveRewardsAsync();
        return Ok(rewards);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<Reward>> GetById(int id)
    {
        var reward = await _rewardService.GetRewardAsync(id);
        if (reward == null)
            return NotFound(new { error = "Reward not found" });
        return Ok(reward);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<Reward>> Create([FromBody] Reward reward)
    {
        var result = await _rewardService.AddRewardAsync(reward);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<Reward>> Update(int id, [FromBody] Reward reward)
    {
        if (id != reward.Id)
            return BadRequest(new { error = "Id mismatch" });

        var existingReward = await _rewardService.GetRewardAsync(id);
        if (existingReward == null)
            return NotFound(new { error = "Reward not found" });

        var result = await _rewardService.UpdateRewardAsync(reward);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var reward = await _rewardService.GetRewardAsync(id);
        if (reward == null)
            return NotFound(new { error = "Reward not found" });

        await _rewardService.DeleteRewardAsync(id);
        return NoContent();
    }
} 