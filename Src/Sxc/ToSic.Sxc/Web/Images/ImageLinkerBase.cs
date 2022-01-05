using System;
using System.Collections.Specialized;
using System.Linq;
using ToSic.Eav.Logging;
using ToSic.Razor.Blade;
using ToSic.Sxc.Data;
using ToSic.Sxc.Web.Url;
using static ToSic.Sxc.Plumbing.ParseObject;
using static ToSic.Sxc.Web.Images.SrcSetPart;

namespace ToSic.Sxc.Web.Images
{
    public abstract partial class ImageLinkerBase: HasLog<ImageLinkerBase>
    {
        internal const string DontSetParam = "(none)";
        const string ResizeModeField = "ResizeMode";
        const string ScaleModeField = "ScaleMode";
        const string QualityField = "Quality";

        /// <summary>
        /// In case a srcSet is being generated with a '*' factor and we don't have a number, assume 1200.
        /// This is an ideal number, as it's quite big but not huge, and will usually be easily divisible by 2,3,4,6 etc.
        /// </summary>
        private const int FallbackWidthForSrcSet = 1200;
        private const int FallbackHeightForSrcSet = 0;

        protected ImageLinkerBase(string logName) : base(logName) { }

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
            var wrapLog = (Debug ? Log : null).SafeCall<string>($"{nameof(url)}:{url}");
            Eav.Parameters.ProtectAgainstMissingParameterNames(noParamOrder, $"{nameof(Image)}", $"{nameof(url)},{nameof(settings)},{nameof(factor)},{nameof(width)}, ...");

            // check common mistakes
            if (aspectRatio != null && height != null)
            {
                wrapLog("error", null);
                const string messageOnlyOneOrNone = "only one or none of these should be provided, other can be zero";
                throw new ArgumentOutOfRangeException($"{nameof(aspectRatio)},{nameof(height)}", messageOnlyOneOrNone);
            }

            // Check if the settings is the expected type or null/other type
            var getSettings = settings as ICanGetNameNotFinal;
            if (Debug) Log.Add($"Has Settings:{getSettings != null}");

            var resizeParams = new ResizeParams();
            if (!string.IsNullOrWhiteSpace(parameters))
                resizeParams.Parameters = UrlHelpers.ParseQueryString(parameters);

            (resizeParams.Width, resizeParams.Height) = FigureOutBestWidthAndHeight(width, height, factor, aspectRatio, getSettings);

            resizeParams.Format = CorrectFormats(RealStringOrNull(format));

            // Aspects which aren't affected by scale
            resizeParams.Quality = IntOrZeroAsNull(quality) ?? IntOrZeroAsNull(getSettings?.Get(QualityField)) ?? 0;
            resizeParams.Mode = KeepBestString(resizeMode, getSettings?.Get(ResizeModeField));
            resizeParams.Scale = CorrectScales(KeepBestString(scaleMode, getSettings?.Get(ScaleModeField)));

            var result = GenerateFinalUrlOrSrcSet(url, srcSet, resizeParams);

            return wrapLog(result, result);
        }

        private string GenerateFinalUrlOrSrcSet(string url, string srcSet, ResizeParams originalParams)
        {
            var srcSetConfig = SrcSetParser.ParseSet(srcSet);

            // Basic case - no srcSet config
            if ((srcSetConfig?.Length ?? 0) == 0)
                return ConstructUrl(url, originalParams);

            var results = srcSetConfig.Select(part =>
            {
                if (part.SizeType == SizeDefault)
                    return ConstructUrl(url, originalParams);

                // Copy the params so we can optimize based on the expected SrcSet specs
                var partParams = new ResizeParams(originalParams);

                // Set width if given (has precedence), otherwise multiply previous by pixel density in necessary
                //var shouldMultiply = part.SizeType == SizePixelDensity || part.SizeType == SizeFactorOf;
                //if (part.Width != 0)
                //    partParams.Width = part.Width;
                //else if (shouldMultiply)
                //    partParams.Width = (int)(part.Size * partParams.Width);
                
                //if (part.Height != 0)
                //    partParams.Height = part.Height;
                //else if (shouldMultiply)
                //    partParams.Height = (int)(part.Size * partParams.Height);

                partParams.Width = BestSrcSetDimension(partParams.Width, part.Width, part, FallbackWidthForSrcSet);
                partParams.Height = BestSrcSetDimension(partParams.Height, part.Height, part, FallbackHeightForSrcSet);

                var size = part.Size;
                var sizeTypeCode = part.SizeType;
                if (sizeTypeCode == SizeFactorOf)
                {
                    size = partParams.Width;
                    sizeTypeCode = SizeWidth;
                }

                return $"{ConstructUrl(url, partParams)} {size}{sizeTypeCode}";
            });
            var result = string.Join(",\n", results);

            return result;
        }

        /// <summary>
        /// Get the best matching dimension (width/height) based on what's specified
        /// </summary>
        private int BestSrcSetDimension(int original, int onSrcSet, SrcSetPart part, int fallbackIfNoOriginal)
        {
            if (onSrcSet != 0) return onSrcSet;
            if (part.SizeType == SizePixelDensity || part.SizeType == SizeFactorOf)
            {
                if (part.SizeType == SizeFactorOf && original == 0) original = fallbackIfNoOriginal;
                return (int)(part.Size * original);
            }
            return original;
        }

        private string ConstructUrl(string url, ResizeParams resizeParams)
        {
            var resizerNvc = new NameValueCollection();
            ImgAddIfRelevant(resizerNvc, "w", resizeParams.Width, "0");
            ImgAddIfRelevant(resizerNvc, "h", resizeParams.Height, "0");
            ImgAddIfRelevant(resizerNvc, "quality", resizeParams.Quality, "0");
            ImgAddIfRelevant(resizerNvc, "mode", resizeParams.Mode, DontSetParam);
            ImgAddIfRelevant(resizerNvc, "scale", resizeParams.Scale, DontSetParam);
            ImgAddIfRelevant(resizerNvc, "format", resizeParams.Format, DontSetParam);

            url = UrlHelpers.AddQueryString(url, resizerNvc);

            if (resizeParams.Parameters != null && resizeParams.Parameters.HasKeys())
                url = UrlHelpers.AddQueryString(url, resizeParams.Parameters);

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


        #region Abstract Stuff

        internal abstract Tuple<int, int> FigureOutBestWidthAndHeight(object width, object height, object factor,
            object aspectRatio, ICanGetNameNotFinal getSettings);

        #endregion
    }
}
