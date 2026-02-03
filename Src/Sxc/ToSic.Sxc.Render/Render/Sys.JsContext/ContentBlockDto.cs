using System.Text.Json.Serialization;
using ToSic.Eav.Apps.Sys.AppJson;
using ToSic.Sxc.Blocks.Sys;

namespace ToSic.Sxc.Render.Sys.JsContext;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class ContentBlockDto : EntityDto
{
    public bool IsCreated { get; }
    public bool IsList { get; }
    public int TemplateId { get; }

    /// <summary>
    /// Query ID so the ui can ???
    /// </summary>
    public int? QueryId { get; }

    /// <summary>
    /// The name to show in the layout button, new v17.07
    /// </summary>
    [JsonPropertyName("queryName")]
    public string? QueryName { get; }

    /// <summary>
    /// The name to show in the layout button, new v17.07
    /// </summary>
    [JsonPropertyName("queryInfo")]
    public string? QueryInfo { get; }

    public string ContentTypeName { get; }
    public string AppUrl { get; }
    public string AppSharedUrl { get; }
    public int? AppSettingsId { get; }
    public int? AppResourcesId { get; }

    public bool IsContent { get; }
    public bool HasContent { get; }
    public bool SupportsAjax { get; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Edition { get; }

    [JsonPropertyName("editions")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Editions { get; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? TemplatePath { get; }

    [JsonPropertyName("templateIsShared")]
    public bool TemplateIsShared { get; }

    /// <summary>
    /// The view name to show on the layout button, new v17
    /// </summary>
    [JsonPropertyName("viewName")]
    public string? ViewName { get; }

    /// <summary>
    /// Will be true if the view was replaced based on URL parameters.
    /// This is to prevent the user from accidentally saving the temporarily set view.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("viewSwitchDisabled")]
    public bool? ViewSwitchDisabled { get; }

    /// <summary>
    /// The app name to show on the layout button, new v17
    /// </summary>
    [JsonPropertyName("appName")]
    public string AppName { get; }

    /// <summary>
    /// Render time in milliseconds to show in the layout button, new v17
    /// </summary>
    [JsonPropertyName("renderMs")]
    public int RenderMs { get; }

    /// <summary>
    /// Tell the UI if LightSpeed was used to render this block, new v17
    /// </summary>
    [JsonPropertyName("renderLightspeed")]
    public bool RenderLightspeed { get; }

    public ContentBlockDto(IBlock block, RenderStatistics? statistics, IAppJsonConfigurationService appJson)
    {
        IsCreated = block.ContentGroupExists;
        IsContent = block.IsContentApp;

        var configuration = block.ConfigurationIsReady ? block.Configuration : null;
        Id = configuration?.Id ?? 0;
        Guid = configuration?.Guid ?? Guid.Empty;
        AppId = block.AppId;

        // App properties
        var app = block.AppOrNull;
        AppName = app?.Name ?? "";
        AppUrl = app?.Path ?? "" + "/";
        AppSharedUrl = app?.PathShared ?? "" + "/";
        AppSettingsId = app?.Settings?.Entity?.Attributes?.Count > 0
            ? app?.Settings?.EntityId
            : null;    // the real id (if entity exists), 0 (if entity missing, but type has fields), or null (if not available)
        AppResourcesId = app?.Resources?.Entity?.Attributes?.Count > 0
            ? app?.Resources?.EntityId
            : null;  // the real id (if entity exists), 0 (if entity missing, but type has fields), or null (if not available)

        // View properties
        var view = block.ViewIsReady ? block.View : null;
        HasContent = view != null && (configuration?.Exists ?? false);

        ZoneId = block.ZoneId;
        TemplateId = view?.Id ?? 0;
        Edition = view?.Edition;
        ViewName = view?.Name;
        ViewSwitchDisabled = view?.IsReplaced;
        TemplatePath = view?.EditionPath.PrefixSlash();
        TemplateIsShared = view?.IsShared ?? false;
        ContentTypeName = view?.ContentType ?? "";

        // Query properties
        var query = view?.Query;
        QueryId = query?.Id; // will be null if not defined
        QueryName = query?.Title;

        try
        {
            if (query != null)
            {
                var streamInfo = block.Data?.Out
                    .Select(pair => new
                    {
                        pair.Key,
                        Count = pair.Value?.List?.Count() ?? 0,
                        FirstType = pair.Value?.List?.FirstOrDefault()?.Type?.Name
                    })
                    .ToList()
                    ?? [];

                // Create a csv list of stream names with count and first type
                var msg = streamInfo.Aggregate("", (current, stream)
                    => current + $"<br>- {stream.Key} ({stream.Count}{stream.FirstType.NullOrGetWith(ft => $", first is {ft}") ?? " items"}), ");

                QueryInfo = $"Query Streams: {msg}";
            }
        }
        catch
        {
            /* ignore */
        }

        IsList = configuration?.View?.UseForList ?? false;
        SupportsAjax = block.IsContentApp || (app?.Configuration?.EnableAjax ?? false);

        RenderMs = statistics?.RenderMs ?? -1;
        RenderLightspeed = statistics?.UseLightSpeed ?? false;

        if (app == null)
            return;

        try
        {
            var appJsonData = appJson.GetAppJson(app.AppId);
            if (appJsonData?.Editions != null)
            {
                Editions = string.Join(",", appJsonData.Editions.Keys);
            }
        }
        catch
        {
            /* ignore */
        }
    }
}