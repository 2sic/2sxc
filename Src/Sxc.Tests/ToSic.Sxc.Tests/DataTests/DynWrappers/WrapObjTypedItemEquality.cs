using Microsoft.VisualStudio.TestTools.UnitTesting;
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
        public void SameWrapTwiceShouldBeEqual_Item()
        {
            var anon = CreateAnon();
            AreEqual(Obj2Item(anon), Obj2Item(anon));
        }
    }
}
