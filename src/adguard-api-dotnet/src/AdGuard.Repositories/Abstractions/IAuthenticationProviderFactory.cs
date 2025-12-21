namespace AdGuard.Repositories.Abstractions;

/// <summary>
/// Interface for authentication provider factory.
/// </summary>
public interface IAuthenticationProviderFactory
{
    /// <summary>
    /// Creates an authentication provider for the specified scheme.
    /// </summary>
    /// <param name="scheme">The authentication scheme.</param>
    /// <returns>The authentication provider.</returns>
    IAuthenticationProvider Create(string scheme);

    /// <summary>
    /// Gets the available authentication schemes.
    /// </summary>
    IReadOnlyCollection<string> AvailableSchemes { get; }

    /// <summary>
    /// Registers an authentication provider for a scheme.
    /// </summary>
    /// <param name="scheme">The authentication scheme.</param>
    /// <param name="providerFactory">Factory function to create the provider.</param>
    void Register(string scheme, Func<IAuthenticationProvider> providerFactory);
}