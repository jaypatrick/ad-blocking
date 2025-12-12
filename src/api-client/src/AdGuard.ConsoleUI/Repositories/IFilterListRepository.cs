namespace AdGuard.ConsoleUI.Repositories;

/// <summary>
/// Repository interface for filter list operations.
/// Abstracts data access from the UI layer.
/// </summary>
public interface IFilterListRepository
{
    /// <summary>
    /// Gets all available filter lists.
    /// </summary>
    Task<List<FilterList>> GetAllAsync();
}
