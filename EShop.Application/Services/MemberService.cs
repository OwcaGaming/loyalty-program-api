using EShop.Domain.Models;
using EShop.Domain.Repositories;

namespace EShop.Application.Services;

public class MemberService : IMemberService
{
    private readonly IMemberRepository _memberRepository;
    private readonly IOrderRepository _orderRepository;
    private const decimal SILVER_TIER_THRESHOLD = 1000m;
    private const decimal GOLD_TIER_THRESHOLD = 5000m;
    private const decimal PLATINUM_TIER_THRESHOLD = 10000m;

    public MemberService(IMemberRepository memberRepository, IOrderRepository orderRepository)
    {
        _memberRepository = memberRepository;
        _orderRepository = orderRepository;
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
        member.Tier = MemberTier.Standard;
        member.DateJoined = DateTime.UtcNow;
        return await _memberRepository.AddAsync(member);
    }

    public async Task<Member> UpdateAsync(Member member)
    {
        return await _memberRepository.UpdateAsync(member);
    }

    public async Task DeleteAsync(int id)
    {
        var member = await _memberRepository.GetByIdAsync(id);
        if (member != null)
        {
            await _memberRepository.DeleteAsync(member);
        }
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _memberRepository.ExistsAsync(m => m.Id == id);
    }

    public async Task<Member?> GetByUserIdAsync(string userId)
    {
        return await _memberRepository.GetByUserIdAsync(userId);
    }

    public async Task<Member?> GetByEmailAsync(string email)
    {
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

        return await _memberRepository.UpdatePointsBalanceAsync(memberId, points);
    }

    public async Task<bool> AddPointsTransactionAsync(int memberId, int points, string description)
    {
        var member = await _memberRepository.GetByIdAsync(memberId);
        if (member == null)
            return false;

        var transaction = new PointsTransaction
        {
            MemberId = memberId,
            Points = points,
            Description = description,
            TransactionDate = DateTime.UtcNow
        };

        var newBalance = member.PointsBalance + points;
        if (newBalance < 0)
            return false;

        member.PointsBalance = newBalance;
        await _memberRepository.UpdateAsync(member);

        return true;
    }

    public async Task<bool> UpdateMemberTierAsync(int memberId, MemberTier tier)
    {
        return await _memberRepository.UpdateMemberTierAsync(memberId, tier);
    }

    public async Task<IEnumerable<Member>> GetMembersByTierAsync(MemberTier tier)
    {
        return await _memberRepository.GetMembersByTierAsync(tier);
    }

    public async Task<IEnumerable<PointsTransaction>> GetPointsTransactionsAsync(int memberId)
    {
        return await _memberRepository.GetPointsTransactionsAsync(memberId);
    }

    public async Task<MemberTier> CalculateMemberTierAsync(int memberId)
    {
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
        return await _orderRepository.GetTotalOrderAmountByMemberAsync(memberId);
    }

    public async Task<int> GetTotalPointsEarnedAsync(int memberId)
    {
        var member = await _memberRepository.GetWithOrderHistoryAsync(memberId);
        if (member == null)
            return 0;

        return member.Orders
            .Where(o => o.Status == OrderStatus.Delivered)
            .Sum(o => o.PointsEarned ?? 0);
    }
} 