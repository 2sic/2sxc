using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToSic.Eav.Security.Files;

namespace ToSic.Sxc.Tests.Adam
{
    [TestClass]
    public class AdamSecurity
    {
        private readonly string[] _badFiles =
        {
            "hello.exe",
            "unhappy.exe ",
            "unhappy 2. exe",
            "bad.cshtml",
            "_notgood.cshtml",
            "trying to be smart.jpg.js"
        };
        
        private readonly string[] _goodFiles =
        {
            "hello.doc",
            "good.jpg",
            "_notbad.png",
            "this is a dot. and we love it.txt",
            "list of flowers.csv"
        };

        [TestMethod]
        public void BadExtensionsCaught()
        {
            var exts = _badFiles.Select(System.IO.Path.GetExtension).ToList();

            exts.ForEach(e =>
                Assert.IsTrue( FileNames.IsKnownRiskyExtension(e), $"expected {e} to be marked as bad")
            );
        }


        [TestMethod]
        public void GoodExtensionsNotCaught()
        {
            var exts = _goodFiles.Select(System.IO.Path.GetExtension).ToList();

            exts.ForEach(e =>
                Assert.IsFalse(FileNames.IsKnownRiskyExtension(e), $"expected {e} to be marked as good")
            );
        }

    }
}
