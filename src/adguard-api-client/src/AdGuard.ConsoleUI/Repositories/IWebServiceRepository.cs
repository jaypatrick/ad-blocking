namespace AdGuard.ConsoleUI.Repositories;

/// <summary>
/// Repository interface for web service operations.
/// Abstracts data access from the UI layer.
/// </summary>
public interface IWebServiceRepository
{
    /// <summary>
    /// Gets all available web services.
    /// </summary>
    Task<List<WebService>> GetAllAsync();
}
