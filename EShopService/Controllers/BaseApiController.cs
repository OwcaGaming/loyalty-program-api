using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EShopService.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public abstract class BaseApiController : ControllerBase
{
    protected IActionResult HandleResult<T>(T result)
    {
        if (result == null)
            return NotFound();
            
        return Ok(result);
    }

    protected string GetUserId()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
            throw new UnauthorizedAccessException("User ID not found in claims");
        return userId;
    }
} 