using EShopDomain.Models;
using EShop.Domain.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EShop.Application.Service
{
    public class PointsService : IPointsService
    {
        private readonly IRepository _repository;
        public PointsService(IRepository repository)
        {
            _repository = repository;
        }
        public Task EarnPointsAsync(int memberId, int points, string description) => _repository.EarnPointsAsync(memberId, points, description);
        public Task<bool> SpendPointsAsync(int memberId, int points, string description) => _repository.SpendPointsAsync(memberId, points, description);
        public Task<List<PointsTransaction>> GetPointsTransactionsByMemberAsync(int memberId) => _repository.GetPointsTransactionsByMemberAsync(memberId);
        public Task<List<PointsTransaction>> GetAllPointsTransactionsAsync() => _repository.GetAllPointsTransactionsAsync();
    }
} 