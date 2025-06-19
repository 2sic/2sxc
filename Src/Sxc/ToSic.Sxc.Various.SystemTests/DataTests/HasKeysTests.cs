using ToSic.Sxc.Data.Sys.Typed;

#pragma warning disable xUnit1026

namespace ToSic.Sxc.DataTests;


public class HasKeysTests
{
    [Fact]
    public void IsFilledNull() 
        => False(HasKeysHelper.IsNotEmpty(null, blankIsEmpty: null));

    [Fact]
    public void IsEmptyNull() 
        => True(HasKeysHelper.IsEmpty(null, blankIsEmpty: null));

    public static IEnumerable<object[]> BlankStrings =>
    [
        [""],
        [" "],
        ["   "],
        ["\t", "tab"],
        ["\t \t", "tabs"],
        ["\n \r", "new lines"],
        ["\u00A0", "non-breaking space"],
        ["&nbsp;", "non-breaking space HTML"],
        [" &nbsp; \n", "non-breaking space HTML"]
    ];

    [Theory]
    [MemberData(nameof(BlankStrings))]
    public void IsFilled_StringsBlank_BlankIsDefault(string value, string? testName = default) 
        => False(HasKeysHelper.IsNotEmpty(value, blankIsEmpty: null));//, testName ?? value + " blankIs: null");

    [Theory]
    [MemberData(nameof(BlankStrings))]
    public void IsFilled_StringsBlank_BlankFalse(string value, string? testName = default) 
        => False(HasKeysHelper.IsNotEmpty(value, blankIsEmpty: false));//, testName ?? value + " blankIs: false");

    [Theory]
    [MemberData(nameof(BlankStrings))]
    public void IsFilled_StringsBlank_BlankTrue(string value, string? testName = default) 
        => True(HasKeysHelper.IsNotEmpty(value, blankIsEmpty: true));//, testName ?? value + " blankIs: true");
        
    [Theory]
    [MemberData(nameof(BlankStrings))]
    public void IsEmpty_StringsBlank_BlankIsDefault(string value, string? testName = default) 
        => True(HasKeysHelper.IsEmpty(value, blankIsEmpty: null));//, testName ?? value + " blankIs: null");

    [Theory]
    [MemberData(nameof(BlankStrings))]
    public void IsEmpty_StringsBlank_BlankFalse(string value, string? testName = default) 
        => False(HasKeysHelper.IsEmpty(value, blankIsEmpty: false));//, testName ?? value + " blankIs: null");

    [Theory]
    [MemberData(nameof(BlankStrings))]
    public void IsEmpty_StringsBlank_BlankTrue(string value, string? testName = default) 
        => True(HasKeysHelper.IsEmpty(value, blankIsEmpty: true));//, testName ?? value + " blankIs: null");


    public static IEnumerable<object[]> SimpleData =>
    [
        [0],
        [-1],
        [27],
        [true],
        [false],
        ["hello"],
        ['x', "Character"]
    ];

    [Theory]
    [MemberData(nameof(SimpleData))]
    public void IsFilled_SimpleData_BlankIsDefault(object value, string? testName = default) 
        => True(HasKeysHelper.IsNotEmpty(value, blankIsEmpty: null));//, testName ?? value + " blankIs: null");

    [Theory]
    [MemberData(nameof(SimpleData))]
    public void IsFilled_SimpleData_BlankFalse(object value, string? testName = default) 
        => True(HasKeysHelper.IsNotEmpty(value, blankIsEmpty: false));//, testName ?? value + " blankIs: false");

    [Theory]
    [MemberData(nameof(SimpleData))]
    public void IsFilled_SimpleData_BlankTrue(object value, string? testName = default) 
        => True(HasKeysHelper.IsNotEmpty(value, blankIsEmpty: true));//, testName ?? value + " blankIs: true");
        

    [Fact]
    public void ContainsDataObject()
    {
        var value = new object();
        True(HasKeysHelper.IsNotEmpty(value, blankIsEmpty: null));//, testName ?? value + " blankIs: null");
        True(HasKeysHelper.IsNotEmpty(value, blankIsEmpty: false));//, testName ?? value + " blankIs: false");
        True(HasKeysHelper.IsNotEmpty(value, blankIsEmpty: true));//, testName ?? value + " blankIs: true");
    }

    [Fact]
    public void ContainsDataListEmpty()
    {
        var value = new List<string>();
        False(HasKeysHelper.IsNotEmpty(value, blankIsEmpty: null));//, testName ?? value + " blankIs: null");
        False(HasKeysHelper.IsNotEmpty(value, blankIsEmpty: false));//, testName ?? value + " blankIs: false");
        False(HasKeysHelper.IsNotEmpty(value, blankIsEmpty: true));//, testName ?? value + " blankIs: true");
    }

    [Fact]
    public void ContainsDataListNonEmpty()
    {
        var value = new List<string> { "hello" };
        var testName = "object";
        True(HasKeysHelper.IsNotEmpty(value, blankIsEmpty: null));//, testName ?? value + " blankIs: null");
        True(HasKeysHelper.IsNotEmpty(value, blankIsEmpty: false));//, testName ?? value + " blankIs: false");
        True(HasKeysHelper.IsNotEmpty(value, blankIsEmpty: true));//, testName ?? value + " blankIs: true");
    }
}