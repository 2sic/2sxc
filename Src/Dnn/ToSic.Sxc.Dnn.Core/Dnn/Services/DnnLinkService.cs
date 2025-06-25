﻿using DotNetNuke.Abstractions;
using ToSic.Eav.Data.ValueConverter.Sys;
using ToSic.Lib.Coding;
using ToSic.Sxc.Dnn.Run;
using ToSic.Sxc.Dnn.Web;
using ToSic.Sxc.Images.Internal;
using ToSic.Sxc.Services;
using ToSic.Sxc.Services.Internal;
using ToSic.Sxc.Sys.Integration.Paths;

namespace ToSic.Sxc.Dnn.Services;

/// <summary>
/// The DNN implementation of the <see cref="ILinkService"/>.
/// </summary>
[PrivateApi("This implementation shouldn't be visible")]
internal class DnnLinkService(
    ImgResizeLinker imgLinker,
    LazySvc<IValueConverter> dnnValueConverterLazy,
    LazySvc<ILinkPaths> linkPathsLazy,
    LazySvc<INavigationManager> navigationManager)
    : LinkServiceBase(imgLinker, linkPathsLazy, connect: [dnnValueConverterLazy, navigationManager])
{
    [PrivateApi] private IDnnContext Dnn => field ??= ExCtx.GetService<IDnnContext>();
    [PrivateApi] private DnnValueConverter DnnValueConverter => field ??= dnnValueConverterLazy.Value as DnnValueConverter;

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
            : navigationManager.Value.NavigateURL(Dnn.Tab.TabID, "", parameters); // NavigateURL returns absolute links

        return CurrentPageUrlWithEventualHashError(pageId, currentPageUrl);
    }


    private string Api(NoParamOrder noParamOrder = default, string path = null)
    {
        if (string.IsNullOrEmpty(path))
            return string.Empty;

        path = path.ForwardSlash();
        path = path.TrimPrefixSlash();

        if (path.PrefixSlash().ToLowerInvariant().Contains("/app/"))
            throw new ArgumentException("Error, path shouldn't have \"app\" part in it. It is expected to be relative to application root.");

        if (!path.PrefixSlash().ToLowerInvariant().Contains("/api/"))
            throw new ArgumentException("Error, path should have \"api\" part in it.");

        var apiRoot = DnnJsApiService.GetApiRoots().AppApiRoot.TrimLastSlash();

        var relativePath = $"{apiRoot}/app/{AppFolder}/{path}";    

        return relativePath;
    }
}