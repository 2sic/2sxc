using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToSic.Sxc.Web;

namespace ToSic.Sxc.Tests.Web
{
    [TestClass]
    public class LinkImageBasicTests
    {
        [TestMethod]
        public void UrlOnly()
        {
            var linker = new ImgResizeLinker();

            var urls = new[]
            {
                "test.jpg",
                "test.png",
                "/test.jpg",
                "//test.jpg",
                "http://www.2sxc.org/test.jpg",
                "weird-extension.abc"
            };

            foreach (var url in urls) Assert.AreEqual(url, linker.Image(url));
        }

        [TestMethod]
        public void BasicWidthAndHeight()
        {
            var linker = new ImgResizeLinker();

            Assert.AreEqual("test.jpg?w=200", linker.Image("test.jpg", width: 200));
            Assert.AreEqual("test.jpg?h=200", linker.Image("test.jpg", height: 200));
            Assert.AreEqual("test.jpg?w=200&h=200", linker.Image("test.jpg", width: 200, height:200));
        }

        [TestMethod]
        public void BasicWidthAndAspectRatio()
        {
            var linker = new ImgResizeLinker();

            Assert.AreEqual("test.jpg?w=200", linker.Image("test.jpg", width: 200, aspectRatio: 0));
            Assert.AreEqual("test.jpg?w=200&h=200", linker.Image("test.jpg", width: 200, aspectRatio: 1));
            Assert.AreEqual("test.jpg?w=200&h=400", linker.Image("test.jpg", width: 200, aspectRatio: 2));
            Assert.AreEqual("test.jpg?w=200&h=100", linker.Image("test.jpg", width: 200, aspectRatio: 0.5));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ErrorHeightAndAspectRatio()
        {
            var linker = new ImgResizeLinker();

            linker.Image("test.jpg", height: 200, aspectRatio: 1);
        }


    }
}
