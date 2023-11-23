using System;
using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToSic.Sxc.Services;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace ToSic.Sxc.Tests.ServicesTests
{
    /// <summary>
    /// Note: there are not many tests here, because the true engine is in the EAV conversion system which is tested very thoroughly already
    /// </summary>
    [TestClass]
    public class ConvertServiceTest:TestBaseSxc
    {
        private const string strGuid = "424e56ce-570a-4747-aee2-44c04caf7f12";
        private static readonly Guid expGuid = new(strGuid);
        [TestMethod] public void ToGuidNull() => AreEqual(Guid.Empty, Csvc().ToGuid(null));
        [TestMethod] public void ToGuidEmpty() => AreEqual(Guid.Empty, Csvc().ToGuid(""));
        [TestMethod] public void ToGuidBasic() => AreEqual(expGuid, Csvc().ToGuid(strGuid));
        [TestMethod] public void ToGuidFallback() => AreEqual(expGuid, Csvc().ToGuid("", expGuid));

        [TestMethod]
        public void ForCodeDate() 
            => AreEqual("2021-09-29T08:45:59.000z", Csvc().ForCode(new DateTime(2021, 09, 29, 08, 45, 59)));

        [TestMethod]
        public void ForCodeBool()
        {
            AreEqual("true", Csvc().ForCode(true));
            AreNotEqual(true.ToString(), Csvc().ForCode(true));
            AreEqual("false", Csvc().ForCode(false));
        }

        [TestMethod]
        public void ForCodeNumberBadCulture()
        {
            // Now change threading culture to a comma-culture and verify that change happened
            System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("de-DE");
            AreEqual("1,11", 1.11.ToString());

            // Now run tests expecting things to "just-work"
            var conv = Csvc();
            AreEqual("0", conv.ForCode("0"));
            AreEqual("1.11", conv.ForCode(1.11f));
            AreEqual("27.42", conv.ForCode(27.42));
            AreEqual("-27.42", conv.ForCode(-27.42));
        }

        /// <summary>
        /// test accessor
        /// </summary>
        /// <returns></returns>
        private IConvertService Csvc() => GetService<IConvertService>();
    }
}
