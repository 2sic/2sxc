using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ToSic.Sxc.Tests.LinksAndImages.LinkImageTests
{
    [TestClass]
    public class ImageWithSettings: LinkImageTestBase
    {
        [TestMethod]
        public void BasicHWandAR()
        {
            var settings = ToDyn(new { Width = 200, Height = 300 });
            TestOnLinkerAndHelper("test.jpg?w=200&h=300", "test.jpg", settings);

            var settings2 = ToDyn(new { Width = 200, AspectRatio = 1 });
            TestOnLinkerAndHelper("test.png?w=200&h=200", "test.png", settings2);

            // if h & ar are given, ar should take precedence
            var settings3 = ToDyn(new { Width = 200, Height = 300, AspectRatio = 1 });
            TestOnLinkerAndHelper("test.png?w=200&h=200", "test.png", settings3);
        }


        [TestMethod]
        public void SettingsWithOverride()
        {
            var settings = ToDyn(new { Width = 200, Height = 300 });
            TestOnLinkerAndHelper("test.jpg?h=300", "test.jpg", settings, width: 0);

            TestOnLinkerAndHelper("test.jpg?w=700&h=300", "test.jpg", settings, width: 700);
            TestOnLinkerAndHelper("test.jpg?w=200&h=550", "test.jpg", settings, height: 550);
            TestOnLinkerAndHelper("test.jpg?w=200&h=100", "test.jpg", settings, aspectRatio: 2);
            TestOnLinkerAndHelper("test.jpg?h=300", "test.jpg", settings, width: 0);
            TestOnLinkerAndHelper("test.jpg?w=200", "test.jpg", settings, height: 0);

            var settings2 = ToDyn(new { Width = 200, AspectRatio = 1 });
            TestOnLinkerAndHelper("test.png?w=200&h=200", "test.png", settings2);

            // if h & ar are given, ar should take precedence
            var settings3 = ToDyn(new { Width = 200, Height = 300, AspectRatio = 1 });
            TestOnLinkerAndHelper("test.png?w=200&h=200", "test.png", settings3);
        }



    }
}
