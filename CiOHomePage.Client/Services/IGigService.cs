using CiOHomePage.Client.Models;

namespace CiOHomePage.Client.Services;

public interface IGigService
{
 Task<IReadOnlyList<Gig>> GetUpcomingAsync(CancellationToken cancellationToken = default);
 Task<Gig?> CreateAsync(Gig gig, CancellationToken cancellationToken = default);
}
