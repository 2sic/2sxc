using System;
using ToSic.Eav.Documentation;
using ToSic.Eav.Helpers;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Context;
using ToSic.Sxc.Dnn.Run;
using ToSic.Sxc.Web;
using ToSic.Sxc.Web.WebApi;

namespace ToSic.Sxc.Dnn.Web
{
    /// <summary>
    /// The DNN implementation of the <see cref="ILinkHelper"/>.
    /// </summary>
    [PublicApi_Stable_ForUseInYourCode]
    public class DnnLinkHelper : ILinkHelper
    {
        private IDnnContext _dnn;
        private IApp _app;

        public DnnLinkHelper(IDnnContext DnnContextOld)
        {
            _dnn = DnnContextOld;
        }

        public void Init(IContextOfBlock context, IApp app)
        {
            ((DnnContextOld) _dnn).Init(context?.Module);
            _app = app;
        }

        /// <inheritdoc />
        public string To(string dontRelyOnParameterOrder = Eav.Parameters.Protector, int? pageId = null, string parameters = null, string api = null)
        {
            // prevent incorrect use without named parameters
            Eav.Parameters.ProtectAgainstMissingParameterNames(dontRelyOnParameterOrder, $"{nameof(To)}", $"{nameof(pageId)},{nameof(parameters)},{nameof(api)}");

            if (api != null) return Api(path: LinkHelpers.CombineApiWithQueryString(api.TrimPrefixSlash(), parameters));

            return parameters == null
                ? _dnn.Tab.FullUrl
                : DotNetNuke.Common.Globals.NavigateURL(pageId ?? _dnn.Tab.TabID, "", parameters);
        }

        /// <inheritdoc />
        public string Base()
        {
            // helper to generate a base path which is also valid on home (special DNN behaviour)
            const string randomxyz = "this-should-never-exist-in-the-url";
            var basePath = To(parameters: randomxyz + "=1");
            return basePath.Substring(0, basePath.IndexOf(randomxyz, StringComparison.Ordinal));
        }

        private string Api(string dontRelyOnParameterOrder = Eav.Parameters.Protector, string path = null)
        {
            Eav.Parameters.ProtectAgainstMissingParameterNames(dontRelyOnParameterOrder, "Api", $"{nameof(path)}");

            if (string.IsNullOrEmpty(path)) return string.Empty;

            path = path.ForwardSlash();
            path = path.TrimPrefixSlash();

            if (path.PrefixSlash().ToLowerInvariant().Contains("/app/"))
                throw new ArgumentException("Error, path shouldn't have \"app\" part in it. It is expected to be relative to application root.");

            if (!path.PrefixSlash().ToLowerInvariant().Contains("/api/"))
                throw new ArgumentException("Error, path should have \"api\" part in it.");

            var apiRoot = DnnJsApiHeader.GetApiRoots().Item2.TrimLastSlash();

            return $"{apiRoot}/app/{_app.Folder}/{path}";
        }
    }
}