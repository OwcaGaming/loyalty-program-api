using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EShop.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace EShop.Application.Services.Auth;

public class AuthService : IAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly JwtSettings _jwtSettings;
    private readonly IMemberService _memberService;

    public AuthService(
        UserManager<User> userManager,
        IOptions<JwtSettings> jwtSettings,
        IMemberService memberService)
    {
        _userManager = userManager;
        _jwtSettings = jwtSettings.Value;
        _memberService = memberService;
    }

    public async Task<AuthResult> RegisterAsync(RegisterRequest request)
    {
        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser != null)
        {
            return new AuthResult(false, new[] { "User with this email already exists" });
        }

        // Create member first
        var member = new Member
        {
            Name = $"{request.FirstName} {request.LastName}",
            Email = request.Email,
            DateJoined = DateTime.UtcNow,
            Tier = MemberTier.Standard
        };

        member = await _memberService.CreateAsync(member);

        var user = new User
        {
            Email = request.Email,
            UserName = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            CreatedAt = DateTime.UtcNow,
            IsActive = true,
            Member = member
        };

        var createUserResult = await _userManager.CreateAsync(user, request.Password);
        if (!createUserResult.Succeeded)
        {
            return new AuthResult(false, createUserResult.Errors.Select(e => e.Description).ToArray());
        }

        // Update member with user ID
        member.UserId = user.Id;
        await _memberService.UpdateAsync(member);

        var token = GenerateJwtToken(user);
        return new AuthResult(true, Array.Empty<string>(), token, user);
    }

    public async Task<AuthResult> LoginAsync(LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null || !user.IsActive)
        {
            return new AuthResult(false, new[] { "Invalid credentials" });
        }

        var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!isPasswordValid)
        {
            return new AuthResult(false, new[] { "Invalid credentials" });
        }

        var token = GenerateJwtToken(user);
        return new AuthResult(true, Array.Empty<string>(), token, user);
    }

    public async Task<AuthResult> ChangePasswordAsync(string userId, string currentPassword, string newPassword)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return new AuthResult(false, new[] { "User not found" });
        }

        var changePasswordResult = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
        if (!changePasswordResult.Succeeded)
        {
            return new AuthResult(false, changePasswordResult.Errors.Select(e => e.Description).ToArray());
        }

        var token = GenerateJwtToken(user);
        return new AuthResult(true, Array.Empty<string>(), token, user);
    }

    public async Task<bool> ValidateTokenAsync(string token)
    {
        if (string.IsNullOrEmpty(token))
            return false;

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtSettings.SecretKey);

        try
        {
            await Task.Run(() => tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = _jwtSettings.Issuer,
                ValidAudience = _jwtSettings.Audience,
                ClockSkew = TimeSpan.Zero
            }, out _));

            return true;
        }
        catch
        {
            return false;
        }
    }

    private string GenerateJwtToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtSettings.SecretKey);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
            new(ClaimTypes.Name, user.UserName ?? user.Email ?? string.Empty),
            new(ClaimTypes.GivenName, user.FirstName),
            new(ClaimTypes.Surname, user.LastName)
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature),
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
} 