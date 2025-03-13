using ToSic.Sxc.Images.Internal;

namespace ToSic.Sxc.Tests.ServicesTests.ImageServiceTests;

public abstract partial  class ImageServiceFormatsBase(TestScenario testScenario = default) : TestBaseSxcDb(testScenario)
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
        IsNotNull(format);
        AreEqual(expected, format.Format);
        AreEqual("", format.MimeType);
    }

    private static void AssertOneFileInfo(string expectedType, IImageFormat imageInfo)
    {
        IsNotNull(imageInfo);
        AreEqual(expectedType, imageInfo.Format);
        AreEqual(ImageConstants.FileTypes[expectedType].MimeType, imageInfo.MimeType);
    }

}