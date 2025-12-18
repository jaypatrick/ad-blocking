namespace AdGuard.ConsoleUI.Services;

/// <summary>
/// Menu service for viewing web services.
/// Uses Repository pattern for data access and Strategy pattern for display.
/// </summary>
public class WebServiceMenuService : IMenuService
{
    private readonly IWebServiceRepository _webServiceRepository;
    private readonly IDisplayStrategy<WebService> _displayStrategy;

    public WebServiceMenuService(
        IWebServiceRepository webServiceRepository,
        IDisplayStrategy<WebService> displayStrategy)
    {
        _webServiceRepository = webServiceRepository ?? throw new ArgumentNullException(nameof(webServiceRepository));
        _displayStrategy = displayStrategy ?? throw new ArgumentNullException(nameof(displayStrategy));
    }

    /// <inheritdoc />
    public string Title => "Web Services";

    /// <inheritdoc />
    public async Task ShowAsync()
    {
        try
        {
            var webServices = await ConsoleHelpers.WithStatusAsync(
                "Fetching web services...",
                () => _webServiceRepository.GetAllAsync());

            _displayStrategy.Display(webServices);
        }
        catch (ApiException ex)
        {
            ConsoleHelpers.ShowApiError(ex);
        }
    }
}
