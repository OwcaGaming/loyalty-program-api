using EShop.Domain.Models;

namespace EShop.Application.Services.Auth;

public record AuthResult(bool Success, string[] Errors, string? Token = null, User? User = null);
public record LoginRequest(string Email, string Password);
public record RegisterRequest(string Email, string Password, string FirstName, string LastName);

public interface IAuthService
{
    Task<AuthResult> RegisterAsync(RegisterRequest request);
    Task<AuthResult> LoginAsync(LoginRequest request);
    Task<AuthResult> ChangePasswordAsync(string userId, string currentPassword, string newPassword);
    Task<bool> ValidateTokenAsync(string token);
} 