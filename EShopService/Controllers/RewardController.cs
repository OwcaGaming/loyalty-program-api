using EShop.Application.Service;
using EShopDomain.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace EShopService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RewardController : ControllerBase
    {
        private readonly IRewardService _rewardService;
        public RewardController(IRewardService rewardService)
        {
            _rewardService = rewardService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var rewards = await _rewardService.GetAllRewardsAsync();
            return Ok(rewards);
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetActive()
        {
            var rewards = await _rewardService.GetActiveRewardsAsync();
            return Ok(rewards);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var reward = await _rewardService.GetRewardAsync(id);
            if (reward == null) return NotFound();
            return Ok(reward);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Reward reward)
        {
            var created = await _rewardService.AddRewardAsync(reward);
            return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Reward reward)
        {
            if (id != reward.Id) return BadRequest();
            var updated = await _rewardService.UpdateRewardAsync(reward);
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _rewardService.DeleteRewardAsync(id);
            return NoContent();
        }
    }
} 