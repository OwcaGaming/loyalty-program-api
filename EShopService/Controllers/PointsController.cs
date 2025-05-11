using EShop.Application.Service;
using EShopDomain.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace EShopService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PointsController : ControllerBase
    {
        private readonly IPointsService _pointsService;
        public PointsController(IPointsService pointsService)
        {
            _pointsService = pointsService;
        }

        [HttpPost("earn")]
        public async Task<IActionResult> Earn([FromQuery] int memberId, [FromQuery] int points, [FromQuery] string description)
        {
            await _pointsService.EarnPointsAsync(memberId, points, description);
            return Ok();
        }

        [HttpPost("spend")]
        public async Task<IActionResult> Spend([FromQuery] int memberId, [FromQuery] int points, [FromQuery] string description)
        {
            var success = await _pointsService.SpendPointsAsync(memberId, points, description);
            if (!success) return BadRequest("Insufficient points");
            return Ok();
        }

        [HttpGet("member/{memberId}")]
        public async Task<IActionResult> GetByMember(int memberId)
        {
            var transactions = await _pointsService.GetPointsTransactionsByMemberAsync(memberId);
            return Ok(transactions);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var transactions = await _pointsService.GetAllPointsTransactionsAsync();
            return Ok(transactions);
        }
    }
} 