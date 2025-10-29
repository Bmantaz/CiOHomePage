using CiOHomePage.Client.Services;
using System.Net.Http.Json;
using System.Linq;

namespace CiOHomePage.Client.Services;

public class HttpCalendarService(HttpClient http) : ICalendarService
{
 private readonly HttpClient _http = http;

 private sealed class CalendarEventDto
 {
 public Guid Id { get; set; }
 public string Title { get; set; } = string.Empty;
 public string Type { get; set; } = "Practice";
 public EventCategory Category { get; set; } = EventCategory.Practice;
 public DateTime StartUtc { get; set; }
 public DateTime EndUtc { get; set; }
 public string? Location { get; set; }
 public string? Notes { get; set; }
 public bool IsBandWide { get; set; } = true;
 public List<RsvpEntry>? Rsvps { get; set; }
 }

 public async Task<IReadOnlyList<CalendarEntry>> GetEntriesAsync(CancellationToken ct = default)
 {
 var data = await _http.GetFromJsonAsync<List<CalendarEventDto>>("api/calendar", ct) ?? [];
 return data
 .Select(d => new CalendarEntry
 {
 Id = d.Id,
 Title = d.Title,
 Type = d.Type,
 Category = d.Category,
 Start = DateTime.SpecifyKind(d.StartUtc, DateTimeKind.Utc).ToLocalTime(),
 End = DateTime.SpecifyKind(d.EndUtc, DateTimeKind.Utc).ToLocalTime(),
 Location = d.Location,
 Notes = d.Notes,
 Rsvps = d.Rsvps ?? new()
 })
 .OrderBy(e => e.Start)
 .ToList();
 }

 public async Task AddOrUpdateAsync(CalendarEntry entry, CancellationToken ct = default)
 {
 var dto = new CalendarEventDto
 {
 Id = entry.Id == Guid.Empty ? Guid.NewGuid() : entry.Id,
 Title = entry.Title,
 Type = entry.Type,
 Category = entry.Category,
 StartUtc = entry.Start.ToUniversalTime(),
 EndUtc = entry.End.ToUniversalTime(),
 Location = entry.Location,
 Notes = entry.Notes,
 IsBandWide = true
 };
 if (entry.Id != Guid.Empty)
 {
 var resp = await _http.PutAsJsonAsync($"api/calendar/{dto.Id}", dto, ct);
 if (resp.IsSuccessStatusCode) return;
 }
 await _http.PostAsJsonAsync("api/calendar", dto, ct);
 }

 public async Task DeleteAsync(Guid id, CancellationToken ct = default)
 {
 await _http.DeleteAsync($"api/calendar/{id}", ct);
 }
}
