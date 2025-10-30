using CiOHomePage.Client.Models;
using System.Net.Http.Json;

namespace CiOHomePage.Client.Services;

public class GigService(HttpClient http) : IGigService
{
 private readonly HttpClient _http = http;

 public async Task<IReadOnlyList<Gig>> GetUpcomingAsync(CancellationToken cancellationToken = default)
 {
 var data = await _http.GetFromJsonAsync<List<Gig>>("api/gigs", cancellationToken) ?? [];
 return data
 .OrderBy(x => x.Date)
 .ToList();
 }

 public async Task<Gig?> CreateAsync(Gig gig, CancellationToken cancellationToken = default)
 {
 var resp = await _http.PostAsJsonAsync("api/gigs", gig, cancellationToken);
 if (!resp.IsSuccessStatusCode) return null;
 return await resp.Content.ReadFromJsonAsync<Gig>(cancellationToken: cancellationToken);
 }
}
