using EShop.Domain.Models;
using EShop.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace EShop.Application.Services;

public class MemberService : IMemberService
{
    private readonly IMemberRepository _memberRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IPointsTransactionRepository _pointsTransactionRepository;
    private readonly IUnitOfWork _unitOfWork;
    private const decimal SILVER_TIER_THRESHOLD = 1000m;
    private const decimal GOLD_TIER_THRESHOLD = 5000m;
    private const decimal PLATINUM_TIER_THRESHOLD = 10000m;

    public MemberService(
        IMemberRepository memberRepository,
        IOrderRepository orderRepository,
        IPointsTransactionRepository pointsTransactionRepository,
        IUnitOfWork unitOfWork)
    {
        _memberRepository = memberRepository;
        _orderRepository = orderRepository;
        _pointsTransactionRepository = pointsTransactionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Member?> GetByIdAsync(int id)
    {
        return await _memberRepository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<Member>> GetAllAsync()
    {
        return await _memberRepository.GetAllAsync();
    }

    public async Task<Member> CreateAsync(Member member)
    {
        if (member == null)
            throw new ArgumentNullException(nameof(member));

        if (string.IsNullOrWhiteSpace(member.Name))
            throw new ArgumentException("Member name is required");

        if (string.IsNullOrWhiteSpace(member.Email))
            throw new ArgumentException("Member email is required");

        if (await _memberRepository.GetByEmailAsync(member.Email) != null)
            throw new InvalidOperationException("A member with this email already exists");

        member.Tier = MemberTier.Standard;
        member.DateJoined = DateTime.UtcNow;
        member.PointsBalance = 0;
        
        return await _memberRepository.AddAsync(member);
    }

    public async Task<Member> UpdateAsync(Member member)
    {
        if (member == null)
            throw new ArgumentNullException(nameof(member));

        var existingMember = await _memberRepository.GetByIdAsync(member.Id);
        if (existingMember == null)
            throw new InvalidOperationException($"Member with ID {member.Id} not found");

        if (string.IsNullOrWhiteSpace(member.Name))
            throw new ArgumentException("Member name is required");

        if (string.IsNullOrWhiteSpace(member.Email))
            throw new ArgumentException("Member email is required");

        var emailOwner = await _memberRepository.GetByEmailAsync(member.Email);
        if (emailOwner != null && emailOwner.Id != member.Id)
            throw new InvalidOperationException("This email is already in use by another member");

        // Preserve certain fields that shouldn't be updated directly
        member.DateJoined = existingMember.DateJoined;
        member.PointsBalance = existingMember.PointsBalance;
        member.Tier = existingMember.Tier;

        return await _memberRepository.UpdateAsync(member);
    }

    public async Task DeleteAsync(int id)
    {
        var member = await _memberRepository.GetByIdAsync(id);
        if (member != null)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();
                await _memberRepository.DeleteAsync(member);
                await _unitOfWork.CommitTransactionAsync();
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _memberRepository.ExistsAsync(m => m.Id == id);
    }

    public async Task<Member?> GetByUserIdAsync(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("User ID is required");

        return await _memberRepository.GetByUserIdAsync(userId);
    }

    public async Task<Member?> GetByEmailAsync(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email is required");

        return await _memberRepository.GetByEmailAsync(email);
    }

    public async Task<Member?> GetWithOrderHistoryAsync(int memberId)
    {
        return await _memberRepository.GetWithOrderHistoryAsync(memberId);
    }

    public async Task<bool> UpdatePointsBalanceAsync(int memberId, int points)
    {
        if (points < 0)
            return false;

        var member = await _memberRepository.GetByIdAsync(memberId);
        if (member == null)
            return false;

        try
        {
            await _unitOfWork.BeginTransactionAsync();

            member.PointsBalance = points;
            await _memberRepository.UpdateAsync(member);

            await _unitOfWork.CommitTransactionAsync();
            return true;
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }

    public async Task<bool> AddPointsTransactionAsync(int memberId, int points, string description)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Transaction description is required");

        var member = await _memberRepository.GetByIdAsync(memberId);
        if (member == null)
            return false;

        var newBalance = member.PointsBalance + points;
        if (newBalance < 0)
            return false;

        try
        {
            await _unitOfWork.BeginTransactionAsync();

            member.PointsBalance = newBalance;
            await _memberRepository.UpdateAsync(member);

            var type = points >= 0 ? PointsTransactionType.Earn : PointsTransactionType.Spend;
            await _pointsTransactionRepository.CreateTransactionAsync(memberId, Math.Abs(points), description, type);

            await _unitOfWork.CommitTransactionAsync();
            return true;
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }

    public async Task<bool> UpdateMemberTierAsync(int memberId, MemberTier tier)
    {
        var member = await _memberRepository.GetByIdAsync(memberId);
        if (member == null)
            return false;

        // Validate tier change based on total spent
        if (tier != MemberTier.Standard)
        {
            var totalSpent = await GetTotalSpentAsync(memberId);
            var calculatedTier = await CalculateMemberTierAsync(memberId);

            if (tier > calculatedTier)
                return false; // Cannot upgrade to a tier higher than earned
        }

        return await _memberRepository.UpdateMemberTierAsync(memberId, tier);
    }

    public async Task<IEnumerable<Member>> GetMembersByTierAsync(MemberTier tier)
    {
        return await _memberRepository.GetMembersByTierAsync(tier);
    }

    public async Task<IEnumerable<PointsTransaction>> GetPointsTransactionsAsync(int memberId)
    {
        var member = await _memberRepository.GetByIdAsync(memberId);
        if (member == null)
            throw new ArgumentException("Member not found");

        return await _pointsTransactionRepository.GetByMemberIdAsync(memberId);
    }

    public async Task<MemberTier> CalculateMemberTierAsync(int memberId)
    {
        var member = await _memberRepository.GetByIdAsync(memberId);
        if (member == null)
            throw new ArgumentException("Member not found");

        var totalSpent = await GetTotalSpentAsync(memberId);

        return totalSpent switch
        {
            >= PLATINUM_TIER_THRESHOLD => MemberTier.Platinum,
            >= GOLD_TIER_THRESHOLD => MemberTier.Gold,
            >= SILVER_TIER_THRESHOLD => MemberTier.Silver,
            _ => MemberTier.Standard
        };
    }

    public async Task<decimal> GetTotalSpentAsync(int memberId)
    {
        var member = await _memberRepository.GetByIdAsync(memberId);
        if (member == null)
            throw new ArgumentException("Member not found");

        return await _orderRepository.GetTotalOrderAmountByMemberAsync(memberId);
    }

    public async Task<int> GetTotalPointsEarnedAsync(int memberId)
    {
        var member = await _memberRepository.GetWithOrderHistoryAsync(memberId);
        if (member == null)
            throw new ArgumentException("Member not found");

        return member.Orders
            .Where(o => o.Status == OrderStatus.Delivered)
            .Sum(o => o.PointsEarned ?? 0);
    }
} 