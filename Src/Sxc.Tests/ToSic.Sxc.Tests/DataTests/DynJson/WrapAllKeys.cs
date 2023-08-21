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
        /// <summary>
        /// Base object for many tests
        /// </summary>
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
        
        /// <summary>
        /// Description of various properties and what they represent (or even if they don't exist)
        /// </summary>
        public static List<PropInfo> BoolKeys = new List<PropInfo>
        {
            new PropInfo("TrueBoolType", true, true, true),
            new PropInfo("TrueBoolTYPE", true, true, true),
            new PropInfo("FalseBoolType", true, true, false),
            new PropInfo("TrueString", true, true, true),
            new PropInfo("FalseString", true, true, false),
            new PropInfo("TrueNumber", true, true, true),
            new PropInfo("FalseNumber", true, true, false),
            new PropInfo("TrueNumberBig", true, true, true),
            new PropInfo("TrueNumberNegative", true, true, true),
            new PropInfo("Something", false, note: "key which doesn't exist"),
            new PropInfo("Dummy", false, note: "key which doesn't exist"),
            new PropInfo("Part1", false, note: "key which doesn't exist"),
            new PropInfo("Dummy.SubDummy", false, note: "key which doesn't exist"),
            new PropInfo("TrueString.SubDummy", false, note: "key which doesn't exist"),
        };

        public static IEnumerable<object[]> BoolKeysInfo => BoolKeys.ToTestEnum();

        private ITyped BoolTestDataStrict => _boolTestDataStrict ?? (_boolTestDataStrict = Obj2Json2TypedStrict(BoolDataAnon));
        private static ITyped _boolTestDataStrict;
        private ITyped BoolTestDataLoose => _boolTestDataLoose ?? (_boolTestDataLoose = Obj2Json2TypedLoose(BoolDataAnon));
        private static ITyped _boolTestDataLoose;

        
        [TestMethod]
        [DynamicData(nameof(BoolKeysInfo))]
        public void JsonBoolPropertyKeys_Typed(PropInfo pti) => AreEqual(pti.Exists, BoolTestDataStrict.TestContainsKey(pti.Name));

        public static IEnumerable<object[]> BoolKeysExist => BoolKeys.ToTestEnum(bk => bk.Exists);

        //[TestMethod]
        //[DynamicData(nameof(BoolKeysExist))]
        //public void JsonBoolPropertyKeys_Get(PropertyTestInfo pti) => AreEqual(pti.Value, BoolTestDataStrict.Get(pti.Name));

        [TestMethod]
        [DynamicData(nameof(BoolKeysExist))]
        public void JsonBoolPropertyKeys_Bool(PropInfo pti) => AreEqual((bool)pti.Value, BoolTestDataStrict.Bool(pti.Name), pti.ToString());

        [TestMethod]
        [DynamicData(nameof(BoolKeysInfo))]
        public void IsEmptyBoolData_Json(PropInfo info) => AreEqual(info.HasData, BoolTestDataStrict.IsNotEmpty(info.Name), info.Name);


        #region Test Invalid Keys in different scenarios Typed, Loose, required: false, etc.

        public static IEnumerable<object[]> BoolInvalidKeys => BoolKeys.ToTestEnum(bk => !bk.Exists);
        
        [TestMethod]
        [DynamicData(nameof(BoolInvalidKeys))]
        [ExpectedException(typeof(ArgumentException))]
        public void StrictExceptions_Typed_ShouldError(PropInfo pti)
            => BoolTestDataStrict.Bool(pti.Name);

        [TestMethod]
        [DynamicData(nameof(BoolInvalidKeys))]
        public void StrictExceptions_Loose_ShouldRetFalse(PropInfo pti)
            => IsFalse(BoolTestDataLoose.Bool(pti.Name));

        [TestMethod]
        [DynamicData(nameof(BoolInvalidKeys))]
        public void StrictExceptions_TypedReqFalse_ShouldRetFalse(PropInfo pti)
            => IsFalse(BoolTestDataStrict.Bool(pti.Name, required: false));

        [TestMethod]
        [DynamicData(nameof(BoolInvalidKeys))]
        public void StrictExceptions_TypedReqFalseFallback_ShouldRetFallback(PropInfo pti)
            => IsTrue(BoolTestDataStrict.Bool(pti.Name, required: false, fallback: true));

        #endregion

        #region Keys Data Deep

        private static object KeysDataDeepAnon = new
        {
            Key1 = "hello",
            Key2 = "goodbye",
            SubObject = new
            {
                SubTitle = "hello",
                SubSub = new
                {
                    SubSubTitle = "hello sub-sub title",
                }
            },
            SubEmpty = new
            {

            }
        };

        /// <summary>
        /// Description of various properties and what they represent (or even if they don't exist)
        /// </summary>
        public static List<PropInfo> KeysDataDeepProps = new List<PropInfo>
        {
            new PropInfo("Key1", true, true, "hello"),
            new PropInfo("Key2", true, true, "goodbye"),
            new PropInfo("Dummy", false, note: "key which doesn't exist"),
            new PropInfo("SubObject", true, true),
            new PropInfo("SubObject.SubTitle", true, true, value: "hello"),
            new PropInfo("SubObject.SubSub", true, true),
            new PropInfo("SubObject.SubSub.SubSubTitle", true, true, "hello sub-sub title"),
            new PropInfo("SubObject.SubTitle.Dummy", false),
            new PropInfo("SubObject.Dummy", false),
            new PropInfo("SubObject.Dummy.Dummy", false),
            new PropInfo("SubEmpty", true, hasData: true),
        };
        
        private ITyped KeysDataObjJsonTyped => _keysDataObjJsonTyped ?? (_keysDataObjJsonTyped = Obj2Json2TypedStrict(KeysDataDeepAnon));
        private static ITyped _keysDataObjJsonTyped;

        private ITyped KeysDataObjTyped => _keysDataObjTyped ?? (_keysDataObjTyped = Obj2Typed(KeysDataDeepAnon));
        private static ITyped _keysDataObjTyped;

        #endregion

        #region Tests Data Deep

        [TestMethod]
        public void KeysCountAny() => IsTrue(KeysDataObjJsonTyped.TestKeys().Any());

        [TestMethod]
        public void KeysCount() => AreEqual(4, KeysDataObjJsonTyped.TestKeys().Count());

        [TestMethod]
        public void KeysCountOnlySpecific1() => AreEqual(1, KeysDataObjJsonTyped.TestKeys(only: new[] { "Key1" }).Count());

        [TestMethod]
        public void KeysCountOnlySpecific2() => AreEqual(2, KeysDataObjJsonTyped.TestKeys(only: new[] { "Key1", "Key2" }).Count());

        [TestMethod]
        public void KeysCountOnlySpecific1of2() => AreEqual(1, KeysDataObjJsonTyped.TestKeys(only: new[] { "Key1", "KeyNonExisting" }).Count());

        [TestMethod]
        public void KeysCountOnlySpecific0() => AreEqual(0, KeysDataObjJsonTyped.TestKeys(only: new[] { "Nonexisting" }).Count());


        public static IEnumerable<object[]> KeysDataProps => KeysDataDeepProps.ToTestEnum();
        public static IEnumerable<object[]> KeysDataPropsExist => KeysDataDeepProps.ToTestEnum(bk => bk.Exists);

        [TestMethod] [DynamicData(nameof(KeysDataPropsExist))]
        public void KeysDataDeepOJT_GetNotNull(PropInfo pti) => IsNotNull(KeysDataObjJsonTyped.Get(pti.Name));

        [TestMethod] [DynamicData(nameof(KeysDataPropsExist))]
        public void KeysDataDeepOT_GetNotNull(PropInfo pti) => IsNotNull(KeysDataObjTyped.Get(pti.Name));

        [TestMethod] [DynamicData(nameof(KeysDataProps))]
        public void KeysDataDeepOJT_ContainsKey(PropInfo pti) => AreEqual(pti.Exists, KeysDataObjJsonTyped.ContainsKey(pti.Name));

        [TestMethod] [DynamicData(nameof(KeysDataProps))]
        public void KeysDataDeepOT_ContainsKey(PropInfo pti) => AreEqual(pti.Exists, KeysDataObjTyped.ContainsKey(pti.Name));

        [TestMethod] [DynamicData(nameof(KeysDataProps))]
        public void KeysDataDeepOJT_IsEmpty(PropInfo pti) => AreEqual(!pti.HasData, KeysDataObjJsonTyped.IsEmpty(pti.Name));
        
        [TestMethod] [DynamicData(nameof(KeysDataProps))]
        public void KeysDataDeepOT_IsEmpty(PropInfo pti) => AreEqual(!pti.HasData, KeysDataObjTyped.IsEmpty(pti.Name));

        [TestMethod] [DynamicData(nameof(KeysDataProps))]
        public void KeysDataDeepOJT_IsNotEmpty(PropInfo pti) => AreEqual(pti.HasData, KeysDataObjJsonTyped.IsNotEmpty(pti.Name));

        [TestMethod] [DynamicData(nameof(KeysDataProps))]
        public void KeysDataDeepOT_IsNotEmpty(PropInfo pti) => AreEqual(pti.HasData, KeysDataObjTyped.IsNotEmpty(pti.Name));

        #endregion

    }
}