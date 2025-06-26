using ToSic.Sxc.Data;

namespace ToSic.Sxc.DataTests.DynJson;


public class WrapJsonValues(DynAndTypedTestHelper helper)
{
    private dynamic BoolDataDynLoose => _dynBool ??= helper.Obj2Json2Dyn(WrapAllKeys.TestDataAnonObject);
    private static object? _dynBool;

    private ITyped BoolTestDataStrict => _boolTestDataStrict ??= helper.Obj2Json2TypedStrict(WrapAllKeys.TestDataAnonObject);
    private static ITyped? _boolTestDataStrict;
    private ITyped BoolTestDataLoose => _boolTestDataLoose ??= helper.Obj2Json2TypedLoose(WrapAllKeys.TestDataAnonObject);
    private static ITyped? _boolTestDataLoose;

    [Fact]
    public void JsonBool_Dyn()
    {
        True(BoolDataDynLoose.TrueBoolType);
        False(BoolDataDynLoose.FalseBoolType);
        Null(BoolDataDynLoose.something);
    }

    [Theory]
    [InlineData(true, "TrueBoolType")]
    [InlineData(false, "FalseBoolType")]
    [InlineData(true, "TrueString")]
    [InlineData(false, "FalseString")]
    [InlineData(true, "TrueNumber")]
    [InlineData(true, "TrueNumberBig")]
    [InlineData(false, "FalseNumber")]
    [InlineData(true, "TrueNumberNegative")]
    public void JsonBoolProperty_Typed(bool expected, string key) 
        => Equal(expected, BoolTestDataStrict.Bool(key));





    [Fact]
    public void JsonNumberProperty()
    {
        var (dyn, _, original) = helper.DynJsonAndOriginal(new 
        {
            IntType = 32,
            BigIntType = 9007199254740991,
            DoubleType = 0.333333333333333314829616256247390992939472198486328125,
        });
        Equal<int>(original.IntType, dyn.IntType);
        Equal<long>(original.BigIntType, dyn.BigIntType);
        Equal<double>(original.DoubleType,dyn.DoubleType);
    }

    [Fact]
    public void ExpectCountOfPropertiesOnNonArray()
    {
        var test = helper.DynJsonAndOriginal(new { Name = "Test" });
        Equal(1, test.Dyn.Count);

        var test2 = helper.DynJsonAndOriginal(new { Name = "Test", Age = 3, Birthday = new DateTime(2022, 1, 1) });
        Equal(3, test2.Dyn.Count);
    }

    [Fact]
    public void EnumerateProperties()
    {
        var test = helper.DynJsonAndOriginal(new { Name = "Test" });
        var testList = (test.Dyn as IEnumerable<object>).ToList();
        Equal(1, testList.Count);
        Equal("Name", testList.First().ToString());

        var test2 = helper.DynJsonAndOriginal(new { Name = "Test", Age = 3, Birthday = new DateTime(2022, 1, 1) });
        var testList2 = (test2.Dyn as IEnumerable<object>).ToList();
        Equal(3, testList2.Count);
        True(testList2.Contains("Name"));
        True(testList2.Contains("Birthday"));
    }


    [Fact]
    public void ObjectWithStringProperty()
    {
        var jsonString = @"{ 
                ""StringType"": ""stringValue"", 
                ""GuidType"": ""00000000-0000-0000-0000-000000000000"",
            }";
        Equal<string>("stringValue", helper.Json2Dyn(jsonString).StringType);
        Equal<string>("00000000-0000-0000-0000-000000000000", helper.Json2Dyn(jsonString).GuidType);
    }

    [Fact]
    public void ObjectWithDateTimeProperty()
    {
        var jsonString = @"{ 
                ""DateTimeType"": ""2022-11-09T23:00:00.000"", 
                ""ZuluTimeType"": ""2021-04-15T21:01:00.000Z"", 
            }";
        Equal<DateTime>(new DateTime(2022,11,9, 23,0,0), helper.Json2Dyn(jsonString).DateTimeType);
        Equal<DateTime>(new DateTime(2021, 4, 15, 21, 1, 0, DateTimeKind.Utc), helper.Json2Dyn(jsonString).ZuluTimeType);
    }

    [Fact]
    public void ObjectWithNullProperty()
    {
        var dynJson = helper.Json2Dyn(@"{ 
                ""NullType"": null, 
                ""UndefinedType"": null, // undefined would not be valid
            }");
        Null(dynJson.NullType);
        Null(dynJson.UndefinedType);
        Null(dynJson.NonExistingProperty);
    }
}