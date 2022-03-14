using System;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using ToSic.Eav.Documentation;
using ToSic.Eav.Logging;
using ToSic.Razor.Blade;
using ToSic.Sxc.Web.Url;
using static ToSic.Sxc.Images.ImageConstants;
using static ToSic.Sxc.Images.SrcSetPart;

namespace ToSic.Sxc.Images
{
    [PrivateApi("Internal stuff")]
    public class ImgResizeLinker : HasLog<ImgResizeLinker>
    {
        public ImgResizeLinker() : base($"{Constants.SxcLogName}.ImgRes") { }

        public bool Debug = false;

        /// <summary>
        /// Make sure this is in sync with the Link.Image
        /// </summary>
        public string Image(
            string url = null,
            object settings = null,
            object factor = null,
            string noParamOrder = Eav.Parameters.Protector,
            object width = null,
            object height = null,
            object quality = null,
            string resizeMode = null,
            string scaleMode = null,
            string format = null,
            object aspectRatio = null,
            string parameters = null,
            object srcset = null // can be a string or a bool:true 
            )
        {
            var wrapLog = (Debug ? Log : null).SafeCall<string>($"{nameof(url)}:{url}, {nameof(srcset)}:{srcset}");

            if (!(settings is IResizeSettings resizeSettings))
                resizeSettings = ResizeParamMerger.BuildResizeSettings(
                    settings, factor, width: width, height: height, quality: quality, resizeMode: resizeMode,
                    scaleMode: scaleMode, format: format, aspectRatio: aspectRatio,
                    parameters: parameters, srcset: srcset);

            var result = GenerateFinalUrlOrSrcSet(url, resizeSettings);

            return wrapLog(result, result);
        }

        public string Image(string url, IResizeSettings settings) => GenerateFinalUrlOrSrcSet(url, settings);

        private string GenerateFinalUrlOrSrcSet(string url, IResizeSettings originalSettings)
        {
            var srcSetConfig = SrcSetParser.ParseSet(originalSettings.SrcSet);

            // Basic case - no srcSet config
            if ((srcSetConfig?.Length ?? 0) == 0)
                return ConstructUrl(url, new ResizeSettings(originalSettings).ApplyFactor());

            var results = srcSetConfig.Select(ssConfig =>
            {
                // Copy the params so we can optimize based on the expected SrcSet specs
                var currentSet = new ResizeSettings(originalSettings, false);

                if (ssConfig.SizeType == SizeDefault)
                    return ConstructUrl(url, currentSet.ApplyFactor());
                
                // Factor is usually 1, but in srcSet scenarios it can have another value
                // Because the settings that made it didn't get incorporated first
                var f = originalSettings.Factor;
                currentSet.Width = BestSrcSetDimension(currentSet.Width, ssConfig.Width, ssConfig,
                    FallbackWidthForSrcSet);
                currentSet.Height = BestSrcSetDimension(currentSet.Height, ssConfig.Height, ssConfig,
                    FallbackHeightForSrcSet);

                currentSet.ApplyFactor();

                return ConstructUrl(url, currentSet) + SrcSetParser.SrcSetSuffix(ssConfig, currentSet.Width);
            });
            var result = string.Join(",\n", results);

            return result;
        }


        /// <summary>
        /// Get the best matching dimension (width/height) based on what's specified
        /// </summary>
        private int BestSrcSetDimension(int original, int onSrcSet, SrcSetPart part, int fallbackIfNoOriginal)
        {
            // SrcSet defined a value, use that
            if (onSrcSet != 0) return onSrcSet;

            // No need to recalculate anything, return original
            if (part.SizeType != SizePixelDensity && part.SizeType != SizeFactorOf) return original;

            // If we're doing a factor-of, we always need an original value. If it's missing, use the fallback
            if (part.SizeType == SizeFactorOf && original == 0) original = fallbackIfNoOriginal;

            // Calculate the expected value based on Size=Scale-Factor * original
            return (int)(part.Size * original);
        }

        private string ConstructUrl(string url, IResizeSettings resizeSettings)
        {
            var resizerNvc = new NameValueCollection();
            ImgAddIfRelevant(resizerNvc, "w", resizeSettings.Width, "0");
            ImgAddIfRelevant(resizerNvc, "h", resizeSettings.Height, "0");
            ImgAddIfRelevant(resizerNvc, "quality", resizeSettings.Quality, "0");
            ImgAddIfRelevant(resizerNvc, "mode", resizeSettings.ResizeMode, DontSetParam);
            ImgAddIfRelevant(resizerNvc, "scale", resizeSettings.ScaleMode, DontSetParam);
            ImgAddIfRelevant(resizerNvc, "format", resizeSettings.Format, DontSetParam);

            url = UrlHelpers.AddQueryString(url, resizerNvc);

            if (resizeSettings.Parameters != null && resizeSettings.Parameters.HasKeys())
                url = UrlHelpers.AddQueryString(url, resizeSettings.Parameters);

            var result = Tags.SafeUrl(url).ToString();
            return result;
        }


        private bool ImgAddIfRelevant(NameValueCollection resizer, string key, object value, string irrelevant = "")
        {
            var wrapLog = (Debug ? Log : null).SafeCall<bool>();
            if (key == null || value == null)
                return wrapLog($"Won't add '{key}', since key or value are null", false);

            var strValue = value.ToString();
            if (string.IsNullOrEmpty(strValue))
                return wrapLog($"Won't add '{key}' since value as string would be null", false);

            if (strValue.Equals(irrelevant, StringComparison.InvariantCultureIgnoreCase))
                return wrapLog($"Won't add '{key}' since value would be irrelevant", false);

            resizer.Add(key, strValue);
            return wrapLog($"Added key {key}", true);
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
