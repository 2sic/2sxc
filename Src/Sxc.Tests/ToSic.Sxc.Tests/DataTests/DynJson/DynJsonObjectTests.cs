using System.Text.Json.Nodes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace ToSic.Sxc.Tests.DataTests.DynJson
{
    [TestClass]
    public class DynJsonObjectTests : DynJsonTestBase
    {
        [TestMethod]
        public void ObjectWithObjects()
        {
            var test = AnonToJsonToDyn(new
            {
                ObjectProperty = new
                {
                    StringProperty = "test",
                    ObjectProperty = new
                    {
                        NumberProperty = 1,
                        ObjectProperty = new
                        {
                            BoolProperty = true
                        }
                    }
                }
            });
 
            var expectedType = new Data.DynamicJacket(new JsonObject(), Factory).GetType();

            IsFalse(test.Dyn.IsList);

            IsInstanceOfType(test.Dyn.ObjectProperty, expectedType);
            IsInstanceOfType(test.Dyn.ObjectProperty.ObjectProperty, expectedType);
            IsInstanceOfType(test.Dyn.ObjectProperty.ObjectProperty.ObjectProperty, expectedType);

            AreEqual<string>("test", test.Dyn["objectproperty"]["STRINGPROPERTY"]);
            AreEqual<string>("test", test.Dyn["ObjectProperty", true]["StringProperty", true]);
            AreEqual<string>("test", test.Dyn.OBJECTPROPERTY.stringproperty);

            AreEqual<int>(1, test.Dyn.ObjectProperty.ObjectProperty.NumberProperty);
            AreEqual<bool>(true, test.Dyn.ObjectProperty.ObjectProperty.ObjectProperty.BoolProperty);
        }
    }
}