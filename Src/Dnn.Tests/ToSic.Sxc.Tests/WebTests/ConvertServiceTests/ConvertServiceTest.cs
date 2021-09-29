using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using ToSic.Sxc.Web;
using ToSic.Testing.Shared;

namespace ToSic.Sxc.Tests.WebTests.ConvertServiceTests
{
    [TestClass]
    public class ConvertServiceTest:EavTestBase
    {
        [TestMethod]
        public void ForCodeDate()
        {
            var conv = ConvertService();
            Assert.AreEqual("2021-09-29T08:45:59.000z", conv.ForCode(new DateTime(2021, 09, 29, 08, 45, 59)));
        }

        [TestMethod]
        public void ForCodeBool()
        {
            var conv = ConvertService();
            Assert.AreEqual("true", conv.ForCode(true));
            Assert.AreNotEqual(true.ToString(), conv.ForCode(true));
            Assert.AreEqual("false", conv.ForCode(false));
        }

        /// <summary>
        /// test accessor
        /// </summary>
        /// <returns></returns>
        private static ConvertService ConvertService()
        {
            var conv = EavTestBase.Resolve<ConvertService>();
            return conv;
        }
    }
}
