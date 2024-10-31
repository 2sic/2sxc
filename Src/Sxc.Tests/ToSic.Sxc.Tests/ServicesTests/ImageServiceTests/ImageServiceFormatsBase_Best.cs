using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToSic.Sxc.Images.Internal;

namespace ToSic.Sxc.Tests.ServicesTests.ImageServiceTests;

public abstract partial class ImageServiceFormatsBase
{
    protected abstract int ExpectedPngFormats { get; }

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
        var formats = this.GetResizeFormatTac(path);
        Assert.IsNotNull(formats);
        Assert.AreEqual(ExpectedPngFormats, formats.Count);
        // If we have many, we expect that the original will be listed as the second alternative
        if (formats.Count > 0)
            AssertOneFileInfo(ImageConstants.Png, formats.Skip(1).First());
    }

    [DataRow(null)]
    [DataRow("")]
    [DataRow(" ")]
    [DataTestMethod]
    public void TestBestEmpty(string path)
    {
        var formats = this.GetResizeFormatTac(path);
        Assert.IsNotNull(formats);
        Assert.AreEqual(0, formats.Count);
    }

    [DataRow("https://domain.com/abc/test.avif")]
    [DataTestMethod]
    public void TestBestUnknownAvif(string path)
    {
        var formats = this.GetResizeFormatTac(path);
        Assert.IsNotNull(formats);
        Assert.AreEqual(0, formats.Count);
    }

    [DataRow("https://domain.com/abc/test.svg?w=2000")]
    [DataTestMethod]
    public void TestBestSvg(string path)
    {
        var formats = this.GetResizeFormatTac(path);
        Assert.IsNotNull(formats);
        Assert.AreEqual(0, formats.Count);
    }
}