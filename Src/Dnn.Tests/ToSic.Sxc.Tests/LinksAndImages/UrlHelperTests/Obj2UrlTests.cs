using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToSic.Sxc.Web.Url;

namespace ToSic.Sxc.Tests.LinksAndImages.UrlHelperTests
{
    [TestClass]
    public class Obj2UrlTests
    {
        private static object TestCase1 = new
        {
            test = 7,
            name = "daniel"
        };


        [TestMethod]
        public void WithArray()
        {
            var obj = new
            {
                array = new int[] { 32, 16, 8 }
            };
            Assert.AreEqual("array=32,16,8", new ObjectToUrl().Serialize(obj));
        }

        [TestMethod]
        public void WithSubArray()
        {
            var obj = new
            {
                prefill = new
                {
                    array = new int[] { 32, 16, 8 }
                }
            };
            Assert.AreEqual("prefill:array=32,16,8", new ObjectToUrl().Serialize(obj));
        }


        [TestMethod]
        public void Basic()
        {
            Assert.AreEqual("test=7&name=daniel", new ObjectToUrl().Serialize(TestCase1));
        }

        [TestMethod]
        public void BasicWithPrefix()
        {
            var prefix = "prefix:";
            Assert.AreEqual($"{prefix}test=7&{prefix}name=daniel", new ObjectToUrl(prefix: prefix).Serialize(TestCase1));
        }

        [TestMethod]
        public void SubObj()
        {
            var obj = new
            {
                test = 7,
                name = "daniel",
                prefill = new
                {
                    title = "new title"
                }
            };
            Assert.AreEqual("test=7&name=daniel&prefill:title=new%20title", new ObjectToUrl().Serialize(obj));
        }

        [TestMethod]
        public void SubSubObj()
        {
            var obj = new
            {
                prefill = new
                {
                    title = "new title",
                    entities = new
                    {
                        name = "daniel",
                        and = "ok"
                    }
                }
            };
            Assert.AreEqual("prefill:title=new%20title&prefill:entities:name=daniel&prefill:entities:and=ok", new ObjectToUrl().Serialize(obj));
        }
    }
}
