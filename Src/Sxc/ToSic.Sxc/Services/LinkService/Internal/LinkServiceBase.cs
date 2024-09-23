using ToSic.Lib.DI;
using ToSic.Razor.Blade;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Context;
using ToSic.Sxc.Data;
using ToSic.Sxc.Images.Internal;
using ToSic.Sxc.Integration.Paths;
using ToSic.Sxc.Web.Internal.Url;

namespace ToSic.Sxc.Services.Internal;

[PrivateApi]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public abstract class LinkServiceBase(ImgResizeLinker imgLinker, LazySvc<ILinkPaths> linkPathsLazy, object[] connect = default)
    : ServiceForDynamicCode($"{SxcLogName}.LnkHlp", connect: [..connect ?? [], linkPathsLazy, imgLinker]), ILinkService
{
    [PrivateApi]
    protected ILinkPaths LinkPaths => linkPathsLazy.Value;

    [PrivateApi]
    protected IApp App => _CodeApiSvc.App;


    /// <inheritdoc />
    public string To(
        NoParamOrder noParamOrder = default,
        int? pageId = null,
        string api = null,
        object parameters = null,
        string type = null,
        string language = null
    )
    {
        var l = (Debug ? Log : null).Fn<string>($"pid:{pageId},api:{api},t:{type},l:{language}");

        // Check initial conflicting values.
        if (pageId != null && api != null)
            throw new ArgumentException($"Only one of the parameters '{nameof(api)}' or '{nameof(pageId)}' can have a value.");

        var strParams = ParametersToString(parameters);
        l.A($"parameters:{strParams}");
 
        // TODO: unclear what would happen if a new parameter would replace an existing - would it just append? that wouldn't be good
        var url = api == null
            ? ToPage(pageId, strParams, language)
            : ToApi(api, strParams);
        l.A($"url:{url}");

        var processed = ExpandUrlIfNecessary(type, url);
        l.A($"expandUrl:{processed}, t:{type}");

        return l.ReturnAsOk(Tags.SafeUrl(processed).ToString());
    }

    private string ExpandUrlIfNecessary(string type, string url)
    {
        if (url == null) return null;

        // Short-Circuit to really not do anything if the type isn't specified
        if (string.IsNullOrEmpty(type)) return url;

        var parts = new UrlParts(url);
        switch (type.ToLowerInvariant())
        {
            case "full":
                if (!parts.IsAbsolute)
                    parts.ReplaceRoot(LinkPaths.GetCurrentRequestUrl());
                return parts.ToLink("full");
            case "//":
                if (!parts.IsAbsolute)
                    parts.ReplaceRoot(LinkPaths.GetCurrentRequestUrl());
                return parts.ToLink("//");
            case "/": // note: "/" isn't officially supported
                return parts.ToLink("/");
            default:
                return url;
        }
    }
     

    protected abstract string ToApi(string api, string parameters = null);

    protected abstract string ToPage(int? pageId, string parameters = null, string language = null);

    protected static string ParametersToString(object parameters)
        => parameters switch
        {
            null => null,
            string strParameters => strParameters.TrimStart(['?', '&']),
            IParameters paramDic => paramDic.ToString(),
            _ => null // Fallback / default
        };


    /// <inheritdoc />
    public virtual string Base()
    {
        // helper to generate a base path which is also valid on home (special DNN behaviour)
        const string randomxyz = "this-should-never-exist-in-the-url";
        var basePath = To(parameters: randomxyz + "=1");
        return basePath.Substring(0, basePath.IndexOf(randomxyz, StringComparison.InvariantCultureIgnoreCase));
    }

    /// <inheritdoc />
    public string Image(
        string url = default,
        object settings = default,
        object factor = default,
        NoParamOrder noParamOrder = default,
        IField field = default,
        object width = default,
        object height = default,
        object quality = default,
        string resizeMode = default,
        string scaleMode = default,
        string format = default,
        object aspectRatio = default,
        string type = default,
        object parameters = default
    )
    {
        // If params were given, ensure it can be used as string, as it could also be a params-object
        var strParams = ParametersToString(parameters);

        // If the url should be expanded to have a full root or something, do this first
        url ??= field?.Parent.Url(field.Name);
        var expandedUrl = ExpandUrlIfNecessary(type, url);

        // Get the image-url(s) as needed
        // Note that srcset is false, so it won't generate a bunch of sources, just one - which is how the API works
        // Anybody that wants a srcset must use the new IImageService for that
        var imageUrl = imgLinker.Image(url: expandedUrl, settings: settings, field: field, factor: factor, width: width, height: height, quality: quality, resizeMode: resizeMode,
            scaleMode: scaleMode, format: format, aspectRatio: aspectRatio, parameters: strParams,
            codeApiSvc: _CodeApiSvc);

        return imageUrl;
    }

    /// <inheritdoc />
    public override bool Debug
    {
        get => base.Debug;
        set
        {
            base.Debug = value;
            imgLinker.Debug = value;
        }
    }

    /// <summary>
    /// Combine api with query string.
    /// </summary>
    public static string CombineApiWithQueryString(string api, string queryString)
    {
        queryString = queryString?.TrimStart(['?', '&']);

        // combine api with query string
        return string.IsNullOrEmpty(queryString)
            ? api
            : api?.IndexOf("?") > 0
                ? $"{api}&{queryString}"
                : $"{api}?{queryString}";
    }

    internal static string CurrentPageUrlWithEventualHashError(int? pageId, string currentPageUrl) 
        => !pageId.HasValue ? currentPageUrl : $"{currentPageUrl}#error-unknown-pageid-{pageId}";
}