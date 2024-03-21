using System.Linq;
using ToSic.Eav.WebApi.Routing;

namespace ToSic.Sxc.Dnn.WebApi;

internal class Roots
{
    public static RootId[] QueryRoots =
    [
        new("qry-auto", AppRoots.AppAuto + "/" + AppParts.Query), 
        new("qry-name", AppRoots.AppNamed + "/" + AppParts.Query)
    ];
    public static RootId[] AppAutoAndNamed =
    [
        new("app-auto", AppRoots.AppAuto), 
        new("app-name",  AppRoots.AppNamed)
    ];
    public static RootId[] Content =
    [
        new("cont-auto", AppRoots.AppAutoContent), 
        new("cont-name", AppRoots.AppNamedContent),
        new("data-auto", AppRoots.AppAutoData), // new, v13
        new("data-name", AppRoots.AppNamedData) // new, v13
    ];

    public static RootId[] AppAutoNamedInclEditions = AppAutoAndNamed
        .Concat(AppAutoAndNamed.Select(rid => new RootId(rid.Name + "-edition", rid.Path + "/" + ValueTokens.Edition)))
        .ToArray();
}

internal struct RootId(string name, string path)
{
    public string Name = name;
    public string Path = path;
}


internal class RouteParts
{
    public const string RouteApiControllerAction = "api/" + ValueTokens.SetControllerAction;
}

internal class ControllerNames
{
    public const string Adam = "Adam";
    public const string AppContent = "AppData";
    public const string AppQuery = "AppQuery";
}