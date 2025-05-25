using EShop.Domain.Models;

namespace EShop.Domain.Repositories;

public interface IPointsTransactionRepository : IRepository<PointsTransaction>
{
    Task<IEnumerable<PointsTransaction>> GetByMemberIdAsync(int memberId);
    Task<PointsTransaction> CreateTransactionAsync(int memberId, int points, string description, PointsTransactionType type);
} 