using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToSic.Sxc.Data;
using ToSic.Sxc.Data.Wrapper;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace ToSic.Sxc.Tests.DataTests.DynJson
{
    [TestClass]
    public class WrapJsonArray : DynAndTypedTestsBase
    {
        private (dynamic Dyn, string Json, string[] Original) StringArrayPrepare() => DynJsonAndOriginal(new[]
        {
            "val1",
            "val2",
            "val3"
        });

        [TestMethod]
        public void StringArray_Dyn()
        {
            var (dyn, _, original) = StringArrayPrepare();
            IsTrue(dyn.IsList);
            AreEqual(original[0], dyn[0]);
            AreEqual(original[1], dyn[1]);
            AreEqual(original[2], dyn[2]);
            AreNotEqual(original[0], original[1]);
        }

        [TestMethod]
        public void StringArrayCount_Dyn()
        {
            var (dyn, _, original) = StringArrayPrepare();
            AreEqual(original.Length, dyn.Count);
        }

        [TestMethod]
        public void StringArrayIterate_Dyn()
        {
            var (dyn, _, _) = StringArrayPrepare();
            foreach (var strItem in dyn) IsNotNull(strItem);
        }

        [TestMethod]
        public void StringArrayNonExistingProperty_Dyn()
        {
            var test = StringArrayPrepare();
            IsNull(test.Dyn.NonExistingProperty);
        }

        [TestMethod]
        public void Test()
        {
            var anon = new[] { "test", "test2", "test3" };
            var json = JsonSerialize(anon);

            var jsonNode = JsonNode.Parse(json);
            var isArray = jsonNode is JsonArray;
            Assert.IsTrue(isArray);
            IsNotNull(jsonNode.AsArray());
        }

        public static IEnumerable<object[]> DetectJsonType => new List<object[]>
        {
            new object[] { true, false, new { something = "hello" } },
            new object[] { true, false, new { } },
            new object[] { true, true, new[] { "hello", "there" } },
            new object[] { true, true, new[] { 1, 2, 3 } },
            new object[] { false, false, "just a string" },
            new object[] { false, false, 27 },
        };

        [TestMethod]
        [DynamicData(nameof(DetectJsonType))]
        public void DetectJsonComplexOfObject(bool expComplex, bool expArray, object value, string testName = default)
        {
            var json = JsonSerialize(value);
            var (isComplex, isArray) = CodeJsonWrapper.AnalyzeJson(json);
            AreEqual(expComplex, isComplex, testName + ":" + value + "; json: " + json);
            AreEqual(expArray, isArray, testName + ":" + value + "; json: " + json);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void WrapArrayToStrictShouldFail() => Obj2Json2TypedStrict(new[] { 1, 2, 3 });

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void WrapArrayValuesToStrictListShouldFail() => Obj2Json2TypedListStrict(new[] { 1, 2, 3 });

        [TestMethod]
        public void WrapArrayToStrictListShouldBeOk()
        {
            var list = Obj2Json2TypedListStrict(new object[] { new { a = 7 }, new { b = 27 } });
            IsNotNull(list);
            AreEqual(2, list.Count());
        }


        private (dynamic Dyn, string Json, string[][] Original) StringArray2dPrepare() => DynJsonAndOriginal(new[]
        {
            new[] {"0-0", "0-1", "0-2"},
            new[] {"1-0", "1-1"},
            new[] {"2-0", "2-1", "2-2", "2-3"},
        });

        [TestMethod]
        public void StringArray2D()
        {
            var test = StringArray2dPrepare();
            AreEqual(test.Original[0][0], test.Dyn[0][0]);
            AreEqual(test.Original[2][3], test.Dyn[2][3]);
        }
        [TestMethod]
        public void StringArray2DNonExistingProperty()
        {
            var (dyn, _, _) = StringArray2dPrepare();
            IsNull(dyn.NonExisting);
            IsNull(dyn[0].NonExisting);
        }

        [TestMethod]
        public void StringArray2DCount()
        {
            var test = StringArray2dPrepare();
            AreEqual(test.Original[0].Length, test.Dyn[0].Count);
        }

        private class MiniObj
        {
            public string Name;
            public int Age;
        }

        private (dynamic Dyn, string Json, MiniObj[] Original) ObjectArrayPrepare() => DynJsonAndOriginal(new[]
        {
            new MiniObj { Name = "T1", Age = 11 },
            new MiniObj { Name = "t2", Age = 22 },
            new MiniObj { Name = "TTT3", Age = 3 }
        });

        [TestMethod]
        public void ObjectArray()
        {
            var test = ObjectArrayPrepare();
            AreEqual(test.Original[0].Name, test.Dyn[0].Name);
            AreNotEqual(test.Original[0].Name, test.Dyn[0].Age);
        }

        [TestMethod]
        public void ObjectArrayNonExistingProperties()
        {
            var test = ObjectArrayPrepare();
            IsNull(test.Dyn.Something);
            IsNull(test.Dyn[0].NonExisting);
        }


        [TestMethod]
        public void MixedArrays2D()
        {
            var (dyn, json, original) = DynJsonAndOriginal(new object[]
            {
                new string[] {"a1"},
                new object[] { 1, "b2"},
                new object[] { true, 2, "c3"}
            });

            var expectedType = typeof(DynamicJacketList);

            IsTrue(dyn.IsList);

            IsInstanceOfType(dyn[2], expectedType);

            AreEqual<string>("a1", dyn[0][0]);
            AreEqual<int>(1, dyn[1][0]);
            IsTrue(dyn[2][0]);
        }

        [TestMethod]
        [Ignore("2023-08-23 2dm - This test can't work - json Arrays don't support keys; may need to re-write the test or delete")]
        public void Keys()
        {
            var anon = new[]
            {
                "hello",
                "goodbye"
            };
            var typed = Obj2Json2TypedStrict(anon);
            IsTrue(typed.TestContainsKey("1"));
            IsFalse(typed.TestContainsKey("3"));
            IsTrue(typed.TestKeys().Any());
            AreEqual(2, typed.TestKeys().Count());
            AreEqual(1, typed.TestKeys(only: new[] { "1" }).Count());
            AreEqual(0, typed.TestKeys(only: new[] { "3" }).Count());
        }

    }
}