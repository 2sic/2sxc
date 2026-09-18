namespace ToSic.Sxc.Blocks.Sys.Views;

[ShowApiWhenReleased(ShowApiMode.Never)]
public static class ViewExtensions
{
    internal static string GetTypeStaticName(this IView view, string groupPart)
        => groupPart.ToLowerInvariant() switch
        {
            ViewParts.ContentLower => view.ContentType,
            ViewParts.PresentationLower => view.PresentationType,
            ViewParts.ListContentLower => view.HeaderType,
            ViewParts.ListPresentationLower => view.HeaderPresentationType,
            _ => throw new NotSupportedException("Unknown group part: " + groupPart)
        };

    /// <summary>
    /// Get the engine name for DI injection
    /// </summary>
    public static string GetEngineName(this IView view)
        => view.Type == ViewConstants.TypeRazorValue
            ? "razor"
            : "token";
}