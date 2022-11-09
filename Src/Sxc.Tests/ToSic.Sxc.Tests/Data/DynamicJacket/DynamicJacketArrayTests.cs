using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.Json.Nodes;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.Data.Tests
{
    [TestClass]
    public class DynamicJacketArrayTests
    {
        public dynamic AsDynamic(string jsonString) => DynamicJacket.AsDynamicJacket(jsonString);

        [TestMethod]
        public void ArrayWithArrays()
        {
            var jsonString = @"[ 
                [""a1""], 
                [ 1, ""b2""], 
                [ true , 2, ""c3"" ], 
            ]";

            var dynamicValue = AsDynamic(jsonString);
            var expectedType = (new DynamicJacketList(new JsonArray())).GetType();

            Assert.IsTrue(dynamicValue.IsList);

            Assert.IsInstanceOfType(dynamicValue[2], expectedType);

            Assert.AreEqual<string>("a1", dynamicValue[0][0]);
            Assert.AreEqual<int>(1, dynamicValue[1][0]);
            Assert.IsTrue(dynamicValue[2][0]);
        }
    }
}