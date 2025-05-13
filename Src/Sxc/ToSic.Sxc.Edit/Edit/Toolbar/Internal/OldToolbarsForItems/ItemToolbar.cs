using System.Text.Json.Serialization;
using ToSic.Eav.Plumbing;
using ToSic.Eav.Serialization;
using ToSic.Sxc.Web;
using IEntity = ToSic.Eav.Data.IEntity;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace ToSic.Sxc.Edit.Toolbar;

// Which can create different generations of toolbars
// The current setup is quite complex as it handles many different scenarios and skips certain values in those scenarios
internal class ItemToolbar: ItemToolbarBase
{
    protected readonly List<ItemToolbarAction> Actions = [];
    protected readonly object ClassicToolbarOrNull;
    protected readonly object Settings;

    public ItemToolbar(IEntity entity, string actions = null, string newType = null, object prefill = null, object settings = null, object toolbar = null): base("TlbCls")
    {
        Settings = settings;

        // Case 1 - we have a classic V3 Toolbar object
        if (toolbar != null)
        {
            ClassicToolbarOrNull = toolbar;
            return;
        }
            
        // Case 2 build a toolbar based on the actions or just from empty definition
        if (actions == null)
        {
            Actions.Add(new(entity) {contentType = newType, prefill = prefill});
            return;
        }

        // Case 3 - we have multiple actions
        var actList = actions.CsvToArrayWithoutEmpty();
        foreach (var act in actList)
            Actions.Add(new(entity)
            {
                action = act,
                contentType = newType,
                prefill = prefill
            });

    }

    private string ToolbarObjJson() => JsonSerializer.Serialize(
        ClassicToolbarOrNull ?? (Actions.Count == 1 ? Actions.First() : (object) Actions), JsonOptions.SafeJsonForHtmlAttributes);


    private string SettingsJson => JsonSerializer.Serialize(Settings, JsonOptions.SafeJsonForHtmlAttributes);


    [JsonIgnore]
    public override string ToolbarAsTag => ToolbarTagTemplate.Replace(ToolbarTagPlaceholder, $" {Build.Attribute(JsonToolbarNodeName, ToolbarJson )} {AttributeSettings()} ");

    protected override string ToolbarJson => ToolbarObjJson();

    private string AttributeSettings() => Build.Attribute(JsonSettingsNodeName, SettingsJson).ToString();

    public override string ToolbarAsAttributes() =>
        Build.Attribute(ToolbarAttributeName,
            "{\"" + JsonToolbarNodeName + "\":" + ToolbarObjJson() + ",\"" + JsonSettingsNodeName + "\":" +
            SettingsJson + "}").ToString();
        
}