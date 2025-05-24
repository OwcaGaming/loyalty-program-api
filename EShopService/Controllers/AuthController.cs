using EShop.Application.Services.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EShopService.Controllers;

public class AuthController : BaseApiController
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var result = await _authService.RegisterAsync(request);
        if (!result.Success)
            return BadRequest(result.Errors);

        return Ok(new { result.Token, result.User });
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var result = await _authService.LoginAsync(request);
        if (!result.Success)
            return BadRequest(result.Errors);

        return Ok(new { result.Token, result.User });
    }

    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword(string currentPassword, string newPassword)
    {
        var userId = GetUserId();
        var result = await _authService.ChangePasswordAsync(userId, currentPassword, newPassword);
        if (!result.Success)
            return BadRequest(result.Errors);

        return Ok(new { result.Token });
    }

    [HttpPost("validate-token")]
    [AllowAnonymous]
    public async Task<IActionResult> ValidateToken(string token)
    {
        var isValid = await _authService.ValidateTokenAsync(token);
        return Ok(new { isValid });
    }
} 