using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.Json.Nodes;
using ToSic.Sxc.Tests.Data.DynamicJacket;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.Data.Tests
{
    [TestClass]
    public class DynamicJacketObjectTests : DynamicJacketTestBase
    {
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

            IsFalse(dynamicValue.IsList);

            IsInstanceOfType(dynamicValue.ObjectProperty, expectedType);
            IsInstanceOfType(dynamicValue.ObjectProperty.ObjectProperty, expectedType);
            IsInstanceOfType(dynamicValue.ObjectProperty.ObjectProperty.ObjectProperty, expectedType);

            AreEqual<string>("test", dynamicValue["objectproperty"]["STRINGPROPERTY"]);
            AreEqual<string>("test", dynamicValue["ObjectProperty", true]["StringProperty", true]);
            AreEqual<string>("test", dynamicValue.OBJECTPROPERTY.stringproperty);

            AreEqual<int>(1, dynamicValue.ObjectProperty.ObjectProperty.NumberProperty);
            AreEqual<bool>(true, dynamicValue.ObjectProperty.ObjectProperty.ObjectProperty.BoolProperty);
        }
    }
}