using Microsoft.VisualStudio.TestTools.UnitTesting;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.Data.Tests
{
    [TestClass]
    public class DynamicJacketTests
    {
        public dynamic AsDynamic(string jsonString) => DynamicJacket.AsDynamicJacket(jsonString);

        [TestMethod]
        public void ObjectWithStringProperty()
        {
            var jsonString = "{ \"FirstName\": \"test\", }";
            Assert.AreEqual<string>("test", AsDynamic(jsonString).FirstName);
        }

        [TestMethod]
        public void ArrayOfObjectsWithStringProperty()
        {
            var jsonString = "[ { \"FirstName\": \"test\" }, { \"FirstName\": \"fn2\" }, ]";
            Assert.AreEqual<string>("test", AsDynamic(jsonString)[0].FirstName);
        }

        [TestMethod]
        public void ObjectWithArrayPropertyOfObjectsWithStringProperty()
        {
            var jsonString = "{ \"a\": [ { \"FirstName\": \"test\" }, { \"FirstName\": \"fn2\" }, ] }";
            Assert.AreEqual<string>("test", AsDynamic(jsonString).a[0].FirstName);
        }

        [TestMethod]
        public void ArrayWithStringArrays()
        {
            var jsonString = "[ [\"test\", \"a2\" ], [\"b1\", \"b2\" ], ]";
            Assert.AreEqual<string>("test", AsDynamic(jsonString)[0][0]);
        }

        [TestMethod]
        public void ObjectWithArrayPropertyWithObjectWithStringArrayProperty()
        {
            var jsonString = "{ \"a\": [ { \"p1\": \"fn1b\", \"p2\": [\"test\", \"a2\" ] }, { \"p1\": \"fn2\", \"p2\": [\"b1\", \"b2\" ]}, ] }";
            Assert.AreEqual<string>("test", AsDynamic(jsonString).a[0].p2[0]);
        }
    }
}