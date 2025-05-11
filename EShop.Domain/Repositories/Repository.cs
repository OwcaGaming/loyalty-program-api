using EShopDomain.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EShop.Domain.Repositories
{
    public class Repository : IRepository
    {
        private readonly DataContext _context;

        public Repository(DataContext dataContext)
        {
            _context = dataContext;
        }

        // Member
        public async Task<Member> AddMemberAsync(Member member)
        {
            _context.Members.Add(member);
            await _context.SaveChangesAsync();
            return member;
        }

        public async Task<Member?> GetMemberAsync(int id)
        {
            return await _context.Members.FindAsync(id);
        }

        public async Task<List<Member>> GetAllMembersAsync()
        {
            return await _context.Members.ToListAsync();
        }

        public async Task<Member> UpdateMemberAsync(Member member)
        {
            _context.Members.Update(member);
            await _context.SaveChangesAsync();
            return member;
        }

        public async Task DeleteMemberAsync(int id)
        {
            var member = await _context.Members.FindAsync(id);
            if (member != null)
            {
                _context.Members.Remove(member);
                await _context.SaveChangesAsync();
            }
        }

        // PointsTransaction
        public async Task<PointsTransaction> AddPointsTransactionAsync(PointsTransaction transaction)
        {
            _context.PointsTransactions.Add(transaction);
            await _context.SaveChangesAsync();
            return transaction;
        }

        public async Task<List<PointsTransaction>> GetPointsTransactionsByMemberAsync(int memberId)
        {
            return await _context.PointsTransactions.Where(t => t.MemberId == memberId).ToListAsync();
        }

        public async Task<List<PointsTransaction>> GetAllPointsTransactionsAsync()
        {
            return await _context.PointsTransactions.ToListAsync();
        }

        // Reward
        public async Task<Reward> AddRewardAsync(Reward reward)
        {
            _context.Rewards.Add(reward);
            await _context.SaveChangesAsync();
            return reward;
        }

        public async Task<Reward?> GetRewardAsync(int id)
        {
            return await _context.Rewards.FindAsync(id);
        }

        public async Task<List<Reward>> GetAllRewardsAsync()
        {
            return await _context.Rewards.ToListAsync();
        }

        public async Task<List<Reward>> GetActiveRewardsAsync()
        {
            return await _context.Rewards.Where(r => r.IsActive).ToListAsync();
        }

        public async Task<Reward> UpdateRewardAsync(Reward reward)
        {
            _context.Rewards.Update(reward);
            await _context.SaveChangesAsync();
            return reward;
        }

        public async Task DeleteRewardAsync(int id)
        {
            var reward = await _context.Rewards.FindAsync(id);
            if (reward != null)
            {
                _context.Rewards.Remove(reward);
                await _context.SaveChangesAsync();
            }
        }

        // Points operations
        public async Task EarnPointsAsync(int memberId, int points, string description)
        {
            var member = await _context.Members.FindAsync(memberId);
            if (member != null)
            {
                member.PointsBalance += points;
                await AddPointsTransactionAsync(new PointsTransaction
                {
                    MemberId = memberId,
                    Points = points,
                    Type = PointsTransactionType.Earn,
                    Description = description
                });
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> SpendPointsAsync(int memberId, int points, string description)
        {
            var member = await _context.Members.FindAsync(memberId);
            if (member != null && member.PointsBalance >= points)
            {
                member.PointsBalance -= points;
                await AddPointsTransactionAsync(new PointsTransaction
                {
                    MemberId = memberId,
                    Points = points,
                    Type = PointsTransactionType.Spend,
                    Description = description
                });
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}
