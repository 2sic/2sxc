using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ToSic.Sxc.Tests.Adam
{
    [TestClass]
    public class AdamSecurity
    {
        private readonly string[] _badFiles =
        {
            "hello.exe",
            "bad.cshtml",
            "_notgood.cshtml",
            "trying to be smart.jpg.js"
        };
        
        private readonly string[] _goodFiles =
        {
            "hello.doc",
            "bad.jpg",
            "_notgood.png",
            "this is a dot. and we love it.txt"
        };

        [TestMethod]
        public void BadExtensionsCaught()
        {
            var exts = _badFiles.Select(System.IO.Path.GetExtension).ToList();

            exts.ForEach(e =>
                Assert.IsTrue(Sxc.Adam.Security.BadExtensions.IsMatch(e), $"expected {e} to be marked as bad")
            );
        }


        [TestMethod]
        public void GoodExtensionsNotCaught()
        {
            var exts = _goodFiles.Select(System.IO.Path.GetExtension).ToList();

            exts.ForEach(e =>
                Assert.IsFalse(Sxc.Adam.Security.BadExtensions.IsMatch(e), $"expected {e} to be marked as bad")
            );
        }

    }
}
