using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToSic.Sxc.Data;

namespace ToSic.Sxc.Tests.DataTests
{
    [TestClass]
    public class HasKeysTests
    {
        [TestMethod]
        public void ContainsDataStringsNull() 
            => Assert.AreEqual(false, HasKeysHelper.ContainsData(null, null));

        [TestMethod]
        [DataRow("")]
        [DataRow( " ")]
        [DataRow( "   ")]
        [DataRow( "\t", "tab")]
        [DataRow( "\t \t", "tabs")]
        [DataRow( "\n \r", "new lines")]
        [DataRow("\u00A0", "non-breaking space")]
        [DataRow( "&nbsp;", "non-breaking space HTML")]
        [DataRow(" &nbsp; \n", "non-breaking space HTML")]
        public void ContainsDataStringsBlank(string value, string testName = default)
        {
            Assert.AreEqual(false, HasKeysHelper.ContainsData(value, null), testName ?? value + " blankIs: null");
            Assert.AreEqual(false, HasKeysHelper.ContainsData(value, false), testName ?? value + " blankIs: false");
            Assert.AreEqual(true, HasKeysHelper.ContainsData(value, true), testName ?? value + " blankIs: true");
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        [DataRow(27)]
        [DataRow(true)]
        [DataRow(false)]
        [DataRow("hello")]
        [DataRow('x', "character")]
        public void ContainsDataSimpleData(object value, string testName = default)
        {
            Assert.AreEqual(true, HasKeysHelper.ContainsData(value, null), testName ?? value + " blankIs: null");
            Assert.AreEqual(true, HasKeysHelper.ContainsData(value, false), testName ?? value + " blankIs: false");
            Assert.AreEqual(true, HasKeysHelper.ContainsData(value, true), testName ?? value + " blankIs: true");
        }

        [TestMethod]
        public void ContainsDataObject()
        {
            var value = new object();
            var testName = "object";
            Assert.AreEqual(true, HasKeysHelper.ContainsData(value, null), testName ?? value + " blankIs: null");
            Assert.AreEqual(true, HasKeysHelper.ContainsData(value, false), testName ?? value + " blankIs: false");
            Assert.AreEqual(true, HasKeysHelper.ContainsData(value, true), testName ?? value + " blankIs: true");
        }

        [TestMethod]
        public void ContainsDataListEmpty()
        {
            var value = new List<string>();
            var testName = "object";
            Assert.AreEqual(false, HasKeysHelper.ContainsData(value, null), testName ?? value + " blankIs: null");
            Assert.AreEqual(false, HasKeysHelper.ContainsData(value, false), testName ?? value + " blankIs: false");
            Assert.AreEqual(false, HasKeysHelper.ContainsData(value, true), testName ?? value + " blankIs: true");
        }

        [TestMethod]
        public void ContainsDataListNonEmpty()
        {
            var value = new List<string> { "hello" };
            var testName = "object";
            Assert.AreEqual(true, HasKeysHelper.ContainsData(value, null), testName ?? value + " blankIs: null");
            Assert.AreEqual(true, HasKeysHelper.ContainsData(value, false), testName ?? value + " blankIs: false");
            Assert.AreEqual(true, HasKeysHelper.ContainsData(value, true), testName ?? value + " blankIs: true");
        }
    }
}
