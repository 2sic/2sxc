using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToSic.Eav.Security.Encryption;
using ToSic.Sxc.Services;
using static ToSic.Sxc.Services.SecureDataService;

namespace ToSic.Sxc.Tests.ServicesTests
{
    [TestClass]
    public class SecureDataTest: TestBaseSxc
    {
        private ISecureDataService GetSecureDataService() => Build<ISecureDataService>();

        [TestMethod] public void NonSecret() => TestDecryptNoSecret("Test");

        [TestMethod] public void NonSecretWithPrefix() => TestDecryptNoSecret(PrefixSecure + "Test", "Secure:Test");
        [TestMethod] public void NonSecretWithLowPrefix() => TestDecryptNoSecret(PrefixSecure.ToLowerInvariant() + "Test", "secure:Test");


        private const string TestGoogleApiKey = "Made-Up-Google-Maps-Key3423";
        private const string EncryptedApiKey = "l1Ju985yqNbkbMVZn90FAoBJxD0mWKwzyMQnW2b90Ac=";

        [DataRow("Google Api - no prefix", TestGoogleApiKey, EncryptedApiKey)]
        [DataRow("Google Api - with Prefix", TestGoogleApiKey, PrefixSecure + EncryptedApiKey)]
        [DataRow("Google Api - prefix lower case", TestGoogleApiKey, "secure:" + EncryptedApiKey)]
        [TestMethod]
        public void TestDecryptPrefixes(string testName, string expected, string original)
        {
            if (expected == null) expected = original;
            var sds = GetSecureDataService().Parse(original);
            Trace.WriteLine($"Test: '{testName}'; Result: {sds.Value}");
            Assert.AreEqual(expected, sds.Value);
            Assert.IsTrue(sds.IsSecure);
        }


        /// <summary>
        /// This doesn't actually test anything, but we can use it to get encrypted values for further testing
        /// and to encrypt stuff we need for our basic encrypted settings
        /// </summary>
        [TestMethod]
        public void DumpEncryptedValue()
        {
            var value = "6LexDw8UAAAAAGdq-9FejM6LaRpkFx5xWU-0mvfQ";
            // value = ""; // put the key to encrypt for the settings here
            var encrypted = BasicAesCryptography.EncryptAesCrypto(value);
            Trace.WriteLine("Encrypted: " + encrypted);
        }

        private void TestDecryptNoSecret(string orig, string expected = null)
        {
            if (expected == null) expected = orig;
            var sds = GetSecureDataService().Parse(orig);
            Assert.AreEqual(expected, sds.Value);
            Assert.IsFalse(sds.IsSecure);
        }

    }
}
