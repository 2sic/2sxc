using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToSic.Sxc.Edit.Toolbar;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace ToSic.Sxc.Tests.EditTests.ToolbarRuleTests;

[TestClass]
public class ToolbarRuleOpPickTests
{
    private static char Pick(string op, ToolbarRuleOps defOp, bool? condition = default)
        => ToolbarRuleOperation.Pick(op, defOp, condition);

    [TestMethod]
    public void UnknownReturnsDefOp1()
        => AreEqual((char)ToolbarRuleOps.OprNone, Pick("", ToolbarRuleOps.OprNone));

    [TestMethod]
    public void UnknownReturnsDefOp2()
        => AreEqual(ToolbarRuleOperation.AddOperation, Pick("", ToolbarRuleOps.OprAdd));

    [DataRow(ToolbarRuleOperation.AddOperation, "+")]
    [DataRow(ToolbarRuleOperation.AddOperation, "add")]
    [DataRow(ToolbarRuleOperation.RemoveOperation, "-")]
    [TestMethod]
    public void KnownReturnsValue(char expected, string operation)
        => AreEqual(expected, Pick(operation, ToolbarRuleOps.OprNone));

    [DataRow("huh")]
    [DataRow("what")]
    [DataRow("/")]
    [DataRow("&")]
    [TestMethod]
    public void KnownReturnsFallback(string operation)
        => AreEqual((char)ToolbarRuleOps.OprNone, Pick(operation, ToolbarRuleOps.OprNone));

    [DataRow(ToolbarRuleOperation.AddOperation, "+")]
    [DataRow(ToolbarRuleOperation.AddOperation, "add")]
    [DataRow(ToolbarRuleOperation.RemoveOperation, "-")]
    [DataRow(ToolbarRuleOperation.ModifyOperation, "modify")]
    [TestMethod]
    public void ConditionNullKeepsBehavior(char expected, string operation)
        => AreEqual(expected, Pick(operation, ToolbarRuleOps.OprUnknown, condition: null));

    [DataRow(ToolbarRuleOperation.AddOperation, "+")]
    [DataRow(ToolbarRuleOperation.AddOperation, "add")]
    [DataRow(ToolbarRuleOperation.RemoveOperation, "-")]
    [DataRow(ToolbarRuleOperation.ModifyOperation, "modify")]
    [TestMethod]
    public void ConditionTrueKeepsBehavior(char expected, string operation)
        => AreEqual(expected, Pick(operation, ToolbarRuleOps.OprUnknown, condition: true));

    [DataRow(ToolbarRuleOperation.RemoveOperation, "")]
    [DataRow(ToolbarRuleOperation.RemoveOperation, " ")]
    [DataRow(ToolbarRuleOperation.RemoveOperation, "+")]
    [DataRow(ToolbarRuleOperation.RemoveOperation, "add")]
    [DataRow(ToolbarRuleOperation.SkipInclude, "-")]
    [DataRow(ToolbarRuleOperation.SkipInclude, "modify")]
    [TestMethod]
    public void ConditionFalseReturnsMinus(char expected, string operation)
        => AreEqual(expected, Pick(operation, ToolbarRuleOps.OprNone, condition: false));


}