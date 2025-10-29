using System.Security.Claims;

namespace CiOHomePage.Client.Authentication;

public interface IBandAuthService
{
 Task<bool> LoginAsync(string username, string password, bool rememberMe = false, CancellationToken ct = default);
 Task LogoutAsync(CancellationToken ct = default);
 Task<ClaimsPrincipal> GetCurrentUserAsync(CancellationToken ct = default);
}
