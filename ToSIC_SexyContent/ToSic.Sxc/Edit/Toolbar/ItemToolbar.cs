using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using IEntity = ToSic.Eav.Data.IEntity;

namespace ToSic.Sxc.Edit.Toolbar
{
    internal class ItemToolbar
    {
        private readonly List<ItemToolbarAction> _actions = new List<ItemToolbarAction>();
        private readonly object _fullConfig;
        private readonly object _fullSettings;

        public ItemToolbar(IEntity dynamicEntity, string actions = null, string newType = null, object prefill = null, object toolbar = null, object settings = null)
        {
            _fullSettings = settings;

            if (toolbar != null)
            {
                // check conflicting parameters
                if (actions != null || newType != null || prefill != null)
                    throw new Exception(
                        "trying to build toolbar but got both toolbar and actions/prefill/newType - this is conflicting, cannot continue");
                _fullConfig = toolbar;
            }
            else // build a toolbar based on the actions or just from empty definition
            {
                if (actions == null)
                    _actions.Add(new ItemToolbarAction(dynamicEntity) {contentType = newType, prefill = prefill});
                else
                {
                    var actList = actions.Split(',').Select(p => p.Trim()).ToList();
                    foreach (string act in actList)
                        _actions.Add(new ItemToolbarAction(dynamicEntity)
                        {
                            action = act,
                            contentType = newType,
                            prefill = prefill
                        });
                }
            }

        }

        private string ToolbarJson  =>  JsonConvert.SerializeObject(_fullConfig ??
            (_actions.Count == 1 ? _actions.First() : (object)_actions));

        private string SettingsJson => JsonConvert.SerializeObject(_fullSettings);

        [JsonIgnore]
        public string Toolbar =>
            $"<ul class=\"sc-menu\" {SexyContent.Html.Build.Attribute("toolbar", ToolbarJson)} {SexyContent.Html.Build.Attribute("settings", SettingsJson)}></ul>";

        [JsonIgnore]
        public string ToolbarAttribute => "{\"toolbar\":" + ToolbarJson + ",\"settings\":"+ SettingsJson + "}"; 
    }
}