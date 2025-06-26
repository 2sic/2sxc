using ToSic.Sxc.Data;
using ToSic.Sxc.Data.Sys.Wrappers;

namespace ToSic.Sxc.DataTests.DynWrappers;

[Startup(typeof(StartupSxcCoreOnly))]
public class WrapDicBasic(ICodeDataPoCoWrapperService wrapper)
{
    private /*DynamicFromDictionary<TKey, TValue>*/ object ToDyn<TKey, TValue>(Dictionary<TKey, TValue> dic)
        => wrapper.FromDictionary(dic ?? []);

    [Fact]
    public void BasicUseDictionary()
    {
        var dic = new Dictionary<string, object>
        {
            ["Name"] = "2sxc",
            ["Description"] = "",
            ["Founded"] = 2012,
            ["Birthday"] = new DateTime(2012, 5, 4),
            ["Truthy"] = true,
        };

        var typed = ToDyn(dic); // as ITyped;
        dynamic dynAnon = typed;

        Null(dynAnon.NotExisting);
        Equal(dic["Name"], dynAnon.Name);
        Equal(dic["Name"], dynAnon.naME);//, "Should be the same irrelevant of case");
        Equal(dic["Birthday"], dynAnon.Birthday);//, "dates should be the same");
        Equal(dic["Truthy"], dynAnon.truthy);

        // 2023-08-07 2dm - DynamicFromDictionary does not implement ITyped as of now

        //True(typed.Has("Name"));
        //True(typed.Has("NAME"));
        //True(typed.Has("Description"));
        //False(typed.Has("NonexistingField"));
    }
    [Fact]
    public void Keys()
    {
        var anon = new Dictionary<string, object>
        {
            ["Key1"] = "hello",
            ["Key2"] = "goodbye"
        };
        var typed = ToDyn(anon) as IHasKeys;
        True(typed.ContainsKey("Key1"));
        False(typed.ContainsKey("Nonexisting"));
        True(typed.Keys().Any());
        Equal(2, typed.Keys().Count());
        Equal(1, typed.Keys(only: ["Key1"]).Count());
        Equal(0, typed.Keys(only: ["Nonexisting"]).Count());
    }

}