using System.Text.Json;
using ToSic.Eav.Data;
using ToSic.Eav.Serialization;
using Build = ToSic.Sxc.Web.Build;

namespace ToSic.Sxc.Edit.Toolbar;

internal class ItemToolbarV14: ItemToolbarV10
{
    public const string ContextAttributeName = "sxc-context";

    public ItemToolbarV14(IEntity entity, IToolbarBuilder toolbar) : base(entity, null, null, null, toolbar, "TlbV14")
    {
        ToolbarBuilder = toolbar;
    }

    protected readonly IToolbarBuilder ToolbarBuilder;

    protected override string ToolbarAttributes(string tlbAttrName) 
        => $" {ContextAttribute()} {Build.Attribute(tlbAttrName, ToolbarJson)} ";

    protected string ContextAttribute()
    {
        var ctx = ToolbarBuilder?.GetContext();
        return ctx == null
            ? null
            : Build.Attribute(ContextAttributeName, JsonSerializer.Serialize(ctx, JsonOptions.SafeJsonForHtmlAttributes)).ToString();
    }

}