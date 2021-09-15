using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using ToSic.Sxc.Data;

namespace ToSic.Sxc.Tests.DynamicData
{
    [TestClass]
    public class DynamicReadObjectTest
    {
        [TestMethod]
        public void BasicUseAndDataTypes()
        {
            var anon = new
            {
                Name = "2sxc",
                Description = "",
                Founded = 2012,
                Birthday = new DateTime(2012, 5, 4),
                Truthy = true,
            };

            var dynAnon = new DynamicReadObject(anon, false) as dynamic;

            Assert.IsNull(dynAnon.NotExisting);
            Assert.AreEqual(anon.Name, dynAnon.Name);
            Assert.AreEqual(anon.Name, dynAnon.naME, "Should be the same irrelevant of case");
            Assert.AreEqual(anon.Birthday, dynAnon.Birthday, "dates should be the same");
            Assert.AreEqual(anon.Truthy, dynAnon.truthy);
        }

        [TestMethod]
        public void SubObjectNotWrapped()
        {
            var anon = new
            {
                Simple = "simple",
                Sub = new
                {
                    SubSub = "Test"
                }
            };

            var dynAnon = new DynamicReadObject(anon, false) as dynamic;
            Assert.AreEqual(anon.Sub, dynAnon.Sub);
        }

        [TestMethod]
        public void SubObjectAutoWrapped()
        {
            var anon = new
            {
                Simple = "simple",
                Sub = new
                {
                    SubSub = "Test"
                }
            };

            var dynAnon = new DynamicReadObject(anon, true) as dynamic;
            Assert.AreNotEqual(anon.Sub, dynAnon.Sub, "Should not be equal, as the Sub should be re-wrapped");
            Assert.AreEqual(anon.Sub.SubSub, dynAnon.sub.subsub);
        }
    }
}
