using EShop.Domain.Data;
using EShop.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace EShop.Domain.Repositories;

public class ProductRepository : Repository<Product>, IProductRepository
{
    public ProductRepository(IApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId)
    {
        return await _dbSet
            .Where(p => p.CategoryId == categoryId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetProductsByIdsAsync(IEnumerable<int> productIds)
    {
        return await _dbSet
            .Where(p => productIds.Contains(p.Id))
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm)
    {
        return await _dbSet
            .Where(p => p.Name.Contains(searchTerm) || p.Description.Contains(searchTerm))
            .ToListAsync();
    }

    public async Task<bool> UpdateStockQuantityAsync(int productId, int quantity)
    {
        var product = await _dbSet.FindAsync(productId);
        if (product == null)
            return false;

        product.StockQuantity = quantity;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<Product>> GetProductsWithLowStockAsync(int threshold = 10)
    {
        return await _dbSet
            .Where(p => p.StockQuantity <= threshold)
            .ToListAsync();
    }
} 