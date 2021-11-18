using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using ToSic.Sxc.Web;
using static System.String;
using IEntity = ToSic.Eav.Data.IEntity;

namespace ToSic.Sxc.Edit.Toolbar
{
    internal class ItemToolbar
    {
        protected readonly List<ItemToolbarAction> Actions = new List<ItemToolbarAction>();
        protected readonly object ToolbarObj;
        protected readonly List<string> ToolbarV10;
        protected readonly object Settings;

        protected readonly ItemToolbarAction TargetV10;

        public ItemToolbar(IEntity entity, string actions = null, string newType = null, object prefill = null, object toolbar = null, object settings = null)
        {
            Settings = settings;

            // Case 1 - use the simpler string format in V10.27
            var toolbarAsStringArray = ToolbarV10OrNull(toolbar);
            if(settings is string || toolbar is string || prefill is string || toolbarAsStringArray != null)
            {
                // 2021-11-18 fix https://github.com/2sic/2sxc/issues/2561 - preserve till EOY in case something else breaks
                //ToolbarV10 = toolbar == null
                //    ? new List<string>()
                //    : toolbar is string tlbString
                //        ? tlbString.Split('|').ToList() //new List<string> {tlbString}
                //        : toolbarAsStringArray; // (toolbar as IEnumerable<string>).ToList();

                // Make sure ToolbarV10 is a real object - this code could also run with toolbar being null
                ToolbarV10 = toolbarAsStringArray ?? new List<string>();

                // check conflicting prefill format
                if (prefill != null && !(prefill is string))
                    throw new Exception("Tried to build toolbar in new V10 format, but prefill is not a string. In V10.27+ it expects a string in url format like field=value&field2=value2");

                TargetV10 = new ItemToolbarAction(entity) { contentType = newType, prefill = prefill };
                return;
            }

            // Case 2 - we have a classic V3 Toolbar object
            if (toolbar != null)
            {
                // check conflicting parameters
                if (actions != null || newType != null || prefill != null)
                    throw new Exception(
                        "trying to build toolbar but got both toolbar and actions/prefill/newType - this is conflicting, cannot continue");
                ToolbarObj = toolbar;
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
            foreach (string act in actList)
                Actions.Add(new ItemToolbarAction(entity)
                {
                    action = act,
                    contentType = newType,
                    prefill = prefill
                });

        }

        private string ToolbarObjJson() => JsonConvert.SerializeObject(
            ToolbarObj ?? (Actions.Count == 1
                ? Actions.First()
                : (object) Actions));

        private bool UseV10 => ToolbarV10 != null;

        private string ToolbarV10Json()
        {
            // add params if we have any
            if (TargetV10 != null)
            {
                var asUrl = GetQueryString(TargetV10);
                if(!IsNullOrWhiteSpace(asUrl)) ToolbarV10.Add("params?" + GetQueryString(TargetV10));
            }

            // Add settings if we have any
            if (Settings != null)
            {
                var asUrl = Settings is string useRaw ? useRaw : GetQueryString(Settings);
                if (!IsNullOrWhiteSpace(asUrl)) ToolbarV10.Add("settings?" + asUrl);
            }

            // return result
            return JsonConvert.SerializeObject(ToolbarV10);
        }

        // 2021-11-18 fix https://github.com/2sic/2sxc/issues/2561 - preserve till EOY in case something else breaks
        ///// <summary>
        ///// Check if the configuration we got is a V10 Toolbar
        ///// </summary>
        ///// <param name="toolbar"></param>
        ///// <returns></returns>
        //private bool ToolbarIsV10Format(object toolbar)
        //{
        //    // Old - doesn't work because it's sometimes seen as an IEnumerable<object>
        //    // return toolbar is IEnumerable<string> array && array.FirstOrDefault() != null;

        //    if (!(toolbar is IEnumerable toolbarEnum)) return false;
        //    if (!(toolbarEnum is IEnumerable<object> objEnum)) return false;
        //    return objEnum.FirstOrDefault() is string;
        //}

        /// <summary>
        /// Check if the configuration we got is a V10 Toolbar
        /// </summary>
        /// <param name="toolbar"></param>
        /// <returns></returns>
        private List<string> ToolbarV10OrNull(object toolbar)
        {
            // Note: This is a bit complex because previously we checked for this:
            // return toolbar is IEnumerable<string> array && array.FirstOrDefault() != null;
            // But that failed, because sometimes razor made the new [] { "..." } be an object list instead
            // This is why it's more complex that you would intuitively do it - see https://github.com/2sic/2sxc/issues/2561

            if (!(toolbar is IEnumerable<object> objEnum)) return null;
            var asArray = objEnum.ToArray();
            return !asArray.All(o => o is string) 
                ? null 
                : asArray.Select(o => o.ToString()).ToList();
        }

        private string SettingsJson => JsonConvert.SerializeObject(Settings);

        [JsonIgnore]
        public string Toolbar =>
            $"<ul class=\"sc-menu\" {Build.Attribute("toolbar", UseV10 ? ToolbarV10Json() : ToolbarObjJson())} { (UseV10 ? null : Build.Attribute("settings", SettingsJson))}></ul>";

        public string ToolbarAttribute() => UseV10 ? ToolbarV10Json() : "{\"toolbar\":" + ToolbarObjJson() + ",\"settings\":"+ SettingsJson + "}";


        public string GetQueryString(object obj)
        {
            var properties = obj.GetType().GetProperties()
                .Where(p => p.GetValue(obj, null) != null)
                .Where(p => !p.GetCustomAttributes(typeof(JsonIgnoreAttribute), false).Any())
                .Select(p => p.Name + "=" + Uri.EscapeUriString(p.GetValue(obj, null).ToString()));

            return Join("&", properties.ToArray());
        }
    }
}