using ToSic.Lib.Coding;
using ToSic.Sxc.Data.Internal.Wrapper;
using ToSic.Sxc.Images;
using ToSic.Sxc.Images.Internal;
using ToSic.Sxc.LinksAndImages.LinkHelperTests;
using ToSic.Sxc.Services;
using ToSic.Sys.Capabilities.Features;
using ToSic.Sys.GetByName;

namespace ToSic.Sxc.LinksAndImages.LinkImageTests;

public class LinkImageTestHelper
{
    private readonly ICodeDataPoCoWrapperService _wrapper;
    private readonly ImgResizeLinker _linker;
    private readonly ILinkService _linkHelper;

    public LinkImageTestHelper(ICodeDataPoCoWrapperService wrapper, ImgResizeLinker linker, ILinkService linkHelper, EavFeaturesLoader featuresLoader)
    {
        _wrapper = wrapper;
        _linker = linker;
        _linkHelper = linkHelper;
        featuresLoader.LoadLicenseAndFeatures();
    }

    public ImgResizeLinker GetLinker() => _linker;
    public ILinkService GetLinkHelper() => _linkHelper;

    public /*WrapObjectDynamic*/ICanGetByName ToDyn(object contents) => (ICanGetByName)_wrapper
        .DynamicFromObject(contents, WrapperSettings.Dyn(children: false, realObjectsToo: false));


    public void TestOnLinkerAndHelper(string expected,
        string url = null,
        object settings = null,
        object factor = null,
        NoParamOrder noParamOrder = default,
        object width = null,
        object height = null,
        object quality = null,
        string resizeMode = null,
        string scaleMode = null,
        string format = null,
        object aspectRatio = null)
    {
        // Test with Linker
        var linker = GetLinker();
        var linkerResult = linker.Image(url: url, settings: settings, factor: factor, width: width, height: height,
            quality: quality, resizeMode: resizeMode, scaleMode: scaleMode, format: format,
            aspectRatio: aspectRatio);
        Equal(expected, linkerResult);//, "Failed on ImgResizeLinker");

        // Skip Helper-tests if using SrcSet as that's not supported in that case
        // Because it would lead to not-expected result
        //if (variants != null) return;

        var linkHelper = GetLinkHelper();
        var helperResult = linkHelper.TestImage(url: url, settings: settings, factor: factor, width: width,
            height: height,
            quality: quality, resizeMode: resizeMode, scaleMode: scaleMode, format: format,
            aspectRatio: aspectRatio);
        Equal(expected, helperResult);//, "Failed on ILinkHelper");
    }


    public void TestOnLinkerSrcSet(string expected,
        string url = null,
        object settings = null,
        object factor = null,
        NoParamOrder noParamOrder = default,
        object width = null,
        object height = null,
        object quality = null,
        string resizeMode = null,
        string scaleMode = null,
        string format = null,
        object aspectRatio = null,
        string variants = null)
    {
        // Test with Linker
        var linker = GetLinker();
        var typedSettings = linker.ResizeParamMerger.BuildResizeSettings(settings: settings, factor: factor,
            width: width,
            height: height,
            quality: quality, resizeMode: resizeMode, scaleMode: scaleMode, format: format,
            aspectRatio: aspectRatio, advanced: AdvancedSettings.Parse(new Recipe(variants: variants)));
        var linkerResult = linker.SrcSet(url, typedSettings, SrcSetType.Img);
        Equal(expected, linkerResult);//, $"Failed on ImgResizeLinker for srcSet '{variants}'");
    }
}