using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace ToSic.SexyContent.Edit.Toolbar
{
    internal class ItemToolbar
    {
        public List<ItemToolbarAction> Actions = new List<ItemToolbarAction>();

        public ItemToolbar(DynamicEntity dynamicEntity, string actions = null, string newType = null, object prefill = null)
        {
            if (actions == null)
                Actions.Add(new ItemToolbarAction(dynamicEntity) {contentType =  newType, prefill = prefill});
            else
            {
                var actList = actions.Split(',').Select(p => p.Trim()).ToList();
                foreach (string act in actList)
                    Actions.Add(new ItemToolbarAction(dynamicEntity) {action = act, contentType = newType, prefill = prefill});
            }

        }

        public string Json  =>  JsonConvert.SerializeObject((Actions.Count == 1) ? Actions.First() : (object)Actions); 

        [JsonIgnore]
        public string ToolbarTemplate = "<ul class=\"sc-menu\" data-toolbar='{0}'></ul>";

        [JsonIgnore]
        public string Toolbar => string.Format(ToolbarTemplate, Json);

    }
}