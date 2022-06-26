using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
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
        public void Basic()
        {
            //var obj = new
            //{
            //    test = 7,
            //    name = "daniel"
            //};
            Assert.AreEqual("test=7&name=daniel", new ObjectToUrl().Obj2Url(TestCase1));
        }

        [TestMethod]
        public void BasicWithPrefix()
        {
            var prefix = "prefill:";
            //var obj = new
            //{
            //    test = 7,
            //    name = "daniel"
            //};
            Assert.AreEqual("prefill:test=7&prefill:name=daniel", new ObjectToUrl(prefix: prefix).Obj2Url(TestCase1));
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
            Assert.AreEqual("test=7&name=daniel&prefill:title=new%20title", new ObjectToUrl().Obj2Url(obj));
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
            Assert.AreEqual("prefill:title=new%20title&prefill:entities:name=daniel&prefill:entities:and=ok", new ObjectToUrl().Obj2Url(obj));
        }
    }
}
