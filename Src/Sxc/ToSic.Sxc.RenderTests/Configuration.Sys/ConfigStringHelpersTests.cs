namespace ToSic.Sxc.Configuration.Sys;

public class ConfigStringHelpersTests
{
    [Theory]
    //[InlineData("", "")]
    [InlineData("value", "value")]
    //[InlineData("value//comment", "value")]
    [InlineData("value //comment", "value")]
    [InlineData("value  //comment", "value")]
    [InlineData("value,another", "value,another")]
    [InlineData("value\nanother", "value|another")] // need bar to separate, because comma could be used
    public void BasicSingleLine(string input, string expected)
    {
        var result = ConfigStringHelpers.ConfigLinesWithoutComments(input);
        //Single(result);
        Equal(expected.Split('|'), result);
    }

    [Theory]
    [InlineData("value\nanother", "value,another")]
    [InlineData("value\n\nanother", "value,another")]
    [InlineData("\nvalue\nanother", "value,another")]
    [InlineData("\nvalue\nanother\n", "value,another")]
    [InlineData("\n\n\nvalue\nanother\n\n", "value,another")]
    [InlineData("value // comment\nanother", "value,another")]
    [InlineData("value // comment\nanother // comment2", "value,another")]
    [InlineData("\nvalue // comment\nanother // comment2\n", "value,another")]
    public void BasicMultiLine(string input, string expected)
    {
        var result = ConfigStringHelpers.ConfigLinesWithoutComments(input);
        Equal(2, result.Count);
        Equal(expected.Split(','), result);
    }

    [Theory]
    [InlineData("key=value", "value")]
    [InlineData("key=value // comment", "value")]
    [InlineData("key=value,value2 // comment", "value,value2")]
    [InlineData("key=value // comment\nanother", "value")]
    public void Values(string input, string expected)
    {
        var result = ConfigStringHelpers.ConfigPairs(input);
        Equal(expected, result[0].Values);
    }
}