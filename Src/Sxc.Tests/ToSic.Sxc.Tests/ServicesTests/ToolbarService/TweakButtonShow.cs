using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToSic.Sxc.Edit.Toolbar.Internal;
using static ToSic.Sxc.Edit.Toolbar.ToolbarConstants;

namespace ToSic.Sxc.Tests.ServicesTests.ToolbarService;

[TestClass]
public class TweakButtonShow: TweakButtonTestsBase
{
    [TestMethod]
    public void ShowTrue() => AssertUi([$"{RuleShow}=true"], NewTb().TacShow(true));

    [TestMethod]
    public void ShowFalse() => AssertUi([$"{RuleShow}=false"], NewTb().TacShow(false));

    [TestMethod]
    public void ShowNoParam() => AssertUi([$"{RuleShow}=true"], NewTb().TacShow());


}