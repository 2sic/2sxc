using static ToSic.Sxc.Edit.Toolbar.ToolbarConstants;

namespace ToSic.Sxc.Tests.ServicesTests.ToolbarService;


public class TweakButtonShow: TweakButtonTestsBase
{
    [Fact]
    public void ShowTrue() => AssertUi([$"{RuleShow}=true"], NewTb().TacShow(true));

    [Fact]
    public void ShowFalse() => AssertUi([$"{RuleShow}=false"], NewTb().TacShow(false));

    [Fact]
    public void ShowNoParam() => AssertUi([$"{RuleShow}=true"], NewTb().TacShow());


}