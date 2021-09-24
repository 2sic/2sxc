using System;
using ToSic.Eav.Documentation;
using ToSic.Eav.Logging;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Code;
using ToSic.Sxc.Context;

namespace ToSic.Sxc.Web
{
    [PrivateApi]
    public abstract class LinkHelper: HasLog, ILinkHelper
    {
        private ImgResizeLinker ImgLinker { get; }
        [PrivateApi] protected IApp App;

        protected LinkHelper(ImgResizeLinker imgLinker) : base($"{Constants.SxcLogName}.LnkHlp")
        {
            ImgLinker = imgLinker;
            ImgLinker.Init(Log);
        }

        public virtual void AddBlockContext(IDynamicCodeRoot codeRoot)
        {
            Log.LinkTo(codeRoot.Log);
            App = codeRoot.App;
        }


        /// <inheritdoc />
        public string To(string noParamOrder = Eav.Parameters.Protector, int? pageId = null, object parameters = null, string api = null)
        {
            // prevent incorrect use without named parameters
            Eav.Parameters.ProtectAgainstMissingParameterNames(noParamOrder, $"{nameof(To)}", $"{nameof(pageId)},{nameof(parameters)},{nameof(api)}");

            // Check initial conflicting values.
            if (pageId != null && api != null)
                throw new ArgumentException($"Multiple properties like '{nameof(api)}' or '{nameof(pageId)}' have a value - only one can be provided.");

            var strParams = ParametersToString(parameters);

            return ToImplementation(pageId, strParams, api);
        }

        protected abstract string ToImplementation(int? pageId = null, string parameters = null, string api = null);

        protected string ParametersToString(object parameters)
        {
            if (parameters is null) return null;
            if (parameters is string strParameters) return strParameters;
            if (parameters is IParameters paramDic) return paramDic.ToString();
            
            // Fallback / default
            return null;
        }

        
        /// <inheritdoc />
        public virtual string Base()
        {
            // helper to generate a base path which is also valid on home (special DNN behaviour)
            const string randomxyz = "this-should-never-exist-in-the-url";
            var basePath = To(parameters: randomxyz + "=1");
            return basePath.Substring(0, basePath.IndexOf(randomxyz, StringComparison.InvariantCultureIgnoreCase));
        }

        /// <inheritdoc />
        public string Image(string url = null,
            object settings = null,
            object factor = null,
            string noParamOrder = Eav.Parameters.Protector,
            object width = null,
            object height = null,
            object quality = null,
            string resizeMode = null,
            string scaleMode = null,
            string format = null,
            object aspectRatio = null)
        {
            return ImgLinker.Image(url, settings, factor, noParamOrder, width, height, quality, resizeMode,
                scaleMode, format, aspectRatio);
        }

        private bool _debug;
        public void SetDebug(bool debug)
        {
            _debug = debug;
            // Set logging on ImageResizeHelper
            ImgLinker.Debug = debug;
        }


    }
}
