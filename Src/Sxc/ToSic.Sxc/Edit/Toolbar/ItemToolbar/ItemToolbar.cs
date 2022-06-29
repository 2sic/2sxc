using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using ToSic.Sxc.Web;
using IEntity = ToSic.Eav.Data.IEntity;

namespace ToSic.Sxc.Edit.Toolbar
{
    // TODO: This should ideally be split into multiple objects with the same interface
    // 2022-06-22 2dm I split all this, but left most of the old code commented so we can better see what to fix
    // Clean up ca. 2022-08 when it has proven stable


    // Which can create different generations of toolbars
    // The current setup is quite complex as it handles many different scenarios and skips certain values in those scenarios
    internal class ItemToolbar: ItemToolbarBase
    {
        protected readonly List<ItemToolbarAction> Actions = new List<ItemToolbarAction>();
        protected readonly object ClassicToolbarOrNull;
        //protected readonly List<string> ToolbarV10;
        protected readonly object Settings;
        //protected readonly IToolbarBuilder ToolbarBuilderOrNull;

        //protected readonly ItemToolbarAction TargetV10;

        public ItemToolbar(IEntity entity, string actions = null, string newType = null, object prefill = null, object settings = null, object toolbar = null): base("TlbCls")
        {
            Settings = settings;

            // Store the toolbar if it's a toolbar builder
            //ToolbarBuilderOrNull = toolbar as IToolbarBuilder;

            //// Case 1 - use the simpler string format in V10.27
            //var toolbarAsStringArray = ItemToolbarV10.ToolbarV10OrNull(toolbar);
            //if(settings is string || toolbar is string || prefill is string || toolbarAsStringArray != null)
            //var (isV10, rules) = ItemToolbarPicker.CheckIfParamsMeanV10(toolbar, settings, prefill);
            //if (isV10)
            //{
            //    // Make sure ToolbarV10 is a real object - this code could also run with toolbar being null
            //    ToolbarV10 = rules ?? new List<string>();

            //    // check conflicting prefill format
            //    if (prefill != null && !(prefill is string))
            //        throw new Exception("Tried to build toolbar in new V10 format, but prefill is not a string. In V10.27+ it expects a string in url format like field=value&field2=value2");

            //    TargetV10 = new ItemToolbarAction(entity) { contentType = newType, prefill = prefill };
            //    return;
            //}

            // Case 2 - we have a classic V3 Toolbar object
            if (toolbar != null)
            {
                //// check conflicting parameters
                //if (actions != null || newType != null || prefill != null)
                //    throw new Exception(
                //        "trying to build toolbar but got both toolbar and actions/prefill/newType - this is conflicting, cannot continue");
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

        //private bool UseV10 => ToolbarV10 != null;

        //private string ToolbarV10Json()
        //{
        //    if (_toolbarV10Json != null) return _toolbarV10Json;
        //    // add params if we have any
        //    if (TargetV10 != null)
        //    {
        //        var asUrl = ObjectAsQueryString(TargetV10);
        //        if(!IsNullOrWhiteSpace(asUrl)) ToolbarV10.Add("params?" + asUrl);
        //    }

        //    // Add settings if we have any
        //    if (Settings != null)
        //    {
        //        var settingsAsUrl = Settings is string useRaw ? useRaw : ObjectAsQueryString(Settings);
        //        if (!IsNullOrWhiteSpace(settingsAsUrl)) ToolbarV10.Add("settings?" + settingsAsUrl);
        //    }

        //    // return result
        //    return _toolbarV10Json = JsonConvert.SerializeObject(ToolbarV10);
        //}

        //private string _toolbarV10Json;
        

        private string SettingsJson => JsonConvert.SerializeObject(Settings);


        [JsonIgnore]
        public override string ToolbarAsTag => ToolbarTagTemplate.Replace(ToolbarTagPlaceholder, $" { null /*ContextAttribute()*/} {Build.Attribute(JsonToolbarNodeName, ToolbarJson )} {AttributeSettings()} ");

        protected override string ToolbarJson => // UseV10 ? ToolbarV10Json() : 
            ToolbarObjJson();

        protected IHybridHtmlString AttributeSettings() => // UseV10 ? null : 
            Build.Attribute(JsonSettingsNodeName, SettingsJson);

        public override string ToolbarAsAttributes() => // UseV10 
            // ? ContextAttribute() + " " + Build.Attribute(ToolbarAttributeName, ToolbarV10Json()) 
            //    :
            Build.Attribute(ToolbarAttributeName,
                "{\"" + JsonToolbarNodeName + "\":" + ToolbarObjJson() + ",\"" + JsonSettingsNodeName + "\":" +
                SettingsJson + "}").ToString();

        //public string ContextAttribute()
        //{
        //    var ctx = ToolbarBuilderOrNull?.Context();
        //    return ctx == null 
        //        ? null 
        //        : Build.Attribute(ContextAttributeName, JsonConvert.SerializeObject(ctx)).ToString();
        //}

        //public string ObjectAsQueryString(object obj)
        //{
        //    var properties = obj.GetType().GetProperties()
        //        .Where(p => p.GetValue(obj, null) != null)
        //        .Where(p => !p.GetCustomAttributes(typeof(JsonIgnoreAttribute), false).Any())
        //        .Select(p => p.Name + "=" + Uri.EscapeUriString(p.GetValue(obj, null).ToString()));

        //    return Join("&", properties.ToArray());
        //}
    }
}