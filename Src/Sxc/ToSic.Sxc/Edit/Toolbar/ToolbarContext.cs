using System.Text.Json.Serialization;
using ToSic.Eav.Apps;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Web.Internal.Url;

namespace ToSic.Sxc.Edit.Toolbar;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
internal class ToolbarContext: IAppIdentity
{
    internal const string CtxZone = "context:zoneId";
    internal const string CtxApp = "context:appId";
    internal const int NotInitialized = -7007;

    internal ToolbarContext(IAppIdentity parent): this(parent.ZoneId, parent.AppId) { }

    public ToolbarContext(int zoneId, int appId)
    {
        ZoneId = zoneId;
        AppId = appId;
    }

    internal ToolbarContext(string custom) => Custom = custom;


    [JsonPropertyName("zoneId")]
    public int ZoneId { get; } = NotInitialized;

    [JsonPropertyName("appId")]
    public int AppId { get; } = NotInitialized;

    [JsonPropertyName("custom")][JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string Custom { get; } = null;
}

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
internal static class ToolbarContextExtensions
{
    public static string ToRuleString(this ToolbarContext tlbCtx) => tlbCtx == null 
        ? null 
        : tlbCtx.Custom.HasValue() 
            ? tlbCtx.Custom 
            : UrlParts.ConnectParameters($"{ToolbarContext.CtxZone}={tlbCtx.ZoneId}", $"{ToolbarContext.CtxApp}={tlbCtx.AppId}");
}