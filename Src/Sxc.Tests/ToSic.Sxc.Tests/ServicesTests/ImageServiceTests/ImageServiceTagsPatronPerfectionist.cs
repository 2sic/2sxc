using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToSic.Eav.Context;
using ToSic.Sxc.Images;
using ToSic.Sxc.Services;
using ToSic.Sxc.Tests.DataForImageTests;
using ToSic.Testing.Shared.Platforms;
using static ToSic.Testing.Shared.TestHelpers;

namespace ToSic.Sxc.Tests.ServicesTests.ImageServiceTests;

[TestClass]
public class ImageServiceTagsPatronPerfectionist: ImageServiceTagsBase
{
    /// <summary>
    /// Start the test with a platform-info that has WebP support
    /// </summary>
    protected override void SetupServices(IServiceCollection services)
    {
        base.SetupServices(services);
        services.AddTransient<IPlatformInfo, TestPlatformPatronPerfectionist>();
    }

    protected override bool TestModeImg => false;


    [DataRow(SrcWebPNone + SrcJpgNone, SrcSetNone, "No Src Set")]
    [DataRow(SrcWebP12 + SrcJpg12, SrcSet12, "With Src Set 1,2")]
    [DataTestMethod]
    public void SourceTagsMultiTests(string expected, string variants, string name) 
        => SourceTagsMultiTest(expected, variants, name);

    [DataRow(SrcWebPNone + SrcJpgNone, SrcSetNone, true, "No Src Set, in-pic")]
    [DataRow(SrcWebPNone + SrcJpgNone, SrcSetNone, false, "No Src Set, in-setting")]
    [DataRow(SrcWebP12 + SrcJpg12, SrcSet12, true, "With Src Set 1,2, in-pic")]
    [DataRow(SrcWebP12 + SrcJpg12, SrcSet12, false, "With Src Set 1,2, in-settings")]
    [DataTestMethod]
    public void PictureTags(string expected, string variants, bool inPicTag, string name)
        => PictureTagInner(expected, variants, inPicTag, name);


    [DataRow("<img src='test.jpg?w=678' loading='lazy' test='value' class='img-fluid'>", 0.75, false, "0.75 with attributes, settings-object")]
    [DataRow("<img src='test.jpg?w=678' loading='lazy' test='value' class='img-fluid'>", 0.75, true, "0.75 with attributes, json-object")]
    [DataTestMethod]
    public void ImgWhichShouldAutoGetAttributes(string expected, double factor, bool json, string name)
    {
        var set = json ? ResizeRecipesData.TestRecipeSetFromJson : ResizeRecipesData.TestRecipeSet();
        var svc = GetService<IImageService>();
        var img = svc.Img("test.jpg", factor: factor, recipe: set);
        Is(expected, img.ToString(), name);
    }

    [DataRow("<img src='test.jpg?w=1000&amp;h=500' class='manual img-fluid' srcset='test.jpg?w=1000&amp;h=500 1x' sizes='100vw'>", false, false, "neither")]
    [DataRow("<img src='test.jpg?w=1000&amp;h=500' class='manual img-fluid' width='1000' srcset='test.jpg?w=1000&amp;h=500 1x' sizes='100vw'>", true, false, "Width only")]
    [DataRow("<img src='test.jpg?w=1000&amp;h=500' class='manual img-fluid' height='500' srcset='test.jpg?w=1000&amp;h=500 1x' sizes='100vw'>", false, true, "Height only")]
    [DataRow("<img src='test.jpg?w=1000&amp;h=500' class='manual img-fluid' width='1000' height='500' srcset='test.jpg?w=1000&amp;h=500 1x' sizes='100vw'>", true, true, "both")]
    [DataTestMethod]
    public void ImgWhichShouldSetWidth(string expected, bool setWidth, bool setHeight, string name)
    {
        var recipe = new Recipe(width: 1000, variants: "1", setWidth: setWidth, setHeight: setHeight,
            attributes: new Dictionary<string, object>
            {
                { "class", "img-fluid" },
                { "sizes", "100vw" }
            });
        var svc = GetService<IImageService>();
        var settings = svc.SettingsTac(aspectRatio: 2 / 1);
        var img = svc.Img("test.jpg", settings: settings, factor: 0.5, imgClass: "manual", recipe: recipe);
        Is(expected, img.ToString(), name);

    }

}