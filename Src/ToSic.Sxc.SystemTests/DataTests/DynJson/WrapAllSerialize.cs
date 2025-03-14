using ToSic.Sxc.DataTests.DynWrappers;
using static System.Text.Json.JsonSerializer;

namespace ToSic.Sxc.DataTests.DynJson;


public class WrapAllSerialize(DynAndTypedTestHelper helper)
{
    [Fact]
    public void AnonToObjSerialize()
    {
        var data = WrapObjBasic.Data;
        var anon = new { data.Name, data.Description, data.Founded, data.Birthday, data.Truthy };
        var typed = helper.Obj2Typed(anon);
        dynamic dynAnon = helper.Obj2WrapObj(anon, false, false);

        var jsonTyped = Serialize(typed);
        var jsonAnon = Serialize(anon);
        var jsonDyn = Serialize(dynAnon);

        Equal(jsonTyped, jsonDyn);
        Equal(jsonTyped, jsonAnon);
    }


    [Fact]
    public void AnonToJsonToWrapperSerialize()
    {
        var data = WrapObjBasic.Data;
        var anon = new
        {
            data.Name,
            data.Description,
            data.Founded,
            // Birthday not used in this test, because the initial default serializer
            // will not use "Z" time, so it will be a bit different.
            //data.Birthday, 
            data.Truthy,
            subData = new { }, // empty sub object
            subFilled = new
            {
                something = "iJungleboy",
                number = 7,
            }
        };
        var jsonAnon = Serialize(anon);


        var typed = helper.Obj2Json2TypedStrict(anon);
        var dynAnon = helper.Obj2Json2Dyn(anon);

        var jsonTyped = Serialize(typed);
        var jsonDyn = Serialize(dynAnon);

        Equal(jsonAnon, jsonTyped);
        Equal(jsonAnon, jsonDyn);
    }
}