using ToSic.Sxc.DataForImageTests;
using ToSic.Sxc.Images;
using ToSic.Sxc.Mocks;
using ToSic.Sxc.Services;
using Xunit.Abstractions;

// Disable warning that the test-name parameter is not used.
#pragma warning disable xUnit1026

namespace ToSic.Sxc.ServicesTests.ImageServiceTests;

/// <summary>
/// Start the test with a platform-info that has WebP support
/// </summary>
[Startup(typeof(StartupSxcWithDbPatronPerfectionist))]
public class ImageServiceTagsPatronPerfectionist(ExecutionContextMock exCtxMock, ITestOutputHelper output)
    : ImageServiceTagsBase(exCtxMock.GetService<IImageService>(), output), IClassFixture<DoFixtureStartup<ScenarioFullPatronsWithDb>>
{
    protected override bool TestModeImg => false;

    // These are necessary, as otherwise the test will automatically look up the "Content" settings for image resizing
    // which would result in a different result.
    // TODO: ALSO create a test which uses null, and expects the proper resized with default settings
    object fakeEmptySettings = new object();

    [InlineData(SrcWebPNone + SrcJpgNone, SrcSetNone, "No Src Set")]
    [InlineData(SrcWebP12 + SrcJpg12, SrcSet12, "With Src Set 1,2")]
    [Theory]
    public void SourceTagsMultiTests(string expected, string variants, string name) 
        => BatchTestManySrcSets(expected, variants, name);

    [InlineData(SrcWebPNone + SrcJpgNone, SrcSetNone, true, "No Src Set, in-pic")]
    [InlineData(SrcWebPNone + SrcJpgNone, SrcSetNone, false, "No Src Set, in-setting")]
    [InlineData(SrcWebP12 + SrcJpg12, SrcSet12, true, "With Src Set 1,2, in-pic")]
    [InlineData(SrcWebP12 + SrcJpg12, SrcSet12, false, "With Src Set 1,2, in-settings")]
    [Theory]
    public void PictureTags(string expected, string variants, bool inPicTag, string name)
        => PictureTagInner(expected, variants, inPicTag, name);


    [Theory]
    [InlineData("<img src='test.jpg?w=678' loading='lazy' test='value' class='img-fluid'>", 0.75, false, "0.75 with attributes, settings-object")]
    [InlineData("<img src='test.jpg?w=678' loading='lazy' test='value' class='img-fluid'>", 0.75, true, "0.75 with attributes, json-object")]
    public void ImgWhichShouldAutoGetAttributes(string expected, double factor, bool json, string name)
    {
        var set = json ? ResizeRecipesData.TestRecipeSetFromJson : ResizeRecipesData.TestRecipeSet();
        var img = ImgSvc.Img("test.jpg", factor: factor, recipe: set, settings: fakeEmptySettings);
        Equal(expected, img.ToString());//, name);
    }

    [Theory]
    [InlineData("<img src='test.jpg?w=1000&amp;h=500' class='manual img-fluid' srcset='test.jpg?w=1000&amp;h=500 1x' sizes='100vw'>", false, false, "neither")]
    [InlineData("<img src='test.jpg?w=1000&amp;h=500' class='manual img-fluid' width='1000' srcset='test.jpg?w=1000&amp;h=500 1x' sizes='100vw'>", true, false, "Width only")]
    [InlineData("<img src='test.jpg?w=1000&amp;h=500' class='manual img-fluid' height='500' srcset='test.jpg?w=1000&amp;h=500 1x' sizes='100vw'>", false, true, "Height only")]
    [InlineData("<img src='test.jpg?w=1000&amp;h=500' class='manual img-fluid' width='1000' height='500' srcset='test.jpg?w=1000&amp;h=500 1x' sizes='100vw'>", true, true, "both")]
    public void ImgWhichShouldSetWidth(string expected, bool setWidth, bool setHeight, string name)
    {
        var recipe = new Recipe(
            width: 1000,
            variants: "1",
            setWidth: setWidth,
            setHeight: setHeight,
            attributes: new Dictionary<string, object?>
            {
                { "class", "img-fluid" },
                { "sizes", "100vw" }
            });
        var settings = ImgSvc.SettingsTac(settings: fakeEmptySettings, aspectRatio: 2 / 1);
        var img = ImgSvc.Img("test.jpg", settings: settings, factor: 0.5, imgClass: "manual", recipe: recipe);
        Equal(expected, img.ToString());//, name);

    }

}