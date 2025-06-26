using ToSic.Razor.Markup;

namespace ToSic.Sxc.Edit.Toolbar;

[ShowApiWhenReleased(ShowApiMode.Never)]
internal class ToolbarRuleBase: RawHtmlString
{

    protected ToolbarRuleBase(): base(string.Empty) {}

    protected ToolbarRuleBase(string rule): base(rule) { }


    public ToolbarContext? Context { get; protected set; }

}