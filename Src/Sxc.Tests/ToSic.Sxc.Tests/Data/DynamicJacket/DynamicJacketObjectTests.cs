using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.Json.Nodes;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.Data.Tests
{
    [TestClass]
    public class DynamicJacketObjectTests
    {
        public dynamic AsDynamic(string jsonString) => DynamicJacket.AsDynamicJacket(jsonString);

        [TestMethod]
        public void ObjectWithObjects()
        {
            var jsonString = @"{ 
                ""ObjectProperty"": { 
                    ""StringProperty"": ""test"",
                    ""ObjectProperty"": { 
                        ""NumberProperty"": 1,
                        ""ObjectProperty"": { 
                            ""BoolProperty"": true,
                        },
                    },
                },
            }";
            var dynamicValue = AsDynamic(jsonString);
            var expectedType = (new DynamicJacket(new JsonObject())).GetType();

            Assert.IsFalse(dynamicValue.IsList);

            Assert.IsInstanceOfType(dynamicValue.ObjectProperty, expectedType);
            Assert.IsInstanceOfType(dynamicValue.ObjectProperty.ObjectProperty, expectedType);
            Assert.IsInstanceOfType(dynamicValue.ObjectProperty.ObjectProperty.ObjectProperty, expectedType);

            Assert.AreEqual<string>("test", dynamicValue["objectproperty"]["STRINGPROPERTY"]);
            Assert.AreEqual<string>("test", dynamicValue["ObjectProperty", true]["StringProperty", true]);
            Assert.AreEqual<string>("test", dynamicValue.OBJECTPROPERTY.stringproperty);

            Assert.AreEqual<int>(1, dynamicValue.ObjectProperty.ObjectProperty.NumberProperty);
            Assert.AreEqual<bool>(true, dynamicValue.ObjectProperty.ObjectProperty.ObjectProperty.BoolProperty);
        }
    }
}