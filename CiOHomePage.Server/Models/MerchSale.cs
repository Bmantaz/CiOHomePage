using System.ComponentModel.DataAnnotations;

namespace CiOHomePage.Server.Models;

public class MerchSale
{
 [Key]
 public int Id { get; set; }
 [Required]
 public string ItemName { get; set; } = string.Empty;
 [Required]
 public string Category { get; set; } = string.Empty;
 [Range(1, int.MaxValue)]
 public int Quantity { get; set; }
 [Range(0, double.MaxValue)]
 public decimal SalePrice { get; set; }
 [Required]
 public DateTime DateSold { get; set; } = DateTime.UtcNow;
}
