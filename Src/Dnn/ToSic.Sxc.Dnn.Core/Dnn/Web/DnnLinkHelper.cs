using System;
using ToSic.Eav.Documentation;
using ToSic.Eav.Helpers;
using ToSic.Sxc.Dnn.Run;
using ToSic.Sxc.Images;
using ToSic.Sxc.Run;
using ToSic.Sxc.Web;

namespace ToSic.Sxc.Dnn.Web
{
    /// <summary>
    /// The DNN implementation of the <see cref="ILinkHelper"/>.
    /// </summary>
    [PrivateApi("This implementation shouldn't be visible")]
    public class DnnLinkHelper : LinkHelperBase
    {
        private readonly Lazy<DnnValueConverter> _dnnValueConverterLazy;



        [PrivateApi]
        public DnnLinkHelper(ImgResizeLinker imgLinker, Lazy<DnnValueConverter> dnnValueConverterLazy, Lazy<ILinkPaths> linkPathsLazy) : base(imgLinker, linkPathsLazy)
        {
            _dnnValueConverterLazy = dnnValueConverterLazy;
        }

        [PrivateApi] private IDnnContext Dnn => _dnn ?? (_dnn = CodeRoot.GetService<IDnnContext>());
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
    }
}