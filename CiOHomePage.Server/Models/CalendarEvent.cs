using System.ComponentModel.DataAnnotations;

namespace CiOHomePage.Server.Models;

public enum EventCategory
{
    Gig = 0,
    Practice = 1,
    BandMeeting = 2,
    DiscordSession = 3
}

public class CalendarEvent
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string Type { get; set; } = "Practice"; // Legacy string type

    [Required]
    public DateTime StartUtc { get; set; }

    [Required]
    public DateTime EndUtc { get; set; }

    [MaxLength(200)]
    public string? Location { get; set; }

    public string? Notes { get; set; }

    [Required]
    public string CreatedByUserId { get; set; } = string.Empty;

    // If true, visible to all band members. Otherwise, only creator can see.
    public bool IsBandWide { get; set; } = true;

    // New category enum
    public EventCategory Category { get; set; } = EventCategory.Practice;

    // RSVPs
    public ICollection<EventRsvp> Rsvps { get; set; } = new List<EventRsvp>();
}

public enum RsvpStatus
{
    Unknown = 0,
    Attending = 1,
    NotAttending = 2,
    Maybe = 3
}

public class EventRsvp
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public Guid EventId { get; set; }
    public CalendarEvent? Event { get; set; }

    [Required]
    public string UserId { get; set; } = string.Empty;

    public RsvpStatus Status { get; set; } = RsvpStatus.Unknown;

    public DateTime RespondedUtc { get; set; } = DateTime.UtcNow;
}
