using System;
using System.Web;
using ToSic.Eav.Documentation;
using ToSic.Eav.Helpers;
using ToSic.Sxc.Dnn.Context;
using ToSic.Sxc.Dnn.Run;
using ToSic.Sxc.Images;
using ToSic.Sxc.Web;
using ToSic.Sxc.Web.WebApi;

namespace ToSic.Sxc.Dnn.Web
{
    /// <summary>
    /// The DNN implementation of the <see cref="ILinkHelper"/>.
    /// </summary>
    [PrivateApi("This implementation shouldn't be visible")]
    public class DnnLinkHelper : LinkHelper
    {
        [PrivateApi]
        public DnnLinkHelper(ImgResizeLinker imgLinker): base(imgLinker) { }

        [PrivateApi] private IDnnContext Dnn => _dnn ?? (_dnn = CodeRoot.GetService<IDnnContext>());
        private IDnnContext _dnn;

        protected override string ToApi(string api, string parameters = null) 
            => Api(path: LinkHelpers.CombineApiWithQueryString(api.TrimPrefixSlash(), parameters));

        protected override string ToPage(int? pageId, string parameters = null) =>
            parameters == null
                ? Dnn.Tab.FullUrl
                : DotNetNuke.Common.Globals.NavigateURL(pageId ?? Dnn.Tab.TabID, "", parameters); // NavigateURL returns absolute links


        private string Api(string noParamOrder = Eav.Parameters.Protector, string path = null)
        {
            Eav.Parameters.ProtectAgainstMissingParameterNames(noParamOrder, "Api", $"{nameof(path)}");

            if (string.IsNullOrEmpty(path)) return string.Empty;

            path = path.ForwardSlash();
            path = path.TrimPrefixSlash();

            if (path.PrefixSlash().ToLowerInvariant().Contains("/app/"))
                throw new ArgumentException("Error, path shouldn't have \"app\" part in it. It is expected to be relative to application root.");

            if (!path.PrefixSlash().ToLowerInvariant().Contains("/api/"))
                throw new ArgumentException("Error, path should have \"api\" part in it.");

            var apiRoot = DnnJsApiHeader.GetApiRoots().Item2.TrimLastSlash();

            var relativePath = $"{apiRoot}/app/{App.Folder}/{path}";    

            return relativePath;
        }

        public override string GetCurrentLinkRoot() => HttpContext.Current?.Request?.Url?.GetLeftPart(UriPartial.Authority) ?? string.Empty;

        public override string GetCurrentRequestUrl() => HttpContext.Current?.Request?.Url?.AbsoluteUri ?? string.Empty;
    }
}