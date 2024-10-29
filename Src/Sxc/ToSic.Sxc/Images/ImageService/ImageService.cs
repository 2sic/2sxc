using ToSic.Eav.Plumbing;
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

    internal IToolbarService ToolbarOrNull => _toolbarSvc.Get(() => _CodeApiSvc?.GetService<IToolbarService>());
    private readonly GetOnce<IToolbarService> _toolbarSvc = new();

    private IPageService PageService => _pageService ??= _CodeApiSvc?.GetService<IPageService>(reuse: true);
    private IPageService _pageService;

    #endregion

    #region Settings Handling

    /// <summary>
    /// Use the given settings or try to use the default content-settings if available
    /// </summary>
    /// <param name="settings"></param>
    /// <returns></returns>
    private object GetBestSettings(object settings)
    {
        var l = Log.Fn<object>(enabled: Debug);

        return settings switch
        {
            null or true => l.Return(GetImageSettingsByName("Content"), "null/default"),
            string strName when strName.HasValue() => l.Return(GetImageSettingsByName(strName), $"name: {strName}"),
            _ => l.Return(settings, "unchanged")
        };
    }

    
    private object GetImageSettingsByName(string strName) => ResizeParamMerger.GetImageSettingsByName(_CodeApiSvc, strName, Debug, Log);

    /// <summary>
    /// Convert to Multi-Resize Settings
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    private AdvancedSettings ToAdv(object value) => AdvancedSettings.Parse(value);

    #endregion

    /// <inheritdoc />
    public IResponsiveImage Img(
        object link = null,
        object settings = null,
        NoParamOrder noParamOrder = default,
        Func<ITweakImage, ITweakImage> tweak = default,
        object factor = null,
        object width = default,
        string imgAlt = null,
        string imgAltFallback = default,
        string imgClass = null,
        object imgAttributes = default,
        object toolbar = default,
        object recipe = null)
    {
        var prefetch = ResponsiveSpecsOfTarget.ExtractSpecs(link);
        var finalSettings = Settings(settings ?? prefetch.ResizeSettingsOrNull, factor: factor, width: width, recipe: recipe);
        ITweakImage tweaker = new TweakImage(
            new(),
            new(Class: imgClass, Alt: imgAlt, AltFallback: imgAltFallback, Attributes: TweakImage.CreateAttribDic(imgAttributes, nameof(imgAttributes))),
            new(),
            ToolbarObj: toolbar
        );
        if (tweak != default) tweaker = tweak(tweaker);

        return new ResponsiveImage(
            this,
            PageService,
            new(prefetch, Settings: finalSettings, Tweaker: tweaker as TweakImage),
            Log);
    }


    /// <inheritdoc />
    public IResponsivePicture Picture(
        object link = default,
        object settings = default,
        NoParamOrder noParamOrder = default,
        Func<ITweakImage, ITweakImage> tweak = default,
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
        var prefetch = ResponsiveSpecsOfTarget.ExtractSpecs(link);
        var finalSettings = Settings(settings ?? prefetch.ResizeSettingsOrNull, factor: factor, width: width, recipe: recipe);
        ITweakImage tweaker = new TweakImage(
            new(),
            new(Class: imgClass, Alt: imgAlt, AltFallback: imgAltFallback, Attributes: TweakImage.CreateAttribDic(imgAttributes, nameof(imgAttributes))),
            new(pictureClass, Attributes: TweakImage.CreateAttribDic(pictureAttributes, nameof(pictureAttributes))),
            ToolbarObj: toolbar
        );
        if (tweak != default) tweaker = tweak(tweaker);
        return new ResponsivePicture(
            this,
            PageService,
            new(prefetch, Settings: finalSettings, Tweaker: tweaker as TweakImage),
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

    //private static IDictionary<string, object> CreateAttribDic(object attributes, string name)
    //    => attributes switch
    //    {
    //        null => null,
    //        IDictionary<string, object> ok => ok.ToInvariant(),
    //        IDictionary<string, string> strDic => strDic.ToDictionary(pair => pair.Key, pair => pair.Value as object, InvariantCultureIgnoreCase),
    //        _ => attributes.IsAnonymous()
    //            ? attributes.ToDicInvariantInsensitive()
    //            : throw new ArgumentException($@"format of {name} unknown: {name.GetType().Name}", nameof(attributes))
    //    };
}