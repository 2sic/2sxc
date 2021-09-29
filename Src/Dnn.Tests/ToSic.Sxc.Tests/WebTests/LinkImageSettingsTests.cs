using Microsoft.VisualStudio.TestTools.UnitTesting;
using static ToSic.Sxc.Tests.WebTests.LinkImageTestHelpers;

namespace ToSic.Sxc.Tests.WebTests
{
    [TestClass]
    public class LinkImageSettingsTests
    {
        [TestMethod]
        public void BasicHWandAR()
        {
            var linker = GetLinker();

            var settings = ToDyn(new { Width = 200, Height = 300 });
            Assert.AreEqual("test.jpg?w=200&h=300", linker.Image("test.jpg", settings));

            var settings2 = ToDyn(new { Width = 200, AspectRatio = 1 });
            Assert.AreEqual("test.png?w=200&h=200", linker.Image("test.png", settings2));

            // if h & ar are given, ar should take precedence
            var settings3 = ToDyn(new { Width = 200, Height = 300, AspectRatio = 1 });
            Assert.AreEqual("test.png?w=200&h=200", linker.Image("test.png", settings3));
        }


        [TestMethod]
        public void SettingsWithOverride()
        {
            var linker = GetLinker();

            var settings = ToDyn(new { Width = 200, Height = 300 });
            Assert.AreEqual("test.jpg?h=300", linker.Image("test.jpg", settings, width: 0));



            Assert.AreEqual("test.jpg?w=700&h=300", linker.Image("test.jpg", settings, width: 700));
            Assert.AreEqual("test.jpg?w=200&h=550", linker.Image("test.jpg", settings, height: 550));
            Assert.AreEqual("test.jpg?w=200&h=100", linker.Image("test.jpg", settings, aspectRatio: 2));
            Assert.AreEqual("test.jpg?h=300", linker.Image("test.jpg", settings, width: 0));
            Assert.AreEqual("test.jpg?w=200", linker.Image("test.jpg", settings, height: 0));


            var settings2 = ToDyn(new { Width = 200, AspectRatio = 1 });
            Assert.AreEqual("test.png?w=200&h=200", linker.Image("test.png", settings2));

            // if h & ar are given, ar should take precedence
            var settings3 = ToDyn(new { Width = 200, Height = 300, AspectRatio = 1 });
            Assert.AreEqual("test.png?w=200&h=200", linker.Image("test.png", settings3));
        }



    }
}
