using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace ToSic.Sxc.Tests.DynamicData
{
    [TestClass]
    public class DynamicReadRealClass
    {
        [TestMethod]
        public void TestMethod1()
        {
            var dynAnon = TestAccessors.DynReadObjT(new SimpleObject {NameAsProperty = "PropName", NameAsValue = "ValName"}, false, false) as dynamic;
            Assert.AreEqual("PropName", dynAnon.NameAsProperty);

            // Simple values shouldn't work, only properties
            Assert.AreEqual(null, dynAnon.NameAsValue);
            Assert.AreNotEqual("ValName", dynAnon.NameAsValue);

        }
    }
}
