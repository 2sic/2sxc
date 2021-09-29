using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Globalization;
using ToSic.Sxc.Web;
using ToSic.Testing.Shared;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace ToSic.Sxc.Tests.WebTests.ConvertServiceTests
{
    [TestClass]
    public class ConvertServiceTest:EavTestBase
    {
        [TestMethod]
        public void ForCodeDate()
        {
            var conv = ConvertService();
            AreEqual("2021-09-29T08:45:59.000z", conv.ForCode(new DateTime(2021, 09, 29, 08, 45, 59)));
        }

        [TestMethod]
        public void ForCodeBool()
        {
            var conv = ConvertService();
            AreEqual("true", conv.ForCode(true));
            AreNotEqual(true.ToString(), conv.ForCode(true));
            AreEqual("false", conv.ForCode(false));
        }

        [TestMethod]
        public void ForCodeNumberBadCulture()
        {
            // Now change threading culture to a comma-culture and verify that change happened
            System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("de-DE");
            AreEqual("1,11", 1.11.ToString());

            // Now run tests expecting things to "just-work"
            var conv = ConvertService();
            AreEqual("0", conv.ForCode("0"));
            AreEqual("1.11", conv.ForCode(1.11f));
            AreEqual("27.42", conv.ForCode(27.42));
            AreEqual("-27.42", conv.ForCode(-27.42));
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
