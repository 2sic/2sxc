namespace ToSic.Sxc.Configuration.Sys;

public class ConfigStringHelpersTests
{
    [Theory]
    //[InlineData("", "")]
    [InlineData("value", "value")]
    [InlineData("value//comment", "value")]
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

    [Fact]
    public void BasicMultiLine()
    {
        var result = ConfigStringHelpers.ConfigLinesWithoutComments("value\nanother");
        Equal(2, result.Count);
        Equal("value", result[0]);
        Equal("another", result[1]);
    }
}
