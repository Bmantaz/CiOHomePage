using System.ComponentModel.DataAnnotations;

namespace CiOHomePage.Server.Models;

public class Gig
{
 [Key]
 public int Id { get; set; }

 [Required]
 public DateTime Date { get; set; }

 [Required]
 [MaxLength(200)]
 public string Venue { get; set; } = string.Empty;

 [Required]
 [MaxLength(200)]
 public string City { get; set; } = string.Empty;

 [Url]
 public string? TicketUrl { get; set; }

 public bool IsSoldOut { get; set; } = false;
}
