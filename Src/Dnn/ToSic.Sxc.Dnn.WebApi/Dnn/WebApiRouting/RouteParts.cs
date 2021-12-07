using System.Linq;

namespace ToSic.Sxc.Dnn.WebApiRouting
{
    /// <summary>
    /// These are all the root paths used in the API
    /// Basically anything after the system api-root, like /api/2sxc/* (in DNN) or /#/api/2sxc/* (in Oqtane)
    /// </summary>
    public class Root {
        public const string App = "app";
        public const string Sys = "sys";
        public const string Cms = "cms";
        public const string Admin = "admin";
    }

    public class Parts
    {
        public const string Auto = "auto";
        public const string Content = "content";
        public const string Data = "data"; // new, v13
        public const string Query = "query";
    }

    public class Roots
    {
        public const string AppAuto = Root.App + "/" + Parts.Auto;
        public const string AppNamed = Root.App + "/" + Token.AppPath;
        public const string ContentAuto = AppAuto + "/" + Parts.Content;
        public const string ContentNamed = AppNamed + "/" + Parts.Content;
        public const string DataAuto = AppAuto + "/" + Parts.Data; // new, v13
        public const string DataNamed = AppNamed + "/" + Parts.Data; // new, v13
        public static RootId[] QueryRoots =
        {
            new RootId("qry-auto", AppAuto + "/" + Parts.Query), 
            new RootId("qry-name", AppNamed + "/" + Parts.Query)
        };
        public static RootId[] AppAutoAndNamed =
        {
            new RootId("app-auto", AppAuto), 
            new RootId("app-name",  AppNamed)
        };
        public static RootId[] Content =
        {
            new RootId("cont-auto", ContentAuto), 
            new RootId("cont-name", ContentNamed),
            new RootId("data-auto", DataAuto), // new, v13
            new RootId("data-name", DataNamed) // new, v13
        };

        public static RootId[] AppAutoNamedInclEditions = AppAutoAndNamed
            .Concat(AppAutoAndNamed.Select(rid => new RootId(rid.Name + "-edition", rid.Path + "/" + Token.Edition)))
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

    /// <summary>
    /// These are the keys / names of fields we use in rout parts
    /// </summary>
    public class Names
    {
        public const string Edition = "edition";
        public const string Controller = "controller";
        public const string Action = "action";
        public const string Name = "name";
        public const string Stream = "stream";
        public const string AppPath = "apppath";
        public const string ContentType = "contenttype";
        public const string Id = "id";
        public const string Guid = "guid";
        public const string Field = "field";
    }

    /// <summary>
    /// These are tokens in the format {xyz} used in routes to identify a part
    /// </summary>
    public class Token
    {
        public const string Name = "{" + Names.Name + "}";
        public const string Stream = "{" + Names.Stream + "}";
        public const string Edition = "{" + Names.Edition + "}";
        public const string AppPath = "{" + Names.AppPath + "}";
        public const string Controller = "{" + Names.Controller + "}";
        public const string Action = "{" + Names.Action + "}";

        public const string ContentType = "{" + Names.ContentType + "}";
        public const string Id = "{" + Names.Id + "}";
        public const string Guid = "{" + Names.Guid + "}";
        public const string Field = "{" + Names.Field + "}";
    }

    /// <summary>
    /// Common combinations of tokes
    /// </summary>
    public class TokenSet
    {
        public const string ConAct = Token.Controller + "/" + Token.Action;
        public const string TypeId = Token.ContentType + "/" + Token.Id;
        public const string TypeGuid = Token.ContentType + "/" + Token.Guid;
        public const string TypeGuidField = TokenSet.TypeGuid + "/" + Token.Field;
        public const string TypeGuidFieldAction = TypeGuidField + "/" + Token.Action;
    }

    internal class RouteParts
    {
        public const string RouteApiControllerAction = "api/" + TokenSet.ConAct;
    }

    public class ControllerNames
    {
        public const string Adam = "Adam";
        public const string AppContent = "AppContent";
        public const string AppQuery = "AppQuery";
    }
}

