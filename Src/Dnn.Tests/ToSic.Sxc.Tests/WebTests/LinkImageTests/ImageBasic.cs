using System;
using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ToSic.Sxc.Tests.WebTests.LinkImageTests
{
    [TestClass]
    public class ImageBasic: LinkImageTestBase
    {
        [TestMethod]
        public void UrlOnly()
        {
            var urls = new[]
            {
                "test.jpg",
                "test.png",
                "/test.jpg",
                "//test.jpg",
                "http://www.2sxc.org/test.jpg",
                "weird-extension.abc"
            };

            foreach (var url in urls) EqualOnLinkerAndHelper(url, url);
        }

        [TestMethod]
        public void BadCharacters()
        {
            EqualOnLinkerAndHelper("test%20picture.jpg?w=200", "test picture.jpg", width: 200);
            EqualOnLinkerAndHelper("gr%C3%A4%C3%9Flich.jpg?h=200", "gräßlich.jpg", height: 200);
            EqualOnLinkerAndHelper("test%20picture.jpg?x=chuchich%C3%A4schtly&w=200", "test picture.jpg?x=chuchichäschtly", width: 200);
        }


        [TestMethod]
        public void BasicWidthAndHeight()
        {
            EqualOnLinkerAndHelper("test.jpg?w=200", "test.jpg", width: 200);
            EqualOnLinkerAndHelper("test.jpg?h=200", "test.jpg", height: 200);
            EqualOnLinkerAndHelper("test.jpg?w=200&h=200", "test.jpg", width: 200, height:200);
        }

        [TestMethod]
        public void BasicWidthAndAspectRatio()
        {
            EqualOnLinkerAndHelper("test.jpg?w=200", "test.jpg", width: 200, aspectRatio: 0);
            EqualOnLinkerAndHelper("test.jpg?w=200&h=200", "test.jpg", width: 200, aspectRatio: 1);
            EqualOnLinkerAndHelper("test.jpg?w=200&h=400", "test.jpg", width: 200, aspectRatio: 0.5);
            EqualOnLinkerAndHelper("test.jpg?w=200&h=100", "test.jpg", width: 200, aspectRatio: 2);
            EqualOnLinkerAndHelper("test.jpg?w=200&h=80", "test.jpg", width: 200, aspectRatio: 2.5);
            
            // Note: in this case it should be 112.5 and will be rounded down by default
            EqualOnLinkerAndHelper("test.jpg?w=200&h=112", "test.jpg", width: 200, aspectRatio: 16f/9);
        }

        [TestMethod]
        public void BasicWidthAndAspectRatioString()
        {
            // Simple Strings
            EqualOnLinkerAndHelper("test.jpg?w=200", "test.jpg", width: 200, aspectRatio: "0");
            EqualOnLinkerAndHelper("test.jpg?w=200&h=200", "test.jpg", width: 200, aspectRatio: "1");
            EqualOnLinkerAndHelper("test.jpg?w=200&h=400", "test.jpg", width: 200, aspectRatio: "0.5");
            EqualOnLinkerAndHelper("test.jpg?w=200&h=100", "test.jpg", width: 200, aspectRatio: "2");
            EqualOnLinkerAndHelper("test.jpg?w=200&h=80", "test.jpg", width: 200, aspectRatio: "2.5");
        }

        [TestMethod]
        public void BasicWidthAndAspectRatioCommaBadCulture()
        {
            // test before setting culture
            EqualOnLinkerAndHelper("test.jpg?w=200&h=400", "test.jpg", width: 200, aspectRatio: "0,5");
            EqualOnLinkerAndHelper("test.jpg?w=200&h=400", "test.jpg", width: 200, aspectRatio: "0.5");

            // Now set culture and run again
            System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("de-DE");
            EqualOnLinkerAndHelper("test.jpg?w=200&h=400", "test.jpg", width: 200, aspectRatio: "0.5");
            EqualOnLinkerAndHelper("test.jpg?w=200&h=400", "test.jpg", width: 200, aspectRatio: "0.5");
            EqualOnLinkerAndHelper("test.jpg?w=200&h=100", "test.jpg", width: 200, aspectRatio: "2");
            EqualOnLinkerAndHelper("test.jpg?w=200&h=80", "test.jpg", width: 200, aspectRatio: "2.5");
        }

        [TestMethod]
        public void BasicWidthAndAspectRatioStringWithSeparator()
        {
            // Simple Strings
            EqualOnLinkerAndHelper("test.jpg?w=200", "test.jpg", width: 200, aspectRatio: "0");
            EqualOnLinkerAndHelper("test.jpg?w=200&h=200", "test.jpg", width: 200, aspectRatio: "1:1");
            EqualOnLinkerAndHelper("test.jpg?w=200&h=200", "test.jpg", width: 200, aspectRatio: "1/1");
            EqualOnLinkerAndHelper("test.jpg?w=200&h=400", "test.jpg", width: 200, aspectRatio: "1:2");
            EqualOnLinkerAndHelper("test.jpg?w=200&h=400", "test.jpg", width: 200, aspectRatio: "1/2");
            EqualOnLinkerAndHelper("test.jpg?w=200&h=100", "test.jpg", width: 200, aspectRatio: "2:1");
            EqualOnLinkerAndHelper("test.jpg?w=200&h=100", "test.jpg", width: 200, aspectRatio: "2/1");
            EqualOnLinkerAndHelper("test.jpg?w=200&h=100", "test.jpg", width: 200, aspectRatio: "2");
            EqualOnLinkerAndHelper("test.jpg?w=200&h=80", "test.jpg", width: 200, aspectRatio: "2.5");
            
            // Note: in this case it should be 112.5 and will be rounded down by default
            EqualOnLinkerAndHelper("test.jpg?w=200&h=112", "test.jpg", width: 200, aspectRatio: "16/9");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ErrorHeightAndAspectRatio()
        {
            var linker = GetLinker();
            linker.Image("test.jpg", height: 200, aspectRatio: 1);
        }


    }
}
