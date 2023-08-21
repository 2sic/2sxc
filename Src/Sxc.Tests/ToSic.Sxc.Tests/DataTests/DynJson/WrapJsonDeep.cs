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
            },
            Array = new object[]
            {
                new { name = "zeroth" },
                new { Name = "first" },
                new { Name = "second" }
            }
        };

        private ITyped DataTyped => _dataTyped ?? (_dataTyped = Obj2Json2TypedStrict(DeepData));
        private ITyped _dataTyped;

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
            AreEqual(27, DataTyped.Get("TopLevelNumber"));
            AreEqual<int>(27, DataTyped.Int("TopLevelNumber"));
            AreEqual(27, DataTyped.Get("TopLevelNUMBER"));
            AreEqual<int>(27, DataTyped.Int("TopLevelNUMBER"));
            AreEqual("test", (DataTyped.Get("OBJECTPROPERTY") as ITyped).Get("stringproperty"));
            AreEqual(1, DataTyped.Get("ObjectProperty.ObjectProperty.NumberProperty"));
            AreEqual<int>(1, DataTyped.Int("ObjectProperty.ObjectProperty.NumberProperty"));
            AreEqual<bool>(true, DataTyped.Bool("ObjectProperty.ObjectProperty.ObjectProperty.BoolProperty"));
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
            //var dyn = Obj2Json2Typed(DeepData);
            var expectedType = typeof(WrapObjectTyped);
            IsInstanceOfType(DataTyped.Get("ObjectProperty"), expectedType);
            IsNull(DataTyped.Get("ObjectPropertyNonExisting", required: false));
            IsInstanceOfType((DataTyped.Get("ObjectProperty") as ITyped).Get("ObjectProperty"), expectedType);
            IsInstanceOfType(DataTyped.Get("ObjectProperty.ObjectProperty"), expectedType);
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