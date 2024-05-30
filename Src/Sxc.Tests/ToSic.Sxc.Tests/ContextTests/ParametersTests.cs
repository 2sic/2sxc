using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using ToSic.Sxc.Context;
using ToSic.Testing.Shared;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;
using static ToSic.Sxc.Tests.ContextTests.ParametersTestData;
using static ToSic.Sxc.Tests.LinksAndImages.ParametersTestExtensions;

namespace ToSic.Sxc.Tests.ContextTests
{
    [TestClass]
    public class ParametersTests
    {
        #region Initial Data and Helper Methods

        /// <summary>
        /// Take the default params, modify them in some way, and verify that the count / result matches expectations
        /// </summary>
        /// <param name="count"></param>
        /// <param name="exp"></param>
        /// <param name="pFunc"></param>
        private void ModifyDefAndVerify(int count, string exp, Func<IParameters, IParameters> pFunc)
        {
            var p = pFunc(ParametersId27SortDescending());
            AreEqual(count, p.Count);
            AreEqual(exp, p.ToString());
        }

        #endregion

        #region Get Tests

        [TestMethod]
        [DataRow("27", "id")]
        [DataRow("descending", "sort")]
        [DataRow(null, "unknown")]
        public void Get(string expected, string key)
            => AreEqual(expected, ParametersId27SortDescending().Get(key));

        [TestMethod]
        [DataRow("27", "id")]
        [DataRow("descending", "sort")]
        [DataRow(null, "unknown")]
        public void GetString(string expected, string key)
            => AreEqual(expected, ParametersId27SortDescending().String(key));

        [TestMethod]
        [DataRow("27", "id")]
        [DataRow("descending", "sort")]
        [DataRow(null, "unknown")]
        public void GetStringT(string expected, string key)
            => AreEqual(expected, ParametersId27SortDescending().Get<string>(key));

        [TestMethod]
        [DataRow(27, "id")]
        [DataRow(0, "sort")]
        [DataRow(0, "unknown")]
        public void GetInt(int expected, string key)
            => AreEqual(expected, ParametersId27SortDescending().Int(key));

        [TestMethod]
        [DataRow(27, "id")]
        [DataRow(0, "sort")]
        [DataRow(0, "unknown")]
        public void GetBool(int expected, string key)
            => AreEqual(expected, ParametersId27SortDescending().Int(key));

        #endregion


        [TestMethod]
        public void BasicParameters()
        {
            var p = ParametersId27SortDescending();
            AreEqual(2, p.Count);
            IsTrue(p.ContainsKey("id"));
            IsTrue(p.ContainsKey("ID"));
        }

        [TestMethod]
        public void BasicCaseSensitivity()
        {
            var p = ParametersId27SortDescending();
            IsTrue(p.ContainsKey("id"));
            IsTrue(p.ContainsKey("ID"));
            IsFalse(p.ContainsKey("fake"));
        }




        #region Add String / Null

        

        [TestMethod]
        [DataRow(3, Id27SortDescending + "&test=wonderful", "test", "wonderful")]
        [DataRow(3, Id27SortDescending + "&test", "test", null, "null")]
        [DataRow(3, Id27SortDescending + "&test", "test", "", "empty")]
        public void AddStringNew(int count, string expected, string key = "id", string value = default, string testName = default)
            => ModifyDefAndVerify(count, expected, p => p.TestAdd(key, value));

        [TestMethod]
        public void AddStringNewNoValue()
            => ModifyDefAndVerify(3, Id27SortDescending + "&test", p => p.TestAdd("test"));

        [TestMethod]
        public void AddStringNewMultipleSameKey()
            => ModifyDefAndVerify(3, Id27SortDescending + "&test=wonderful&test=awesome",
                p => p.TestAdd("test", "wonderful").TestAdd("Test", "awesome"));



        #endregion

        #region Add / Set boolean

        [TestMethod]
        [DataRow(3, Id27SortDescending + "&test=true", "test", true)]
        [DataRow(3, Id27SortDescending + "&test=false", "test", false)]
        public void AddBool(int count, string expected, string key, bool value, string testName = default)
            => ModifyDefAndVerify(count, expected, p => p.TestAdd(key, value));


        [TestMethod]
        [DataRow(3, Id27SortDescending + "&test=true", "test", true)]
        [DataRow(3, Id27SortDescending + "&test=false", "test", false)]
        [DataRow(2, "id=true&sort=descending", "id", true, "replace int-id with bool id")]
        [DataRow(2, "id=false&sort=descending", "id", false, "replace int-id with bool id")]
        public void SetBool(int count, string expected, string key, bool value, string testName = default)
            => ModifyDefAndVerify(count, expected, p => p.TestSet(key, value));

        #endregion

        #region Add Numbers

        [TestMethod]
        [DataRow(3, Id27SortDescending + "&test=7", "test", 7)]
        [DataRow(3, Id27SortDescending + "&test=-7", "test", -7)]
        [DataRow(3, Id27SortDescending + "&test=7", "test", 7L)]
        [DataRow(3, Id27SortDescending + "&test=7.7", "test", 7.7)]
        [DataRow(3, Id27SortDescending + "&test=-7.7", "test", -7.7)]
        [DataRow(3, Id27SortDescending + "&test=7.7", "test", 7.7F)]
        [DataRow(3, Id27SortDescending + "&test=-7.7", "test", -7.7F)]
        public void AddNumberObjects(int count, string expected, string key, object value, string testName = default)
            => ModifyDefAndVerify(count, expected, p => p.TestAdd(key, value));

        #endregion

        #region Add Dates

        private static readonly DateTime TestDate = new(2042, 4, 2);
        private static readonly DateTime TestDateTime = new(2042, 4, 2, 3, 4, 56);

        [TestMethod]
        public void AddNewDate()
            => ModifyDefAndVerify(3, Id27SortDescending + "&test=2042-04-02", p => p.TestAdd("test", TestDate));
        
        [TestMethod]
        public void AddNewDateTime()
            => ModifyDefAndVerify(3, Id27SortDescending + "&test=2042-04-02T03:04:56", p => p.TestAdd("test", TestDateTime));

        #endregion

        [TestMethod]
        [DataRow(3, Id27SortDescending + "&test=wonderful", "test", "wonderful")]
        [DataRow(3, Id27SortDescending + "&test", "test", null, "null")]
        [DataRow(3, Id27SortDescending + "&test", "test", "", "empty")]
        public void SetNew(int count, string expected, string key = "id", string value = default, string testName = default)
            => ModifyDefAndVerify(count, expected, p => p.TestSet(key, value));

        [TestMethod]
        public void SetNewNoValue()
            => ModifyDefAndVerify(3, Id27SortDescending + "&test", p=>p.TestSet("test"));


        [TestMethod]
        public void SetNewMultipleSameKey()
            => ModifyDefAndVerify(3, Id27SortDescending + "&test=awesome", p => p.TestSet("test", "wonderful").TestSet("test", "awesome"));

        [TestMethod]
        [DataRow(2, "id=27&id=42&sort=descending", "id", "42")]
        [DataRow(2, Id27SortDescending, "id", "", "empty string")]
        [DataRow(2, Id27SortDescending, "id", null, "null string")]
        public void AddExisting(int count, string expected, string key = "id", string value = default, string testName = default)
            => ModifyDefAndVerify(count, expected, p => p.TestAdd(key, value));


        [TestMethod]
        [DataRow(2, "id=42&sort=descending", "id", "42")]
        [DataRow(2, "id&sort=descending", "id", "", "reset to empty string")]
        [DataRow(2, "id&sort=descending", "id", null, "reset to null")]
        public void SetExisting(int count, string expected, string key = "id", string value = default, string testName = default)
            => ModifyDefAndVerify(count, expected, p => p.TestSet(key, value));

        [TestMethod]
        [DataRow(1, "sort=descending", "id", "remove existing")]
        [DataRow(2, Id27SortDescending, "something", "remove non-existing")]
        public void Remove(int count, string expected, string key, string testName = default)
            => ModifyDefAndVerify(count, expected, p => p.TestRemove(key));

        [TestMethod]
        public void RemoveOfMultiple()
        {
            var p = ParametersId27SortDescending().TestAdd("id", 42).TestAdd("id", "hello")
                .TestRemove("id", "42");
            AreEqual("sort=descending&id=27&id=hello", p.ToString());
        }

        [TestMethod]
        [DataRow("id=27&id=hello", "id=27&id=42&id=hello", "id", "42")]
        [DataRow("id=27&id=42&id=hello", "id=27&id=42&id=hello", "id", "999")]
        [DataRow("id=27&id=42&id=hello", "id=27&id=42&id=hello", "notdefined", "999")]
        [DataRow("id=27&id=42", "id=27&id=42&id=hello", "id", "HELLO")]
        [DataRow("ID=27&ID=42", "id=27&id=42&id=hello", "ID", "HELLO", "removing a specific key will reset key casing")]
        public void RemoveOfMultipleFromBlank(string expected, string initial, string key, string rmvValue, string note = default)
        {
            var p = initial.AsParameters().TestRemove(key, rmvValue);
            AreEqual(expected, p.ToString(), note);
        }

        [TestMethod]
        [DataRow(true, "id")]
        [DataRow(true, "sort")]
        [DataRow(false, "dummy")]
        [DataRow(false, "id.xyz")]
        [DataRow(false, "id.xyz.abc")]
        public void ContainsKey(bool exists, string key, string testName = default)
            => AreEqual(exists, ParametersId27SortDescending().ContainsKey(key));

        #region Count Tests

        public class ParameterCountTest
        {
            public int Count;
            public Func<IParameters, IParameters> Prepare = p => p;
        }

        private static IEnumerable<object[]> CountTests => new List<ParameterCountTest>
        {
            new() { Count = 2 },
            new() { Count = 2, Prepare = p => p.TestSet("id") },
            new() { Count = 3, Prepare = p => p.TestSet("new") },
            new() { Count = 1, Prepare = p => p.TestRemove("id") },
        }.ToTestEnum();

        [TestMethod]
        [DynamicData(nameof(CountTests))]
        public void CountKeys(ParameterCountTest pct)
            => AreEqual(pct.Count, pct.Prepare(ParametersId27SortDescending()).Keys().Count());

        #endregion


        [TestMethod]
        [DataRow(true, "id")]
        [DataRow(true, "sort")]
        [DataRow(false, "dummy")]
        public void IsNotEmpty(bool expected, string key, string testName = default)
            => AreEqual(expected, ParametersId27SortDescending().IsNotEmpty(key), testName);

        [TestMethod]
        [DataRow("id=42&sort=descending", "id", "42", "should replace")]
        [DataRow(Id27SortDescending + "&new=hello", "new", "hello", "should append")]
        [DataRow("sort=descending", "id", "27", "should remove")]
        [DataRow("id&sort=descending", "id", "", "empty value should remove")]
        [DataRow("id&sort=descending", "id", null, "null should ???")]
        public void ToggleFrom27Descending(string expected, string key, string value, string note)
            => AreEqual(expected, ParametersId27SortDescending().TestToggle(key, value).ToString(), note);

        [TestMethod]
        [DataRow("id=42", "id", "42", "should add")]
        [DataRow("new=hello", "new", "hello", "should add")]
        public void ToggleFromEmpty(string expected, string key, string value, string note)
        {
            var p = "".AsParameters().TestToggle(key, value);
            AreEqual(expected, p.ToString(), note);
        }

        [TestMethod]
        [DataRow("id=42", "id", "42", "should replace")]
        [DataRow("id=hello", "id", "hello", "should replace")]
        public void ToggleFromExisting(string expected, string key, string value, string note)
        {
            var pOriginal = NewParameters(new() { { "id", "999" } });
            var p = pOriginal.TestToggle(key, value);
            AreEqual(expected, p.ToString(), note);
        }

        [TestMethod]
        [DataRow("id=42", "id=42", "id", "should preserve")]
        [DataRow("id=42", "id=42&sort=descending", "id", "should preserve id only")]
        [DataRow("id=42&id=27", "id=42&id=27&sort=descending", "id", "should preserve id only")]
        [DataRow("sort=descending", "id=42&sort=descending", "sort", "should preserve id only")]
        public void Filter(string expected, string initial, string names, string testNotes = default)
        {
            var p = initial.AsParameters().Filter(names);
            AreEqual(expected, p.ToString(), testNotes);
        }
    }

}
