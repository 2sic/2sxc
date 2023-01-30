using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToSic.Lib.Logging;
using ToSic.Sxc.Services;
using static ToSic.Sxc.Services.SecureDataService;

namespace ToSic.Sxc.Tests.ServicesTests
{
    [TestClass]
    public class SecureDataTest: TestBaseSxc
    {
        private ISecureDataService GetSecureDataService() => Build<ISecureDataService>();

        private const string TestGoogleApiKey = "Made-Up-Google-Maps-Key3423";
        private const string EncryptedApiKey = "l1Ju985yqNbkbMVZn90FAoBJxD0mWKwzyMQnW2b90Ac=";

        [DataRow("No secret", "Test", "Test", false)]
        [DataRow("No secret with Prefix", PrefixSecure + "Test", PrefixSecure + "Test", false)]
        [DataRow("No secret with different cased prefix", "SECure:Test", "SECure:Test", false)]
        [DataRow("Google Api - no prefix, treat as not encrypted", TestGoogleApiKey, TestGoogleApiKey, false)]
        [DataRow("Google Api - with Prefix", TestGoogleApiKey, PrefixSecure + EncryptedApiKey)]
        [DataRow("Google Api - prefix lower case", TestGoogleApiKey, "secure:" + EncryptedApiKey)]
        [TestMethod]
        public void TestDecryptPrefixes(string testName, string expected, string original, bool mustBeSecure = true)
        {
            if (expected == null) expected = original;
            var sds = GetSecureDataService().Parse(original);
            Trace.WriteLine($"Test: '{testName}'; Result: {sds.Value}");
            Assert.AreEqual(expected, sds.Value);
            Assert.AreEqual(mustBeSecure, sds.IsSecure, "Must be Secure");
        }


        /// <summary>
        /// This doesn't actually test anything, but we can use it to get encrypted values for further testing
        /// and to encrypt stuff we need for our basic encrypted settings
        /// </summary>
        [DataRow("Test Message 1", "Test Message - Original")]
        [DataRow("Test Message 2", "1234567890")]
        [TestMethod]
        public void TestRoundTrip(string testName, string value)
        {
            var secDataService = GetSecureDataService();
            var encrypted = secDataService.Create(value);
            Trace.WriteLine("Encrypted: " + encrypted);

            var decrypted = secDataService.Parse(encrypted);
            Assert.AreEqual(value, decrypted);
            Trace.WriteLine($"Parsed/Restored: '{decrypted}'");
            Trace.WriteLine("Log Dump");
            Trace.WriteLine(secDataService.Log.Dump());
        }

    }
}
