using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Logging;

namespace VerdeEnergy.WebUI.Components.Layout;

public class LoggingErrorBoundary : ErrorBoundary
{
    [Inject]
    private ILogger<LoggingErrorBoundary> Logger { get; set; } = null!;

    [Inject]
    private NavigationManager Navigation { get; set; } = null!;

    [Parameter]
    public EventCallback<Exception> OnError { get; set; }

    protected override Task OnErrorAsync(Exception exception)
    {
        Logger.LogError(exception, "Unhandled component exception on path: {Path}", Navigation.Uri);
        if (OnError.HasDelegate)
        {
            return OnError.InvokeAsync(exception);
        }
        return Task.CompletedTask;
    }
}
