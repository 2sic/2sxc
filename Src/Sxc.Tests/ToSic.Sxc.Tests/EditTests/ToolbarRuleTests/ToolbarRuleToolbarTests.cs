using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToSic.Sxc.Edit.Toolbar;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;
using static ToSic.Sxc.Edit.Toolbar.ToolbarRuleToolbar;

namespace ToSic.Sxc.Tests.EditTests.ToolbarRuleTests;

[TestClass]
public class ToolbarRuleToolbarTests
{
    [TestMethod]
    public void CommandIsCorrect()
        => AreEqual(RuleToolbar, new ToolbarRuleToolbar().Command);

    [TestMethod]
    public void NameNone()
        => AreEqual(RuleToolbar, new ToolbarRuleToolbar().ToString());

    [TestMethod]
    public void NameEmpty()
        => AreEqual($"{RuleToolbar}={Empty}", new ToolbarRuleToolbar(Empty).ToString());
}