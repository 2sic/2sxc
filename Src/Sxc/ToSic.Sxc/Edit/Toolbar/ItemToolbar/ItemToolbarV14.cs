using Newtonsoft.Json;
using ToSic.Eav.Data;
using Build = ToSic.Sxc.Web.Build;

namespace ToSic.Sxc.Edit.Toolbar
{
    internal class ItemToolbarV14: ItemToolbarV10
    {
        public const string ContextAttributeName = "sxc-context";

        public ItemToolbarV14(IEntity entity, IToolbarBuilder toolbar = null) : base(entity, null, null, null, toolbar, "TlbV13")
        {
            ToolbarBuilder = toolbar;
        }

        protected readonly IToolbarBuilder ToolbarBuilder;

        protected override string ToolbarAttributes(string tlbAttrName) 
            => $" {ContextAttribute()} {Build.Attribute(tlbAttrName, ToolbarJson)} ";

        protected string ContextAttribute()
        {
            var ctx = ToolbarBuilder?.Context();
            return ctx == null
                ? null
                : Build.Attribute(ContextAttributeName, JsonConvert.SerializeObject(ctx)).ToString();
        }

    }
}
