using ToSic.Razor.Markup;

namespace ToSic.Sxc.Edit.Toolbar;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class ToolbarRuleBase: RawHtmlString
{

    protected ToolbarRuleBase(): base(string.Empty) {}

    protected ToolbarRuleBase(string rule): base(rule) { }


    public ToolbarContext Context { get; protected set; }

}