using System.Net.Http.Json;

namespace CiOHomePage.Client.Services;

public class MerchService(HttpClient http) : IMerchService
{
 private readonly HttpClient _http = http;
 public async Task<IReadOnlyList<MerchSaleDto>> GetAllAsync(CancellationToken ct = default)
 => await _http.GetFromJsonAsync<List<MerchSaleDto>>("api/merch", ct) ?? [];

 public async Task<(decimal revenue, IReadOnlyList<(string item, int qty)> top)> GetSummaryAsync(CancellationToken ct = default)
 {
 var obj = await _http.GetFromJsonAsync<SummaryResponse>("api/merch/summary", ct) ?? new SummaryResponse();
 var top = obj.top?.Select(t => (t.Item, t.Qty)).ToList() ?? [];
 return (obj.revenue, top);
 }

 public async Task CreateAsync(MerchSaleDto sale, CancellationToken ct = default)
 => await _http.PostAsJsonAsync("api/merch", sale, ct);

 public async Task DeleteAsync(int id, CancellationToken ct = default)
 => await _http.DeleteAsync($"api/merch/{id}", ct);

 private sealed class SummaryResponse
 {
 public decimal revenue { get; set; }
 public List<TopItem>? top { get; set; }
 }
 private sealed class TopItem { public string Item { get; set; } = string.Empty; public int Qty { get; set; } }
}
