using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToSic.Sxc.Data;
using ToSic.Sxc.Data.Internal.Wrapper;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace ToSic.Sxc.Tests.DataTests.DynWrappers
{
    [TestClass]
    public class WrapObjBasic: DynAndTypedTestsBase
    {
        public class TestData
        {
            public string Name => "2sic";
            public string Description => "Some description";
            /// <summary> This one is not a real property but just a value! </summary>
            public string DescriptionAsProperty = "Some description";
            public int Founded => 2012;
            public DateTime Birthday { get; } = new(2012, 5, 4);
            public bool Truthy => true;
        }
        public static TestData Data = new();

        [TestMethod]
        public void SourceAnonymous()
        {
            var anon = new { Data.Name, Data.Description, Data.Founded, Data.Birthday, Data.Truthy };
            var typed = Obj2Typed(anon);
            dynamic dynAnon = Obj2WrapObj(anon, false, false);

            IsNull(dynAnon.NotExisting);
            AreEqual(anon.Name, dynAnon.Name);
            AreEqual(anon.Name, dynAnon.naME, "Should be the same irrelevant of case");
            AreEqual(anon.Birthday, dynAnon.Birthday, "dates should be the same");
            AreEqual(anon.Truthy, dynAnon.truthy);

            IsTrue(typed.TestContainsKey("Name"));
            IsTrue(typed.TestContainsKey("NAME"));
            IsTrue(typed.TestContainsKey("Description"));
            IsFalse(typed.TestContainsKey("NonexistingField"));
        }

        [TestMethod]
        public void SourceTyped()
        {
            var data = new TestData();
            var typed = Obj2Typed(data);
            dynamic dynAnon = Obj2WrapObj(data, false, false);

            IsNull(dynAnon.NotExisting);
            AreEqual(data.Name, dynAnon.Name);
            AreEqual(data.Name, dynAnon.naME, "Should be the same irrelevant of case");
            // This line is different from the anonymous test
            AreNotEqual(data.DescriptionAsProperty, dynAnon.DescriptionAsProperty, "Should NOT work for values, only properties");
            AreEqual(null, dynAnon.DescriptionAsProperty, "Should NOT work for values, only properties");
            AreEqual(data.Birthday, dynAnon.Birthday, "dates should be the same");
            AreEqual(data.Truthy, dynAnon.truthy);

            IsTrue(typed.TestContainsKey("Name"));
            IsTrue(typed.TestContainsKey("NAME"));
            IsTrue(typed.TestContainsKey("Description"));
            IsFalse(typed.TestContainsKey("NonexistingField"));
        }

        [TestMethod]
        public void RequiredDynDefaultExisting() => IsTrue(Obj2WrapObjAsDyn(Data).Truthy);

        [TestMethod]
        public void RequiredDynDefaultNonExisting() => IsNull(Obj2WrapObjAsDyn(Data).TruthyXyz);

        [TestMethod]
        public void RequiredTypedDefaultExisting() => IsTrue(Obj2Typed(Data).Bool(nameof(TestData.Truthy)));

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void RequiredTypedDefaultNonExisting() =>
            Obj2Typed(Data).Bool("FakeName");

        [TestMethod]
        public void RequiredTypedDefaultNonExistingWithParam() =>
            IsFalse(Obj2Typed(Data).Bool("FakeName", required: false));

        [TestMethod]
        public void RequiredTypedDefaultNonExistingWithParamFallback() =>
            IsTrue(Obj2Typed(Data).Bool("FakeName", fallback: true, required: false));

        [TestMethod]
        public void RequiredTypedNonStrictNonExistingWithParam() =>
            IsFalse(Obj2Typed(Data, WrapperSettings.Typed(true, true, false)).Bool("FakeName"));






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
            var typed = Obj2Typed(anon);
            IsTrue(typed.TestContainsKey("Key1"));
            IsFalse(typed.TestContainsKey("Nonexisting"));
            IsTrue(typed.TestKeys().Any());
            AreEqual(3, typed.TestKeys().Count());
            AreEqual(1, typed.TestKeys(only: new[] { "Key1" }).Count());
            AreEqual(0, typed.TestKeys(only: new[] { "Nonexisting" }).Count());
        }

        [TestMethod] public void DeepParent() => IsTrue(DataForDeepKeys.TestContainsKey("Deep"));

        [TestMethod] public void DeepSub1() => IsTrue(DataForDeepKeys.TestContainsKey("Deep.Sub1"));
        [TestMethod] public void DeepDeeper() => IsTrue(DataForDeepKeys.TestContainsKey("Deep.Deeper"));
        [TestMethod] public void DeepDeeperSub() => IsTrue(DataForDeepKeys.TestContainsKey("Deep.Deeper.SubSub1"));
        [TestMethod] public void DeepHasArray() => IsTrue(DataForDeepKeys.TestContainsKey("List"));

        // Note: Arrays are not supported
        //IsTrue(typed.TestContainsKey("List.L1"));
        //IsTrue(typed.TestContainsKey("List.L2"));

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
                var typed = Obj2Typed(anon);
                return typed;
            }
        }
    }
}
