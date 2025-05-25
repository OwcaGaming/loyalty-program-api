using EShop.Domain.Models;
using EShop.Domain.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EShop.Application.Service
{
    public class PointsService : IPointsService
    {
        private readonly IMemberRepository _memberRepository;
        private readonly IPointsTransactionRepository _transactionRepository;

        public PointsService(
            IMemberRepository memberRepository,
            IPointsTransactionRepository transactionRepository)
        {
            _memberRepository = memberRepository;
            _transactionRepository = transactionRepository;
        }

        public async Task EarnPointsAsync(int memberId, int points, string description)
        {
            var member = await _memberRepository.GetByIdAsync(memberId);
            if (member != null)
            {
                member.PointsBalance += points;
                await _memberRepository.UpdateAsync(member);
                await _transactionRepository.CreateTransactionAsync(memberId, points, description, PointsTransactionType.Earn);
            }
        }

        public async Task<bool> SpendPointsAsync(int memberId, int points, string description)
        {
            var member = await _memberRepository.GetByIdAsync(memberId);
            if (member != null && member.PointsBalance >= points)
            {
                member.PointsBalance -= points;
                await _memberRepository.UpdateAsync(member);
                await _transactionRepository.CreateTransactionAsync(memberId, points, description, PointsTransactionType.Spend);
                return true;
            }
            return false;
        }

        public async Task<List<PointsTransaction>> GetPointsTransactionsByMemberAsync(int memberId)
        {
            var transactions = await _transactionRepository.GetByMemberIdAsync(memberId);
            return transactions.ToList();
        }

        public async Task<List<PointsTransaction>> GetAllPointsTransactionsAsync()
        {
            var transactions = await _transactionRepository.GetAllAsync();
            return transactions.ToList();
        }
    }
} 