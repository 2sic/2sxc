using static ToSic.Sxc.Edit.Toolbar.Sys.Rules.ToolbarRuleOperation;

namespace ToSic.Sxc.Tests.EditTests.ToolbarRuleTests;


public class ToolbarRuleBasicTests
{
    [Fact]
    public void VerbIsInCommandAndToString()
        => Equal("edit", new ToolbarRuleForTest("edit").ToString());

    [Fact]
    public void VerbWithoutOperationToString()
        => Equal("edit", new ToolbarRuleForTest("edit").ToString());

    [Fact]
    public void VerbWithOpPlusToString()
        => Equal("+edit", new ToolbarRuleForTest("edit", operation: AddOperation).ToString());

    [Fact]
    public void VerbWithOpMinusToString()
        => Equal("-edit", new ToolbarRuleForTest("edit", operation: RemoveOperation).ToString());

    [Fact]
    public void VerbWithOpModToString()
        => Equal("%edit", new ToolbarRuleForTest("edit", operation: ModifyOperation).ToString());

    [Fact]
    public void VerbWithOpSkipToString()
        => Equal("", new ToolbarRuleForTest("edit", operation: SkipInclude).ToString());

    [Fact]
    public void UiShouldBeAdded()
        => Equal("edit&test=abc", new ToolbarRuleForTest("edit", ui: "test=abc").ToString());

    [Fact]
    public void ParamsShouldBeAdded()
        => Equal("edit?test=param", new ToolbarRuleForTest("edit", parameters: "test=param").ToString());

    [Fact]
    public void ParamsAndUiShouldBeAdded()
        => Equal("edit&ui=abc?test=param", new ToolbarRuleForTest("edit", ui: "ui=abc", parameters: "test=param").ToString());

    [Fact]
    public void OpMinusWithMoreToStringShouldNotKeepParams()
        => Equal("-edit", new ToolbarRuleForTest("edit", ui: "test=test", operation: RemoveOperation).ToString());
}