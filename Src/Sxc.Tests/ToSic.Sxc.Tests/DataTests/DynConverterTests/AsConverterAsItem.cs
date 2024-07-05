using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ToSic.Sxc.Tests.DataTests.DynConverterTests
{
    [TestClass]
    public class AsConverterAsItem: AsConverterTestsBase
    {
        [TestMethod]
        public void AsItemWithFakeOk()
        {
            var item = Cdf.TacAsItem(Cdf.TacFakeEntity(0), propsRequired: true);
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

            var item = Cdf.TacAsItem(data, propsRequired: true);
        }
    }
}
