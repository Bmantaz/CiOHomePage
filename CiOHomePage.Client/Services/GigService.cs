using CiOHomePage.Client.Models;
using System.Net.Http.Json;

namespace CiOHomePage.Client.Services;

public class GigService(HttpClient http) : IGigService
{
 private readonly HttpClient _http = http;

 public async Task<IReadOnlyList<Gig>> GetUpcomingAsync(CancellationToken cancellationToken = default)
 {
 var data = await _http.GetFromJsonAsync<List<Gig>>("sample-data/gigs.json", cancellationToken) ?? [];
 var now = DateTime.UtcNow.Date;
 return data
 .Where(x => x.Date.Date >= now)
 .OrderBy(x => x.Date)
 .ToList();
 }
}
