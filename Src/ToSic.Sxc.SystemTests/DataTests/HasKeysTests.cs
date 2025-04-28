using ToSic.Sxc.Data.Internal.Typed;

namespace ToSic.Sxc.DataTests;


public class HasKeysTests
{
    [Fact]
    public void IsFilledNull() 
        => Equal(false, HasKeysHelper.IsNotEmpty(null, blankIsEmpty: null));

    [Fact]
    public void IsEmptyNull() 
        => Equal(true, HasKeysHelper.IsEmpty(null, blankIsEmpty: null));

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
    public void IsFilled_StringsBlank_BlankIsDefault(string value, string testName = default) 
        => Equal(false, HasKeysHelper.IsNotEmpty(value, blankIsEmpty: null));//, testName ?? value + " blankIs: null");

    [Theory]
    [MemberData(nameof(BlankStrings))]
    public void IsFilled_StringsBlank_BlankFalse(string value, string testName = default) 
        => Equal(false, HasKeysHelper.IsNotEmpty(value, blankIsEmpty: false));//, testName ?? value + " blankIs: false");

    [Theory]
    [MemberData(nameof(BlankStrings))]
    public void IsFilled_StringsBlank_BlankTrue(string value, string testName = default) 
        => Equal(true, HasKeysHelper.IsNotEmpty(value, blankIsEmpty: true));//, testName ?? value + " blankIs: true");
        
    [Theory]
    [MemberData(nameof(BlankStrings))]
    public void IsEmpty_StringsBlank_BlankIsDefault(string value, string testName = default) 
        => Equal(true, HasKeysHelper.IsEmpty(value, blankIsEmpty: null));//, testName ?? value + " blankIs: null");

    [Theory]
    [MemberData(nameof(BlankStrings))]
    public void IsEmpty_StringsBlank_BlankFalse(string value, string testName = default) 
        => Equal(false, HasKeysHelper.IsEmpty(value, blankIsEmpty: false));//, testName ?? value + " blankIs: null");

    [Theory]
    [MemberData(nameof(BlankStrings))]
    public void IsEmpty_StringsBlank_BlankTrue(string value, string testName = default) 
        => Equal(true, HasKeysHelper.IsEmpty(value, blankIsEmpty: true));//, testName ?? value + " blankIs: null");


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
    public void IsFilled_SimpleData_BlankIsDefault(object value, string testName = default) 
        => Equal(true, HasKeysHelper.IsNotEmpty(value, blankIsEmpty: null));//, testName ?? value + " blankIs: null");

    [Theory]
    [MemberData(nameof(SimpleData))]
    public void IsFilled_SimpleData_BlankFalse(object value, string testName = default) 
        => Equal(true, HasKeysHelper.IsNotEmpty(value, blankIsEmpty: false));//, testName ?? value + " blankIs: false");

    [Theory]
    [MemberData(nameof(SimpleData))]
    public void IsFilled_SimpleData_BlankTrue(object value, string testName = default) 
        => Equal(true, HasKeysHelper.IsNotEmpty(value, blankIsEmpty: true));//, testName ?? value + " blankIs: true");
        

    [Fact]
    public void ContainsDataObject()
    {
        var value = new object();
        var testName = "object";
        Equal(true, HasKeysHelper.IsNotEmpty(value, blankIsEmpty: null));//, testName ?? value + " blankIs: null");
        Equal(true, HasKeysHelper.IsNotEmpty(value, blankIsEmpty: false));//, testName ?? value + " blankIs: false");
        Equal(true, HasKeysHelper.IsNotEmpty(value, blankIsEmpty: true));//, testName ?? value + " blankIs: true");
    }

    [Fact]
    public void ContainsDataListEmpty()
    {
        var value = new List<string>();
        var testName = "object";
        Equal(false, HasKeysHelper.IsNotEmpty(value, blankIsEmpty: null));//, testName ?? value + " blankIs: null");
        Equal(false, HasKeysHelper.IsNotEmpty(value, blankIsEmpty: false));//, testName ?? value + " blankIs: false");
        Equal(false, HasKeysHelper.IsNotEmpty(value, blankIsEmpty: true));//, testName ?? value + " blankIs: true");
    }

    [Fact]
    public void ContainsDataListNonEmpty()
    {
        var value = new List<string> { "hello" };
        var testName = "object";
        Equal(true, HasKeysHelper.IsNotEmpty(value, blankIsEmpty: null));//, testName ?? value + " blankIs: null");
        Equal(true, HasKeysHelper.IsNotEmpty(value, blankIsEmpty: false));//, testName ?? value + " blankIs: false");
        Equal(true, HasKeysHelper.IsNotEmpty(value, blankIsEmpty: true));//, testName ?? value + " blankIs: true");
    }
}