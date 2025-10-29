using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace CiOHomePage.Client.Authentication;

public class BandAuthStateProvider(IBandAuthService auth) : AuthenticationStateProvider
{
 private readonly IBandAuthService _auth = auth;

 public override async Task<AuthenticationState> GetAuthenticationStateAsync()
 {
 var user = await _auth.GetCurrentUserAsync();
 return new AuthenticationState(user);
 }

 public void NotifyAuthStateChanged() => NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
}
