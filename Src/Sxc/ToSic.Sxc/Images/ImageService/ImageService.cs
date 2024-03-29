﻿using ToSic.Eav.Generics;
using ToSic.Eav.Plumbing;
using ToSic.Lib.Helpers;
using ToSic.Sxc.Images.Internal;
using ToSic.Sxc.Services;
using ToSic.Sxc.Services.Internal;
using static System.StringComparer;

namespace ToSic.Sxc.Images;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
internal partial class ImageService: ServiceForDynamicCode, IImageService
{
    #region Constructor and Inits

    public ImageService(ImgResizeLinker imgLinker, IFeaturesService features) : base(SxcLogName + ".ImgSvc")
    {
        ConnectServices(
            Features = features,
            ImgLinker = imgLinker
        );
    }

    internal ImgResizeLinker ImgLinker { get; }
    internal IFeaturesService Features { get; }

    internal IEditService EditOrNull => _CodeApiSvc?.Edit;

    internal IToolbarService ToolbarOrNull => _toolbarSvc.Get(() => _CodeApiSvc?.GetService<IToolbarService>());
    private readonly GetOnce<IToolbarService> _toolbarSvc = new();

    #endregion

    #region Settings Handling

    /// <summary>
    /// Use the given settings or try to use the default content-settings if available
    /// </summary>
    /// <param name="settings"></param>
    /// <returns></returns>
    private object GetBestSettings(object settings)
    {
        var l = Debug ? Log.Fn<object>() : null;

        return settings switch
        {
            null or true => l.Return(GetCodeRootSettingsByName("Content"), "null/default"),
            string strName when strName.HasValue() => l.Return(GetCodeRootSettingsByName(strName), $"name: {strName}"),
            _ => l.Return(settings, "unchanged")
        };
    }

    private object GetCodeRootSettingsByName(string strName)
    {
        var l = Debug ? Log.Fn<object>($"{strName}; code root: {_CodeApiSvc != null}") : null;
        var result = _CodeApiSvc?.Settings?.Get($"Settings.Images.{strName}");
        return l.Return((object)result, $"found: {result != null}");
    }

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
        object factor = null,
        object width = default,
        string imgAlt = null,
        string imgAltFallback = default,
        string imgClass = null,
        object imgAttributes = default,
        object toolbar = default,
        object recipe = null)
        => new ResponsiveImage(this,
            new(link, noParamOrder,
                Settings(settings, factor: factor, width: width, recipe: recipe),
                imgAlt: imgAlt, imgAltFallback: imgAltFallback, imgClass: imgClass, imgAttributes: CreateAttribDic(imgAttributes), toolbar: toolbar),
            Log);


    /// <inheritdoc />
    public IResponsivePicture Picture(
        object link = default,
        object settings = default,
        NoParamOrder noParamOrder = default,
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
        => new ResponsivePicture(this,
            new(link, noParamOrder,
                Settings(settings, factor: factor, width: width, recipe: recipe),
                imgAlt: imgAlt, imgAltFallback: imgAltFallback,
                imgClass: imgClass, imgAttributes: CreateAttribDic(imgAttributes), pictureClass: pictureClass, pictureAttributes: CreateAttribDic(pictureAttributes), toolbar: toolbar),
            Log);

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

    private IDictionary<string, object> CreateAttribDic(object attributes)
    {
        if (attributes == null) return null;
        if (attributes is IDictionary<string, object> ok) return ok.ToInvariant();
        if (attributes is IDictionary<string, string> strDic)
            return strDic.ToDictionary(pair => pair.Key, pair => pair.Value as object, InvariantCultureIgnoreCase);
        if (attributes.IsAnonymous()) return attributes.ToDicInvariantInsensitive();
        throw new ArgumentException("format unknown", nameof(attributes));
    }
}