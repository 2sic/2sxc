using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToSic.Sxc.Images;
using ToSic.Sxc.Services.Image;

namespace ToSic.Sxc.Tests.ServicesTests
{
    public partial class ImageServiceFormats
    {
        [DataRow("test.png")]
        [DataRow(".png")]
        [DataRow("png")]
        [DataRow("PNG")]
        [DataRow("test.png?w=2000")]
        [DataRow("/abc/test.png")]
        [DataRow("/abc/test.png?w=2000")]
        [DataRow("//domain.com/abc/test.png")]
        [DataRow("https://domain.com/abc/test.png")]
        [DataTestMethod]
        public void TestBestPng(string path)
        {
            var formats = Build<IImageService>().GetFormat(path).ResizeFormats;
            Assert.IsNotNull(formats);
            Assert.AreEqual(2, formats.Count);
            AssertOneFileInfo(ImageConstants.Png, formats.Skip(1).First());
        }

        [DataRow(null)]
        [DataRow("")]
        [DataRow(" ")]
        [DataTestMethod]
        public void TestBestEmpty(string path)
        {
            var formats = Build<IImageService>().GetFormat(path).ResizeFormats;
            Assert.IsNotNull(formats);
            Assert.AreEqual(0, formats.Count);
        }

        [DataRow("https://domain.com/abc/test.avif")]
        [DataTestMethod]
        public void TestBestUnknownAvif(string path)
        {
            var formats = Build<IImageService>().GetFormat(path).ResizeFormats;
            Assert.IsNotNull(formats);
            Assert.AreEqual(0, formats.Count);
            //AssertUnknownFileInfo("avif", formats.First());
        }

        [DataRow("https://domain.com/abc/test.svg?w=2000")]
        [DataTestMethod]
        public void TestBestSvg(string path)
        {
            var formats = Build<IImageService>().GetFormat(path).ResizeFormats;
            Assert.IsNotNull(formats);
            Assert.AreEqual(0, formats.Count);
            //AssertOneFileInfo(ImageConstants.Svg, formats.First());
            //Assert.AreEqual(false, formats.First().CanResize);
        }
    }
}
