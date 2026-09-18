using Microsoft.AspNetCore.Components;

namespace UiEmbed.Components.Common;

public static class NavigationUtils
{
    public static string? SafeReturnUrl(NavigationManager nav)
    {
        var query = System.Web.HttpUtility.ParseQueryString(new Uri(nav.Uri).Query);
        var url = query["ReturnUrl"];
        return !string.IsNullOrEmpty(url) && url.StartsWith('/') && !url.StartsWith("//")
            ? url
            : null;
    }
}