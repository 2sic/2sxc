namespace ToSic.Sxc.Tests.PlumbingTests;


public class CleanParamTests
{
    [Theory]
    // Expected Nulls
    [InlineData(null, null, "null")]
    // Expected Floats from other number formats
    [InlineData(0d, 0d, "double zero")]
    [InlineData(0d, 0f, "float zero")]
    //[InlineData(1.1f, 1.1f, "float 1.1")] // edge case, conversion results in 1.1000000238418579 rounding errors
    [InlineData(1.1d, 1.1d, "double 1.1")]
    // Expected Doubles
    [InlineData(0d, 0, "int zero")]
    [InlineData(0d, "0", "string zero")]
    [InlineData(0d, "0.0", "string 0.0")]
    [InlineData(7d, 7, "int 7")]
    [InlineData(7d, "7", "string 7")]
    [InlineData(7.1d, "7.1", "string 7.1")]
    [InlineData(6.9d, "6.9", "string 6.9")]
    public void DoubleOrNull(object expected, object data, string message)
        => Equal(expected, ParseObject.DoubleOrNull(data));

    [Fact]
    public void DoubleOrNullEdgeCase()
        => Equal(1.1f, ParseObject.DoubleOrNull(1.1f));

    [Fact]
    public void FloatOrNullObject()
        => Equal(null, ParseObject.DoubleOrNull(new()));

    [Fact]
    public void FloatOrNullOld()
    {
        // Expected Nulls
        Null(ParseObject.DoubleOrNull(null));
        Null(ParseObject.DoubleOrNull(new()));

        // Expected Floats from other number formats
        Equal(0f, ParseObject.DoubleOrNull(0d));//, "double zero");
        Equal(0f, ParseObject.DoubleOrNull(0f));//, "float zero");
        Equal(1.1f, ParseObject.DoubleOrNull(1.1f));//, "float 1.1");
        Equal(1.1d, ParseObject.DoubleOrNull(1.1d));//, "double 1.1");

        // Expected Floats
        Equal(0f, ParseObject.DoubleOrNull(0));//, "int zero");
        Equal(0f, ParseObject.DoubleOrNull("0"));//, "string zero");
        Equal(0f, ParseObject.DoubleOrNull("0.0"));//, "string 0.0");
        Equal(7f, ParseObject.DoubleOrNull(7));//, "int 7");
        Equal(7f, ParseObject.DoubleOrNull("7"));//, "string 7");
        Equal(7.1d, ParseObject.DoubleOrNull("7.1"));//, "string 7.1");
        Equal(6.9d, ParseObject.DoubleOrNull("6.9"));//, "string 6.9");
    }

    [Theory]
    // Check non-calculations
    [InlineData(0d, 0)]
    [InlineData(0d, "0")]
    [InlineData(2d, "2")]
    [InlineData(null, "")]
    // Check calculations
    [InlineData(1d, "1/1")]
    [InlineData(1d, "1:1")]
    [InlineData(0.5, "1/2")]
    [InlineData(0.5, "1:2")]
    [InlineData(2d, "2/1")]
    [InlineData(2d, "2:1")]
    [InlineData(16d / 9, "16:9")]
    [InlineData(16d / 9, "16/9")]
    // Bad calculations
    [InlineData(null, "/1")]
    [InlineData(null, ":1")]
    [InlineData(null, "1:0")]
    [InlineData(null, "0:0")]
    public void DoubleOrNullWithCalculation(double? expected, object data)
        => Equal(expected, ParseObject.DoubleOrNullWithCalculation(data));


    [Theory]
    [InlineData(null, null, "null")]
    [InlineData(0, 0, "int zero")]
    [InlineData(0, 0f, "float zero")]
    [InlineData(0, 0d, "double zero")]
    [InlineData(0, "0", "string zero")]
    [InlineData(0, "0.0", "string 0.0")]
    [InlineData(7, 7, "int 7")]
    [InlineData(7, "7", "string 7")]
    [InlineData(7, "7.1", "string 7.1")]
    [InlineData(7, 7.1f, "float 7.1")]
    [InlineData(7, 7.1d, "double 7.1")]
    [InlineData(7, "6.9", "string 6.9")]
    [InlineData(7, 6.9f, "float 6.9")]
    [InlineData(7, 6.9d, "double 6.9")]
    public void IntOrNull(int? expected, object data, string message)
        => Equal(expected, ParseObject.IntOrNull(data));

    [Fact]
    public void IntOrNullObject()
        => Equal(null, ParseObject.IntOrNull(new()));



    [Theory]
    [InlineData(null, null, "null")]
    [InlineData(null, 0, "int zero")]
    [InlineData(null, "0", "string zero")]
    [InlineData(null, "0.0", "string 0.0")]
    [InlineData(null, 0f, "float zero")]
    [InlineData(null, 0d, "double zero")]
    [InlineData(7, 7, "int 7")]
    [InlineData(7, "7", "string 7")]
    [InlineData(7, "7.1", "string 7.1")]
    [InlineData(7, "6.9", "string 6.9")]
    public void IntOrZeroNull(int? expected, object data, string message)
        => Equal(expected, ParseObject.IntOrZeroAsNull(data));

    [Fact]
    public void IntOrZeroNullObject()
        => Equal(null, ParseObject.IntOrZeroAsNull(new()));


    [Theory]
    [InlineData(null, null, "null")]
    [InlineData("0", 0, "int zero")]
    [InlineData("0", "0", "string zero")]
    [InlineData("0.0", "0.0", "string 0.0")]
    [InlineData("0", 0f, "float zero")]
    [InlineData("0", 0d, "double zero")]
    [InlineData("7", 7, "int 7")]
    [InlineData("7", "7", "string 7")]
    [InlineData("7.1", "7.1", "string 7.1")]
    [InlineData("6.9", "6.9", "string 6.9")]
    public void RealStringOrNull(string expected, object data, string message)
        => Equal(expected, ParseObject.RealStringOrNull(data));

    [Fact]
    public void RealStringOrNullWithObject()
        => Equal(null, ParseObject.RealStringOrNull(new()));


    [Theory]
    [InlineData(true, 0)]
    [InlineData(true, 0.0001f)]
    [InlineData(true, -0.009f)]
    [InlineData(false, 0.2f)]
    [InlineData(false, 2f)]
    public void DoubleNearZero(bool expected, double data)
        => Equal(expected, ParseObject.DNearZero(data));
}