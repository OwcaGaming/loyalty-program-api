using EShop.Application.Service;
using EShop.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Earn([FromBody] EarnPointsRequest request)
        {
            await _pointsService.EarnPointsAsync(request.MemberId, request.Points, request.Description);
            return Ok(new { message = "Points earned successfully" });
        }

        [HttpPost("spend")]
        [Authorize]
        public async Task<IActionResult> Spend([FromBody] SpendPointsRequest request)
        {
            var success = await _pointsService.SpendPointsAsync(request.MemberId, request.Points, request.Description);
            if (!success)
                return BadRequest(new { error = "Insufficient points" });
            return Ok(new { message = "Points spent successfully" });
        }

        [HttpGet("member/{memberId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<PointsTransaction>>> GetByMember(int memberId)
        {
            var transactions = await _pointsService.GetPointsTransactionsByMemberAsync(memberId);
            return Ok(transactions);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<PointsTransaction>>> GetAll()
        {
            var transactions = await _pointsService.GetAllPointsTransactionsAsync();
            return Ok(transactions);
        }
    }

    public class EarnPointsRequest
    {
        public required int MemberId { get; set; }
        public required int Points { get; set; }
        public required string Description { get; set; }
    }

    public class SpendPointsRequest
    {
        public required int MemberId { get; set; }
        public required int Points { get; set; }
        public required string Description { get; set; }
    }
} 