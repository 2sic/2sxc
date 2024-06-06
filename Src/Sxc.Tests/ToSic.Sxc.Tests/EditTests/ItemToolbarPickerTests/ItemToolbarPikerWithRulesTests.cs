using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using ToSic.Sxc.Edit.Toolbar;
using ToSic.Sxc.Tests.EditTests.ToolbarRuleTests;

namespace ToSic.Sxc.Tests.EditTests.ItemToolbarPickerTests;

[TestClass]
public class ItemToolbarPikerWithRulesTests
{
    [DataRow(new[] { "edit" }, "edit")]
    [DataRow(new[] { "edit", "add" }, "edit,add")]
    [TestMethod]
    public void BasicConvert(string[] expected, string namesCsv)
    {
        var list = namesCsv
            .Split(',')
            .Select(s => new ToolbarRuleForTest(s))
            .Cast<ToolbarRule>()
            .ToList();

        var converted = ItemToolbarPicker.ToolbarV10OrNull(list);
        CollectionAssert.AreEqual(expected, converted);
    }

    [DataRow(new[] { "edit" }, " edit")]
    [DataRow(new[] { "edit", "add" }, " edit, add")]
    [DataRow(new[] { "+edit", "%add" }, "+edit,%add")]
    [DataRow(new[] { "+edit" }, "+edit,^add", DisplayName = "skip add")]
    [DataRow(new[] { "+edit", "new" }, "+edit,^add, new", DisplayName = "skip add")]
    [TestMethod]
    public void BasicConvertWithOp(string[] expected, string namesCsv)
    {
        var list = namesCsv
            .Split(',')
            .Select(s =>
            {
                var op = s[0];
                s = s.Substring(1);
                return new ToolbarRuleForTest(s, operation: op);
            })
            .Cast<ToolbarRule>()
            .ToList();

        var converted = ItemToolbarPicker.ToolbarV10OrNull(list);
        CollectionAssert.AreEqual(expected, converted);
    }
}