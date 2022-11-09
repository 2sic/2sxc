using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToSic.Sxc.Tests.Data.DynamicJacket;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.Data.Tests
{
    [TestClass]
    public class DynamicJacketMixTests: DynamicJacketTestBase
    {
        [TestMethod]
        public void ObjectWithStringProperty()
        {
            var test = PrepareTest( new { FirstName = "test" });
            AreEqual<string>("test", test.Dyn.FirstName);
        }

        [TestMethod]
        public void ArrayOfObjectsWithStringProperty()
        {
            var test = PrepareTest(new object[]
            {
                new { FirstName = "test" }, 
                new { FirstName = "fn2" }
            });
            AreEqual<string>("test", test.Dyn[0].FirstName);
        }

        [TestMethod]
        public void ObjectWithArrayPropertyOfObjectsWithStringProperty()
        {
            var test = PrepareTest(new
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
            var test = PrepareTest(new
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