using System.ComponentModel.DataAnnotations;

namespace CiOHomePage.Models;

public class CalendarEvent
{
 [Key] public Guid Id { get; set; } = Guid.NewGuid();

 [Required, MaxLength(200)]
 public string Title { get; set; } = string.Empty;

 [Required, MaxLength(50)]
 public string Type { get; set; } = "Practice"; // Practice, Gig, Studio

 [Required] public DateTime StartUtc { get; set; }
 [Required] public DateTime EndUtc { get; set; }

 [MaxLength(200)] public string? Location { get; set; }
 public string? Notes { get; set; }

 [Required] public string CreatedByUserId { get; set; } = string.Empty;
 public bool IsBandWide { get; set; } = true;
}
