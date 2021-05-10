using System;
using ToSic.Eav.Documentation;
using ToSic.Eav.Helpers;
using ToSic.Sxc.Apps;
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

        public DnnLinkHelper Init(IDnnContext dnn, IApp app)
        {
            _dnn = dnn;
            _app = app;
            return this;
        }

        /// <inheritdoc />
        public string To(string dontRelyOnParameterOrder = Eav.Constants.RandomProtectionParameter, int? pageId = null, string parameters = null, string api = null)
        {
            // prevent incorrect use without named parameters
            Eav.Constants.ProtectAgainstMissingParameterNames(dontRelyOnParameterOrder, $"{nameof(To)}", $"{nameof(pageId)},{nameof(parameters)},{nameof(api)}");
            
            if (api == null)
            {
                var targetPage = pageId ?? _dnn.Tab.TabID;

                var parametersToUse = parameters;
                return parametersToUse == null
                    ? _dnn.Tab.FullUrl
                    : DotNetNuke.Common.Globals.NavigateURL(targetPage, "", parametersToUse);
            }
            api = api.TrimPrefixSlash();

            // Move queryString part from 'api' to 'parameters'.
            LinkHelpers.NormalizeQueryString(ref api, ref parameters);

            var path = parameters == null ? api : $"{api}?{parameters}";

            return Api(path: path);
        }

        /// <inheritdoc />
        public string Base()
        {
            // helper to generate a base path which is also valid on home (special DNN behaviour)
            const string randomxyz = "this-should-never-exist-in-the-url";
            var basePath = To(parameters: randomxyz + "=1");
            return basePath.Substring(0, basePath.IndexOf(randomxyz, StringComparison.Ordinal));
        }

        private string Api(string dontRelyOnParameterOrder = Eav.Constants.RandomProtectionParameter, string path = null)
        {
            Eav.Constants.ProtectAgainstMissingParameterNames(dontRelyOnParameterOrder, "Api", $"{nameof(path)}");

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