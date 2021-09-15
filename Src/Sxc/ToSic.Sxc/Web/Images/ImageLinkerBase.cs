using System;
using System.Collections.Generic;
using ToSic.Eav.Logging;
using ToSic.Razor.Blade;
using ToSic.Sxc.Data;
using static ToSic.Sxc.Web.CleanParam;

namespace ToSic.Sxc.Web.Images
{
    public abstract class ImageLinkerBase: HasLog<ImageLinkerBase>
    {
        protected ImageLinkerBase(string logName) : base(logName) { }

        public bool Debug = false;

        /// <summary>
        /// Make sure this is in sync with the Link.Image
        /// </summary>
        public string Image(string url = null,
            object settings = null,
            object factor = null,
            string dontRelyOnParameterOrder = Eav.Parameters.Protector,
            object width = null,
            object height = null,
            object quality = null,
            string resizeMode = null,
            string scaleMode = null,
            string format = null,
            object aspectRatio = null)
        {
            var wrapLog = (Debug ? Log : null).SafeCall($"{nameof(url)}:{url}");
            Eav.Parameters.ProtectAgainstMissingParameterNames(dontRelyOnParameterOrder, $"{nameof(Image)}", $"{nameof(url)},{nameof(settings)},{nameof(factor)},{nameof(width)}, ...");

            // check common mistakes
            if (aspectRatio != null && height != null)
            {
                wrapLog?.Invoke("error");
                const string messageOnlyOneOrNone = "only one or none of these should be provided, other can be zero";
                throw new ArgumentOutOfRangeException($"{nameof(aspectRatio)},{nameof(height)}", messageOnlyOneOrNone);
            }

            // Check if the settings is the expected type or null/other type
            var getSettings = settings as ICanGetNameNotFinal;
            if (Debug) Log.Add($"Has Settings:{getSettings != null}");

            // Special case, if the settings are just an anonymous object
            // In that case try to convert to a Dynamic object
            // Not active for now, as of now it must always be done with AsDynamic(...)
            // Reason is that we're not sure if this would have a performance overhead we would like to avoid
            //if (getSettings is null && !(settings is null))
            //{
            //    if (Debug) Log.Add($"Conversion to {nameof(ICanGetNameNotFinal)} failed, will try to convert automatically");
            //    getSettings = new DynamicReadObject(settings, true);
            //}

            var resizedNew = FigureOutBestWidthAndHeight(width, height, factor, aspectRatio, getSettings);

            var formToUse = RealStringOrNull(format);

            // Aspects which aren't affected by scale
            var qFinal = IntOrZeroAsNull(quality) ?? IntOrZeroAsNull(getSettings?.Get("Quality")) ?? 0;
            string mToUse = KeepBestString(resizeMode, getSettings?.Get("ResizeMode"));
            string sToUse = KeepBestString(scaleMode, getSettings?.Get("ScaleMode"));

            var resizer = new List<KeyValuePair<string, string>>();
            ImgAddIfRelevant(resizer, "w", resizedNew.Item1, "0");
            ImgAddIfRelevant(resizer, "h", resizedNew.Item2, "0");
            ImgAddIfRelevant(resizer, "quality", qFinal, "0");
            ImgAddIfRelevant(resizer, "mode", mToUse);
            ImgAddIfRelevant(resizer, "scale", ImgResizeLinker.CorrectScales(sToUse));
            ImgAddIfRelevant(resizer, "format", ImgResizeLinker.CorrectFormats(formToUse));

            url = QueryHelper.AddQueryString(url, resizer);

            var result = Tags.SafeUrl(url).ToString();
            wrapLog?.Invoke(result);
            return result;
        }

        private bool ImgAddIfRelevant(ICollection<KeyValuePair<string, string>> resizer, string key, object value, string irrelevant = "")
        {
            var wrapLog = (Debug ? Log : null).SafeCall<bool>();
            if (key == null || value == null)
                return wrapLog($"Won't add '{key}', since key or value are null", false);

            var strValue = value.ToString();
            if (string.IsNullOrEmpty(strValue))
                return wrapLog($"Won't add '{key}' since value as string would be null", false);

            if (strValue.Equals(irrelevant, StringComparison.InvariantCultureIgnoreCase))
                return wrapLog($"Won't add '{key}' since value would be irrelevant", false);

            resizer.Add(new KeyValuePair<string, string>(key, strValue));
            return wrapLog($"Added key {key}", true);
        }


        #region Abstract Stuff

        internal abstract Tuple<int, int> FigureOutBestWidthAndHeight(object width, object height, object factor,
            object aspectRatio, ICanGetNameNotFinal getSettings);

        #endregion
    }
}
