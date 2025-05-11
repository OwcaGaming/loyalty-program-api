using EShopDomain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EShop.Application.Service
{
    public interface IPointsService
    {
        Task EarnPointsAsync(int memberId, int points, string description);
        Task<bool> SpendPointsAsync(int memberId, int points, string description);
        Task<List<PointsTransaction>> GetPointsTransactionsByMemberAsync(int memberId);
        Task<List<PointsTransaction>> GetAllPointsTransactionsAsync();
    }
} 