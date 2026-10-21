using ToSic.Eav.WebApi.Sys.Routing;
using RouteTokens = ToSic.Sxc.Dnn.WebApi.Routing.Sys.DnnRouteTokens;

namespace ToSic.Sxc.Dnn.WebApi.Routing.Sys;

/// <summary>
/// Root paths to app specific APIs.
/// </summary>
[InternalApi_DoNotUse_MayChangeWithoutNotice("only made public for docs")]
[ShowApiWhenReleased(ShowApiMode.Never)]
public class DnnRoutingAppRoots
{
    public const string AppAuto = $"{Areas.App}/{AppParts.Auto}";
    public const string AppAutoContent = $"{Areas.App}/{AppParts.Auto}/{AppParts.Content}";
    public const string AppAutoData = $"{Areas.App}/{AppParts.Auto}/{AppParts.Data}";

    public const string AppNamed = $"{Areas.App}/{RouteTokens.AppPath}";
    public const string AppNamedContent = $"{Areas.App}/{RouteTokens.AppPath}/{AppParts.Content}";
    public const string AppNamedData = $"{Areas.App}/{RouteTokens.AppPath}/{AppParts.Data}";

    internal static RouteRoot[] AppQueryRoots =
    [
        new("qry-auto", $"{AppAuto}/{AppParts.Query}"), 
        new("qry-name", $"{AppNamed}/{AppParts.Query}")
    ];

    private static readonly RouteRoot[] AppAutoAndNamedRaw =
    [
        new("app-auto", AppAuto), 
        new("app-name",  AppNamed)
    ];

    internal static RouteRoot[] AppAutoNamed =
    [
        .. AppAutoAndNamedRaw,
        .. AppAutoAndNamedRaw.Select(rid => new RouteRoot($"{rid.Name}-edition", $"{rid.Path}/{RouteTokens.Edition}"))
    ];

    internal static RouteRoot[] AppContents =
    [
        new("cont-auto", AppAutoContent), 
        new("cont-name", AppNamedContent),
        new("data-auto", AppAutoData), 
        new("data-name", AppNamedData) 
    ];

}

internal record struct RouteRoot(string Name, string Path);

/// <summary>
/// Controller names, needed to register routes.
/// </summary>
[InternalApi_DoNotUse_MayChangeWithoutNotice("only made public for docs")]
[ShowApiWhenReleased(ShowApiMode.Never)]
public class DnnRoutingControllerNames
{
    public const string Adam = "Adam";
    public const string AppData = "AppData";
    public const string AppQuery = "AppQuery";
}


