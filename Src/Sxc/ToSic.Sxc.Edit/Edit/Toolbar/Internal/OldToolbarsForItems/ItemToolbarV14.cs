using System.Text.Json;
using ToSic.Eav.Serialization;
using Build = ToSic.Sxc.Web.Build;

namespace ToSic.Sxc.Edit.Toolbar;

internal class ItemToolbarV14(IEntity entity, IToolbarBuilder toolbar)
    : ItemToolbarV10(entity, null, null, null, toolbar, "TlbV14")
{
    public const string ContextAttributeName = "sxc-context";

    protected readonly IToolbarBuilder ToolbarBuilder = toolbar;

    protected override string ToolbarAttributes(string tlbAttrName) 
        => $" {ContextAttribute()} {Build.Attribute(tlbAttrName, ToolbarJson)} ";

    protected string ContextAttribute()
    {
        var ctx = (ToolbarBuilder as IToolbarBuilderInternal)?.GetContext();
        return ctx == null
            ? null
            : Build.Attribute(ContextAttributeName, JsonSerializer.Serialize(ctx, JsonOptions.SafeJsonForHtmlAttributes)).ToString();
    }

}