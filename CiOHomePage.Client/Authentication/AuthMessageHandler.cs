using System.Net.Http.Headers;
using Microsoft.JSInterop;

namespace CiOHomePage.Client.Authentication;

public class AuthMessageHandler(IJSRuntime js) : DelegatingHandler
{
 private readonly IJSRuntime _js = js;
 private const string TokenKey = "cio_token";

 protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
 {
 try
 {
 var token = await _js.InvokeAsync<string?>("localStorage.getItem", TokenKey);
 if (!string.IsNullOrWhiteSpace(token))
 {
 request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
 }
 }
 catch { /* ignore */ }
 return await base.SendAsync(request, cancellationToken);
 }
}
