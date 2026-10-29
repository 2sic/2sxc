using ToSic.Eav.WebApi.Sys.Routing;

namespace ToSic.Sxc.Dnn.WebApi.Routing.Sys;

/// <summary>
/// These are tokens in the format {xyz} used in routes to identify a part.
/// </summary>
[InternalApi_DoNotUse_MayChangeWithoutNotice("only made public for docs")]
[ShowApiWhenReleased(ShowApiMode.Never)]
public class DnnRouteTokens
{
    // Single Placeholders
    public const string Name = "{" + VarNames.Name + "}";
    public const string Stream = "{" + VarNames.Stream + "}";
    public const string Edition = "{" + VarNames.Edition + "}";
    public const string AppPath = "{" + VarNames.AppPath + "}";
    public const string Controller = "{" + VarNames.Controller + "}";
    public const string Action = "{" + VarNames.Action + "}";

    public const string ContentType = "{" + VarNames.ContentType + "}";
    public const string Id = "{" + VarNames.Id + "}";
    public const string Guid = "{" + VarNames.Guid + "}";
    public const string Field = "{" + VarNames.Field + "}";

    // Sets / Pairs of placeholders
    public const string ControllerAndAction = $"{Controller}/{Action}";
    public const string TypeAndId = $"{ContentType}/{Id}";
    public const string TypeAndGuid = $"{ContentType}/{Guid}";
    public const string TypeGuidField = $"{TypeAndGuid}/{Field}";
    public const string TypeGuidFieldAction = $"{TypeGuidField}/{Action}";

    public const string RouteApiControllerAction = $"api/{ControllerAndAction}";

}