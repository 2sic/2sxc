using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static ToSic.Eav.Parameters;

namespace ToSic.Sxc.Tests.DataTests.DynConverterTests
{
    [TestClass]
    public class AsConverterAsItem: AsConverterTestsBase
    {
        [TestMethod]
        public void AsItemWithFakeOk()
        {
            var item = AsC.AsItem(AsC.FakeEntity(0), noParamOrder: Protector, strict: true);
            Assert.IsNotNull(item);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AsItemWithAnonFail()
        {
            var data = new
            {
                Title = "This is a title",
                Birthday = new DateTime(2012, 02, 07)
            };

            var item = AsC.AsItem(data, noParamOrder: Protector, strict: true);
        }
    }
}
