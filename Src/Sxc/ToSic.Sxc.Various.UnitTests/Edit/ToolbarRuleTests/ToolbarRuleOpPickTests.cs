using ToSic.Sxc.Edit.Toolbar;
using ToSic.Sxc.Edit.Toolbar.Sys.Rules;

namespace ToSic.Sxc.Tests.EditTests.ToolbarRuleTests;


public class ToolbarRuleOpPickTests
{
    private static char TacPick(string op, ToolbarRuleOps defOp, bool? condition = default)
        => ToolbarRuleOperation.Pick(op, defOp, condition);

    [Fact]
    public void UnknownReturnsDefOp1()
        => Equal((char)ToolbarRuleOps.OprNone, TacPick("", ToolbarRuleOps.OprNone));

    [Fact]
    public void UnknownReturnsDefOp2()
        => Equal(ToolbarRuleOperation.AddOperation, TacPick("", ToolbarRuleOps.OprAdd));

    [Theory]
    [InlineData(ToolbarRuleOperation.AddOperation, "+")]
    [InlineData(ToolbarRuleOperation.AddOperation, "add")]
    [InlineData(ToolbarRuleOperation.RemoveOperation, "-")]
    public void KnownReturnsValue(char expected, string operation)
        => Equal(expected, TacPick(operation, ToolbarRuleOps.OprNone));

    [Theory]
    [InlineData("huh")]
    [InlineData("what")]
    [InlineData("/")]
    [InlineData("&")]
    public void KnownReturnsFallback(string operation)
        => Equal((char)ToolbarRuleOps.OprNone, TacPick(operation, ToolbarRuleOps.OprNone));

    [Theory]
    [InlineData(ToolbarRuleOperation.AddOperation, "+")]
    [InlineData(ToolbarRuleOperation.AddOperation, "add")]
    [InlineData(ToolbarRuleOperation.RemoveOperation, "-")]
    [InlineData(ToolbarRuleOperation.ModifyOperation, "modify")]
    public void ConditionNullKeepsBehavior(char expected, string operation)
        => Equal(expected, TacPick(operation, ToolbarRuleOps.OprUnknown, condition: null));

    [Theory]
    [InlineData(ToolbarRuleOperation.AddOperation, "+")]
    [InlineData(ToolbarRuleOperation.AddOperation, "add")]
    [InlineData(ToolbarRuleOperation.RemoveOperation, "-")]
    [InlineData(ToolbarRuleOperation.ModifyOperation, "modify")]
    public void ConditionTrueKeepsBehavior(char expected, string operation)
        => Equal(expected, TacPick(operation, ToolbarRuleOps.OprUnknown, condition: true));

    /// <summary>
    /// Any kind of operation - if condition false - should not be added at all.
    /// </summary>
    /// <param name="expected"></param>
    /// <param name="operation"></param>
    [Theory]
    [InlineData("")]
    [InlineData( " ")]
    [InlineData( "+")]
    [InlineData( "add")]
    [InlineData("-")]
    [InlineData("modify")]
    public void ConditionFalseReturnsMinus(string operation)
        => Equal(ToolbarRuleOperation.SkipInclude, TacPick(operation, ToolbarRuleOps.OprNone, condition: false));


}