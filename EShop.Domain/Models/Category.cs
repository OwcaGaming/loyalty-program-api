using System.ComponentModel.DataAnnotations;

namespace EShop.Domain.Models;

public class Category : BaseModel
{
    [Required]
    public string Name { get; set; }
    
    public string Description { get; set; }
    
    public string ImageUrl { get; set; }
    
    public int? ParentCategoryId { get; set; }
    public Category ParentCategory { get; set; }
    
    public ICollection<Category> SubCategories { get; set; }
    public ICollection<Product> Products { get; set; }
} 