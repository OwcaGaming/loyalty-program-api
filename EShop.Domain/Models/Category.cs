using System.ComponentModel.DataAnnotations;

namespace EShop.Domain.Models;

public class Category : BaseModel
{
    [Required]
    public required string Name { get; set; }
    
    public required string Description { get; set; }
    
    public string? ImageUrl { get; set; }
    
    public int? ParentCategoryId { get; set; }
    public Category? ParentCategory { get; set; }
    
    public ICollection<Category> SubCategories { get; set; } = new List<Category>();
    public ICollection<Product> Products { get; set; } = new List<Product>();
} 