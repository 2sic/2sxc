using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Specialized;
using System.Linq;
using ToSic.Sxc.Context;
using ToSic.Testing.Shared;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;
using static ToSic.Sxc.Tests.LinksAndImages.ParametersTestExtensions;

namespace ToSic.Sxc.Tests.ContextTests
{
    [TestClass]
    public class ParametersTests
    {
        #region Initial Data and Helper Methods

        private static IParameters GetTestParameters() => NewParameters(new NameValueCollection
        {
            { "id", "27" },
            { "sort", "descending" }
        });

        private const string Unmodified = "id=27&sort=descending";

        public class ParameterCountTest
        {
            public int Count;
            public Func<IParameters, IParameters> Prepare = p => p;
        }
        //private class ParameterTest<TValue>: ParameterCountTest
        //{
        //    //public int Count;
        //    public string Expected = Unmodified;
        //    public string Key = "id";
        //    public TValue Value = default;
        //    //public Func<IParameters, IParameters> Func;
        //}

        /// <summary>
        /// Take the default params, modify them in some way, and verify that the count / result matches expectations
        /// </summary>
        /// <param name="count"></param>
        /// <param name="exp"></param>
        /// <param name="pFunc"></param>
        private void ModifyDefAndVerify(int count, string exp, Func<IParameters, IParameters> pFunc)
        {
            var p = pFunc(GetTestParameters());
            AreEqual(count, p.Count);
            AreEqual(exp, p.ToString());
        }

        #endregion

        #region Very Basic Tests - ToString etc.

        [TestMethod]
        public void ParamsToString() => AreEqual(Unmodified, GetTestParameters().ToString());

        #endregion

        [TestMethod]
        public void BasicParameters()
        {
            var p = GetTestParameters();
            AreEqual(2, p.Count);
            IsTrue(p.ContainsKey("id"));
            IsTrue(p.ContainsKey("ID"));
        }

        [TestMethod]
        public void BasicCaseSensitivity()
        {
            var p = GetTestParameters();
            IsTrue(p.ContainsKey("id"));
            IsTrue(p.ContainsKey("ID"));
            IsFalse(p.ContainsKey("fake"));
        }




        #region Add String / Null

        

        [TestMethod]
        [DataRow(3, Unmodified + "&test=wonderful", "test", "wonderful")]
        [DataRow(3, Unmodified + "&test", "test", null, "null")]
        [DataRow(3, Unmodified + "&test", "test", "", "empty")]
        public void AddStringNew(int count, string expected, string key = "id", string value = default, string testName = default)
            => ModifyDefAndVerify(count, expected, p => p.TestAdd(key, value));

        [TestMethod]
        public void AddStringNewNoValue()
            => ModifyDefAndVerify(3, Unmodified + "&test", p => p.TestAdd("test"));

        [TestMethod]
        public void AddStringNewMultipleSameKey()
            => ModifyDefAndVerify(3, Unmodified + "&test=wonderful&test=awesome",
                p => p.TestAdd("test", "wonderful").TestAdd("Test", "awesome"));



        #endregion

        #region Add / Set boolean

        [TestMethod]
        [DataRow(3, Unmodified + "&test=true", "test", true)]
        [DataRow(3, Unmodified + "&test=false", "test", false)]
        public void AddBool(int count, string expected, string key, bool value, string testName = default)
            => ModifyDefAndVerify(count, expected, p => p.TestAdd(key, value));


        [TestMethod]
        [DataRow(3, Unmodified + "&test=true", "test", true)]
        [DataRow(3, Unmodified + "&test=false", "test", false)]
        [DataRow(2, "id=true&sort=descending", "id", true, "replace int-id with bool id")]
        [DataRow(2, "id=false&sort=descending", "id", false, "replace int-id with bool id")]
        public void SetBool(int count, string expected, string key, bool value, string testName = default)
            => ModifyDefAndVerify(count, expected, p => p.TestSet(key, value));

        #endregion

        #region Add Numbers

        [TestMethod]
        [DataRow(3, Unmodified + "&test=7", "test", 7)]
        [DataRow(3, Unmodified + "&test=-7", "test", -7)]
        [DataRow(3, Unmodified + "&test=7", "test", 7L)]
        [DataRow(3, Unmodified + "&test=7.7", "test", 7.7)]
        [DataRow(3, Unmodified + "&test=-7.7", "test", -7.7)]
        [DataRow(3, Unmodified + "&test=7.7", "test", 7.7F)]
        [DataRow(3, Unmodified + "&test=-7.7", "test", -7.7F)]
        public void AddNumberObjects(int count, string expected, string key, object value, string testName = default)
            => ModifyDefAndVerify(count, expected, p => p.TestAdd(key, value));

        #endregion

        #region Add Dates

        private static readonly DateTime TestDate = new DateTime(2042, 4, 2);
        private static readonly DateTime TestDateTime = new DateTime(2042, 4, 2, 3, 4, 56);

        [TestMethod]
        public void AddNewDate()
            => ModifyDefAndVerify(3, Unmodified + "&test=2042-04-02", p => p.TestAdd("test", TestDate));
        
        [TestMethod]
        public void AddNewDateTime()
            => ModifyDefAndVerify(3, Unmodified + "&test=2042-04-02T03:04:56", p => p.TestAdd("test", TestDateTime));

        #endregion

        [TestMethod]
        [DataRow(3, Unmodified + "&test=wonderful", "test", "wonderful")]
        [DataRow(3, Unmodified + "&test", "test", null, "null")]
        [DataRow(3, Unmodified + "&test", "test", "", "empty")]
        public void SetNew(int count, string expected, string key = "id", string value = default, string testName = default)
            => ModifyDefAndVerify(count, expected, p => p.TestSet(key, value));

        [TestMethod]
        public void SetNewNoValue()
            => ModifyDefAndVerify(3, Unmodified + "&test", p=>p.TestSet("test"));


        [TestMethod]
        public void SetNewMultipleSameKey()
            => ModifyDefAndVerify(3, Unmodified + "&test=awesome", p => p.TestSet("test", "wonderful").TestSet("test", "awesome"));

        [TestMethod]
        [DataRow(2, "id=27&id=42&sort=descending", "id", "42")]
        [DataRow(2, Unmodified, "id", "", "empty string")]
        [DataRow(2, Unmodified, "id", null, "null string")]
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
        [DataRow(2, Unmodified, "something", "remove non-existing")]
        public void Remove(int count, string expected, string key, string testName = default)
            => ModifyDefAndVerify(count, expected, p => p.TestRemove(key));

        [TestMethod]
        [DataRow(true, "id")]
        [DataRow(true, "sort")]
        [DataRow(false, "dummy")]
        [DataRow(false, "id.xyz")]
        [DataRow(false, "id.xyz.abc")]
        public void ContainsKey(bool exists, string key, string testName = default)
            => AreEqual(exists, GetTestParameters().ContainsKey(key));

        #region Count Tests

        private static IEnumerable<object[]> CountTests => new List<ParameterCountTest>
        {
            new ParameterCountTest { Count = 2 },
            new ParameterCountTest { Count = 2, Prepare = p => p.TestSet("id") },
            new ParameterCountTest { Count = 3, Prepare = p => p.TestSet("new") },
            new ParameterCountTest { Count = 1, Prepare = p => p.TestRemove("id") },
        }.ToTestEnum();

        [TestMethod]
        [DynamicData(nameof(CountTests))]
        public void CountKeys(ParameterCountTest pct)
            => AreEqual(pct.Count, pct.Prepare(GetTestParameters()).Keys().Count());

        #endregion


        [TestMethod]
        [DataRow(true, "id")]
        [DataRow(true, "sort")]
        [DataRow(false, "dummy")]
        public void IsNotEmpty(bool expected, string key, string testName = default)
            => AreEqual(expected, GetTestParameters().IsNotEmpty(key), testName);
    }

}
