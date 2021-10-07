using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Globalization;
using ToSic.Sxc.Web;
using ToSic.Testing.Shared;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace ToSic.Sxc.Tests.WebTests.ConvertServiceTests
{
    /// <summary>
    /// Note: there are not many tests here, because the true engine is in the EAV conversion system which is tested very thoroughly already
    /// </summary>
    [TestClass]
    public class ConvertServiceTest:EavTestBase
    {
        private const string strGuid = "424e56ce-570a-4747-aee2-44c04caf7f12";
        private static readonly Guid expGuid = new Guid(strGuid);
        [TestMethod] public void ToGuidNull() => AreEqual(Guid.Empty, ConvertService().ToGuid(null));
        [TestMethod] public void ToGuidEmpty() => AreEqual(Guid.Empty, ConvertService().ToGuid(""));
        [TestMethod] public void ToGuidBasic() => AreEqual(expGuid, ConvertService().ToGuid(strGuid));
        [TestMethod] public void ToGuidFallback() => AreEqual(expGuid, ConvertService().ToGuid("", expGuid));

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
        private static IConvertService ConvertService()
        {
            var conv = Resolve<IConvertService>();
            return conv;
        }
    }
}
