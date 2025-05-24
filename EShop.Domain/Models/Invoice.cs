using System.ComponentModel.DataAnnotations;

namespace EShop.Domain.Models;

public class Invoice : BaseModel
{
    [Required]
    public string InvoiceNumber { get; set; }
    
    public int OrderId { get; set; }
    public Order Order { get; set; }
    
    [Required]
    public decimal TotalAmount { get; set; }
    
    public decimal? TaxAmount { get; set; }
    
    public string? PdfUrl { get; set; }
    
    public DateTime IssuedAt { get; set; }
    
    public DateTime? PaidAt { get; set; }
    
    public InvoiceStatus Status { get; set; }
}

public enum InvoiceStatus
{
    Draft,
    Issued,
    Paid,
    Cancelled,
    Refunded
} 