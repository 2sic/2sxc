using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToSic.Sxc.Images.Internal;

namespace ToSic.Sxc.Tests.ServicesTests.ImageServiceTests;

public abstract partial  class ImageServiceFormatsBase: TestBaseSxcDb
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
    public void TestPng(string path)
    {
        var fileInfo = this.GetFormatTac(path);
        AssertOneFileInfo(ImageConstants.Png, fileInfo);
    }


    [DataRow("//domain.com/abc/test.jpg")]
    [DataRow("//domain.com/abc/test.jpeg")]
    [DataTestMethod]
    public void TestJpg(string path)
    {
        var fileInfo = this.GetFormatTac(path);
        AssertOneFileInfo(ImageConstants.Jpg, fileInfo);
    }

    [DataRow("docx", "docx")]
    [DataRow(".docx", "docx")]
    [DataRow(".x", "x")]
    [TestMethod]
    public void TestUnknown(string path, string expected)
    {
        var typeInfo = this.GetFormatTac(path);
        AssertUnknownFileInfo(expected, typeInfo);
    }

    private static void AssertUnknownFileInfo(string expected, IImageFormat format)
    {
        Assert.IsNotNull(format);
        Assert.AreEqual(expected, format.Format);
        Assert.AreEqual("", format.MimeType);
    }

    private static void AssertOneFileInfo(string expectedType, IImageFormat imageInfo)
    {
        Assert.IsNotNull(imageInfo);
        Assert.AreEqual(expectedType, imageInfo.Format);
        Assert.AreEqual(ImageConstants.FileTypes[expectedType].MimeType, imageInfo.MimeType);
    }

}