using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using ToSic.Sxc.Web;
using IEntity = ToSic.Eav.Data.IEntity;

namespace ToSic.Sxc.Edit.Toolbar
{
    // Which can create different generations of toolbars
    // The current setup is quite complex as it handles many different scenarios and skips certain values in those scenarios
    internal class ItemToolbar: ItemToolbarBase
    {
        protected readonly List<ItemToolbarAction> Actions = new List<ItemToolbarAction>();
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
                Actions.Add(new ItemToolbarAction(entity) {contentType = newType, prefill = prefill});
                return;
            }

            // Case 3 - we have multiple actions
            var actList = actions.Split(',').Select(p => p.Trim()).ToList();
            foreach (var act in actList)
                Actions.Add(new ItemToolbarAction(entity)
                {
                    action = act,
                    contentType = newType,
                    prefill = prefill
                });

        }

        private string ToolbarObjJson() => JsonConvert.SerializeObject(
            ClassicToolbarOrNull ?? (Actions.Count == 1 ? Actions.First() : (object) Actions));


        private string SettingsJson => JsonConvert.SerializeObject(Settings);


        [JsonIgnore]
        public override string ToolbarAsTag => ToolbarTagTemplate.Replace(ToolbarTagPlaceholder, $" {Build.Attribute(JsonToolbarNodeName, ToolbarJson )} {AttributeSettings()} ");

        protected override string ToolbarJson => ToolbarObjJson();

        private IHybridHtmlString AttributeSettings() => Build.Attribute(JsonSettingsNodeName, SettingsJson);

        public override string ToolbarAsAttributes() =>
            Build.Attribute(ToolbarAttributeName,
                "{\"" + JsonToolbarNodeName + "\":" + ToolbarObjJson() + ",\"" + JsonSettingsNodeName + "\":" +
                SettingsJson + "}").ToString();
        
    }
}