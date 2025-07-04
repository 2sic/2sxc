﻿using System.Text.Json.Serialization;
using ToSic.Eav.Apps;

namespace ToSic.Sxc.Edit.Toolbar;

[ShowApiWhenReleased(ShowApiMode.Never)]
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

    internal ToolbarContext(string custom)
        => Custom = custom;


    [JsonPropertyName("zoneId")]
    public int ZoneId { get; } = NotInitialized;

    [JsonPropertyName("appId")]
    public int AppId { get; } = NotInitialized;

    [JsonPropertyName("custom")][JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Custom { get; }
}