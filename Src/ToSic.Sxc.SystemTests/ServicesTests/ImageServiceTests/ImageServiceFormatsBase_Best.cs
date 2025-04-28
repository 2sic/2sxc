using ToSic.Sxc.Images.Internal;

namespace ToSic.Sxc.ServicesTests.ImageServiceTests;

public abstract partial class ImageServiceFormatsBase
{
    protected abstract int ExpectedPngFormats { get; }

    [InlineData("test.png")]
    [InlineData(".png")]
    [InlineData("png")]
    [InlineData("PNG")]
    [InlineData("test.png?w=2000")]
    [InlineData("/abc/test.png")]
    [InlineData("/abc/test.png?w=2000")]
    [InlineData("//domain.com/abc/test.png")]
    [InlineData("https://domain.com/abc/test.png")]
    [Theory]
    public void TestBestPng(string path)
    {
        var formats = imgSvc.GetResizeFormatTac(path);
        NotNull(formats);
        Equal(ExpectedPngFormats, formats.Count);
        // If we have many, we expect that the original will be listed as the second alternative
        if (formats.Count > 0)
            AssertOneFileInfo(ImageConstants.Png, formats.Skip(1).First());
    }

    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [Theory]
    public void TestBestEmpty(string path)
    {
        var formats = imgSvc.GetResizeFormatTac(path);
        NotNull(formats);
        Equal(0, formats.Count);
    }

    [InlineData("https://domain.com/abc/test.avif")]
    [Theory]
    public void TestBestUnknownAvif(string path)
    {
        var formats = imgSvc.GetResizeFormatTac(path);
        NotNull(formats);
        Equal(0, formats.Count);
    }

    [InlineData("https://domain.com/abc/test.svg?w=2000")]
    [Theory]
    public void TestBestSvg(string path)
    {
        var formats = imgSvc.GetResizeFormatTac(path);
        NotNull(formats);
        Equal(0, formats.Count);
    }
}