using System;
using ToSic.Lib.Documentation;
using ToSic.Eav.Helpers;
using ToSic.Lib.DI;
using ToSic.Sxc.Dnn.Run;
using ToSic.Sxc.Dnn.Web;
using ToSic.Sxc.Images;
using ToSic.Sxc.Run;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.Dnn.Services
{
    /// <summary>
    /// The DNN implementation of the <see cref="ILinkService"/>.
    /// </summary>
    [PrivateApi("This implementation shouldn't be visible")]
    public class DnnLinkService : LinkServiceBase
    {
        public DnnLinkService(ImgResizeLinker imgLinker, LazySvc<DnnValueConverter> dnnValueConverterLazy,
            LazySvc<ILinkPaths> linkPathsLazy) : base(imgLinker, linkPathsLazy)
            => ConnectServices(
                _dnnValueConverterLazy = dnnValueConverterLazy
            );
        private readonly LazySvc<DnnValueConverter> _dnnValueConverterLazy;

        [PrivateApi] private IDnnContext Dnn => _dnn ?? (_dnn = _DynCodeRoot.GetService<IDnnContext>());
        private IDnnContext _dnn;
        [PrivateApi] private DnnValueConverter DnnValueConverter => _dnnValueConverter ?? (_dnnValueConverter = _dnnValueConverterLazy.Value);
        private DnnValueConverter _dnnValueConverter;

        protected override string ToApi(string api, string parameters = null) 
            => Api(path: CombineApiWithQueryString(api.TrimPrefixSlash(), parameters));

        protected override string ToPage(int? pageId, string parameters = null, string language = null)
        {
            if (pageId.HasValue)
            {
                var url = DnnValueConverter.ResolvePageLink(pageId.Value, language, parameters);
                if (!string.IsNullOrEmpty(url)) return url;
            }
            
            var currentPageUrl = parameters == null
                ? Dnn.Tab.FullUrl
                : DotNetNuke.Common.Globals.NavigateURL(Dnn.Tab.TabID, "", parameters); // NavigateURL returns absolute links

            return CurrentPageUrlWithEventualHashError(pageId, currentPageUrl);
        }


        private string Api(string noParamOrder = Eav.Parameters.Protector, string path = null)
        {
            Eav.Parameters.Protect(noParamOrder, $"{nameof(path)}");

            if (string.IsNullOrEmpty(path)) return string.Empty;

            path = path.ForwardSlash();
            path = path.TrimPrefixSlash();

            if (path.PrefixSlash().ToLowerInvariant().Contains("/app/"))
                throw new ArgumentException("Error, path shouldn't have \"app\" part in it. It is expected to be relative to application root.");

            if (!path.PrefixSlash().ToLowerInvariant().Contains("/api/"))
                throw new ArgumentException("Error, path should have \"api\" part in it.");

            var apiRoot = DnnJsApi.GetApiRoots().Item2.TrimLastSlash();

            var relativePath = $"{apiRoot}/app/{App.Folder}/{path}";    

            return relativePath;
        }
    }
}