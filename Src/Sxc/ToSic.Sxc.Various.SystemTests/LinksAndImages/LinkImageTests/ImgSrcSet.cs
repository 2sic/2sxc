﻿using ToSic.Sxc.Images;
using ToSic.Sxc.Images.Sys;

namespace ToSic.Sxc.LinksAndImages.LinkImageTests;


public class ImgSrcSet(LinkImageTestHelper helper)// : LinkImageTestBase
{
    [Theory]
    [InlineData("test.jpg")]
    [InlineData("test.png")]
    [InlineData("/test.jpg")]
    [InlineData("//test.jpg")]
    [InlineData("http://www.2sxc.org/test.jpg")]
    [InlineData("weird-extension.abc")]
    public void EmptySrcSet(string url) 
        => helper.TestOnLinkerSrcSet(url, url, variants: "d");

    [Theory]
    [InlineData("test.jpg?w=1000 1000w", "test.jpg", "1000")]
    [InlineData("test.jpg?w=1000 1000w", "test.jpg", "1000w")]
    [InlineData("test.jpg?w=1000 1000w,\ntest.jpg?w=2000 2000w", "test.jpg", "1000,2000")]
    [InlineData("test.jpg?w=500 500w,\ntest.jpg?w=1000 1000w,\ntest.jpg?w=2000 2000w", "test.jpg", "500w,1000w,2000w")]
    public void SrcSetUrlOnlyW(string expected, string url, string variants) 
        => helper.TestOnLinkerSrcSet(expected, url, variants: variants);



    [Theory]
    [InlineData("test.jpg 1x", "test.jpg", "1")]
    [InlineData("test.jpg 1.5x", "test.jpg", "1.5x")]
    public void SrcSetUrlOnlyX(string expected, string url, string variants) 
        => helper.TestOnLinkerSrcSet(expected, url, variants: variants);

    [Theory]
    [InlineData("test.jpg?w=1800 1.5x", "test.jpg", "1.5x")]
    [InlineData("test.jpg?w=1200 1x", "test.jpg", "1")]
    [InlineData("test.jpg?w=1200 1x,\ntest.jpg?w=1800 1.5x,\ntest.jpg?w=2400 2x", "test.jpg", "1x,1.5x,2")]
    [InlineData("test.jpg?w=1200 1x,\ntest.jpg?w=1800 1.5x,\ntest.jpg?w=2000 2x", "test.jpg", "1x,1.5x,2x=2000")]
    [InlineData("test.jpg?w=1200 1x,\ntest.jpg?w=1800 1.5x,\ntest.jpg?w=2000&h=1000 2x", "test.jpg", "1x,1.5x,2x=2000:1000")]
    public void SrcSetUrlXAndWidth(string expected, string url, string variants)
        => helper.TestOnLinkerSrcSet(expected, url, width: 1200, variants: variants);




    [Theory]
    [InlineData("test.jpg?w=1200 1200w", "test.jpg", "1*")]
    [InlineData("test.jpg?w=1800 1800w", "test.jpg", "1.5*")]
    [InlineData("test.jpg?w=600 600w", "test.jpg", "0.5*")]
    [InlineData("test.jpg?w=600 600w", "test.jpg", "1:2*")]
    [InlineData("test.jpg?w=600 600w", "test.jpg", "1/2*")]
    [InlineData("test.jpg?w=600 600w", "test.jpg", "1/2")] // without '*' it auto-detects a proportion
    [InlineData("test.jpg?w=600 600w", "test.jpg", "1:2")] // without '*' it auto-detects a proportion
    public void SrcSetUrlOnlyStar(string expected, string url, string variants) 
        => helper.TestOnLinkerSrcSet(expected, url, variants: variants);

    [Theory]
    [InlineData("test.jpg?w=120 120w", "test.jpg", "1*")]
    [InlineData("test.jpg?w=180 180w", "test.jpg", "1.5*")]
    [InlineData("test.jpg?w=120 120w,\ntest.jpg?w=180 180w,\ntest.jpg?w=240 240w", "test.jpg", "1*,1.5*,2*")]
    // Note: These two cases don't make sense but could be configured - as soon as you give an exact pixel, the 2* doesn't do much
    [InlineData("test.jpg?w=120 120w,\ntest.jpg?w=200 200w", "test.jpg", "1*,2*=200")]
    [InlineData("test.jpg?w=120 120w,\ntest.jpg?w=200&h=180 200w", "test.jpg", "1*,2*=200:180")]
    public void SrcSetUrlStarAndWidth(string expected, string url, string variants)
        => helper.TestOnLinkerSrcSet(expected, url, width: 120, variants: variants);


    [Fact]
    public void WipDoubleResize()
    {
        var linker = helper.GetLinker();
        var settings = linker.ResizeParamMerger.BuildResizeSettings(width: 1000, factor: 0.5, advanced: AdvancedSettings.Parse("0.5"));
        var src = linker.SrcSet("test.jpg", settings, SrcSetType.Img);
        Equal("test.jpg?w=250 250w", src);//, "Src should be a quarter now");

    }
}