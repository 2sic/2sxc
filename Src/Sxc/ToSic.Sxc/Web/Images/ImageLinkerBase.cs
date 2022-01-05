using System;
using System.Collections.Specialized;
using System.Linq;
using ToSic.Eav.Logging;
using ToSic.Razor.Blade;
using ToSic.Sxc.Data;
using ToSic.Sxc.Web.Url;
using static ToSic.Sxc.Plumbing.ParseObject;

namespace ToSic.Sxc.Web.Images
{
    public abstract partial class ImageLinkerBase: HasLog<ImageLinkerBase>
    {
        internal const string DontSetParam = "(none)";

        const string ResizeModeField = "ResizeMode";
        const string ScaleModeField = "ScaleMode";
        const string QualityField = "Quality";

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

            string result;
            var srcSetConfig = SrcSetParser.ParseSet(srcSet);

            if ((srcSetConfig?.Length ?? 0) == 0)
                result = ConstructUrl(url, resizeParams);
            else
            {
                var results = srcSetConfig.Select(part =>
                {
                    if (part.SizeType == SrcSetPart.SizeDefault)
                        return ConstructUrl(url, resizeParams);
                    var paramCopy = new ResizeParams(resizeParams);
                    if (part.Width != 0) paramCopy.Width = part.Width;
                    if (part.Height != 0) paramCopy.Height = part.Height;
                    return $"{ConstructUrl(url, resizeParams)} {part.Size}{part.SizeType}";
                });
                result = string.Join(",\n", results);
            }

            return wrapLog(result, result);
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
