using System;
using System.Collections.Specialized;
using System.Linq;
using Connect.Koi;
using ToSic.Eav.Apps.Decorators;
using ToSic.Eav.Configuration;
using ToSic.Eav.Documentation;
using ToSic.Eav.Logging;
using ToSic.Razor.Blade;
using ToSic.Sxc.Data;
using ToSic.Sxc.Web.Url;
using static ToSic.Sxc.Configuration.Features.BuiltInFeatures;
using static ToSic.Sxc.Images.ImageConstants;
using static ToSic.Sxc.Images.RecipeVariant;

namespace ToSic.Sxc.Images
{
    [PrivateApi("Internal stuff")]
    public class ImgResizeLinker : HasLog, ICanDebug
    {
        public ImgResizeLinker(Lazy<IFeaturesInternal> features, Lazy<ICss> koi) : base($"{Constants.SxcLogName}.ImgRes")
        {
            _features = features;
            _koi = koi;
            DimGen = new ResizeDimensionGenerator().Init(Log);
        }
        private readonly Lazy<IFeaturesInternal> _features;
        private readonly Lazy<ICss> _koi;

        public bool Debug { get; set; }

        public readonly ResizeDimensionGenerator DimGen;

        /// <summary>
        /// Make sure this is in sync with the Link.Image
        /// </summary>
        public string Image(
            string url = default,
            object settings = default,
            object factor = default,
            string noParamOrder = Eav.Parameters.Protector,
            IDynamicField field = default,  // todo
            object width = default,
            object height = default,
            object quality = default,
            string resizeMode = default,
            string scaleMode = default,
            string format = default,
            object aspectRatio = default,
            string parameters = default
            )
        {
            var wrapLog = (Debug ? Log : null).Fn<string>($"{nameof(url)}:{url}");

            // Modern case - all settings have already been prepared, the other settings are ignored
            if (settings is ResizeSettings resizeSettings)
            {
                var basic = ImageOnly(url, resizeSettings, field).Url;
                return wrapLog.Return(basic, "prepared:" + basic);
            }

            resizeSettings = ResizeParamMerger.BuildResizeSettings(
                settings, factor: factor, width: width, height: height, quality: quality, resizeMode: resizeMode,
                scaleMode: scaleMode, format: format, aspectRatio: aspectRatio,
                parameters: parameters);

            var result = ImageOnly(url, resizeSettings, field).Url;
            return wrapLog.Return(result, "built:" + result);
        }
        
        public OneResize ImageOnly(string url, ResizeSettings settings, IDynamicField field)
        {
            var wrapLog = Log.Fn<OneResize>();
            var srcSetSettings = settings.Find(SrcSetType.Img, _features.Value.IsEnabled(ImageServiceUseFactors), _koi.Value.Framework);
            return wrapLog.Return(ConstructUrl(url, settings, srcSetSettings, field), "no srcset");
        }
        

        public string SrcSet(string url, ResizeSettings settings, SrcSetType srcSetType, IDynamicField field = null)
        {
            var wrapLog = Log.Fn<string>();

            var srcSetSettings = settings.Find(srcSetType, _features.Value.IsEnabled(ImageServiceUseFactors), _koi.Value.Framework);

            var srcSetParts = srcSetSettings?.VariantsParsed;

            // Basic case -no srcSet config. In this case the src-set can just contain the url.
            if ((srcSetParts?.Length ?? 0) == 0)
                return wrapLog.Return(ConstructUrl(url, settings, srcSetSettings, field).Url, "no srcset");

            var results = srcSetParts.Select(ssPart =>
            {
                if (ssPart.SizeType == SizeDefault)
                    return ConstructUrl(url, settings, srcSetSettings, null, ssPart);

                var one = ConstructUrl(url, settings, srcSetSettings, field: field, partDef: ssPart);
                // this must happen at the end
                one.Suffix = ssPart.SrcSetSuffix(one.Width); // SrcSetParser.SrcSetSuffix(ssPart, one.Width);
                return one;
            });
            var result = string.Join(",\n", results.Select(r => r.UrlWithSuffix));

            return wrapLog.Return(result, "srcset");
        }



        private OneResize ConstructUrl(string url, ResizeSettings resizeSettings, Recipe srcSetSettings, IDynamicField field, RecipeVariant partDef = null)
        {
            var one = DimGen.ResizeDimensions(resizeSettings, srcSetSettings, partDef);
            one.Recipe = srcSetSettings;

            var imgDecorator = field?.ImageDecoratorOrNull;

            var resizeMode = resizeSettings.ResizeMode;
            if (imgDecorator?.CropBehavior == ImageDecorator.NoCrop)
            {
                resizeMode = ImageConstants.ModeMax;
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


        private bool ImgAddIfRelevant(NameValueCollection resizer, string key, object value, string irrelevant = "")
        {
            var wrapLog = (Debug ? Log : null).Fn<bool>();
            if (key == null || value == null)
                return wrapLog.Return(false, $"Won't add '{key}', since key or value are null");

            var strValue = value.ToString();
            if (string.IsNullOrEmpty(strValue))
                return wrapLog.Return(false, $"Won't add '{key}' since value as string would be null");

            if (strValue.Equals(irrelevant, StringComparison.InvariantCultureIgnoreCase))
                return wrapLog.Return(false, $"Won't add '{key}' since value would be irrelevant");

            resizer.Add(key, strValue);
            return wrapLog.Return(true, $"Added key {key}");
        }


        internal ResizeParamMerger ResizeParamMerger
        {
            get
            {
                if (_resizeParamMerger != null) return _resizeParamMerger;
                _resizeParamMerger = new ResizeParamMerger().Init(Log);
                if (Debug) _resizeParamMerger.Debug = true;
                return _resizeParamMerger;
            }
        }
        private ResizeParamMerger _resizeParamMerger;
        
    }
}
