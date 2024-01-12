using System.Text.Json.Serialization;
using ToSic.Eav.Helpers;
using ToSic.Sxc.Blocks.Internal;
using ToSic.Sxc.Blocks.Internal.Render;

namespace ToSic.Sxc.Web.Internal.JsContextEdit;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class ContentBlockDto : EntityDto
{
    public bool IsCreated { get; }
    public bool IsList { get; }
    public int TemplateId { get; }
    public int? QueryId { get; }
    public string ContentTypeName { get; }
    public string AppUrl { get; }
    public string AppSharedUrl { get; }
    public int? AppSettingsId { get; }
    public int? AppResourcesId { get; }

    public bool IsContent { get; }
    public bool HasContent { get; }
    public bool SupportsAjax { get; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string Edition { get; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string TemplatePath { get; }

    [JsonPropertyName("templateIsShared")]
    public bool TemplateIsShared { get; }

    /// <summary>
    /// The view name to show on the layout button, new v17
    /// </summary>
    [JsonPropertyName("viewName")]
    public string ViewName { get; }

    /// <summary>
    /// The app name to show on the layout button, new v17
    /// </summary>
    [JsonPropertyName("appName")]
    public string AppName { get; }

    [JsonPropertyName("renderMs")]
    public int RenderMs { get; }

    [JsonPropertyName("renderLightspeed")]
    public bool RenderLightspeed { get; }

    public ContentBlockDto(IBlock block, RenderStatistics statistics)
    {
        IsCreated = block.ContentGroupExists;
        IsContent = block.IsContentApp;
        var app = block.App;

        Id = block.Configuration?.Id ?? 0;
        Guid = block.Configuration?.Guid ?? Guid.Empty;
        AppId = block.AppId;
        AppName = app?.Name ?? "";
        AppUrl = app?.Path ?? "" + "/";
        AppSharedUrl = app?.PathShared ?? "" + "/";
        AppSettingsId = app?.Settings?.Entity?.Attributes?.Count > 0
            ? app?.Settings?.EntityId : null;    // the real id (if entity exists), 0 (if entity missing, but type has fields), or null (if not available)
        AppResourcesId = app?.Resources?.Entity?.Attributes?.Count > 0
            ? app?.Resources?.EntityId : null;  // the real id (if entity exists), 0 (if entity missing, but type has fields), or null (if not available)

        HasContent = block.View != null && (block.Configuration?.Exists ?? false);

        ZoneId = block.ZoneId;
        TemplateId = block.View?.Id ?? 0;
        Edition = block.View?.Edition;
        ViewName = block.View?.Name;
        TemplatePath = block.View?.EditionPath.PrefixSlash();
        TemplateIsShared = block.View?.IsShared ?? false;
        QueryId = block.View?.Query?.Id; // will be null if not defined
        ContentTypeName = block.View?.ContentType ?? "";
        IsList = block.Configuration?.View?.UseForList ?? false;
        SupportsAjax = block.IsContentApp || (block.App?.Configuration?.EnableAjax ?? false);

        RenderMs = statistics?.RenderMs ?? -1;
        RenderLightspeed = statistics?.UseLightSpeed ?? false;
    }
}