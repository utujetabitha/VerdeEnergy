using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;

namespace WebUI.Components;

public abstract class AuthPageBase : ComponentBase
{
	[Inject] protected IAntiforgery Antiforgery { get; set; } = default!;
	[Inject] protected IHttpContextAccessor HttpContextAccessor { get; set; } = default!;
	[Inject] protected NavigationManager Navigation { get; set; } = default!;
	[Inject] protected ILogger<AuthPageBase> Logger { get; set; } = default!;

	protected string? RequestVerificationToken { get; private set; }

	protected override void OnInitialized()
	{
		try
		{
			if (HttpContextAccessor.HttpContext is { } context)
			{
				RequestVerificationToken = Antiforgery.GetAndStoreTokens(context).RequestToken;
			}
		}
		catch (Exception ex)
		{
			Logger.LogWarning(ex, "Could not generate antiforgery token on auth page at {Path}.", Navigation.Uri);
		}
	}

	protected string? QueryValue(string key)
	{
		try
		{
			if (string.IsNullOrWhiteSpace(Navigation.Uri))
			{
				return null;
			}
			if (!Uri.TryCreate(Navigation.Uri, UriKind.Absolute, out var uri))
			{
				return null;
			}
			var query = QueryHelpers.ParseQuery(uri.Query);
			return query.TryGetValue(key, out var value) && value.Count > 0 ? value.ToString() : null;
		}
		catch (Exception ex)
		{
			Logger.LogWarning(ex, "Failed to parse query parameter '{Key}' from URI.", key);
			return null;
		}
	}
}