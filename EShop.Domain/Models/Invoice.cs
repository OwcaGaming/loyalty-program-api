using System.ComponentModel.DataAnnotations;

namespace EShop.Domain.Models;

public class Invoice : BaseModel
{
    [Required]
    public required string InvoiceNumber { get; set; }
    
    public int OrderId { get; set; }
    public required Order Order { get; set; }
    
    public DateTime IssueDate { get; set; } = DateTime.UtcNow;
    
    public DateTime? DueDate { get; set; }
    
    public decimal TotalAmount { get; set; }
    
    public decimal? TaxAmount { get; set; }
    
    public InvoiceStatus Status { get; set; } = InvoiceStatus.Pending;
}

public enum InvoiceStatus
{
    Pending,
    Paid,
    Overdue,
    Cancelled
} 