using System;
using System.Collections.Generic;
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
        private ImgResizeLinker ImgLinker { get; }
        [PrivateApi] protected IApp App;

        protected LinkHelper(ImgResizeLinker imgLinker) : base($"{Constants.SxcLogName}.LnkHlp")
        {
            ImgLinker = imgLinker.Init(Log);
        }

        public virtual void Init(IContextOfBlock context, IApp app, ILog parentLog)
        {
            Log.LinkTo(parentLog);
            App = app;
        }


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

        /// <inheritdoc />
        [PrivateApi]
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
            var wrapLog = (_debug ? Log : null).SafeCall($"{nameof(url)}:{url}");
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
            if (_debug) Log.Add($"Has Settings:{getSettings != null}");

            var resizedNew = ImgLinker.FigureOutBestWidthAndHeight(width, height, factor, aspectRatio, getSettings);

            var formToUse = ImgLinker.RealStringOrNull(format);

            // Aspects which aren't affected by scale
            var qFinal = ImgLinker.IntOrNull(quality)
                         ?? ImgLinker.IntOrNull(getSettings?.Get("Quality")) ?? 0;
            string mToUse = ImgLinker.KeepBestParam(resizeMode, getSettings?.Get("ResizeMode"));
            string sToUse = ImgLinker.KeepBestParam(scaleMode, getSettings?.Get("ScaleMode"));

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

        private bool _debug;
        public void SetDebug(bool debug)
        {
            _debug = debug;
            // Set logging on ImageResizeHelper
            ImgLinker.Debug = debug;
        }


        private bool ImgAddIfRelevant(ICollection<KeyValuePair<string, string>> resizer, string key, object value, string irrelevant = "")
        {
            var wrapLog = (_debug ? Log : null).SafeCall<bool>();
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
    }
}
