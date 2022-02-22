using System.Linq;
using ToSic.Eav.WebApi.Routing;

namespace ToSic.Sxc.Dnn.WebApiRouting
{
    public class Roots
    {
        public static RootId[] QueryRoots =
        {
            new RootId("qry-auto", AppRoots.AppAuto + "/" + AppParts.Query), 
            new RootId("qry-name", AppRoots.AppNamed + "/" + AppParts.Query)
        };
        public static RootId[] AppAutoAndNamed =
        {
            new RootId("app-auto", AppRoots.AppAuto), 
            new RootId("app-name",  AppRoots.AppNamed)
        };
        public static RootId[] Content =
        {
            new RootId("cont-auto", AppRoots.AppAutoContent), 
            new RootId("cont-name", AppRoots.AppNamedContent),
            new RootId("data-auto", AppRoots.AppAutoData), // new, v13
            new RootId("data-name", AppRoots.AppNamedData) // new, v13
        };

        public static RootId[] AppAutoNamedInclEditions = AppAutoAndNamed
            .Concat(AppAutoAndNamed.Select(rid => new RootId(rid.Name + "-edition", rid.Path + "/" + ValueTokens.Edition)))
            .ToArray();
    }

    public struct RootId
    {
        public string Name;
        public string Path;

        public RootId(string name, string path)
        {
            Name = name;
            Path = path;
        }
    }


    internal class RouteParts
    {
        public const string RouteApiControllerAction = "api/" + ValueTokens.SetControllerAction;
    }

    public class ControllerNames
    {
        public const string Adam = "Adam";
        public const string AppContent = "AppContent";
        public const string AppQuery = "AppQuery";
    }
}

