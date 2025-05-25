using EShop.Domain.Data;
using EShop.Domain.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EShop.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Member> Members { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<PointsTransaction> PointsTransactions { get; set; }
    public DbSet<Reward> Rewards { get; set; }
    public DbSet<RewardRedemption> RewardRedemptions { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Configure decimal precision for money-related properties
        builder.Entity<Product>()
            .Property(p => p.Price)
            .HasPrecision(18, 2);

        builder.Entity<Order>()
            .Property(o => o.TotalAmount)
            .HasPrecision(18, 2);

        builder.Entity<OrderItem>()
            .Property(oi => oi.UnitPrice)
            .HasPrecision(18, 2);

        builder.Entity<Reward>()
            .Property(r => r.DiscountAmount)
            .HasPrecision(18, 2);

        // Configure relationships
        builder.Entity<Order>()
            .HasMany(o => o.OrderItems)
            .WithOne(oi => oi.Order)
            .HasForeignKey(oi => oi.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Member>()
            .HasMany(m => m.Orders)
            .WithOne(o => o.Member)
            .HasForeignKey(o => o.MemberId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Member>()
            .HasMany(m => m.PointsTransactions)
            .WithOne(pt => pt.Member)
            .HasForeignKey(pt => pt.MemberId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Product>()
            .HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<RewardRedemption>()
            .HasOne(r => r.Member)
            .WithMany(m => m.RewardRedemptions)
            .HasForeignKey(r => r.MemberId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<RewardRedemption>()
            .HasOne(r => r.Reward)
            .WithMany(r => r.Redemptions)
            .HasForeignKey(r => r.RewardId)
            .OnDelete(DeleteBehavior.Restrict);
    }
} 