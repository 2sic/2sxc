using System.Linq;
using System.Text.Json.Nodes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToSic.Sxc.Data;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace ToSic.Sxc.Tests.DataTests.DynJson
{
    [TestClass]
    public class DynJsonArrays : DynJsonTestBase
    {
        private (dynamic Dyn, string Json, string[] Original) StringArrayPrepare() => AnonToJsonToDyn(new[]
        {
            "val1",
            "val2",
            "val3"
        });

        [TestMethod]
        public void StringArray()
        {
            var (dyn, _, original) = StringArrayPrepare();
            IsTrue(dyn.IsList);
            AreEqual(original[0], dyn[0]);
            AreEqual(original[1], dyn[1]);
            AreEqual(original[2], dyn[2]);
            AreNotEqual(original[0], original[1]);
        }

        [TestMethod]
        public void StringArrayCount()
        {
            var (dyn, _, original) = StringArrayPrepare();
            AreEqual(original.Length, dyn.Count);
        }

        [TestMethod]
        public void StringArrayIterate()
        {
            var (dyn, _, original) = StringArrayPrepare();
            foreach (var strItem in dyn) IsNotNull(strItem);
        }

        [TestMethod]
        public void StringArrayNonExistingProperty()
        {
            var test = StringArrayPrepare();
            IsNull(test.Dyn.NonExistingProperty);
        }

        private (dynamic Dyn, string Json, string[][] Original) StringArray2dPrepare() => AnonToJsonToDyn(new[]
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

        private (dynamic Dyn, string Json, MiniObj[] Original) ObjectArrayPrepare() => AnonToJsonToDyn(new[]
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
            var (dyn, json, original) = AnonToJsonToDyn(new object[]
            {
                new string[] {"a1"},
                new object[] { 1, "b2"},
                new object[] { true, 2, "c3"}
            });

            var expectedType = new DynamicJacketList(new JsonArray(), Factory).GetType();

            IsTrue(dyn.IsList);

            IsInstanceOfType(dyn[2], expectedType);

            AreEqual<string>("a1", dyn[0][0]);
            AreEqual<int>(1, dyn[1][0]);
            IsTrue(dyn[2][0]);
        }

        [TestMethod]
        public void Keys()
        {
            var anon = new[]
            {
                "hello",
                "goodbye"
            };
            var typed = AsDynamic(anon) as ITyped;
            IsTrue(typed.ContainsKey("1"));
            IsFalse(typed.ContainsKey("3"));
            IsTrue(typed.Keys().Any());
            AreEqual(2, typed.Keys().Count());
            AreEqual(1, typed.Keys(only: new[] { "1" }).Count());
            AreEqual(0, typed.Keys(only: new[] { "3" }).Count());
        }

    }
}