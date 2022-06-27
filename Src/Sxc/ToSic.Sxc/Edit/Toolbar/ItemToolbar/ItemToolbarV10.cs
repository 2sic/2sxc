using System.Collections.Generic;
using Newtonsoft.Json;
using ToSic.Eav.Data;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Web.Url;
using static System.String;
using Build = ToSic.Sxc.Web.Build;

namespace ToSic.Sxc.Edit.Toolbar
{
    internal class ItemToolbarV10: ItemToolbarBase
    {
        public ItemToolbarV10(IEntity entity, string newType = null, string prefill = null, string settings = null, object toolbar = null, string logName = null) : base( logName ?? "TlbV10")
        {
            Settings = settings;
            Rules = ItemToolbarPicker.ToolbarV10OrNull(toolbar) ?? new List<string>();
            TargetAction = new EntityEditInfo(entity) { contentType = newType, prefill = prefill };
        }

        protected readonly string Settings;
        protected readonly List<string> Rules;
        protected readonly EntityEditInfo TargetAction;

        public override string ToolbarAsTag 
            => ToolbarTagTemplate.Replace(ToolbarTagPlaceholder, ToolbarAttributes(JsonToolbarNodeName));

        protected override string ToolbarJson => _toolbarJson.Get(ToolbarV10Json);
        private readonly ValueGetOnce<string> _toolbarJson = new ValueGetOnce<string>();

        public override string ToolbarAsAttributes() => ToolbarAttributes(ToolbarAttributeName);

        protected virtual string ToolbarAttributes(string tlbAttrName) => $" {Build.Attribute(tlbAttrName, ToolbarJson)} ";

        private string ToolbarV10Json()
        {
            // add params if we have any
            if (TargetAction != null)
            {
                //var asUrl = ObjectAsQueryString(TargetAction);
                //if (!IsNullOrWhiteSpace(asUrl)) Rules.Add("params?" + asUrl);

                var asUrl = new ObjectToUrl().SerializeIfNotString(TargetAction);
                if (!IsNullOrWhiteSpace(asUrl)) Rules.Add("params?" + asUrl);
            }

            // Add settings if we have any
            if (Settings.HasValue()) Rules.Add($"settings?{Settings}");

            return JsonConvert.SerializeObject(Rules);
        }


        //public string ObjectAsQueryString(object obj)
        //{
        //    var newSerialize = new ObjectToUrl().SerializeIfNotString(obj);

        //    var properties = obj.GetType().GetProperties()
        //        .Where(p => p.GetValue(obj, null) != null)
        //        .Where(p => !p.GetCustomAttributes(typeof(JsonIgnoreAttribute), false).Any())
        //        .Select(p => p.Name + "=" + Uri.EscapeUriString(p.GetValue(obj, null).ToString()));

        //    return Join("&", properties.ToArray());
        //}

    }
}
