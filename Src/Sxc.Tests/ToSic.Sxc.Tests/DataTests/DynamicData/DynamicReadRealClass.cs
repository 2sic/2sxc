using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToSic.Sxc.Data;

namespace ToSic.Sxc.Tests.DynamicData
{
    [TestClass]
    public class DynamicReadRealClass: TestBaseSxcDb
    {
        [TestMethod]
        public void TestMethod1()
        {
            var dynAnon = GetService<DynamicWrapperFactory>().FromObject(
                new SimpleObject {NameAsProperty = "PropName", NameAsValue = "ValName"},
                false, false) as dynamic;
            Assert.AreEqual("PropName", dynAnon.NameAsProperty);

            // Simple values shouldn't work, only properties
            Assert.AreEqual(null, dynAnon.NameAsValue);
            Assert.AreNotEqual("ValName", dynAnon.NameAsValue);

        }
    }
}
