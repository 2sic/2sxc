using ToSic.Sxc.Data;
using ToSic.Sxc.Data.Sys.Wrappers;

namespace ToSic.Sxc.DataTests.DynWrappers;


public class WrapObjBasic(DynAndTypedTestHelper helper)
{
    public class TestData
    {
        public string Name => "2sic";
        public string Description => "Some description";
        /// <summary> This one is not a real property but just a value! </summary>
        public string DescriptionAsProperty = "Some description";
        public int Founded => 2012;
        public DateTime Birthday { get; } = new(2012, 5, 4);
        public bool Truthy => true;
    }
    public static TestData Data = new();

    [Fact]
    public void SourceAnonymous()
    {
        var anon = new { Data.Name, Data.Description, Data.Founded, Data.Birthday, Data.Truthy };
        var typed = helper.Obj2Typed(anon);
        dynamic dynAnon = helper.Obj2WrapObj(anon, false, false);

        Null(dynAnon.NotExisting);
        Equal(anon.Name, dynAnon.Name);
        Equal(anon.Name, dynAnon.naME);//, "Should be the same irrelevant of case");
        Equal(anon.Birthday, dynAnon.Birthday);//, "dates should be the same");
        Equal(anon.Truthy, dynAnon.truthy);

        True(typed.TestContainsKey("Name"));
        True(typed.TestContainsKey("NAME"));
        True(typed.TestContainsKey("Description"));
        False(typed.TestContainsKey("NonexistingField"));
    }

    [Fact]
    public void SourceTyped()
    {
        var data = new TestData();
        var typed = helper.Obj2Typed(data);
        dynamic dynAnon = helper.Obj2WrapObj(data, false, false);

        Null(dynAnon.NotExisting);
        Equal(data.Name, (string) dynAnon.Name);
        Equal(data.Name, (string) dynAnon.naME);//, "Should be the same irrelevant of case");

        // This line is different from the anonymous test
        NotEqual(data.DescriptionAsProperty, (object) dynAnon.DescriptionAsProperty);//, "Should NOT work for values, only properties");
        Equal(null, (object) dynAnon.DescriptionAsProperty);//, "Should NOT work for values, only properties");
        Equal(data.Birthday, (DateTime) dynAnon.Birthday);//, "dates should be the same");
        Equal(data.Truthy, (bool) dynAnon.truthy);

        True(typed.TestContainsKey("Name"));
        True(typed.TestContainsKey("NAME"));
        True(typed.TestContainsKey("Description"));
        False(typed.TestContainsKey("NonexistingField"));
    }

    [Fact]
    public void RequiredDynDefaultExisting() => True(helper.Obj2WrapObjAsDyn(Data).Truthy);

    [Fact]
    public void RequiredDynDefaultNonExisting() => Null(helper.Obj2WrapObjAsDyn(Data).TruthyXyz);

    [Fact]
    public void RequiredTypedDefaultExisting() => True(helper.Obj2Typed(Data).Bool(nameof(TestData.Truthy)));

    [Fact]
    //[ExpectedException(typeof(ArgumentException))]
    public void RequiredTypedDefaultNonExisting() =>
        Throws<ArgumentException>(() => helper.Obj2Typed(Data).Bool("FakeName"));

    [Fact]
    public void RequiredTypedDefaultNonExistingWithParam() =>
        False(helper.Obj2Typed(Data).Bool("FakeName", required: false));

    [Fact]
    public void RequiredTypedDefaultNonExistingWithParamFallback() =>
        True(helper.Obj2Typed(Data).Bool("FakeName", fallback: true, required: false));

    [Fact]
    public void RequiredTypedNonStrictNonExistingWithParam() =>
        False(helper.Obj2Typed(Data, WrapperSettings.Typed(true, true, false)).Bool("FakeName"));






    [Fact]
    public void Keys()
    {
        var anon = new
        {
            Key1 = "hello",
            Key2 = "goodbye",
            Deep = new 
            {
                Sub1 = "hello",
                Sub2 = "hello",
                Deeper = new
                {
                    SubSub1 = "hello"
                }
            }
        };
        var typed = helper.Obj2Typed(anon);
        True(typed.TestContainsKey("Key1"));
        False(typed.TestContainsKey("Nonexisting"));
        True(typed.TestKeys().Any());
        Equal(3, typed.TestKeys().Count());
        Equal(1, typed.TestKeys(only: ["Key1"]).Count());
        Equal(0, typed.TestKeys(only: ["Nonexisting"]).Count());
    }

    [Fact] public void DeepParent() => True(DataForDeepKeys.TestContainsKey("Deep"));

    [Fact] public void DeepSub1() => True(DataForDeepKeys.TestContainsKey("Deep.Sub1"));
    [Fact] public void DeepDeeper() => True(DataForDeepKeys.TestContainsKey("Deep.Deeper"));
    [Fact] public void DeepDeeperSub() => True(DataForDeepKeys.TestContainsKey("Deep.Deeper.SubSub1"));
    [Fact] public void DeepHasArray() => True(DataForDeepKeys.TestContainsKey("List"));

    // Note: Arrays are not supported
    //True(typed.TestContainsKey("List.L1"));
    //True(typed.TestContainsKey("List.L2"));

    private ITyped DataForDeepKeys
    {
        get
        {
            var anon = new
            {
                Key1 = "hello",
                Key2 = "goodbye",
                Deep = new
                {
                    Sub1 = "hello",
                    Sub2 = "hello",
                    Deeper = new
                    {
                        SubSub1 = "hello"
                    }
                },
                List = new object[]
                {
                    new
                    {
                        L1 = "hello",
                    },
                    new
                    {
                        L2 = "hello",
                    }
                }
            };
            var typed = helper.Obj2Typed(anon);
            return typed;
        }
    }
}