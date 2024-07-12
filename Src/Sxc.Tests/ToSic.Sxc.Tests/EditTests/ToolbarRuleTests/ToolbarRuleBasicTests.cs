using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;
using static ToSic.Sxc.Edit.Toolbar.ToolbarRuleOperation;

namespace ToSic.Sxc.Tests.EditTests.ToolbarRuleTests;

[TestClass]
public class ToolbarRuleBasicTests
{
    [TestMethod]
    public void VerbIsInCommandAndToString()
        => AreEqual("edit", new ToolbarRuleForTest("edit").ToString());

    [TestMethod]
    public void VerbWithoutOperationToString()
        => AreEqual("edit", new ToolbarRuleForTest("edit").ToString());

    [TestMethod]
    public void VerbWithOpPlusToString()
        => AreEqual("+edit", new ToolbarRuleForTest("edit", operation: AddOperation).ToString());

    [TestMethod]
    public void VerbWithOpMinusToString()
        => AreEqual("-edit", new ToolbarRuleForTest("edit", operation: RemoveOperation).ToString());

    [TestMethod]
    public void VerbWithOpModToString()
        => AreEqual("%edit", new ToolbarRuleForTest("edit", operation: ModifyOperation).ToString());

    [TestMethod]
    public void VerbWithOpSkipToString()
        => AreEqual("", new ToolbarRuleForTest("edit", operation: SkipInclude).ToString());

    [TestMethod]
    public void UiShouldBeAdded()
        => AreEqual("edit&test=abc", new ToolbarRuleForTest("edit", ui: "test=abc").ToString());

    [TestMethod]
    public void ParamsShouldBeAdded()
        => AreEqual("edit?test=param", new ToolbarRuleForTest("edit", parameters: "test=param").ToString());

    [TestMethod]
    public void ParamsAndUiShouldBeAdded()
        => AreEqual("edit&ui=abc?test=param", new ToolbarRuleForTest("edit", ui: "ui=abc", parameters: "test=param").ToString());

    [TestMethod]
    public void OpMinusWithMoreToStringShouldNotKeepParams()
        => AreEqual("-edit", new ToolbarRuleForTest("edit", ui: "test=test", operation: RemoveOperation).ToString());
}