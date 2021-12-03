using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using ToSic.Eav.Metadata;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Web;
using ToSic.Sxc.Web.Url;
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
        protected readonly object MetadataForV12;

        protected readonly ItemToolbarAction TargetV10;

        public ItemToolbar(IEntity entity, string actions = null, string newType = null, object metadataFor = null, object prefill = null, object settings = null, object toolbar = null)
        {
            Settings = settings;
            MetadataForV12 = metadataFor;

            // Case 1 - use the simpler string format in V10.27
            var toolbarAsStringArray = ToolbarV10OrNull(toolbar);
            if(settings is string || toolbar is string || prefill is string || toolbarAsStringArray != null)
            {
                // 2021-11-18 fix https://github.com/2sic/2sxc/issues/2561 - preserve till EOY in case something else breaks
                // #cleanup EOY 2021
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
            foreach (var act in actList)
                Actions.Add(new ItemToolbarAction(entity)
                {
                    action = act,
                    contentType = newType,
                    prefill = prefill
                });

        }

        private string ToolbarObjJson() => JsonConvert.SerializeObject(
            ToolbarObj ?? (Actions.Count == 1 ? Actions.First() : (object) Actions));

        private bool UseV10 => ToolbarV10 != null;

        private string ToolbarV10Json()
        {
            if (_toolbarV10Json != null) return _toolbarV10Json;
            // add params if we have any
            if (TargetV10 != null)
            {
                var asUrl = ObjectAsQueryString(TargetV10);
                if(!IsNullOrWhiteSpace(asUrl)) ToolbarV10.Add("params?" + asUrl);
            }

            // Add settings if we have any
            if (Settings != null)
            {
                var settingsAsUrl = Settings is string useRaw ? useRaw : ObjectAsQueryString(Settings);
                if (!IsNullOrWhiteSpace(settingsAsUrl)) ToolbarV10.Add("settings?" + settingsAsUrl);
            }

            UpdateMetadataCommandV12();

            // return result
            return _toolbarV10Json = JsonConvert.SerializeObject(ToolbarV10);
        }

        private void UpdateMetadataCommandV12()
        {
            if (MetadataForV12 == null) return;

            // 1. check if it's a valid target
            ITarget target = null;
            if (MetadataForV12 is ITarget pureTarget) target = pureTarget;
            else if (MetadataForV12 is IMetadataOf mdOf) target = mdOf.MetadataId;
            else if (MetadataForV12 is IHasMetadata hasMd) target = hasMd.Metadata.MetadataId;
            else if (MetadataForV12 is IIsMetadataTarget isTarget) target = isTarget.MetadataId;

            if (target == null) return;

            // 2. build target string
            var mdFor = "for=" + target.TargetType + "," +
                        (target.KeyGuid != null ? "guid," + target.KeyGuid
                            : target.KeyString != null ? "string," + target.KeyString
                            : "number," + target.KeyNumber);

            // 3. check if there is already a rule for this? otherwise ignore!
            var existing = ToolbarV10.FirstOrDefault(rule => rule?.Trim().TrimStart('+').TrimStart('%').StartsWith("metadata") ?? false);

            // 4. add / update rule
            var newRule = existing ?? "+metadata";

            newRule = UrlHelpers.AddQueryString(newRule, mdFor);
            var index = ToolbarV10.IndexOf(existing);
            if (index == -1) return;
            ToolbarV10[index] = newRule;
        }

        private string _toolbarV10Json;

        // 2021-11-18 fix https://github.com/2sic/2sxc/issues/2561 - preserve till EOY in case something else breaks
        // #cleanup EOY 2021
        ///// <summary>
        ///// Check if the configuration we got is a V10 Toolbar
        ///// </summary>
        ///// <param name="toolbar"></param>
        ///// <returns></returns>
        //private bool ToolbarIsV10Format(object toolbar)
        //{
        //    // return toolbar is IEnumerable<string> array && array.FirstOrDefault() != null;
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
            return !asArray.All(o => o is string || o is IString)
                ? null 
                : asArray.Select(o => o.ToString()).ToList();
        }

        private string SettingsJson => JsonConvert.SerializeObject(Settings);

        [JsonIgnore]
        public string Toolbar =>
            $"<ul class=\"sc-menu\" {Build.Attribute("toolbar", UseV10 ? ToolbarV10Json() : ToolbarObjJson())} { (UseV10 ? null : Build.Attribute("settings", SettingsJson))}></ul>";

        public string ToolbarAttribute() => UseV10 ? ToolbarV10Json() : "{\"toolbar\":" + ToolbarObjJson() + ",\"settings\":"+ SettingsJson + "}";


        public string ObjectAsQueryString(object obj)
        {
            var properties = obj.GetType().GetProperties()
                .Where(p => p.GetValue(obj, null) != null)
                .Where(p => !p.GetCustomAttributes(typeof(JsonIgnoreAttribute), false).Any())
                .Select(p => p.Name + "=" + Uri.EscapeUriString(p.GetValue(obj, null).ToString()));

            return Join("&", properties.ToArray());
        }
    }
}