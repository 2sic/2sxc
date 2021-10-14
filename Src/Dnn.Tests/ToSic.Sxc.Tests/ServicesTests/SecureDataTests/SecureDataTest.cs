using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using ToSic.Eav.Security.Encryption;
using ToSic.Sxc.Services;
using ToSic.Testing.Shared;
using static ToSic.Sxc.Services.SecureDataService;

namespace ToSic.Sxc.Tests.ServicesTests.SecureDataTests
{
    [TestClass]
    public class SecureDataTest: EavTestBase
    {
        private ISecureDataService GetSDS() => Resolve<ISecureDataService>();

        [TestMethod] public void NonSecret() => TestDecryptNoSecret("Test");

        [TestMethod] public void NonSecretWithPrefix() => TestDecryptNoSecret(PrefixSecure + "Test", "Test");
        [TestMethod] public void NonSecretWithLowPrefix() => TestDecryptNoSecret(PrefixSecure.ToLowerInvariant() + "Test", "Test");


        private const string TestGoogleApiKey = "Made-Up-Google-Maps-Key3423";
        private const string EncryptedApiKey = "l1Ju985yqNbkbMVZn90FAoBJxD0mWKwzyMQnW2b90Ac=";

        [TestMethod] public void TestDecryptStandard() => TestDecryptSecret(EncryptedApiKey, TestGoogleApiKey);
        [TestMethod] public void TestDecryptWithPrefix() => TestDecryptSecret(PrefixSecure + EncryptedApiKey, TestGoogleApiKey);
        [TestMethod] public void TestDecryptWithPrefixLowercase() => TestDecryptSecret(PrefixSecure.ToLowerInvariant() + EncryptedApiKey, TestGoogleApiKey);

        [TestMethod]
        [Ignore]
        public void TestGoogleMapsApiKey()
        {
            TestDecryptSecret("Secret:YPIieaLHdWhoeI1q0ULQv4WjGQMce2dKZ2apW7IVumwTzAap9LLJ4dmBQfDABC1g", "");
        }
        /// <summary>
        /// This doesn't actually test anything, but we can use it to get encrypted values for further testing
        /// </summary>
        [TestMethod]
        public void DumpEncryptedValue()
        {
            var encrypted = BasicAesCryptography.Encrypt("6LexDw8UAAAAAGdq-9FejM6LaRpkFx5xWU-0mvfQ");
            Trace.WriteLine("Encrypted: " + encrypted);
        }

        private void TestDecryptNoSecret(string orig, string expected = null)
        {
            if (expected == null) expected = orig;
            var sds = GetSDS().Parse(orig);
            Assert.AreEqual(expected, sds.Value);
            Assert.IsFalse(sds.IsSecure);
        }
        private void TestDecryptSecret(string orig, string expected = null)
        {
            if (expected == null) expected = orig;
            var sds = GetSDS().Parse(orig);
            Trace.WriteLine("Result: " + sds.Value);
            Assert.AreEqual(expected, sds.Value);
            Assert.IsTrue(sds.IsSecure);
        }

    }
}
