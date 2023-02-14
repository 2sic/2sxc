using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToSic.Sxc.Oqt.Server.Plumbing;

namespace ToSic.Eav.Security.Encryption.Tests
{
    [TestClass()]
    public class AesCryptographyServiceTests
    {
        [TestMethod()]
        [Ignore]
        public void DecryptTest()
        {
#pragma warning disable CS0219
            var x = "secure:pycbhspVSBHE662IjdEfFG8rwwCdxN9jCQaMJK6/QfLl/JxaDhAk+6q1WU4BSXw4;iv:HUyYDwdMhsuiaxZo3TG4Zg==";
            var v = "pycbhspVSBHE662IjdEfFG8rwwCdxN9jCQaMJK6/QfLl/JxaDhAk+6q1WU4BSXw4";
#pragma warning restore CS0219
            var r = new Rfc2898NetCoreGenerator();
            var aes = new AesCryptographyService(r);
            var ret = aes.DecryptFromBase64(v, new AesConfiguration(true) { InitializationVector64 = "HUyYDwdMhsuiaxZo3TG4Zg==" } );
            Assert.AreEqual("todo", ret);
        }
    }
}