using static ToSic.Sxc.Edit.Toolbar.Sys.ToolbarConstants;

namespace ToSic.Sxc.Tests.ServicesTests.ToolbarService;


public class TweakButtonOther: TweakButtonTestsBase
{
    [Fact]
    public void Tooltip()
        => AssertUi([$"{RuleTooltip}=Hello"], NewTb().TacTooltip("Hello"));

    [Fact]
    public void Group()
        => AssertUi([$"{RuleGroup}=Hello"], NewTb().TacGroup("Hello"));

    [Fact]
    public void Icon()
        => AssertUiJson([new { icon = "bomb" }], NewTb().TacIcon("bomb"));

    [Fact]
    public void Classes()
        => AssertUi([$"{RuleClass}=enhanced"], NewTb().TacClasses("enhanced"));

    [Fact]
    public void Position()
        => AssertUi([$"{RulePosition}=4"], NewTb().TacPosition(4));

    [Fact]
    public void Ui1String()
        => AssertUi(["Hello"], NewTb().TacUi("Hello"));

    [Fact]
    public void Ui2Strings()
        => AssertUi(["Hello=World"], NewTb().TacUi("Hello", "World"));

    [Fact]
    public void Ui1ObjectA()
        => AssertUi([new { hello = "world" }], NewTb().TacUi(new { hello = "world"}));

    [Fact]
    public void Ui1ObjectB()
        => AssertUi([new { hello = "world", name = "iJungleboy" }], NewTb().TacUi(new { hello = "world", name = "iJungleboy"}));


}