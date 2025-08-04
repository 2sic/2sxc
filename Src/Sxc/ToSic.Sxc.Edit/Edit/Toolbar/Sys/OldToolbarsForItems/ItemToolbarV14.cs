using System.Text.Json;
using ToSic.Eav.Serialization.Sys.Json;
using ToSic.Sxc.Web.Sys.Html;

namespace ToSic.Sxc.Edit.Toolbar.Sys;

internal class ItemToolbarV14(IToolbarBuilder toolbar, IEntity? entity = null)
    : ItemToolbarV10(entity, null, null, null, toolbar, "TlbV14")
{
    public const string ContextAttributeName = "sxc-context";

    protected readonly IToolbarBuilder ToolbarBuilder = toolbar;

    protected override string ToolbarAttributes(string tlbAttrName) 
        => $" {ContextAttribute()} {HtmlAttribute.Create(tlbAttrName, ToolbarJson)} ";

    protected string? ContextAttribute()
    {
        var ctx = (ToolbarBuilder as IToolbarBuilderInternal)?.GetContext();
        return ctx == null
            ? null
            : HtmlAttribute.Create(ContextAttributeName, JsonSerializer.Serialize(ctx, JsonOptions.SafeJsonForHtmlAttributes)).ToString();
    }

}