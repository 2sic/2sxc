using System.Collections.Concurrent;
using System.Collections.Specialized;
using Connect.Koi;
using ToSic.Eav.Context;
using ToSic.Eav.Internal.Features;
using ToSic.Eav.Metadata;
using ToSic.Lib.DI;
using ToSic.Lib.Services;
using ToSic.Razor.Blade;
using ToSic.Sxc.Code.Internal;
using ToSic.Sxc.Data;
using ToSic.Sxc.Web.Internal.Url;
using static ToSic.Sxc.Configuration.Internal.SxcFeatures;
using static ToSic.Sxc.Images.Internal.ImageConstants;
using static ToSic.Sxc.Images.Internal.ImageDecorator;
using static ToSic.Sxc.Images.RecipeVariant;

namespace ToSic.Sxc.Images.Internal;

[PrivateApi("Internal stuff")]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class ImgResizeLinker(
    LazySvc<IEavFeaturesService> features,
    LazySvc<ICss> koi,
    LazySvc<ISite> siteLazy,
    ResizeDimensionGenerator dimGen)
    : ServiceBase($"{SxcLogName}.ImgRes", connect: [features, koi, dimGen, siteLazy]), ICanDebug
{
    public bool Debug { get; set; }

    internal readonly ResizeDimensionGenerator DimGen = dimGen;

    /// <summary>
    /// Make sure this is in sync with the Link.Image
    /// </summary>
    public string Image(
        string url = default,
        object settings = default,
        object factor = default,
        NoParamOrder noParamOrder = default,
        IField field = default,  // todo
        object width = default,
        object height = default,
        object quality = default,
        string resizeMode = default,
        string scaleMode = default,
        string format = default,
        object aspectRatio = default,
        string parameters = default,
        ICodeApiService codeApiSvc = default
    )
    {
        var l = (Debug ? Log : null).Fn<string>($"{nameof(url)}:{url}");

        // Modern case - all settings have already been prepared, the other settings are ignored
        if (settings is ResizeSettings resizeSettings)
        {
            var basic = ImageOnly(url, resizeSettings, field).Url;
            return l.Return(basic, "prepared:" + basic);
        }

        resizeSettings = ResizeParamMerger.BuildResizeSettings(
            settings, factor: factor, width: width, height: height, quality: quality, resizeMode: resizeMode,
            scaleMode: scaleMode, format: format, aspectRatio: aspectRatio,
            parameters: parameters, codeApiSvc: codeApiSvc);

        var result = ImageOnly(url, resizeSettings, field).Url;
        return l.Return(result, "built:" + result);
    }
        
    internal OneResize ImageOnly(string url, ResizeSettings settings, IHasMetadata field)
    {
        var l = Log.Fn<OneResize>();
        var srcSetSettings = settings.Find(SrcSetType.Img, features.Value.IsEnabled(ImageServiceUseFactors), koi.Value.Framework);
        return l.Return(ConstructUrl(url, settings, srcSetSettings, field), "no srcset");
    }
        

    internal string SrcSet(string url, ResizeSettings settings, SrcSetType srcSetType, IHasMetadata field = null)
    {
        var l = Log.Fn<string>();

        var srcSetSettings = settings.Find(srcSetType, features.Value.IsEnabled(ImageServiceUseFactors), koi.Value.Framework);

        var srcSetParts = srcSetSettings?.VariantsParsed;

        // Basic case -no srcSet config. In this case the src-set can just contain the url.
        if ((srcSetParts?.Length ?? 0) == 0)
            return l.Return(ConstructUrl(url, settings, srcSetSettings, field).Url, "no srcset");

        var results = srcSetParts.Select(ssPart =>
        {
            if (ssPart.SizeType == SizeDefault)
                return ConstructUrl(url, settings, srcSetSettings, null, ssPart);

            var one = ConstructUrl(url, settings, srcSetSettings, field: field, partDef: ssPart);
            // this must happen at the end
            one.Suffix = ssPart.SrcSetSuffix(one.Width);
            return one;
        });
        var result = string.Join(",\n", results.Select(r => r.UrlWithSuffix));

        return l.Return(result, "srcset");
    }



    private OneResize ConstructUrl(string url, ResizeSettings resizeSettings, Recipe srcSetSettings, IHasMetadata field, RecipeVariant partDef = null)
    {
        var one = DimGen.ResizeDimensions(resizeSettings, srcSetSettings, partDef);
        one.Recipe = srcSetSettings;

        var imgDecorator = field == null ? null 
            : _imgDecCache.GetOrAdd(field, f => GetOrNull(f, siteLazy.Value.SafeLanguagePriorityCodes()));

        var resizeMode = resizeSettings.ResizeMode;
        if (imgDecorator?.CropBehavior == NoCrop)
        {
            resizeMode = ModeMax;
            one.ShowAll = true;
            one.Height = 0; // if we show all, the height may not match crop-height
        }

        var resizerNvc = new NameValueCollection();
        ImgAddIfRelevant(resizerNvc, "w", one.Width, "0");
        ImgAddIfRelevant(resizerNvc, "h", one.Height, "0");
        ImgAddIfRelevant(resizerNvc, "quality", resizeSettings.Quality, "0");
        ImgAddIfRelevant(resizerNvc, "mode", resizeMode, DontSetParam);
        ImgAddIfRelevant(resizerNvc, "scale", resizeSettings.ScaleMode, DontSetParam);
        ImgAddIfRelevant(resizerNvc, "format", resizeSettings.Format, DontSetParam);

        // Get resize instructions of the data if it has any
        var modifier = imgDecorator?.GetAnchorOrNull();
        if (modifier?.Param != null)
            ImgAddIfRelevant(resizerNvc, modifier.Value.Param, modifier.Value.Value);

        url = UrlHelpers.AddQueryString(url, resizerNvc);

        if (resizeSettings.Parameters != null && resizeSettings.Parameters.HasKeys())
            url = UrlHelpers.AddQueryString(url, resizeSettings.Parameters);

        var result = Tags.SafeUrl(url).ToString();
        one.Url = result;
        return one;
    }

    // cache buffer settings which had already been looked up
    private readonly ConcurrentDictionary<IHasMetadata, ImageDecorator> _imgDecCache = new();


    private bool ImgAddIfRelevant(NameValueCollection resizer, string key, object value, string irrelevant = "")
    {
        var l = (Debug ? Log : null).Fn<bool>();
        if (key == null || value == null)
            return l.ReturnFalse($"Won't add '{key}', since key or value are null");

        var strValue = value.ToString();
        if (string.IsNullOrEmpty(strValue))
            return l.ReturnFalse($"Won't add '{key}' since value as string would be null");

        if (strValue.Equals(irrelevant, StringComparison.InvariantCultureIgnoreCase))
            return l.ReturnFalse($"Won't add '{key}' since value would be irrelevant");

        resizer.Add(key, strValue);
        return l.ReturnTrue($"Added key {key}");
    }


    internal ResizeParamMerger ResizeParamMerger
    {
        get
        {
            if (_resizeParamMerger != null) return _resizeParamMerger;
            _resizeParamMerger = new(Log);
            if (Debug) _resizeParamMerger.Debug = true;
            return _resizeParamMerger;
        }
    }
    private ResizeParamMerger _resizeParamMerger;
        
}