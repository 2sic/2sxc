using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToSic.Sxc.Tests.Data.DynamicJacket;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.Data.Tests
{
    [TestClass]
    public class DynamicJacketMixTests: DynamicJacketTestBase
    {
        // TODO: @STV make clearer using the base-methods and anonymous object to start with, instead of hard-to-read-json-strings
        [TestMethod]
        public void ObjectWithStringProperty()
        {
            var jsonString = "{ \"FirstName\": \"test\", }";
            AreEqual<string>("test", AsDynamic(jsonString).FirstName);
        }

        [TestMethod]
        public void ArrayOfObjectsWithStringProperty()
        {
            var jsonString = "[ { \"FirstName\": \"test\" }, { \"FirstName\": \"fn2\" }, ]";
            AreEqual<string>("test", AsDynamic(jsonString)[0].FirstName);
        }

        [TestMethod]
        public void ObjectWithArrayPropertyOfObjectsWithStringProperty()
        {
            var test = PrepareTest(new
            {
                a = new object[] { new { FirstName = "test" }, new { FirstName = "fn2" } }
            });
            AreEqual<string>("test", test.Dyn.a[0].FirstName);
        }

        [TestMethod]
        public void ObjectWithArrayPropertyWithObjectWithStringArrayProperty()
        {
            var jsonString = "{ \"a\": [ { \"p1\": \"fn1b\", \"p2\": [\"test\", \"a2\" ] }, { \"p1\": \"fn2\", \"p2\": [\"b1\", \"b2\" ]}, ] }";
            AreEqual<string>("test", AsDynamic(jsonString).a[0].p2[0]);
        }
    }
}