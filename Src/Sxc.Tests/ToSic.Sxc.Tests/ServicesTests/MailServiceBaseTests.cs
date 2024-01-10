using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToSic.Sxc.Services;
using ToSic.Sxc.Services.Internal;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace ToSic.Sxc.Tests.ServicesTests
{
    [TestClass]
    public class MailServiceBaseTests
    {
        [TestMethod]
        public void AutoDetectHtmlTest()
        {
            IsFalse(MailServiceBase.AutoDetectHtml(null));
            IsFalse(MailServiceBase.AutoDetectHtml(string.Empty));
            IsFalse(MailServiceBase.AutoDetectHtml("text"));
            IsTrue(MailServiceBase.AutoDetectHtml("<b>html</b>"));
        }

        [TestMethod]
        public void NormalizeEmailSeparatorsTest()
        {
            IsNull(MailServiceBase.NormalizeEmailSeparators(null));
            IsNull(MailServiceBase.NormalizeEmailSeparators(string.Empty));
            // all standard separators
            AreEqual(",,,,", MailServiceBase.NormalizeEmailSeparators(",,,,"));
            // all non standard separators
            AreEqual(",,,,", MailServiceBase.NormalizeEmailSeparators(";;;;"));
            // some non standard separators
            AreEqual(",,,,", MailServiceBase.NormalizeEmailSeparators(",,;;"));
        }
    }
}