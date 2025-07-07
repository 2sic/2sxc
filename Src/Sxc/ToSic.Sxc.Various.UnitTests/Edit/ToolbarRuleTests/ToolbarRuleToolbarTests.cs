using ToSic.Sxc.Edit.Toolbar;
using ToSic.Sxc.Edit.Toolbar.Sys.Rules;
using static ToSic.Sxc.Edit.Toolbar.Sys.Rules.ToolbarRuleToolbar;

namespace ToSic.Sxc.Tests.EditTests.ToolbarRuleTests;


public class ToolbarRuleToolbarTests
{
    [Fact]
    public void CommandIsCorrect()
        => Equal(RuleToolbar, new ToolbarRuleToolbar().Command);

    [Fact]
    public void NameNone()
        => Equal(RuleToolbar, new ToolbarRuleToolbar().ToString());

    [Fact]
    public void NameEmpty()
        => Equal($"{RuleToolbar}={ToolbarRuleToolbar.Empty}", new ToolbarRuleToolbar(ToolbarRuleToolbar.Empty).ToString());
}