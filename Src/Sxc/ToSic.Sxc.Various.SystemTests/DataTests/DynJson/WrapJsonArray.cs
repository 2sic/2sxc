using System.Collections;
using System.Text.Json.Nodes;
using ToSic.Sxc.Data.Sys.DynamicJacket;
using ToSic.Sxc.Data.Sys.Wrappers;

namespace ToSic.Sxc.DataTests.DynJson;


public class WrapJsonArray(DynAndTypedTestHelper helper)
{
    private (dynamic Dyn, string Json, string[] Original) StringArrayPrepare() => helper.DynJsonAndOriginal(new[]
    {
        "val1",
        "val2",
        "val3"
    });

    [Fact]
    public void StringArray_Dyn()
    {
        var (dyn, _, original) = StringArrayPrepare();
        True(dyn.IsList);
        Equal(original[0], dyn[0]);
        Equal(original[1], dyn[1]);
        Equal(original[2], dyn[2]);
        NotEqual(original[0], original[1]);
    }

    [Fact]
    public void StringArrayCount_Dyn()
    {
        var (dyn, _, original) = StringArrayPrepare();
        Equal(original.Length, dyn.Count);
    }

    [Fact]
    public void StringArrayIterate_Dyn()
    {
        var (dyn, _, _) = StringArrayPrepare();
        foreach (var strItem in dyn) NotNull(strItem);
    }

    [Fact]
    public void StringArrayNonExistingProperty_Dyn()
    {
        var test = StringArrayPrepare();
        Null(test.Dyn.NonExistingProperty);
    }

    [Fact]
    public void Test()
    {
        var anon = new[] { "test", "test2", "test3" };
        var json = helper.JsonSerialize(anon);

        var jsonNode = JsonNode.Parse(json);
        var isArray = jsonNode is JsonArray;
        True(isArray);
        NotNull(jsonNode.AsArray());
    }

    public static IEnumerable<object[]> DetectJsonType => new List<object[]>
    {
        new object[] { true, false, new { something = "hello" } },
        new object[] { true, false, new { } },
        new object[] { true, true, new[] { "hello", "there" } },
        new object[] { true, true, new[] { 1, 2, 3 } },
        new object[] { false, false, "just a string" },
        new object[] { false, false, 27 },
    };

    [Theory]
    [MemberData(nameof(DetectJsonType))]
    public void DetectJsonComplexOfObject(bool expComplex, bool expArray, object value, string? testName = default)
    {
        var json = helper.JsonSerialize(value);
        var (isComplex, isArray) = JsonProcessingHelpers.AnalyzeJson(json);
        Equal(expComplex, isComplex);//, testName + ":" + value + "; json: " + json);
        Equal(expArray, isArray);//, testName + ":" + value + "; json: " + json);
    }

    [Fact]
    //[ExpectedException(typeof(ArgumentException))]
    public void WrapArrayToStrictShouldFail() =>
        Throws<ArgumentException>(() => helper.Obj2Json2TypedStrict(new[] { 1, 2, 3 }));

    [Fact]
    //[ExpectedException(typeof(ArgumentException))]
    public void WrapArrayValuesToStrictListShouldFail() =>
        Throws<ArgumentException>(() => helper.Obj2Json2TypedListStrict(new[] { 1, 2, 3 }));

    [Fact]
    public void WrapArrayToStrictListShouldBeOk()
    {
        var list = helper.Obj2Json2TypedListStrict(new object[] { new { a = 7 }, new { b = 27 } });
        NotNull(list);
        Equal(2, list.Count());
    }

    [Fact]
    public void ArrayPropertyShouldWork()
    {
        var anon = new
        {
            items = new[] { 14, 27, 33 }
        };
        var typed = helper.Obj2Json2TypedStrict(anon);
        NotNull(typed);
        var items = typed.Get(nameof(anon.items));
        NotNull(items);
        var itemList = items as IEnumerable;
        NotNull(itemList);
        Equal(3, itemList.Cast<object>().Count());
        Equal(14, itemList.Cast<object>().FirstOrDefault());
    }

    [Fact]
    public void TestJsonValueBehavior()
    {
        var anon = new { x = 27, y = 203.02003050 };
        var json = JsonNode.Parse(helper.JsonSerialize(anon));
        IsType<JsonObject>(json);
        var xPropExists = json.AsObject().TryGetPropertyValue(nameof(anon.x), out var xProp);
        True(xPropExists);

        var asVal = xProp.AsValue();
        IsAssignableFrom<JsonValue>(asVal);
        var data = asVal.GetValue<int>();
        IsType<int>(data);

        var maybeInt = JsonProcessingHelpers.JsonValueGetContents(asVal);
        IsType<int>(maybeInt);
        Equal(27, maybeInt);

        var yPropExists = json.AsObject().TryGetPropertyValue(nameof(anon.y), out var yProp);
        asVal = yProp.AsValue();
        var maybeDouble = JsonProcessingHelpers.JsonValueGetContents(asVal);
        IsType<double>(maybeDouble);
        IsNotType<int>(maybeDouble);
        Equal(203.02003050, maybeDouble);
    }


    private (dynamic Dyn, string Json, string[][] Original) StringArray2dPrepare() => helper.DynJsonAndOriginal(new[]
    {
        new[] {"0-0", "0-1", "0-2"},
        new[] {"1-0", "1-1"},
        new[] {"2-0", "2-1", "2-2", "2-3"},
    });

    [Fact]
    public void StringArray2D()
    {
        var test = StringArray2dPrepare();
        Equal(test.Original[0][0], test.Dyn[0][0]);
        Equal(test.Original[2][3], test.Dyn[2][3]);
    }
    [Fact]
    public void StringArray2DNonExistingProperty()
    {
        var (dyn, _, _) = StringArray2dPrepare();
        Null(dyn.NonExisting);
        Null(dyn[0].NonExisting);
    }

    [Fact]
    public void StringArray2DCount()
    {
        var test = StringArray2dPrepare();
        Equal(test.Original[0].Length, test.Dyn[0].Count);
    }

    private class MiniObj
    {
        public string Name;
        public int Age;
    }

    private (dynamic Dyn, string Json, MiniObj[] Original) ObjectArrayPrepare() => helper.DynJsonAndOriginal(new[]
    {
        new MiniObj { Name = "T1", Age = 11 },
        new MiniObj { Name = "t2", Age = 22 },
        new MiniObj { Name = "TTT3", Age = 3 }
    });

    [Fact]
    public void ObjectArray()
    {
        var test = ObjectArrayPrepare();
        Equal(test.Original[0].Name, (string)test.Dyn[0].Name);
        NotEqual<object>(test.Original[0].Name, test.Dyn[0].Age);
    }

    [Fact]
    public void ObjectArrayNonExistingProperties()
    {
        var test = ObjectArrayPrepare();
        Null(test.Dyn.Something);
        Null(test.Dyn[0].NonExisting);
    }


    [Fact]
    public void MixedArrays2D()
    {
        var (dyn, json, original) = helper.DynJsonAndOriginal(new object[]
        {
            new string[] {"a1"},
            new object[] { 1, "b2"},
            new object[] { true, 2, "c3"}
        });

        var expectedType = typeof(DynamicJacketList);

        True(dyn.IsList);

        IsType(expectedType, dyn[2]);

        Equal<string>("a1", dyn[0][0]);
        Equal<int>(1, dyn[1][0]);
        True(dyn[2][0]);
    }

    [Fact(Skip = "2023-08-23 2dm - This test can't work - json Arrays don't support keys; may need to re-write the test or delete")]
    public void Keys()
    {
        var anon = new[]
        {
            "hello",
            "goodbye"
        };
        var typed = helper.Obj2Json2TypedStrict(anon);
        True(typed.TestContainsKey("1"));
        False(typed.TestContainsKey("3"));
        True(typed.TestKeys().Any());
        Equal(2, typed.TestKeys().Count());
        Equal(1, typed.TestKeys(only: ["1"]).Count());
        Equal(0, typed.TestKeys(only: ["3"]).Count());
    }

}