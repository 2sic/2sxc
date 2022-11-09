using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.Json.Nodes;
using ToSic.Sxc.Tests.Data.DynamicJacket;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.Data.Tests
{
    [TestClass]
    public class DynamicJacketArrays : DynamicJacketTestBase
    {
        private (dynamic Dyn, string Json, string[] Original) StringArrayPrepare() => PrepareTest(new[]
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

        private (dynamic Dyn, string Json, string[][] Original) StringArray2dPrepare() => PrepareTest(new[]
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

        private (dynamic Dyn, string Json, MiniObj[] Original) ObjectArrayPrepare() => PrepareTest(new[]
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
        public void ExpectCount0OnNonArray()
        {
            var test = PrepareTest(new { Name = "Test" });
            AreEqual(0, test.Dyn.Count);
        }


        [TestMethod]
        public void MixedArrays2D()
        {
            var (dyn, json, original) = PrepareTest(new object[]
            {
                new string[] {"a1"},
                new object[] { 1, "b2"},
                new object[] { true, 2, "c3"}
            });

            var expectedType = (new DynamicJacketList(new JsonArray())).GetType();

            IsTrue(dyn.IsList);

            IsInstanceOfType(dyn[2], expectedType);

            AreEqual<string>("a1", dyn[0][0]);
            AreEqual<int>(1, dyn[1][0]);
            IsTrue(dyn[2][0]);
        }
    }
}