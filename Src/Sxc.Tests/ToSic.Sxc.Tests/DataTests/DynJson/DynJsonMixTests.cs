using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace ToSic.Sxc.Tests.DataTests.DynJson
{
    [TestClass]
    public class DynJsonMixTests: DynJsonTestBase
    {
        [TestMethod]
        public void ObjectWithStringProperty()
        {
            var test = AnonToJsonToDyn( new { FirstName = "test" });
            AreEqual<string>("test", test.Dyn.FirstName);
        }

        [TestMethod]
        public void ArrayOfObjectsWithStringProperty()
        {
            var test = AnonToJsonToDyn(new object[]
            {
                new { FirstName = "test" }, 
                new { FirstName = "fn2" }
            });
            AreEqual<string>("test", test.Dyn[0].FirstName);
        }

        [TestMethod]
        public void ObjectWithArrayPropertyOfObjectsWithStringProperty()
        {
            var test = AnonToJsonToDyn(new
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
            var test = AnonToJsonToDyn(new
            {
                a = new object[]
                {
                    new { p1 = "fn1", p2 = new [] { "test", "a2" } },
                    new { p1 = "fn2", p2 = new [] { "b1", "b2" } },
                }
            });
            AreEqual<string>("test", test.Dyn.a[0].p2[0]);
        }

        [TestMethod]
        public void Gps()
        {
            var test = AnonToJsonToDyn(new { Lat = 43.508075, Long = 16.4665157 });
            AreEqual<double>(43.508075, test.Dyn.Lat);
            AreEqual<double>(16.4665157, test.Dyn.Long);
        }
    }
}