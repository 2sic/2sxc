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
            string srcSet = null)
        {
            var wrapLog = (Debug ? Log : null).SafeCall<string>($"{nameof(url)}:{url}, {nameof(srcSet)}:{srcSet}");

            var resizeParams = ResizeParamMerger.BuildResizeParameters(
                settings, factor, noParamOrder, width, height, quality, resizeMode, scaleMode, format, aspectRatio, parameters);
            
            var result = GenerateFinalUrlOrSrcSet(url, srcSet, resizeParams);

            return wrapLog(result, result);
        }

        private string GenerateFinalUrlOrSrcSet(string url, string srcSet, IResizeParameters originalParameters)
        {
            var srcSetConfig = SrcSetParser.ParseSet(srcSet);

            // Basic case - no srcSet config
            if ((srcSetConfig?.Length ?? 0) == 0)
                return ConstructUrl(url, originalParameters);

            var results = srcSetConfig.Select(part =>
            {
                if (part.SizeType == SizeDefault)
                    return ConstructUrl(url, originalParameters);

                // Copy the params so we can optimize based on the expected SrcSet specs
                var partParams = new ResizeParameters(originalParameters);
                
                partParams.Width = BestSrcSetDimension(partParams.Width, part.Width, part, FallbackWidthForSrcSet);
                partParams.Height = BestSrcSetDimension(partParams.Height, part.Height, part, FallbackHeightForSrcSet);

                var size = part.Size;
                var sizeTypeCode = part.SizeType;
                if (sizeTypeCode == SizeFactorOf)
                {
                    size = partParams.Width;
                    sizeTypeCode = SizeWidth;
                }

                return $"{ConstructUrl(url, partParams)} {size.ToString(CultureInfo.InvariantCulture)}{sizeTypeCode}";
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

            // Calculate the expected size based on the original and factor
            return (int)(part.Size * original);
        }

        private string ConstructUrl(string url, IResizeParameters resizeParameters)
        {
            var resizerNvc = new NameValueCollection();
            ImgAddIfRelevant(resizerNvc, "w", resizeParameters.Width, "0");
            ImgAddIfRelevant(resizerNvc, "h", resizeParameters.Height, "0");
            ImgAddIfRelevant(resizerNvc, "quality", resizeParameters.Quality, "0");
            ImgAddIfRelevant(resizerNvc, "mode", resizeParameters.Mode, DontSetParam);
            ImgAddIfRelevant(resizerNvc, "scale", resizeParameters.Scale, DontSetParam);
            ImgAddIfRelevant(resizerNvc, "format", resizeParameters.Format, DontSetParam);

            url = UrlHelpers.AddQueryString(url, resizerNvc);

            if (resizeParameters.Parameters != null && resizeParameters.Parameters.HasKeys())
                url = UrlHelpers.AddQueryString(url, resizeParameters.Parameters);

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
