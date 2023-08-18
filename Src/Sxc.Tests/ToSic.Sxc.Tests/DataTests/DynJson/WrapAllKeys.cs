using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToSic.Sxc.Data;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace ToSic.Sxc.Tests.DataTests.DynJson
{
    [TestClass]
    public class WrapAllKeys : DynAndTypedTestsBase
    {
        public static readonly object BoolDataAnon = new
        {
            TrueBoolType = true,
            FalseBoolType = false,
            TrueString = "true",
            FalseString = "false",
            TrueNumber = 1,
            FalseNumber = 0,
            TrueNumberBig = 27,
            TrueNumberNegative = -1,    
        };

        private ITyped BoolTestDataStrict => _boolTestDataStrict ?? (_boolTestDataStrict = Obj2Json2TypedStrict(BoolDataAnon));
        private static ITyped _boolTestDataStrict;
        private ITyped BoolTestDataLoose => _boolTestDataLoose ?? (_boolTestDataLoose = Obj2Json2TypedLoose(BoolDataAnon));
        private static ITyped _boolTestDataLoose;

        [TestMethod]
        [DataRow(true, "TrueBoolType")]
        [DataRow(true, "TrueBoolTYPE")]
        [DataRow(true, "FalseBoolType")]
        [DataRow(false, "something")]
        public void JsonBoolPropertyKeys_Typed(bool expected, string key)
            => AreEqual(expected, BoolTestDataStrict.TestContainsKey(key));


        public static IEnumerable<object[]> InvalidKeys => new[]
        {
            new object[] {"Key1"},
            new object[] {"dummy"},
            new object[] {"Part1" },
            new object[] {"Deep1" }
        };


        [TestMethod]
        [DynamicData(nameof(InvalidKeys))]
        [ExpectedException(typeof(ArgumentException))]
        public void StrictExceptions_Typed(string key)
            => BoolTestDataStrict.Bool(key);

        [TestMethod]
        [DynamicData(nameof(InvalidKeys))]
        public void StrictExceptions_Loose(string key)
            => IsFalse(BoolTestDataLoose.Bool(key));

        [TestMethod]
        [DynamicData(nameof(InvalidKeys))]
        public void StrictExceptions_TypedReqFalse(string key)
            => IsFalse(BoolTestDataStrict.Bool(key, required: false));

        [TestMethod]
        [DynamicData(nameof(InvalidKeys))]
        public void StrictExceptions_TypedReqFalseFallback(string key)
            => IsTrue(BoolTestDataStrict.Bool(key, required: false, fallback: true));

        private static object KeysDataAnon = new
        {
            Key1 = "hello",
            Key2 = "goodbye",
            SubObject = new
            {
                SubSub = "hello",
            },
            SubEmpty = new
            {

            }
        };

        private ITyped KeysData => _keysData ?? (_keysData = Obj2Json2TypedStrict(KeysDataAnon));
        private static ITyped _keysData;

        [TestMethod]
        public void KeysCountAny() => IsTrue(KeysData.TestKeys().Any());

        [TestMethod]
        public void KeysCount() => AreEqual(4, KeysData.TestKeys().Count());

        [TestMethod]
        public void KeysCountOnlySpecific1() => AreEqual(1, KeysData.TestKeys(only: new[] { "Key1" }).Count());
        [TestMethod]
        public void KeysCountOnlySpecific2() => AreEqual(2, KeysData.TestKeys(only: new[] { "Key1", "Key2" }).Count());
        [TestMethod]
        public void KeysCountOnlySpecific1of2() => AreEqual(1, KeysData.TestKeys(only: new[] { "Key1", "KeyNonExisting" }).Count());
        [TestMethod]
        public void KeysCountOnlySpecific0() => AreEqual(0, KeysData.TestKeys(only: new[] { "Nonexisting" }).Count());

        [TestMethod]
        [DataRow(true, "Key1")]
        [DataRow(false, "NonExisting")]
        [DataRow(true, "SubEmpty")]
        [DataRow(true, "SubObject")]
        [DataRow(true, "SubObject.SubSub")]
        [DataRow(false, "SubObject.Xyz")]
        [DataRow(false, "SubObject.Xyz.Abc")]
        public void ContainsKey(bool expected, string key) 
            => AreEqual(expected, KeysData.ContainsKey(key));


        //[TestMethod]
        //public void ContainsData(bool expected, string key)
        //{
        //    // var contains = KeysData.contain
        //}
    }
}