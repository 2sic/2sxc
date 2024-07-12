using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToSic.Sxc.Edit.Toolbar.Internal;
using static ToSic.Sxc.Edit.Toolbar.ToolbarConstants;

namespace ToSic.Sxc.Tests.ServicesTests.ToolbarService;

[TestClass]
public class TweakButtonOther: TweakButtonTestsBase
{
    [TestMethod]
    public void Tooltip()
        => AssertUi([$"{RuleTooltip}=Hello"], NewTb().TacTooltip("Hello"));

    [TestMethod]
    public void Group()
        => AssertUi([$"{RuleGroup}=Hello"], NewTb().TacGroup("Hello"));

    [TestMethod]
    public void Icon()
        => AssertUiJson([new { icon = "bomb" }], NewTb().TacIcon("bomb"));

    [TestMethod]
    public void Classes()
        => AssertUi([$"{RuleClass}=enhanced"], NewTb().TacClasses("enhanced"));

    [TestMethod]
    public void Position()
        => AssertUi([$"{RulePosition}=4"], NewTb().TacPosition(4));

    [TestMethod]
    public void Ui1String()
        => AssertUi(["Hello"], NewTb().TacUi("Hello"));

    [TestMethod]
    public void Ui2Strings()
        => AssertUi(["Hello=World"], NewTb().TacUi("Hello", "World"));

    [TestMethod]
    public void Ui1ObjectA()
        => AssertUi([new { hello = "world" }], NewTb().TacUi(new { hello = "world"}));

    [TestMethod]
    public void Ui1ObjectB()
        => AssertUi([new { hello = "world", name = "iJungleboy" }], NewTb().TacUi(new { hello = "world", name = "iJungleboy"}));


}