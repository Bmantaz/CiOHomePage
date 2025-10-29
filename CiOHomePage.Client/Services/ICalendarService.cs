namespace CiOHomePage.Client.Services;

public enum EventCategory
{
    Gig = 0,
    Practice = 1,
    BandMeeting = 2,
    DiscordSession = 3
}

public enum RsvpStatus
{
    Unknown = 0,
    Attending = 1,
    NotAttending = 2,
    Maybe = 3
}

public sealed class RsvpEntry
{
    public string UserId { get; set; } = string.Empty;
    public RsvpStatus Status { get; set; }
}

public interface ICalendarService
{
    Task<IReadOnlyList<CalendarEntry>> GetEntriesAsync(CancellationToken ct = default);
    Task AddOrUpdateAsync(CalendarEntry entry, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}

public sealed class CalendarEntry
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Type { get; set; } = "Practice"; // Legacy string type
    public EventCategory Category { get; set; } = EventCategory.Practice;
    public string? Location { get; set; }
    public string? Notes { get; set; }
    public List<RsvpEntry> Rsvps { get; set; } = new();
}
