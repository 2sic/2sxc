using System.Text.Json;
using ToSic.Eav.Plumbing;
using ToSic.Eav.Serialization;
using ToSic.Lib.Helpers;
using ToSic.Sxc.Web.Internal.Url;
using static System.String;
using Build = ToSic.Sxc.Web.Build;

namespace ToSic.Sxc.Edit.Toolbar;

internal class ItemToolbarV10(
    IEntity entity,
    string newType = null,
    string prefill = null,
    string settings = null,
    object toolbar = null,
    string logName = null)
    : ItemToolbarBase(logName ?? "TlbV10")
{
    protected readonly string Settings = settings;
    protected readonly List<string> Rules = ItemToolbarPicker.ToolbarV10OrNull(toolbar) ?? [];
    protected readonly EntityEditInfo TargetAction = new(entity) { contentType = newType, prefill = prefill };

    public override string ToolbarAsTag 
        => ToolbarTagTemplate.Replace(ToolbarTagPlaceholder, ToolbarAttributes(JsonToolbarNodeName));

    protected override string ToolbarJson => _toolbarJson.Get(ToolbarV10Json);
    private readonly GetOnce<string> _toolbarJson = new();

    public override string ToolbarAsAttributes() => ToolbarAttributes(ToolbarAttributeName);

    protected virtual string ToolbarAttributes(string tlbAttrName) => $" {Build.Attribute(tlbAttrName, ToolbarJson)} ";

    private string ToolbarV10Json()
    {
        // add params if we have any
        if (TargetAction != null)
        {
            var asUrl = new ObjectToUrl().Serialize(TargetAction);
            if (!IsNullOrWhiteSpace(asUrl)) Rules.Add("params?" + asUrl);
        }

        // Add settings if we have any
        if (Settings.HasValue()) Rules.Add($"settings?{Settings}");

        return JsonSerializer.Serialize(Rules, JsonOptions.SafeJsonForHtmlAttributes);
    }

}