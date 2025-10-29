namespace CiOHomePage.Client.Services;

public record MerchSaleDto(int Id, string ItemName, string Category, int Quantity, decimal SalePrice, DateTime DateSold);

public interface IMerchService
{
 Task<IReadOnlyList<MerchSaleDto>> GetAllAsync(CancellationToken ct = default);
 Task<(decimal revenue, IReadOnlyList<(string item,int qty)> top)> GetSummaryAsync(CancellationToken ct = default);
 Task CreateAsync(MerchSaleDto sale, CancellationToken ct = default);
 Task DeleteAsync(int id, CancellationToken ct = default);
}
