using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Specialized;
using ToSic.Sxc.Context;
using ToSic.Sxc.Context.Query;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace ToSic.Sxc.Tests.ContextTests
{
    [TestClass]
    public class ParametersTests
    {
        [TestMethod]
        public void BasicParameters()
        {
            var p = GetTestParameters();
            AreEqual(2, p.Count);
            IsTrue(p.ContainsKey("id"));
            IsTrue(p.ContainsKey("ID"));
        }

        [TestMethod]
        public void NotCaseSensitive()
        {
            var p = GetTestParameters();
            IsTrue(p.ContainsKey("id"));
            IsTrue(p.ContainsKey("ID"));
            IsFalse(p.ContainsKey("fake"));
        }


        [TestMethod]
        public void ParamsToString()
        {
            var p = GetTestParameters();
            AreEqual("id=27&sort=descending", p.ToString());
        }

        private void TestParam(int count, string exp, Func<IParameters, IParameters> pFunc)
        {
            var p = pFunc(GetTestParameters());
            AreEqual(count, p.Count);
            AreEqual(exp, p.ToString());
        }

        #region Add String / Null

        

        [TestMethod] public void ParameterAdd() 
            => TestParam(3, "id=27&sort=descending&test=wonderful", p => p.Add("test", "wonderful"));

        [TestMethod] public void ParameterAddNull()
            => TestParam(3, "id=27&sort=descending&test", p => p.Add("test", null));

        [TestMethod]
        public void ParameterAddNoValue()
            => TestParam(3, "id=27&sort=descending&test", p => p.Add("test"));

        [TestMethod]
        public void ParameterAddEmptyString()
            => TestParam(3, "id=27&sort=descending&test", p => p.Add("test", string.Empty));

        [TestMethod]
        public void ParameterAddMultipleSameKey()
            => TestParam(3, "id=27&sort=descending&test=wonderful&test=awesome",
                p => p.Add("test", "wonderful").Add("Test", "awesome"));

        #endregion

        #region Add / Set boolean

        [TestMethod]
        public void AddBoolTrue() => TestParam(3, "id=27&sort=descending&test=true", p => p.Add("test", true));
        [TestMethod]
        public void AddBoolFalse() => TestParam(3, "id=27&sort=descending&test=false", p => p.Add("test", false));

        [TestMethod]
        public void SetAddBoolTrue() => TestParam(3, "id=27&sort=descending&test=true", p => p.Set("test", true));
        [TestMethod]
        public void SetAddBoolFalse() => TestParam(3, "id=27&sort=descending&test=false", p => p.Set("test", false));

        [TestMethod]
        public void SetBoolTrue() => TestParam(2, "id=true&sort=descending", p => p.Set("id", true));
        [TestMethod]
        public void SetBoolFalse() => TestParam(2, "id=false&sort=descending", p => p.Set("id", false));

        #endregion

        #region Add Numbers

        [TestMethod]
        public void AddInt7() => TestParam(3, "id=27&sort=descending&test=7", p => p.Add("test", 7));
        [TestMethod]
        public void AddIntMinus7() => TestParam(3, "id=27&sort=descending&test=-7", p => p.Add("test", -7));
        
        [TestMethod]
        public void AddLong7() => TestParam(3, "id=27&sort=descending&test=7", p => p.Add("test", 7L));
        [TestMethod]
        public void AddLongMinus7() => TestParam(3, "id=27&sort=descending&test=-7", p => p.Add("test", -7L));

        [TestMethod]
        public void AddFloat7dot7() => TestParam(3, "id=27&sort=descending&test=7.7", p => p.Add("test", 7.7F));
        [TestMethod]
        public void AddFloatMinus7dot7() => TestParam(3, "id=27&sort=descending&test=-7.7", p => p.Add("test", -7.7F));
        [TestMethod]
        public void AddDouble7dot7() => TestParam(3, "id=27&sort=descending&test=7.7", p => p.Add("test", 7.7));
        [TestMethod]
        public void AddDoubleMinus7dot7() => TestParam(3, "id=27&sort=descending&test=-7.7", p => p.Add("test", -7.7));

        #endregion

        #region Add Dates

        private static readonly DateTime TestDate = new DateTime(2042, 4, 2);
        private static readonly DateTime TestDateTime = new DateTime(2042, 4, 2, 3, 4, 56);

        [TestMethod]
        public void AddDate() => TestParam(3, "id=27&sort=descending&test=2042-04-02", p => p.Add("test", TestDate));
        
        [TestMethod]
        public void AddDateTime() => TestParam(3, "id=27&sort=descending&test=2042-04-02T03:04:56", p => p.Add("test", TestDateTime));

        #endregion

        [TestMethod]
        public void ParameterSet()
        {
            var p = GetTestParameters().Set("test", "wonderful");
            AreEqual(3, p.Count);
            AreEqual("id=27&sort=descending&test=wonderful", p.ToString());
        }

        [TestMethod]
        public void ParameterSetNullValue()
        {
            var p = GetTestParameters().Set("test", null);
            AreEqual(3, p.Count);
            AreEqual("id=27&sort=descending&test", p.ToString());
        }
        [TestMethod]
        public void ParameterSetNoValue()
        {
            var p = GetTestParameters().Set("test");
            AreEqual(3, p.Count);
            AreEqual("id=27&sort=descending&test", p.ToString());
        }
        [TestMethod]
        public void ParameterSetEmptyString()
        {
            var p = GetTestParameters().Set("test", string.Empty);
            AreEqual(3, p.Count);
            AreEqual("id=27&sort=descending&test", p.ToString());
        }

        [TestMethod]
        public void ParameterSetMultipleSameKey()
        {
            var p = GetTestParameters().Set("test", "wonderful").Set("test", "awesome");
            AreEqual(3, p.Count);
            AreEqual("id=27&sort=descending&test=awesome", p.ToString());
        }

        [TestMethod]
        public void ParameterAddExisting()
        {
            var p = GetTestParameters().Add("id", "42");
            AreEqual(2, p.Count);
            AreEqual("id=27&id=42&sort=descending", p.ToString());
        }

        [TestMethod]
        public void ParameterAddExistingEmptyString()
        {
            var p = GetTestParameters().Add("id", string.Empty);
            AreEqual(2, p.Count);
            AreEqual("id=27&sort=descending", p.ToString());
        }
        [TestMethod]
        public void ParameterAddExistingNull()
        {
            var p = GetTestParameters().Add("id", null);
            AreEqual(2, p.Count);
            AreEqual("id=27&sort=descending", p.ToString());
        }

        [TestMethod]
        public void ParameterSetExisting()
        {
            var p = GetTestParameters().Set("id", "42");
            AreEqual(2, p.Count);
            AreEqual("id=42&sort=descending", p.ToString());
        }


        private static Parameters GetTestParameters()
        {
            var p = new Parameters(new NameValueCollection
            {
                { "id", "27" },
                { "sort", "descending" }
            });
            return p;
        }

    }

}
