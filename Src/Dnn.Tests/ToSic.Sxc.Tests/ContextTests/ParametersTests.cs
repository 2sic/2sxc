using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Specialized;
using ToSic.Sxc.Context.Query;

namespace ToSic.Sxc.Tests.ContextTests
{
    [TestClass]
    public class ParametersTests
    {
        [TestMethod]
        public void BasicParameters()
        {
            var p = GetTestParameters();
            Assert.AreEqual(2, p.Count);
            Assert.IsTrue(p.ContainsKey("id"));
            Assert.IsTrue(p.ContainsKey("ID"));
        }

        [TestMethod]
        public void NotCaseSensitive()
        {
            var p = GetTestParameters();
            Assert.IsTrue(p.ContainsKey("id"));
            Assert.IsTrue(p.ContainsKey("ID"));
            Assert.IsFalse(p.ContainsKey("fake"));
        }


        [TestMethod]
        public void ParamsToString()
        {
            var p = GetTestParameters();
            Assert.AreEqual("id=27&sort=descending", p.ToString());
        }

        [TestMethod]
        public void ParameterAdd()
        {
            var p = GetTestParameters().Add("test", "wonderful");
            Assert.AreEqual(3, p.Count);
            Assert.AreEqual("id=27&sort=descending&test=wonderful", (p as Parameters).ToString());
        }

        [TestMethod]
        public void ParameterAddExisting()
        {
            var p = GetTestParameters().Add("id", "42");
            Assert.AreEqual(2, p.Count);
            Assert.AreEqual("id=42&sort=descending", (p as Parameters).ToString());
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
