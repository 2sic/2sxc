using ToSic.Sxc.Images.Internal;
using ToSic.Sxc.Images.Sys;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.ServicesTests.ImageServiceTests;

[Startup(typeof(StartupSxcCoreOnly))]
public abstract partial  class ImageServiceFormatsBase(IImageService imgSvc)
{
    [Theory]
    [InlineData("test.png")]
    [InlineData(".png")]
    [InlineData("png")]
    [InlineData("PNG")]
    [InlineData("test.png?w=2000")]
    [InlineData("/abc/test.png")]
    [InlineData("/abc/test.png?w=2000")]
    [InlineData("//domain.com/abc/test.png")]
    [InlineData("https://domain.com/abc/test.png")]
    public void TestPng(string path)
    {
        var fileInfo = imgSvc.GetFormatTac(path);
        AssertOneFileInfo(ImageConstants.Png, fileInfo);
    }


    [Theory]
    [InlineData("//domain.com/abc/test.jpg")]
    [InlineData("//domain.com/abc/test.jpeg")]
    public void TestJpg(string path)
    {
        var fileInfo = imgSvc.GetFormatTac(path);
        AssertOneFileInfo(ImageConstants.Jpg, fileInfo);
    }

    [Theory]
    [InlineData("docx", "docx")]
    [InlineData(".docx", "docx")]
    [InlineData(".x", "x")]
    public void TestUnknown(string path, string expected)
    {
        var typeInfo = imgSvc.GetFormatTac(path);
        AssertUnknownFileInfo(expected, typeInfo);
    }

    private static void AssertUnknownFileInfo(string expected, IImageFormat format)
    {
        NotNull(format);
        Equal(expected, format.Format);
        Equal("", format.MimeType);
    }

    private static void AssertOneFileInfo(string expectedType, IImageFormat imageInfo)
    {
        NotNull(imageInfo);
        Equal(expectedType, imageInfo.Format);
        Equal(ImageConstants.FileTypes[expectedType].MimeType, imageInfo.MimeType);
    }

}