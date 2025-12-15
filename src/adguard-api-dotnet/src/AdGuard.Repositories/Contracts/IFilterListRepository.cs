using AdGuard.Repositories.Abstractions;

namespace AdGuard.Repositories.Contracts;

/// <summary>
/// Repository interface for filter list operations (read-only).
/// </summary>
public interface IFilterListRepository : IReadOnlyRepository<FilterList>
{
}
