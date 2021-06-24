using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Documentation;
using ToSic.Eav.Logging;
using ToSic.Razor.Blade;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Context;
using ToSic.Sxc.Data;

namespace ToSic.Sxc.Web
{
    [PrivateApi]
    public abstract class LinkHelper: HasLog, ILinkHelper
    {
        [PrivateApi] protected IApp App;

        protected LinkHelper() : base("Sxc.LnkHlp") { }

        public virtual void Init(IContextOfBlock context, IApp app) => App = app;

        
        /// <inheritdoc />
        public abstract string To(string dontRelyOnParameterOrder = Eav.Parameters.Protector, 
            int? pageId = null,
            string parameters = null,
            string api = null);

        
        /// <inheritdoc />
        public virtual string Base()
        {
            // helper to generate a base path which is also valid on home (special DNN behaviour)
            const string randomxyz = "this-should-never-exist-in-the-url";
            var basePath = To(parameters: randomxyz + "=1");
            return basePath.Substring(0, basePath.IndexOf(randomxyz, StringComparison.InvariantCultureIgnoreCase));
        }

        [PrivateApi]
        public string Image(string url = null,
            object settings = null,
            object factor = null,
            string dontRelyOnParameterOrder =
                "Rule: all params must be named (https://r.2sxc.org/named-params), Example: \'enable: true, version: 10\'",
            object width = null,
            object height = null,
            object quality = null,
            string resizeMode = null,
            string scaleMode = null,
            string format = null,
            object aspectRatio = null)
        {
            Eav.Parameters.ProtectAgainstMissingParameterNames(dontRelyOnParameterOrder, $"{nameof(Image)}", $"{nameof(url)},{nameof(settings)},{nameof(factor)},{nameof(width)}, ...");
            // check common mistakes
            const string messageOnlyOneOrNone = "only one or none of these should be provided, other can be zero";
            if (factor != null) {
                if(width != null) 
                    throw new ArgumentOutOfRangeException($"{nameof(factor)},{nameof(width)}", messageOnlyOneOrNone);
                if (height != null && ImgResizeLinker.ToNullOrString(height) != "0")
                    throw new ArgumentOutOfRangeException($"{nameof(factor)},{nameof(height)}", messageOnlyOneOrNone);
                if(aspectRatio != null && ImgResizeLinker.ToNullOrString(aspectRatio) != "0") 
                    throw new ArgumentOutOfRangeException($"{nameof(factor)},{nameof(aspectRatio)}", messageOnlyOneOrNone);
            }

            if (aspectRatio != null && height != null)
                throw new ArgumentOutOfRangeException($"{nameof(aspectRatio)},{nameof(height)}", messageOnlyOneOrNone);

            // todo
            // - handle aspectratio

            // Pre-Clean the values - all as strings
            var getSettings = settings as ICanGetNameNotFinal;
            string wToUse = ImgResizeLinker.KeepBestParam(width, getSettings?.Get("Width"));
            string hToUse = ImgResizeLinker.KeepBestParam(height, getSettings?.Get("Height"));
            string arToUse = ImgResizeLinker.KeepBestParam(aspectRatio, getSettings?.Get("AspectRatio"));
            string qToUse = ImgResizeLinker.KeepBestParam(quality, getSettings?.Get("Quality"));
            string mToUse = ImgResizeLinker.KeepBestParam(resizeMode, getSettings?.Get("ResizeMode"));
            string sToUse = ImgResizeLinker.KeepBestParam(scaleMode, getSettings?.Get("ScaleMode"));
            string formToUse = ImgResizeLinker.KeepBestParam(format, null);
            string factToUse = ImgResizeLinker.KeepBestParam(factor, null);
            //string maxW = ImgResizeLinker.KeepBestParam(maxWidth, null);
            //string maxH = ImgResizeLinker.KeepBestParam(maxHeight, null);
            
            // Range checks - all will then be 0, max, or in between
            var wInt = ImgResizeLinker.ImgKeepInRange(wToUse, ImgResizeLinker.MaxSize); // max of ImageResizer.net
            var hInt = ImgResizeLinker.ImgKeepInRange(hToUse, ImgResizeLinker.MaxSize);
            //var maxWint = ImgResizeLinker.ImgKeepInRange(maxW, ImgResizeLinker.MaxSize);
            //var maxHint = ImgResizeLinker.ImgKeepInRange(maxH, ImgResizeLinker.MaxSize);
            var qInt = ImgResizeLinker.ImgKeepInRange(qToUse, ImgResizeLinker.MaxQuality);

            Tuple<int,int> resized = ImgResizeLinker.Rescale(wInt, hInt, factToUse, arToUse);

            var resizer = new List<KeyValuePair<string, string>>();
            ImgAddIfRelevant(resizer, "w", resized.Item1, "0");
            ImgAddIfRelevant(resizer, "h", resized.Item2, "0");
            ImgAddIfRelevant(resizer, "quality", qInt, "0");
            ImgAddIfRelevant(resizer, "mode", mToUse);
            ImgAddIfRelevant(resizer, "scale", ImgResizeLinker.CorrectScales(sToUse));
            ImgAddIfRelevant(resizer, "format", ImgResizeLinker.CorrectFormats(formToUse));
            //ImgAddIfRelevant(resizer, "maxwidth", maxWint, "0");
            //ImgAddIfRelevant(resizer, "maxheight", maxHint, "0");

            var urlParams = string.Join("&", resizer.Select(pair => pair.Key + "=" + pair.Value));
            return Tags.SafeUrl(url + "?" + urlParams).ToString();
        }


        private void ImgAddIfRelevant(ICollection<KeyValuePair<string, string>> resizer, string key, object value, string irrelevant = "")
        {
            if (key == null || value == null) return;
            var strValue = value.ToString();
            if (string.IsNullOrEmpty(strValue)) return;
            if (strValue.Equals(irrelevant, StringComparison.InvariantCultureIgnoreCase)) return;
            resizer.Add(new KeyValuePair<string, string>(key, strValue));
        }
    }
}
