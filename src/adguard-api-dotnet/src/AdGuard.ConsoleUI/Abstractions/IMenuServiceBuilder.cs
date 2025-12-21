namespace AdGuard.ConsoleUI.Abstractions;

/// <summary>
/// Interface for menu service builders (fluent configuration).
/// </summary>
public interface IMenuServiceBuilder
{
    /// <summary>
    /// Configures the menu identifier.
    /// </summary>
    /// <param name="id">The menu identifier.</param>
    /// <returns>The builder for chaining.</returns>
    IMenuServiceBuilder WithId(string id);

    /// <summary>
    /// Configures the display order.
    /// </summary>
    /// <param name="order">The display order.</param>
    /// <returns>The builder for chaining.</returns>
    IMenuServiceBuilder WithOrder(int order);

    /// <summary>
    /// Configures the menu as requiring authentication.
    /// </summary>
    /// <param name="required">Whether authentication is required.</param>
    /// <returns>The builder for chaining.</returns>
    IMenuServiceBuilder RequiresAuth(bool required = true);

    /// <summary>
    /// Builds and registers the menu service.
    /// </summary>
    void Build();
}