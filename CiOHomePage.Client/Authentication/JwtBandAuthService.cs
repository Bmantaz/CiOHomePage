using System;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Microsoft.JSInterop;

namespace CiOHomePage.Client.Authentication;

public class JwtBandAuthService(HttpClient http, IJSRuntime js) : IBandAuthService
{
 private readonly HttpClient _http = http;
 private readonly IJSRuntime _js = js;
 private const string TokenKey = "cio_token";

 public async Task<bool> LoginAsync(string username, string password, bool rememberMe = false, CancellationToken ct = default)
 {
 var res = await _http.PostAsJsonAsync("api/auth/login", new { username, password }, ct);
 if (!res.IsSuccessStatusCode) return false;
 var json = await res.Content.ReadAsStringAsync(ct);
 using var doc = JsonDocument.Parse(json);
 var token = doc.RootElement.GetProperty("token").GetString();
 if (string.IsNullOrWhiteSpace(token)) return false;
 await _js.InvokeVoidAsync("localStorage.setItem", TokenKey, token);
 _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
 return true;
 }

 public async Task LogoutAsync(CancellationToken ct = default)
 {
 await _js.InvokeVoidAsync("localStorage.removeItem", TokenKey);
 _http.DefaultRequestHeaders.Authorization = null;
 }

 public async Task<ClaimsPrincipal> GetCurrentUserAsync(CancellationToken ct = default)
 {
 var token = await _js.InvokeAsync<string?>("localStorage.getItem", TokenKey);
 if (string.IsNullOrWhiteSpace(token)) return new ClaimsPrincipal(new ClaimsIdentity());
 _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
 var claims = ParseJwtClaims(token);
 var identity = new ClaimsIdentity(claims, "jwt");
 return new ClaimsPrincipal(identity);
 }

 private static IEnumerable<Claim> ParseJwtClaims(string jwt)
 {
 var parts = jwt.Split('.');
 if (parts.Length !=3) yield break;
 var payload = parts[1];
 var json = Encoding.UTF8.GetString(Base64UrlDecode(payload));
 using var doc = JsonDocument.Parse(json);
 foreach (var property in doc.RootElement.EnumerateObject())
 {
 var name = property.Name;
 var value = property.Value.ToString();
 switch (name)
 {
 case "name":
 yield return new Claim(ClaimTypes.Name, value);
 break;
 case "nameid":
 yield return new Claim(ClaimTypes.NameIdentifier, value);
 break;
 case "role":
 yield return new Claim(ClaimTypes.Role, value);
 break;
 default:
 yield return new Claim(name, value);
 break;
 }
 }
 }

 private static byte[] Base64UrlDecode(string input)
 {
 // Replace URL-safe chars
 string s = input.Replace('-', '+').Replace('_', '/');
 // Pad with '=' to length multiple of4
 int padding =4 - (s.Length %4);
 if (padding is >0 and <4)
 {
 s = s.PadRight(s.Length + padding, '=');
 }
 return Convert.FromBase64String(s);
 }
}
