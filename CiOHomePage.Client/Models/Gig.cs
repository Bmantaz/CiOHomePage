namespace CiOHomePage.Client.Models;

public class Gig
{
 public int Id { get; set; }
 public DateTime Date { get; set; }
 public string Venue { get; set; } = string.Empty; // Spillested
 public string City { get; set; } = string.Empty;
 public string? TicketUrl { get; set; }
 public bool IsSoldOut { get; set; } = false;
}
