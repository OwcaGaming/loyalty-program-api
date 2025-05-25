using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace EShop.Domain.Models;

public class User : IdentityUser
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required Member Member { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsActive { get; set; } = true;

    public string FullName => $"{FirstName} {LastName}";

    public User()
    {
        Member = new Member
        {
            Name = $"{FirstName} {LastName}",
            Email = Email ?? string.Empty,
            DateJoined = DateTime.UtcNow,
            Tier = MemberTier.Standard
        };
    }
} 