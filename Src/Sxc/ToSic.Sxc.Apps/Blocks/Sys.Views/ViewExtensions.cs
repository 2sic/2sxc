using ToSic.Eav.Apps.Views.Sys;

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

    public static bool IsRazor(this IView view) => view.Type == SxcViewConstants.TypeRazorValue;

    /// <summary>
    /// Get the engine name for DI injection
    /// </summary>
    public static string GetEngineName(this IView view)
        => view.Type == SxcViewConstants.TypeRazorValue
            ? "razor"
            : "token";

    public static IEntity? GetQueryEntity(this IView view) => view.Entity.Children(SxcViewConstants.FieldPipeline).FirstOrDefault();

}