using EShop.Domain.Models;
using EShop.Domain.Repositories;

namespace EShop.Application.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;

    public ProductService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        return await _productRepository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        return await _productRepository.GetAllAsync();
    }

    public async Task<Product> CreateAsync(Product product)
    {
        return await _productRepository.AddAsync(product);
    }

    public async Task<Product> UpdateAsync(Product product)
    {
        return await _productRepository.UpdateAsync(product);
    }

    public async Task DeleteAsync(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product != null)
        {
            await _productRepository.DeleteAsync(product);
        }
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _productRepository.ExistsAsync(p => p.Id == id);
    }

    public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId)
    {
        return await _productRepository.GetProductsByCategoryAsync(categoryId);
    }

    public async Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm)
    {
        return await _productRepository.SearchProductsAsync(searchTerm);
    }

    public async Task<bool> UpdateStockQuantityAsync(int productId, int quantity)
    {
        if (quantity < 0)
            return false;

        return await _productRepository.UpdateStockQuantityAsync(productId, quantity);
    }

    public async Task<IEnumerable<Product>> GetProductsWithLowStockAsync(int threshold = 10)
    {
        return await _productRepository.GetProductsWithLowStockAsync(threshold);
    }

    public async Task<bool> IsInStockAsync(int productId, int requestedQuantity)
    {
        var product = await _productRepository.GetByIdAsync(productId);
        return product != null && product.StockQuantity >= requestedQuantity;
    }

    public async Task<int> GetAvailableStockAsync(int productId)
    {
        var product = await _productRepository.GetByIdAsync(productId);
        return product?.StockQuantity ?? 0;
    }
} 