using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace ToSic.Sxc.Tests.DataTests.DynJson
{
    [TestClass]
    public class WrapJsonMixed: DynAndTypedTestsBase
    {
        [TestMethod]
        public void ObjectWithStringProperty()
        {
            var test = DynJsonAndOriginal( new { FirstName = "test" });
            AreEqual<string>("test", test.Dyn.FirstName);
        }

        [TestMethod]
        public void ArrayOfObjectsWithStringProperty()
        {
            var test = DynJsonAndOriginal(new object[]
            {
                new { FirstName = "test" }, 
                new { FirstName = "fn2" }
            });
            AreEqual<string>("test", test.Dyn[0].FirstName);
        }

        [TestMethod]
        public void ObjectWithArrayPropertyOfObjectsWithStringProperty()
        {
            var test = DynJsonAndOriginal(new
            {
                a = new object[]
                {
                    new { FirstName = "test" }, 
                    new { FirstName = "fn2" }
                }
            });
            AreEqual<string>("test", test.Dyn.a[0].FirstName);
        }

        [TestMethod]
        public void ObjectWithArrayPropertyWithObjectWithStringArrayProperty()
        {
            var test = DynJsonAndOriginal(new
            {
                a = new object[]
                {
                    new { p1 = "fn1", p2 = new [] { "test", "a2" } },
                    new { p1 = "fn2", p2 = new [] { "b1", "b2" } },
                }
            });
            AreEqual<string>("test", test.Dyn.a[0].p2[0]);
        }
        
    }
}