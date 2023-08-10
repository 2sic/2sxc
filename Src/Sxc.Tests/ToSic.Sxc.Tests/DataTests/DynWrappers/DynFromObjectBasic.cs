using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToSic.Sxc.Data;
using ToSic.Sxc.Data.Wrapper;
using static System.Text.Json.JsonSerializer;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace ToSic.Sxc.Tests.DataTests.DynWrappers
{
    [TestClass]
    public class DynFromObjectBasic: DynWrapperTestBase
    {
        private class TestData
        {
            public string Name => "2sic";
            public string Description => "Some description";
            /// <summary> This one is not a real property but just a value! </summary>
            public string DescriptionAsProperty = "Some description";
            public int Founded => 2012;
            public DateTime Birthday { get; } = new DateTime(2012, 5, 4);
            public bool Truthy => true;
        }
        private static TestData Data = new TestData();

        [TestMethod]
        public void SourceAnonymous()
        {
            var anon = new { Data.Name, Data.Description, Data.Founded, Data.Birthday, Data.Truthy };
            var typed = TypedFromObject(anon);
            dynamic dynAnon = DynFromObject(anon, false, false);

            IsNull(dynAnon.NotExisting);
            AreEqual(anon.Name, dynAnon.Name);
            AreEqual(anon.Name, dynAnon.naME, "Should be the same irrelevant of case");
            AreEqual(anon.Birthday, dynAnon.Birthday, "dates should be the same");
            AreEqual(anon.Truthy, dynAnon.truthy);

            IsTrue(typed.ContainsKey("Name"));
            IsTrue(typed.ContainsKey("NAME"));
            IsTrue(typed.ContainsKey("Description"));
            IsFalse(typed.ContainsKey("NonexistingField"));
        }

        [TestMethod]
        public void SourceTyped()
        {
            var data = new TestData();
            var typed = TypedFromObject(data);
            dynamic dynAnon = DynFromObject(data, false, false);

            IsNull(dynAnon.NotExisting);
            AreEqual(data.Name, dynAnon.Name);
            AreEqual(data.Name, dynAnon.naME, "Should be the same irrelevant of case");
            // This line is different from the anonymous test
            AreNotEqual(data.DescriptionAsProperty, dynAnon.DescriptionAsProperty, "Should NOT work for values, only properties");
            AreEqual(null, dynAnon.DescriptionAsProperty, "Should NOT work for values, only properties");
            AreEqual(data.Birthday, dynAnon.Birthday, "dates should be the same");
            AreEqual(data.Truthy, dynAnon.truthy);

            IsTrue(typed.ContainsKey("Name"));
            IsTrue(typed.ContainsKey("NAME"));
            IsTrue(typed.ContainsKey("Description"));
            IsFalse(typed.ContainsKey("NonexistingField"));
        }

        [TestMethod]
        public void RequiredDynDefaultExisting() => IsTrue(AsDynamic(Data).Truthy);

        [TestMethod]
        public void RequiredDynDefaultNonExisting() => IsNull(AsDynamic(Data).TruthyXyz);

        [TestMethod]
        public void RequiredTypedDefaultExisting() => IsTrue(TypedFromObject(Data).Bool(nameof(TestData.Truthy)));

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void RequiredTypedDefaultNonExisting() =>
            TypedFromObject(Data).Bool("FakeName");

        [TestMethod]
        public void RequiredTypedDefaultNonExistingWithParam() =>
            IsFalse(TypedFromObject(Data).Bool("FakeName", required: false));

        [TestMethod]
        public void RequiredTypedDefaultNonExistingWithParamFallback() =>
            IsTrue(TypedFromObject(Data).Bool("FakeName", fallback: true, required: false));

        [TestMethod]
        public void RequiredTypedNonStrictNonExistingWithParam() =>
            IsFalse(TypedFromObject(Data, WrapperSettings.Typed(true, true, false)).Bool("FakeName"));

        [TestMethod]
        public void JsonSerialization()
        {
            var anon = new { Data.Name, Data.Description, Data.Founded, Data.Birthday, Data.Truthy };
            var typed = TypedFromObject(anon);
            dynamic dynAnon = DynFromObject(anon, false, false);

            var jsonTyped = Serialize(typed);
            var jsonAnon = Serialize(anon);
            var jsonDyn = Serialize(dynAnon);

            AreEqual(jsonTyped, jsonDyn);
            AreEqual(jsonTyped, jsonAnon);
        }





        [TestMethod]
        public void Keys()
        {
            var anon = new
            {
                Key1 = "hello",
                Key2 = "goodbye",
                Deep = new 
                {
                    Sub1 = "hello",
                    Sub2 = "hello",
                    Deeper = new
                    {
                        SubSub1 = "hello"
                    }
                }
            };
            var typed = TypedFromObject(anon);
            IsTrue(typed.ContainsKey("Key1"));
            IsFalse(typed.ContainsKey("Nonexisting"));
            IsTrue(typed.Keys().Any());
            AreEqual(3, typed.Keys().Count());
            AreEqual(1, typed.Keys(only: new[] { "Key1" }).Count());
            AreEqual(0, typed.Keys(only: new[] { "Nonexisting" }).Count());
        }

        [TestMethod] public void DeepParent() => IsTrue(DataForDeepKeys.ContainsKey("Deep"));

        [TestMethod] public void DeepSub1() => IsTrue(DataForDeepKeys.ContainsKey("Deep.Sub1"));
        [TestMethod] public void DeepDeeper() => IsTrue(DataForDeepKeys.ContainsKey("Deep.Deeper"));
        [TestMethod] public void DeepDeeperSub() => IsTrue(DataForDeepKeys.ContainsKey("Deep.Deeper.SubSub1"));
        [TestMethod] public void DeepHasArray() => IsTrue(DataForDeepKeys.ContainsKey("List"));

        // Note: Arrays are not supported
        //IsTrue(typed.ContainsKey("List.L1"));
        //IsTrue(typed.ContainsKey("List.L2"));

        private ITyped DataForDeepKeys
        {
            get
            {
                var anon = new
                {
                    Key1 = "hello",
                    Key2 = "goodbye",
                    Deep = new
                    {
                        Sub1 = "hello",
                        Sub2 = "hello",
                        Deeper = new
                        {
                            SubSub1 = "hello"
                        }
                    },
                    List = new object[]
                    {
                        new
                        {
                            L1 = "hello",
                        },
                        new
                        {
                            L2 = "hello",
                        }
                    }
                };
                var typed = TypedFromObject(anon);
                return typed;
            }
        }
    }
}
