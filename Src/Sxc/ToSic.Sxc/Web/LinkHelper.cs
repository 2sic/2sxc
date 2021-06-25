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

            if (aspectRatio != null && height != null)
                throw new ArgumentOutOfRangeException($"{nameof(aspectRatio)},{nameof(height)}", messageOnlyOneOrNone);

            // Try to pre-process parameters and prefer them
            var wParam = ImgResizeLinker.IntOrNull(width);
            var hParam = ImgResizeLinker.IntOrNull(height);
            

            // Pre-Clean the values - all as strings
            var getSettings = settings as ICanGetNameNotFinal;
            int wSafe = wParam ?? ImgResizeLinker.IntOrNull(getSettings?.Get("Width")) ?? 0;
            int hSafe = hParam ?? ImgResizeLinker.IntOrNull(getSettings?.Get("Height")) ?? 0;

            
            var factorFinal = ImgResizeLinker.FloatOrNull(factor) ?? 0;
            var arFinal = ImgResizeLinker.FloatOrNull(aspectRatio) 
                          ?? ImgResizeLinker.IntOrNull(getSettings?.Get("AspectRatio")) ?? 0;


            // if either param h/w was null, then do a rescaling on the param which comes from the settings
            // But ignore the other one!
            Tuple<int, int> resizedNew = factorFinal != 0 && (wParam == null || hParam == null)
                ? ImgResizeLinker.Rescale(wSafe, hSafe, factorFinal, arFinal, wParam == null, hParam == null)
                : new Tuple<int, int>(wSafe, hSafe);

            resizedNew = ImgResizeLinker.KeepInRangeProportional(resizedNew);
            
            var formToUse = ImgResizeLinker.RealStringOrNull(format);

            // Aspects which aren't affected by scale
            var qFinal = ImgResizeLinker.IntOrNull(quality)
                         ?? ImgResizeLinker.IntOrNull(getSettings?.Get("Quality")) ?? 0;
            string mToUse = ImgResizeLinker.KeepBestParam(resizeMode, getSettings?.Get("ResizeMode"));
            string sToUse = ImgResizeLinker.KeepBestParam(scaleMode, getSettings?.Get("ScaleMode"));

            var resizer = new List<KeyValuePair<string, string>>();
            ImgAddIfRelevant(resizer, "w", resizedNew.Item1, "0");
            ImgAddIfRelevant(resizer, "h", resizedNew.Item2, "0");
            ImgAddIfRelevant(resizer, "quality", qFinal, "0");
            ImgAddIfRelevant(resizer, "mode", mToUse);
            ImgAddIfRelevant(resizer, "scale", ImgResizeLinker.CorrectScales(sToUse));
            ImgAddIfRelevant(resizer, "format", ImgResizeLinker.CorrectFormats(formToUse));

            var urlParams = string.Join("&", resizer.Select(pair => pair.Key + "=" + pair.Value));
            if (!string.IsNullOrWhiteSpace(urlParams)) urlParams = "?" + urlParams;
            
            // todo: in future also try to combine existing params - so if the url already has a "?..." we should merge these
            
            return Tags.SafeUrl(url + urlParams).ToString();
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
