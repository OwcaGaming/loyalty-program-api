using EShop.Application.Service;
using EShop.Domain.Models;
using EShop.Domain.Repositories;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;

namespace EShop.Application.Services;

public class RewardService : IRewardService
{
    private readonly IRewardRepository _rewardRepository;
    private readonly IMemberRepository _memberRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RewardService(
        IRewardRepository rewardRepository, 
        IMemberRepository memberRepository,
        IUnitOfWork unitOfWork)
    {
        _rewardRepository = rewardRepository;
        _memberRepository = memberRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<List<Reward>> GetAllRewardsAsync()
    {
        var result = await _rewardRepository.GetAllAsync();
        return result.ToList();
    }

    public async Task<List<Reward>> GetActiveRewardsAsync()
    {
        var result = await _rewardRepository.GetActiveRewardsAsync();
        return result.ToList();
    }

    public async Task<Reward?> GetRewardAsync(int id)
    {
        return await _rewardRepository.GetByIdAsync(id);
    }

    public async Task<Reward?> AddRewardAsync(Reward reward)
    {
        if (reward == null)
            return null;

        if (string.IsNullOrWhiteSpace(reward.Name))
            throw new ArgumentException("Reward name is required");

        if (reward.PointsCost <= 0)
            throw new ArgumentException("Points cost must be greater than zero");

        if (reward.StockQuantity < 0)
            throw new ArgumentException("Stock quantity cannot be negative");

        reward.CreatedAt = DateTime.UtcNow;
        reward.IsActive = true;
        return await _rewardRepository.AddAsync(reward);
    }

    public async Task<Reward?> UpdateRewardAsync(Reward reward)
    {
        if (reward == null)
            return null;

        var existingReward = await _rewardRepository.GetByIdAsync(reward.Id);
        if (existingReward == null)
            return null;

        if (string.IsNullOrWhiteSpace(reward.Name))
            throw new ArgumentException("Reward name is required");

        if (reward.PointsCost <= 0)
            throw new ArgumentException("Points cost must be greater than zero");

        if (reward.StockQuantity < 0)
            throw new ArgumentException("Stock quantity cannot be negative");

        reward.UpdatedAt = DateTime.UtcNow;
        return await _rewardRepository.UpdateAsync(reward);
    }

    public async Task<bool> DeleteRewardAsync(int id)
    {
        var reward = await _rewardRepository.GetByIdAsync(id);
        if (reward == null)
            return false;

        await _rewardRepository.DeleteAsync(reward);
        return true;
    }

    public async Task<bool> RedeemRewardAsync(int memberId, int rewardId)
    {
        var member = await _memberRepository.GetByIdAsync(memberId);
        if (member == null)
            return false;

        var reward = await _rewardRepository.GetByIdAsync(rewardId);
        if (reward == null || !reward.IsActive || reward.StockQuantity <= 0)
            return false;

        if (member.PointsBalance < reward.PointsCost)
            return false;

        try
        {
            await _unitOfWork.BeginTransactionAsync();

            // Update member points
            await _memberRepository.UpdatePointsBalanceAsync(memberId, member.PointsBalance - reward.PointsCost);

            // Update reward stock
            reward.StockQuantity--;
            if (reward.StockQuantity == 0)
                reward.IsActive = false;

            await _rewardRepository.UpdateAsync(reward);

            // Create redemption record
            var redemption = new RewardRedemption
            {
                MemberId = memberId,
                RewardId = rewardId,
                RedeemedAt = DateTime.UtcNow,
                PointsCost = reward.PointsCost
            };

            await _rewardRepository.AddRedemptionAsync(redemption);

            await _unitOfWork.CommitTransactionAsync();
            return true;
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }

    public async Task<List<RewardRedemption>> GetMemberRedemptionsAsync(int memberId)
    {
        return await _rewardRepository.GetMemberRedemptionsAsync(memberId);
    }

    public async Task<bool> UpdateStockQuantityAsync(int rewardId, int quantity)
    {
        if (quantity < 0)
            return false;

        var reward = await _rewardRepository.GetByIdAsync(rewardId);
        if (reward == null)
            return false;

        try
        {
            await _unitOfWork.BeginTransactionAsync();

            reward.StockQuantity = quantity;
            reward.IsActive = quantity > 0;
            reward.UpdatedAt = DateTime.UtcNow;

            await _rewardRepository.UpdateAsync(reward);

            await _unitOfWork.CommitTransactionAsync();
            return true;
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }

    public async Task<List<Reward>> GetRewardsByPointsRangeAsync(int minPoints, int maxPoints)
    {
        if (minPoints < 0 || maxPoints < minPoints)
            return new List<Reward>();

        return await _rewardRepository.GetRewardsByPointsRangeAsync(minPoints, maxPoints);
    }
} 