using EShop.Domain.Models;

namespace EShop.Application.Services;

public interface IProductService : IService<Product>
{
    Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId);
    Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm);
    Task<bool> UpdateStockQuantityAsync(int productId, int quantity);
    Task<IEnumerable<Product>> GetProductsWithLowStockAsync(int threshold = 10);
    Task<bool> IsInStockAsync(int productId, int requestedQuantity);
    Task<int> GetAvailableStockAsync(int productId);
} 