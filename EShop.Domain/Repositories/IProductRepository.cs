using EShop.Domain.Models;

namespace EShop.Domain.Repositories;

public interface IProductRepository : IRepository<Product>
{
    Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId);
    Task<IEnumerable<Product>> GetProductsByIdsAsync(IEnumerable<int> productIds);
    Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm);
    Task<bool> UpdateStockQuantityAsync(int productId, int quantity);
    Task<IEnumerable<Product>> GetProductsWithLowStockAsync(int threshold = 10);
} 