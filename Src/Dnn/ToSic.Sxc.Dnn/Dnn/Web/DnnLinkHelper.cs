using System;
using System.Web;
using ToSic.Eav.Documentation;
using ToSic.Eav.Helpers;
using ToSic.Sxc.Code;
using ToSic.Sxc.Dnn.Run;
using ToSic.Sxc.Web;
using ToSic.Sxc.Web.WebApi;

namespace ToSic.Sxc.Dnn.Web
{
    /// <summary>
    /// The DNN implementation of the <see cref="ILinkHelper"/>.
    /// </summary>
    [PublicApi_Stable_ForUseInYourCode]
    public class DnnLinkHelper : LinkHelper
    {
        [PrivateApi] private readonly IDnnContext _dnn;

        [PrivateApi]
        public DnnLinkHelper(IDnnContext dnnContext, ImgResizeLinker imgLinker): base(imgLinker)
        {
            _dnn = dnnContext;
        }

        public override void AddBlockContext(IDynamicCodeRoot codeRoot)
        {
            base.AddBlockContext(codeRoot);
            ((DnnContextOld) _dnn).Init(codeRoot.Block?.Context?.Module);
        }

        /// <inheritdoc />
        public override string To(string dontRelyOnParameterOrder = Eav.Parameters.Protector, int? pageId = null, string parameters = null, string api = null)
        {
            // prevent incorrect use without named parameters
            Eav.Parameters.ProtectAgainstMissingParameterNames(dontRelyOnParameterOrder, $"{nameof(To)}", $"{nameof(pageId)},{nameof(parameters)},{nameof(api)}");

            if (api != null) return Api(path: LinkHelpers.CombineApiWithQueryString(api.TrimPrefixSlash(), parameters));

            return parameters == null
                ? _dnn.Tab.FullUrl
                : DotNetNuke.Common.Globals.NavigateURL(pageId ?? _dnn.Tab.TabID, "", parameters); // NavigateURL returns absolute links
        }

        private string Api(string dontRelyOnParameterOrder = Eav.Parameters.Protector, string path = null, bool absoluteUrl = true)
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

            var relativePath = $"{apiRoot}/app/{App.Folder}/{path}";    

            return absoluteUrl ? $"{GetDomainName()}{relativePath}" : relativePath;
        }

        private string GetDomainName()
        {
            return HttpContext.Current?.Request?.Url?.GetLeftPart(UriPartial.Authority) ?? string.Empty;
        }
    }
}