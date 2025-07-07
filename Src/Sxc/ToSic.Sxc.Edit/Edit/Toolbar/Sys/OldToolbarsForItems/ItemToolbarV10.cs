using System.Text.Json;
using ToSic.Eav.Serialization.Sys.Json;
using ToSic.Sxc.Web.Sys.Html;
using ToSic.Sxc.Web.Sys.Url;
using ToSic.Sys.Utils;
using static System.String;

namespace ToSic.Sxc.Edit.Toolbar.Sys;

internal class ItemToolbarV10(
    IEntity? entity,
    string? newType = null,
    string? prefill = null,
    string? settings = null,
    object? toolbar = null,
    string? logName = null)
    : ItemToolbarBase(logName ?? "TlbV10")
{
    protected readonly string? Settings = settings;
    protected readonly List<string> Rules = ItemToolbarPicker.ToolbarV10OrNull(toolbar) ?? [];
    protected readonly EntityEditInfo TargetAction = new(entity) { contentType = newType, prefill = prefill };

    public override string ToolbarAsTag 
        => ToolbarTagTemplate.Replace(ToolbarTagPlaceholder, ToolbarAttributes(JsonToolbarNodeName));

    protected override string? ToolbarJson => _toolbarJson.Get(ToolbarV10Json);
    private readonly GetOnce<string> _toolbarJson = new();

    public override string ToolbarAsAttributes() => ToolbarAttributes(ToolbarAttributeName);

    protected virtual string ToolbarAttributes(string tlbAttrName) => $" {HtmlAttribute.Create(tlbAttrName, ToolbarJson)} ";

    private string ToolbarV10Json()
    {
        // add params if we have any
        if (TargetAction != null! /* paranoid */)
        {
            var asUrl = new ObjectToUrl().Serialize(TargetAction);
            if (!IsNullOrWhiteSpace(asUrl))
                Rules.Add("params?" + asUrl);
        }

        // Add settings if we have any
        if (Settings.HasValue())
            Rules.Add($"settings?{Settings}");

        return JsonSerializer.Serialize(Rules, JsonOptions.SafeJsonForHtmlAttributes);
    }

}