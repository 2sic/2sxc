using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToSic.Sxc.Data;
using ToSic.Sxc.Data.Typed;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace ToSic.Sxc.Tests.DataTests.DynJson
{
    [TestClass]
    public class WrapJsonDeep : DynAndTypedTestsBase
    {
        private static object DeepData = new
        {
            TopLevelNumber = 27,
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
        };

        [TestMethod]
        public void DeepValueAndCasingWithDyn()
        {
            var dyn = Obj2Json2Dyn(DeepData);
            AreEqual<int>(27, dyn.TopLevelNumber);
            AreEqual<int>(27, dyn.TopLevelNUMBER);
            AreEqual<string>("test", dyn.OBJECTPROPERTY.stringproperty);
            AreEqual<int>(1, dyn.ObjectProperty.ObjectProperty.NumberProperty);
            AreEqual<bool>(true, dyn.ObjectProperty.ObjectProperty.ObjectProperty.BoolProperty);
        }

        [TestMethod]
        public void DeepValueAndCasingWithTyped()
        {
            var dyn = Obj2Json2Typed(DeepData);
            AreEqual(27, dyn.Get("TopLevelNumber"));
            AreEqual<int>(27, dyn.Int("TopLevelNumber"));
            AreEqual(27, dyn.Get("TopLevelNUMBER"));
            AreEqual<int>(27, dyn.Int("TopLevelNUMBER"));
            // TODO!!! 
            //AreEqual<string>("test", dyn.Child("OBJECTPROPERTY").Get.stringproperty);
            //AreEqual<int>(1, dyn.ObjectProperty.ObjectProperty.NumberProperty);
            //AreEqual<bool>(true, dyn.ObjectProperty.ObjectProperty.ObjectProperty.BoolProperty);
        }

        [TestMethod]
        public void DeepObjectIsNotList()
        {
            var dyn = Obj2Json2Dyn(DeepData);
            IsFalse(dyn.IsList);
        }

        [TestMethod]
        public void DeepTypeCheckDyn()
        {
            var dyn = Obj2Json2Dyn(DeepData);
            var expectedType = typeof(DynamicJacket);
            IsInstanceOfType(dyn.ObjectProperty, expectedType);
            IsInstanceOfType(dyn.ObjectProperty.ObjectProperty, expectedType);
            IsInstanceOfType(dyn.ObjectProperty.ObjectProperty.ObjectPROPERTY, expectedType);
            IsNull(dyn.ObjectProperty.ObjectProperty.ObjectIncorrect);
        }

        [TestMethod]
        public void DeepTypeCheckTyped()
        {
            var dyn = Obj2Json2Typed(DeepData);
            var expectedType = typeof(WrapObjectTyped);
            IsInstanceOfType(dyn.Get("ObjectProperty"), expectedType);
            IsNull(dyn.Get("ObjectPropertyNonExisting", required: false));
            IsInstanceOfType((dyn.Get("ObjectProperty") as ITyped).Get("ObjectProperty"), expectedType);
            //IsInstanceOfType(dyn.ObjectProperty.ObjectProperty.ObjectPROPERTY, expectedType);
            // TODO!
            //IsNull(dyn.ObjectProperty.ObjectProperty.ObjectIncorrect);
        }

        [TestMethod]
        public void UsePropertyIndexersAndCasing()
        {
            var dyn = Obj2Json2Dyn(DeepData);
            AreEqual<string>("test", dyn["objectproperty"]["STRINGPROPERTY"]);
            // 2023-08-17 2dm disabled the two-property indexer
            //AreEqual<string>("test", test.Dyn["ObjectProperty", true]["StringProperty", true]);
        }
    }
}