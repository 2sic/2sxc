using System.Collections.Generic;
using System.Text.Json;
using ToSic.Eav.Data;
using ToSic.Eav.Plumbing;
using ToSic.Eav.Serialization;
using ToSic.Lib.Helpers;
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
}
