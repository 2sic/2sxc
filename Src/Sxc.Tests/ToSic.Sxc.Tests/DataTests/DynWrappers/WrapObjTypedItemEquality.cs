using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToSic.Sxc.Data.Internal.Typed;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace ToSic.Sxc.Tests.DataTests.DynWrappers
{
    [TestClass]
    public class WrapObjTypedItemEquality : DynAndTypedTestsBase
    {
        private static object CreateAnon() => new
        {
            Simple = "simple",
            Sub = new
            {
                SubSub = "Test"
            }
        };

        [TestMethod]
        public void DiffWrapTwiceShouldBeDiff_Typed()
            => AreNotEqual(Obj2Typed(CreateAnon()), Obj2Typed(CreateAnon()));

        [TestMethod]
        public void DiffWrapTwiceShouldBeDiff_Item()
            => AreNotEqual(Obj2Item(CreateAnon()), Obj2Item(CreateAnon()));

        [TestMethod]
        public void SameWrapTwiceShouldBeEqual_Typed()
        {
            var anon = CreateAnon();
            AreEqual(Obj2Typed(anon), Obj2Typed(anon));
        }

        [TestMethod]
        [Ignore("This can't work, because we can't overload == on interfaces - pls preserve this test, so it's documented")]
        public void SameWrapTwiceShouldBeEqual_TypedUsingEqEq()
        {
            var anon = CreateAnon();
            var a = Obj2Typed(anon);
            var b = Obj2Typed(anon);
            var c = Obj2Typed(anon) as WrapObjectTyped;
            // ReSharper disable EqualExpressionComparison
            // ReSharper disable PossibleUnintendedReferenceComparison
            var x = c == c;
            IsTrue(a == a, "Test basic comparison");
            IsTrue(a as WrapObjectTyped == b, "a as ... == b should work");
            IsTrue(a == b, "a == b should work");
            // ReSharper restore EqualExpressionComparison
            // ReSharper restore PossibleUnintendedReferenceComparison
        }

        [TestMethod]
        public void SameWrapTwiceShouldBeEqual_Item()
        {
            var anon = CreateAnon();
            AreEqual(Obj2Item(anon), Obj2Item(anon));
        }
    }
}
