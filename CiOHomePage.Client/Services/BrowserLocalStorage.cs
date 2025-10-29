using Microsoft.JSInterop;

namespace CiOHomePage.Client.Services;

public class BrowserLocalStorage(IJSRuntime js) : ILocalStorage
{
 private readonly IJSRuntime _js = js;
 public async Task SetAsync(string key, string value, bool persistent = true)
 {
 var storage = persistent ? "localStorage" : "sessionStorage";
 await _js.InvokeVoidAsync("eval", $"{storage}.setItem('{key}', {System.Text.Json.JsonSerializer.Serialize(value)})");
 }

 public async Task<string?> GetAsync(string key)
 {
 var result = await _js.InvokeAsync<string?>("eval", $"localStorage.getItem('{key}') ?? sessionStorage.getItem('{key}')");
 return result;
 }

 public async Task RemoveAsync(string key)
 {
 await _js.InvokeVoidAsync("eval", $"localStorage.removeItem('{key}'); sessionStorage.removeItem('{key}')");
 }
}
