using System.Net.Http.Json;
using System.Text.Json;

namespace CiOHomePage.Client.Services;

public class LocalCalendarService(ILocalStorage storage) : ICalendarService
{
 private const string CalendarKey = "cio_calendar";
 private readonly ILocalStorage _storage = storage;

 public async Task<IReadOnlyList<CalendarEntry>> GetEntriesAsync(CancellationToken ct = default)
 {
 var json = await _storage.GetAsync(CalendarKey);
 if (string.IsNullOrWhiteSpace(json)) return [];
 try
 {
 var entries = JsonSerializer.Deserialize<List<CalendarEntry>>(json) ?? [];
 return entries.OrderBy(e => e.Start).ToList();
 }
 catch
 {
 return [];
 }
 }

 public async Task AddOrUpdateAsync(CalendarEntry entry, CancellationToken ct = default)
 {
 var list = (await GetEntriesAsync()).ToList();
 var idx = list.FindIndex(e => e.Id == entry.Id);
 if (idx >=0) list[idx] = entry; else list.Add(entry);
 var json = JsonSerializer.Serialize(list);
 await _storage.SetAsync(CalendarKey, json, persistent: true);
 }

 public async Task DeleteAsync(Guid id, CancellationToken ct = default)
 {
 var list = (await GetEntriesAsync()).ToList();
 list.RemoveAll(e => e.Id == id);
 var json = JsonSerializer.Serialize(list);
 await _storage.SetAsync(CalendarKey, json, persistent: true);
 }
}
