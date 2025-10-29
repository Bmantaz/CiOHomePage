namespace CiOHomePage.Client.Services;

public interface ILocalStorage
{
 Task SetAsync(string key, string value, bool persistent = true);
 Task<string?> GetAsync(string key);
 Task RemoveAsync(string key);
}
