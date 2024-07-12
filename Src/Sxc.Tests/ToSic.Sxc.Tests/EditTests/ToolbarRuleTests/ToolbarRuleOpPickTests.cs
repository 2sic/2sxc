using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToSic.Sxc.Edit.Toolbar;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace ToSic.Sxc.Tests.EditTests.ToolbarRuleTests;

[TestClass]
public class ToolbarRuleOpPickTests
{
    private static char TacPick(string op, ToolbarRuleOps defOp, bool? condition = default)
        => ToolbarRuleOperation.Pick(op, defOp, condition);

    [TestMethod]
    public void UnknownReturnsDefOp1()
        => AreEqual((char)ToolbarRuleOps.OprNone, TacPick("", ToolbarRuleOps.OprNone));

    [TestMethod]
    public void UnknownReturnsDefOp2()
        => AreEqual(ToolbarRuleOperation.AddOperation, TacPick("", ToolbarRuleOps.OprAdd));

    [DataRow(ToolbarRuleOperation.AddOperation, "+")]
    [DataRow(ToolbarRuleOperation.AddOperation, "add")]
    [DataRow(ToolbarRuleOperation.RemoveOperation, "-")]
    [TestMethod]
    public void KnownReturnsValue(char expected, string operation)
        => AreEqual(expected, TacPick(operation, ToolbarRuleOps.OprNone));

    [DataRow("huh")]
    [DataRow("what")]
    [DataRow("/")]
    [DataRow("&")]
    [TestMethod]
    public void KnownReturnsFallback(string operation)
        => AreEqual((char)ToolbarRuleOps.OprNone, TacPick(operation, ToolbarRuleOps.OprNone));

    [DataRow(ToolbarRuleOperation.AddOperation, "+")]
    [DataRow(ToolbarRuleOperation.AddOperation, "add")]
    [DataRow(ToolbarRuleOperation.RemoveOperation, "-")]
    [DataRow(ToolbarRuleOperation.ModifyOperation, "modify")]
    [TestMethod]
    public void ConditionNullKeepsBehavior(char expected, string operation)
        => AreEqual(expected, TacPick(operation, ToolbarRuleOps.OprUnknown, condition: null));

    [DataRow(ToolbarRuleOperation.AddOperation, "+")]
    [DataRow(ToolbarRuleOperation.AddOperation, "add")]
    [DataRow(ToolbarRuleOperation.RemoveOperation, "-")]
    [DataRow(ToolbarRuleOperation.ModifyOperation, "modify")]
    [TestMethod]
    public void ConditionTrueKeepsBehavior(char expected, string operation)
        => AreEqual(expected, TacPick(operation, ToolbarRuleOps.OprUnknown, condition: true));

    /// <summary>
    /// Any kind of operation - if condition false - should not be added at all.
    /// </summary>
    /// <param name="expected"></param>
    /// <param name="operation"></param>
    [DataRow("")]
    [DataRow( " ")]
    [DataRow( "+")]
    [DataRow( "add")]
    [DataRow("-")]
    [DataRow("modify")]
    [TestMethod]
    public void ConditionFalseReturnsMinus(string operation)
        => AreEqual(ToolbarRuleOperation.SkipInclude, TacPick(operation, ToolbarRuleOps.OprNone, condition: false));


}