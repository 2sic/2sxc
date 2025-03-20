using ToSic.Sxc.Data.Internal.Typed;

namespace ToSic.Sxc.DataTests.DynWrappers;


public class WrapObjTypedItemEquality(DynAndTypedTestHelper helper)
{
    private static object CreateAnon() => new
    {
        Simple = "simple",
        Sub = new
        {
            SubSub = "Test"
        }
    };

    [Fact]
    public void DiffWrapTwiceShouldBeDiff_Typed()
        => NotEqual(helper.Obj2Typed(CreateAnon()), helper.Obj2Typed(CreateAnon()));

    [Fact]
    public void DiffWrapTwiceShouldBeDiff_Item()
        => NotEqual(helper.Obj2Item(CreateAnon()), helper.Obj2Item(CreateAnon()));

    [Fact]
    public void SameWrapTwiceShouldBeEqual_Typed()
    {
        var anon = CreateAnon();
        Equal(helper.Obj2Typed(anon), helper.Obj2Typed(anon));
    }

    [Fact(Skip="This can't work, because we can't overload == on interfaces - pls preserve this test, so it's documented")]
    public void SameWrapTwiceShouldBeEqual_TypedUsingEqEq()
    {
        var anon = CreateAnon();
        var a = helper.Obj2Typed(anon);
        var b = helper.Obj2Typed(anon);
        var c = helper.Obj2Typed(anon) as WrapObjectTyped;
        // ReSharper disable EqualExpressionComparison
        // ReSharper disable PossibleUnintendedReferenceComparison
        var x = c == c;
        True(a == a, "Test basic comparison");
        True(a as WrapObjectTyped == b, "a as ... == b should work");
        True(a == b, "a == b should work");
        // ReSharper restore EqualExpressionComparison
        // ReSharper restore PossibleUnintendedReferenceComparison
    }

    [Fact]
    public void SameWrapTwiceShouldBeEqual_Item()
    {
        var anon = CreateAnon();
        Equal(helper.Obj2Item(anon), helper.Obj2Item(anon));
    }
}