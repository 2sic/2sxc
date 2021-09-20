using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToSic.Sxc.Web;
using static ToSic.Sxc.Tests.Web.LinkImageTestHelpers;

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
            var linker = GetLinker();

            Assert.AreEqual("test.jpg?w=200", linker.Image("test.jpg", width: 200));
            Assert.AreEqual("test.jpg?h=200", linker.Image("test.jpg", height: 200));
            Assert.AreEqual("test.jpg?w=200&h=200", linker.Image("test.jpg", width: 200, height:200));
        }

        [TestMethod]
        public void BasicWidthAndAspectRatio()
        {
            var linker = GetLinker();

            Assert.AreEqual("test.jpg?w=200", linker.Image("test.jpg", width: 200, aspectRatio: 0));
            Assert.AreEqual("test.jpg?w=200&h=200", linker.Image("test.jpg", width: 200, aspectRatio: 1));
            Assert.AreEqual("test.jpg?w=200&h=400", linker.Image("test.jpg", width: 200, aspectRatio: 0.5));
            Assert.AreEqual("test.jpg?w=200&h=100", linker.Image("test.jpg", width: 200, aspectRatio: 2));
            Assert.AreEqual("test.jpg?w=200&h=80", linker.Image("test.jpg", width: 200, aspectRatio: 2.5));
            
            // Note: in this case it should be 112.5 and will be rounded down by default
            Assert.AreEqual("test.jpg?w=200&h=112", linker.Image("test.jpg", width: 200, aspectRatio: 16f/9));
        }

        [TestMethod]
        public void BasicWidthAndAspectRatioString()
        {
            var linker = GetLinker();

            // Simple Strings
            Assert.AreEqual("test.jpg?w=200", linker.Image("test.jpg", width: 200, aspectRatio: "0"));
            Assert.AreEqual("test.jpg?w=200&h=200", linker.Image("test.jpg", width: 200, aspectRatio: "1"));
            Assert.AreEqual("test.jpg?w=200&h=400", linker.Image("test.jpg", width: 200, aspectRatio: "0.5"));
            Assert.AreEqual("test.jpg?w=200&h=100", linker.Image("test.jpg", width: 200, aspectRatio: "2"));
            Assert.AreEqual("test.jpg?w=200&h=80", linker.Image("test.jpg", width: 200, aspectRatio: "2.5"));
        }

        [TestMethod]
        public void BasicWidthAndAspectRatioStringWithSeparator()
        {
            var linker = GetLinker();

            // Simple Strings
            Assert.AreEqual("test.jpg?w=200", linker.Image("test.jpg", width: 200, aspectRatio: "0"));
            Assert.AreEqual("test.jpg?w=200&h=200", linker.Image("test.jpg", width: 200, aspectRatio: "1:1"));
            Assert.AreEqual("test.jpg?w=200&h=200", linker.Image("test.jpg", width: 200, aspectRatio: "1/1"));
            Assert.AreEqual("test.jpg?w=200&h=400", linker.Image("test.jpg", width: 200, aspectRatio: "1:2"));
            Assert.AreEqual("test.jpg?w=200&h=400", linker.Image("test.jpg", width: 200, aspectRatio: "1/2"));
            Assert.AreEqual("test.jpg?w=200&h=100", linker.Image("test.jpg", width: 200, aspectRatio: "2:1"));
            Assert.AreEqual("test.jpg?w=200&h=100", linker.Image("test.jpg", width: 200, aspectRatio: "2/1"));
            Assert.AreEqual("test.jpg?w=200&h=100", linker.Image("test.jpg", width: 200, aspectRatio: "2"));
            Assert.AreEqual("test.jpg?w=200&h=80", linker.Image("test.jpg", width: 200, aspectRatio: "2.5"));
            
            // Note: in this case it should be 112.5 and will be rounded down by default
            Assert.AreEqual("test.jpg?w=200&h=112", linker.Image("test.jpg", width: 200, aspectRatio: "16/9"));
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
