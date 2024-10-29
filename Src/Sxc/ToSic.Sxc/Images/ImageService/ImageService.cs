using ToSic.Lib.Helpers;
using ToSic.Sxc.Images.Internal;
using ToSic.Sxc.Services;
using ToSic.Sxc.Services.Internal;

namespace ToSic.Sxc.Images;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
internal partial class ImageService(ImgResizeLinker imgLinker, IFeaturesService features)
    : ServiceForDynamicCode(SxcLogName + ".ImgSvc", connect: [features, imgLinker]), IImageService
{
    #region Constructor and Inits

    internal ImgResizeLinker ImgLinker { get; } = imgLinker;
    internal IFeaturesService Features { get; } = features;

    internal IEditService EditOrNull => _CodeApiSvc?.Edit;

    internal IToolbarService ToolbarOrNull => _toolbarSvc.Get(() => _CodeApiSvc?.GetService<IToolbarService>(reuse: true));
    private readonly GetOnce<IToolbarService> _toolbarSvc = new();

    private IPageService PageService => _pageService ??= _CodeApiSvc?.GetService<IPageService>(reuse: true);
    private IPageService _pageService;

    #endregion

    /// <inheritdoc />
    public IResponsiveImage Img(
        object link = null,
        object settings = null,
        NoParamOrder noParamOrder = default,
        Func<ITweakMedia, ITweakMedia> tweak = default,
        object factor = null,
        object width = default,
        string imgAlt = null,
        string imgAltFallback = default,
        string imgClass = null,
        object imgAttributes = default,
        object toolbar = default,
        object recipe = null)
    {
        var specs = ResponsiveSpecsOfTarget.ExtractSpecs(link);
        var finalSettings = SettingsInternal(settings ?? specs.ResizeSettingsOrNull, factor: factor, width: width, recipe: recipe);
        ITweakMedia tweaker = new TweakMedia(
            this,
            specs,
            finalSettings,
            new(),
            new(Class: imgClass, Alt: imgAlt, AltFallback: imgAltFallback, Attributes: TweakMedia.CreateAttribDic(imgAttributes, nameof(imgAttributes))),
            new(),
            ToolbarObj: toolbar
        );
        if (tweak != default) tweaker = tweak(tweaker);

        return new ResponsiveImage(
            this,
            PageService,
            new(specs, Tweaker: tweaker as TweakMedia),
            Log);
    }


    /// <inheritdoc />
    public IResponsivePicture Picture(
        object link = default,
        object settings = default,
        NoParamOrder noParamOrder = default,
        Func<ITweakMedia, ITweakMedia> tweak = default,
        object factor = default,
        object width = default,
        string imgAlt = default,
        string imgAltFallback = default,
        string imgClass = default,
        object imgAttributes = default,
        string pictureClass = default,
        object pictureAttributes = default,
        object toolbar = default,
        object recipe = default)
    {
        var specs = ResponsiveSpecsOfTarget.ExtractSpecs(link);
        var finalSettings = SettingsInternal(settings ?? specs.ResizeSettingsOrNull, factor: factor, width: width, recipe: recipe);
        ITweakMedia tweaker = new TweakMedia(
            this,
            specs,
            finalSettings,
            new(),
            new(Class: imgClass, Alt: imgAlt, AltFallback: imgAltFallback, Attributes: TweakMedia.CreateAttribDic(imgAttributes, nameof(imgAttributes))),
            new(pictureClass, Attributes: TweakMedia.CreateAttribDic(pictureAttributes, nameof(pictureAttributes))),
            ToolbarObj: toolbar
        );
        if (tweak != default) tweaker = tweak(tweaker);
        return new ResponsivePicture(
            this,
            PageService,
            new(specs, Tweaker: tweaker as TweakMedia),
            Log);
    }

    /// <inheritdoc />
    public override bool Debug
    {
        get => _debug;
        set
        {
            _debug = value;
            ImgLinker.Debug = value;
            Features.Debug = value;
        }
    }
    private bool _debug;

}