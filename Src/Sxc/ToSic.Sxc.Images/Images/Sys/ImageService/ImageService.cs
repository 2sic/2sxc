using ToSic.Lib.Helpers;
using ToSic.Sxc.Images.Internal;
using ToSic.Sxc.Services;
using ToSic.Sxc.Services.Internal;

namespace ToSic.Sxc.Images.Sys;

[ShowApiWhenReleased(ShowApiMode.Never)]
internal partial class ImageService(ImgResizeLinker imgLinker, IFeaturesService features)
    : ServiceWithContext(SxcLogName + ".ImgSvc", connect: [features, imgLinker]), IImageService
{
    #region Constructor and Inits

    internal ImgResizeLinker ImgLinker { get; } = imgLinker;
    internal IFeaturesService Features { get; } = features;

    internal IEditService? EditOrNull => ExCtx.GetService<IEditService>(reuse: true);

    internal IToolbarService? ToolbarOrNull => _toolbarSvc.Get(() => ExCtx.GetService<IToolbarService>(reuse: true));
    private readonly GetOnce<IToolbarService?> _toolbarSvc = new();

    [field: AllowNull, MaybeNull]
    private IPageService PageService => field ??= ExCtx.GetService<IPageService>(reuse: true);

    #endregion

    /// <inheritdoc />
    public IResponsiveImage Img(
        object? link = null,
        object? settings = null,
        NoParamOrder noParamOrder = default,
        Func<ITweakMedia, ITweakMedia>? tweak = default,
        object? factor = null,
        object? width = default,
        string? imgAlt = null,
        string? imgAltFallback = default,
        string? imgClass = null,
        object? imgAttributes = default,
        object? toolbar = default,
        object? recipe = null)
    {
        var specs = ResponsiveSpecsOfTarget.ExtractSpecs(link);
        var finalSettings = SettingsInternal(settings ?? specs.ResizeSettingsOrNull(), factor: factor, width: width, recipe: recipe);
        ITweakMedia tweaker = new TweakMedia(
            ImageSvc: this,
            TargetSpecs: specs,
            ResizeSettings: finalSettings,
            VDec: new(),
            Img: new(Class: imgClass, Alt: imgAlt, AltFallback: imgAltFallback, Attributes: TweakMedia.CreateAttribDic(imgAttributes, nameof(imgAttributes))),
            Pic: new(),
            ToolbarObj: toolbar
        );
        if (tweak != default) tweaker = tweak(tweaker);

        return new ResponsiveImage(
            this,
            PageService,
            new(specs, Tweaker: (TweakMedia)tweaker),
            Log);
    }


    /// <inheritdoc />
    public IResponsivePicture Picture(
        object? link = default,
        object? settings = default,
        NoParamOrder noParamOrder = default,
        Func<ITweakMedia, ITweakMedia>? tweak = default,
        object? factor = default,
        object? width = default,
        string? imgAlt = default,
        string? imgAltFallback = default,
        string? imgClass = default,
        object? imgAttributes = default,
        string? pictureClass = default,
        object? pictureAttributes = default,
        object? toolbar = default,
        object? recipe = default)
    {
        var specs = ResponsiveSpecsOfTarget.ExtractSpecs(link);
        var finalSettings = SettingsInternal(settings ?? specs.ResizeSettingsOrNull(), factor: factor, width: width, recipe: recipe);
        ITweakMedia tweaker = new TweakMedia(
            ImageSvc: this,
            TargetSpecs: specs,
            ResizeSettings: finalSettings,
            VDec: new(),
            Img: new(Class: imgClass, Alt: imgAlt, AltFallback: imgAltFallback, Attributes: TweakMedia.CreateAttribDic(imgAttributes, nameof(imgAttributes))),
            Pic: new(pictureClass, Attributes: TweakMedia.CreateAttribDic(pictureAttributes, nameof(pictureAttributes))),
            ToolbarObj: toolbar
        );
        if (tweak != default)
            tweaker = tweak(tweaker);
        return new ResponsivePicture(
            this,
            PageService,
            new(specs, Tweaker: (TweakMedia)tweaker),
            Log);
    }

    /// <inheritdoc />
    public override bool Debug
    {
        get;
        set
        {
            field = value;
            ImgLinker.Debug = value;
            Features.Debug = value;
        }
    }
}