using EShop.Domain.Models;
using EShop.Domain.Repositories;
using EShop.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EShop.Infrastructure.Repositories;

public class ProductRepository : Repository<Product>, IProductRepository
{
    private new readonly ApplicationDbContext _context;

    public ProductRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId)
    {
        return await _context.Products
            .Include(p => p.Category)
            .Where(p => p.CategoryId == categoryId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetProductsByIdsAsync(IEnumerable<int> ids)
    {
        return await _context.Products
            .Include(p => p.Category)
            .Where(p => ids.Contains(p.Id))
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm)
    {
        return await _context.Products
            .Include(p => p.Category)
            .Where(p => p.Name.Contains(searchTerm) || 
                       p.Description.Contains(searchTerm) ||
                       p.SKU.Contains(searchTerm))
            .ToListAsync();
    }

    public async Task<bool> UpdateStockQuantityAsync(int productId, int quantity)
    {
        var product = await _context.Products.FindAsync(productId);
        if (product == null)
            return false;

        product.StockQuantity = quantity;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<Product>> GetProductsWithLowStockAsync(int threshold)
    {
        return await _context.Products
            .Include(p => p.Category)
            .Where(p => p.StockQuantity <= threshold)
            .ToListAsync();
    }
} 