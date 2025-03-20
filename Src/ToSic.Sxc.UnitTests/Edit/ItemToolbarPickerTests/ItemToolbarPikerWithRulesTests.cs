using ToSic.Sxc.Edit.Toolbar;
using ToSic.Sxc.Tests.EditTests.ToolbarRuleTests;

namespace ToSic.Sxc.Tests.EditTests.ItemToolbarPickerTests;


public class ItemToolbarPikerWithRulesTests
{
    [Theory]
    [InlineData(new[] { "edit" }, "edit")]
    [InlineData(new[] { "edit", "add" }, "edit,add")]
    public void BasicConvert(string[] expected, string namesCsv)
    {
        var list = namesCsv
            .Split(',')
            .Select(s => new ToolbarRuleForTest(s))
            .Cast<ToolbarRule>()
            .ToList();

        var converted = ItemToolbarPicker.ToolbarV10OrNull(list);
        Assert.Equal(expected, converted);
    }

    [Theory]
    [InlineData(new[] { "edit" }, " edit")]
    [InlineData(new[] { "edit", "add" }, " edit, add")]
    [InlineData(new[] { "+edit", "%add" }, "+edit,%add")]
    [InlineData(new[] { "+edit" }, "+edit,^add", /* note*/ "skip add")]
    [InlineData(new[] { "+edit", "new" }, "+edit,^add, new", /* note */ "skip add")]
    public void BasicConvertWithOp(string[] expected, string namesCsv, string? note = null)
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
        Assert.Equal(expected, converted);
    }
}