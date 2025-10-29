using System.Security.Claims;

namespace CiOHomePage.Client.Authentication;

// Retained for compatibility but unused; prefer JwtBandAuthService registered in DI.
public class BandAuthService : IBandAuthService
{
 public Task<bool> LoginAsync(string username, string password, bool rememberMe = false, CancellationToken ct = default) => Task.FromResult(false);
 public Task LogoutAsync(CancellationToken ct = default) => Task.CompletedTask;
 public Task<ClaimsPrincipal> GetCurrentUserAsync(CancellationToken ct = default) => Task.FromResult(new ClaimsPrincipal(new ClaimsIdentity()));
}
