using ToSic.Sxc.Data;
using ToSic.Sxc.Data.Internal.Typed;

namespace ToSic.Sxc.DataTests.DynJson;


public class WrapJsonDeep(DynAndTypedTestHelper helper)
{
    private static object DeepData = new
    {
        TopLevelNumber = 27,
        ObjectProperty = new
        {
            StringProperty = "test",
            ObjectProperty = new
            {
                NumberProperty = 1,
                ObjectProperty = new
                {
                    BoolProperty = true
                }
            }
        },
        Array = new object[]
        {
            new { name = "zeroth" },
            new { Name = "first" },
            new { Name = "second" }
        }
    };

    private ITyped DataTyped => _dataTyped ??= helper.Obj2Json2TypedStrict(DeepData);
    private ITyped _dataTyped;

    [Fact]
    public void DeepValueAndCasingWithDyn()
    {
        var dyn = helper.Obj2Json2Dyn(DeepData);
        Equal<int>(27, dyn.TopLevelNumber);
        Equal<int>(27, dyn.TopLevelNUMBER);
        Equal<string>("test", dyn.OBJECTPROPERTY.stringproperty);
        Equal<int>(1, dyn.ObjectProperty.ObjectProperty.NumberProperty);
        Equal<bool>(true, dyn.ObjectProperty.ObjectProperty.ObjectProperty.BoolProperty);
    }

    [Fact]
    public void DeepValueAndCasingWithTyped()
    {
        Equal(27, DataTyped.Get("TopLevelNumber"));
        Equal<int>(27, DataTyped.Int("TopLevelNumber"));
        Equal(27, DataTyped.Get("TopLevelNUMBER"));
        Equal<int>(27, DataTyped.Int("TopLevelNUMBER"));
        Equal("test", (DataTyped.Get("OBJECTPROPERTY") as ITyped).Get("stringproperty"));
        Equal(1, DataTyped.Get("ObjectProperty.ObjectProperty.NumberProperty"));
        Equal<int>(1, DataTyped.Int("ObjectProperty.ObjectProperty.NumberProperty"));
        Equal<bool>(true, DataTyped.Bool("ObjectProperty.ObjectProperty.ObjectProperty.BoolProperty"));
    }

    [Fact]
    public void DeepObjectIsNotList()
    {
        var dyn = helper.Obj2Json2Dyn(DeepData);
        False(dyn.IsList);
    }

    [Fact]
    public void DeepTypeCheckDyn()
    {
        var dyn = helper.Obj2Json2Dyn(DeepData);
        var expectedType = typeof(DynamicJacket);
        IsType(expectedType, dyn.ObjectProperty);
        IsType(expectedType, dyn.ObjectProperty.ObjectProperty);
        IsType(expectedType, dyn.ObjectProperty.ObjectProperty.ObjectPROPERTY);
        Null(dyn.ObjectProperty.ObjectProperty.ObjectIncorrect);
    }

    [Fact]
    public void DeepTypeCheckTyped()
    {
        //var dyn = Obj2Json2Typed(DeepData);
        var expectedType = typeof(WrapObjectTyped);
        IsType(expectedType, DataTyped.Get("ObjectProperty"));
        Null(DataTyped.Get("ObjectPropertyNonExisting", required: false));
        IsType(expectedType, (DataTyped.Get("ObjectProperty") as ITyped).Get("ObjectProperty"));
        IsType(expectedType, DataTyped.Get("ObjectProperty.ObjectProperty"));
        //IsType(dyn.ObjectProperty.ObjectProperty.ObjectPROPERTY, expectedType);
        // TODO!
        //Null(dyn.ObjectProperty.ObjectProperty.ObjectIncorrect);
    }

    [Fact]
    public void UsePropertyIndexersAndCasing()
    {
        var dyn = helper.Obj2Json2Dyn(DeepData);
        Equal<string>("test", dyn["objectproperty"]["STRINGPROPERTY"]);
        // 2023-08-17 2dm disabled the two-property indexer
        //Equal<string>("test", test.Dyn["ObjectProperty", true]["StringProperty", true]);
    }

    private const string ReSerializeExpected = @"{
  ""TopLevelNumber"": 27,
  ""ObjectProperty"": {
    ""StringProperty"": ""test"",
    ""ObjectProperty"": {
      ""NumberProperty"": 1,
      ""ObjectProperty"": {
        ""BoolProperty"": true
      }
    }
  },
  ""Array"": [
    {
      ""name"": ""zeroth""
    },
    {
      ""Name"": ""first""
    },
    {
      ""Name"": ""second""
    }
  ]
}";

    private const string ReSerializeObjectProperty = @"{
  ""StringProperty"": ""test"",
  ""ObjectProperty"": {
    ""NumberProperty"": 1,
    ""ObjectProperty"": {
      ""BoolProperty"": true
    }
  }
}";

    [Fact]
    public void ToStringAll_Dyn()
    {
        var dyn = helper.Obj2Json2Dyn(DeepData);
        Equal(ReSerializeExpected, dyn.ToString());
    }
    [Fact]
    public void ToStringAll_Typed()
    {
        var dyn = helper.Obj2Json2TypedStrict(DeepData);
        Equal(ReSerializeExpected, dyn.ToString());
    }

    [Fact]
    public void ToStringTopLevelNumber_Dyn()
    {
        var dyn = helper.Obj2Json2Dyn(DeepData);
        Equal("27", dyn.TopLevelNumber.ToString());
    }
    [Fact]
    public void ToStringTopLevelNumber_Typed()
    {
        var dyn = helper.Obj2Json2TypedStrict(DeepData);
        Equal("27", dyn.Get("TopLevelNumber").ToString());
    }

    [Fact]
    public void ToStringObjectProperty_Dyn()
    {
        var dyn = helper.Obj2Json2Dyn(DeepData);
        Equal(ReSerializeObjectProperty, dyn.ObjectProperty.ToString());
    }
    [Fact]
    public void ToStringObjectProperty_Typed()
    {
        var dyn = helper.Obj2Json2TypedStrict(DeepData);
        Equal(ReSerializeObjectProperty, dyn.Get("ObjectProperty").ToString());
    }
}