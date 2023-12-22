using System;
using System.Text.Json.Serialization;
using ToSic.Eav.Helpers;
using ToSic.Sxc.Blocks;

namespace ToSic.Sxc.Edit.ClientContextInfo;

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

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] public string Edition { get; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] public string TemplatePath { get; }
    public bool TemplateIsShared { get; }

    public ContentBlockDto(IBlock block)
    {
        IsCreated = block.ContentGroupExists;
        IsContent = block.IsContentApp;
        var app = block.App;

        Id = block.Configuration?.Id ?? 0;
        Guid = block.Configuration?.Guid ?? Guid.Empty;
        AppId = block.AppId;
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
        TemplatePath = block.View?.EditionPath.PrefixSlash();
        TemplateIsShared = block.View?.IsShared ?? false;
        QueryId = block.View?.Query?.Id; // will be null if not defined
        ContentTypeName = block.View?.ContentType ?? "";
        IsList = block.Configuration?.View?.UseForList ?? false;
        SupportsAjax = block.IsContentApp || (block.App?.Configuration?.EnableAjax ?? false);
    }
}