namespace ToSic.Sxc.DataTests.DynJson;


public class WrapJsonMixed(DynAndTypedTestHelper helper)
{
    [Fact]
    public void ObjectWithStringProperty()
    {
        var test = helper.DynJsonAndOriginal( new { FirstName = "test" });
        Equal<string>("test", test.Dyn.FirstName);
    }

    [Fact]
    public void ArrayOfObjectsWithStringProperty()
    {
        var test = helper.DynJsonAndOriginal(new object[]
        {
            new { FirstName = "test" }, 
            new { FirstName = "fn2" }
        });
        Equal<string>("test", test.Dyn[0].FirstName);
    }

    [Fact]
    public void ObjectWithArrayPropertyOfObjectsWithStringProperty()
    {
        var test = helper.DynJsonAndOriginal(new
        {
            a = new object[]
            {
                new { FirstName = "test" }, 
                new { FirstName = "fn2" }
            }
        });
        Equal<string>("test", test.Dyn.a[0].FirstName);
    }

    [Fact]
    public void ObjectWithArrayPropertyWithObjectWithStringArrayProperty()
    {
        var test = helper.DynJsonAndOriginal(new
        {
            a = new object[]
            {
                new { p1 = "fn1", p2 = new [] { "test", "a2" } },
                new { p1 = "fn2", p2 = new [] { "b1", "b2" } },
            }
        });
        Equal<string>("test", test.Dyn.a[0].p2[0]);
    }
        
}