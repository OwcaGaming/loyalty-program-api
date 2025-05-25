using EShop.Domain.Models;
using EShop.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;
using System.Text;
using Newtonsoft.Json;

namespace EShopService.IntegrationTests.Controllers;

public class ProductControllerIntegrationTest : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly WebApplicationFactory<Program> _factory;

    public ProductControllerIntegrationTest(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                var dbContextOptions = services
                    .SingleOrDefault(service => service.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

                if (dbContextOptions != null)
                    services.Remove(dbContextOptions);

                services.AddDbContext<ApplicationDbContext>(options => 
                    options.UseInMemoryDatabase("TestDb"));
            });
        });

        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task GetAll_ReturnsAllProducts()
    {
        // Arrange
        using (var scope = _factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await dbContext.Database.EnsureDeletedAsync();
            await dbContext.Database.EnsureCreatedAsync();

            var category = new Category 
            { 
                Name = "Test Category",
                Description = "Test Category Description"
            };
            dbContext.Categories.Add(category);
            await dbContext.SaveChangesAsync();

            dbContext.Products.AddRange(
                new Product 
                { 
                    Name = "Product1",
                    Description = "Description 1",
                    SKU = "SKU001",
                    Price = 10.99m,
                    StockQuantity = 100,
                    Category = category
                },
                new Product 
                { 
                    Name = "Product2",
                    Description = "Description 2",
                    SKU = "SKU002",
                    Price = 20.99m,
                    StockQuantity = 50,
                    Category = category
                }
            );
            await dbContext.SaveChangesAsync();
        }

        // Act
        var response = await _client.GetAsync("/api/products");

        // Assert
        response.EnsureSuccessStatusCode();
        var products = await response.Content.ReadFromJsonAsync<List<Product>>();
        Assert.NotNull(products);
        Assert.Equal(2, products.Count);
    }

    [Fact]
    public async Task Create_ValidProduct_ReturnsCreatedProduct()
    {
        // Arrange
        Category category;
        using (var scope = _factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await dbContext.Database.EnsureDeletedAsync();
            await dbContext.Database.EnsureCreatedAsync();

            category = new Category 
            { 
                Name = "Test Category",
                Description = "Test Category Description"
            };
            dbContext.Categories.Add(category);
            await dbContext.SaveChangesAsync();
        }

        var product = new Product
        {
            Name = "New Product",
            Description = "Test Description",
            SKU = "SKU003",
            Price = 29.99m,
            StockQuantity = 100,
            Category = category
        };

        // Act
        var json = JsonConvert.SerializeObject(product);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("/api/products", content);

        // Assert
        response.EnsureSuccessStatusCode();
        var createdProduct = await response.Content.ReadFromJsonAsync<Product>();
        Assert.NotNull(createdProduct);
        Assert.Equal(product.Name, createdProduct.Name);
        Assert.Equal(product.Price, createdProduct.Price);
    }

    [Fact]
    public async Task Update_ValidProduct_ReturnsUpdatedProduct()
    {
        // Arrange
        Product existingProduct;
        Category category;
        using (var scope = _factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await dbContext.Database.EnsureDeletedAsync();
            await dbContext.Database.EnsureCreatedAsync();

            category = new Category 
            { 
                Name = "Test Category",
                Description = "Test Category Description"
            };
            dbContext.Categories.Add(category);
            await dbContext.SaveChangesAsync();

            existingProduct = new Product 
            { 
                Name = "Original Product",
                Description = "Original Description",
                SKU = "SKU004",
                Price = 10.99m,
                StockQuantity = 100,
                Category = category
            };
            dbContext.Products.Add(existingProduct);
            await dbContext.SaveChangesAsync();
        }

        var updatedProduct = new Product
        {
            Id = existingProduct.Id,
            Name = "Updated Product",
            Description = "Updated Description",
            SKU = "SKU004",
            Price = 15.99m,
            StockQuantity = 150,
            Category = category
        };

        // Act
        var json = JsonConvert.SerializeObject(updatedProduct);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _client.PutAsync($"/api/products/{existingProduct.Id}", content);

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<Product>();
        Assert.NotNull(result);
        Assert.Equal(updatedProduct.Name, result.Name);
        Assert.Equal(updatedProduct.Price, result.Price);
    }

    [Fact]
    public async Task Delete_ExistingProduct_ReturnsNoContent()
    {
        // Arrange
        int productId;
        using (var scope = _factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await dbContext.Database.EnsureDeletedAsync();
            await dbContext.Database.EnsureCreatedAsync();

            var category = new Category 
            { 
                Name = "Test Category",
                Description = "Test Category Description"
            };
            dbContext.Categories.Add(category);
            await dbContext.SaveChangesAsync();

            var product = new Product 
            { 
                Name = "Product to Delete",
                Description = "Description to Delete",
                SKU = "SKU005",
                Price = 10.99m,
                StockQuantity = 100,
                Category = category
            };
            dbContext.Products.Add(product);
            await dbContext.SaveChangesAsync();
            productId = product.Id;
        }

        // Act
        var response = await _client.DeleteAsync($"/api/products/{productId}");

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.NoContent, response.StatusCode);

        // Verify product is deleted
        using (var scope = _factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var deleted = await dbContext.Products.FindAsync(productId);
            Assert.Null(deleted);
        }
    }
}
