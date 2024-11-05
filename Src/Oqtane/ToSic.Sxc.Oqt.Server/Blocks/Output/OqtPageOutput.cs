using Oqtane.Models;
using Oqtane.Shared;
using System;
using ToSic.Eav.Helpers;
using ToSic.Lib.Services;
using ToSic.Sxc.Blocks.Internal;
using ToSic.Sxc.Blocks.Internal.Render;
using ToSic.Sxc.Oqt.Shared;
using ToSic.Sxc.Web.Internal.JsContext;
using ToSic.Sxc.Web.Internal.PageFeatures;

namespace ToSic.Sxc.Oqt.Server.Blocks.Output;

[PrivateApi]
internal partial class OqtPageOutput(
    SiteState siteState,
    IBlockResourceExtractor blockResourceExtractor,
    IJsApiService jsApiService)
    : ServiceBase($"{OqtConstants.OqtLogPrefix}.AssHdr", connect: [siteState, blockResourceExtractor, jsApiService])
{
    #region Constructor and DI

    public void Init(IOqtSxcViewBuilder parent, IRenderResult renderResult)
    {
        Parent = parent;
        RenderResult = renderResult;
    }

    protected IOqtSxcViewBuilder Parent;
    protected IRenderResult RenderResult;

    #endregion


    private bool AddJsCore => Features.Contains(SxcPageFeatures.JsCore);
    private bool AddJsEdit => Features.Contains(SxcPageFeatures.JsCmsInternal);


    /// <summary>
    /// The JavaScripts needed
    /// </summary>
    /// <returns></returns>
    public IEnumerable<string> Scripts()
    {
        var list = new List<string>();

        // v12.03, Oqtane 2.2 with Bootstrap 5 do not includes jQuery any more
        // as Oqtane 2.1 with Bootstrap 4
        if (Features.Contains(SxcPageFeatures.JQuery)) 
            list.Add("//code.jquery.com/jquery-3.5.1.min.js");

        if (AddJsCore) list.Add($"{OqtConstants.UiRoot}/{SxcPageFeatures.JsCore.UrlInDist}");

        if (AddJsEdit) list.Add($"{OqtConstants.UiRoot}/{SxcPageFeatures.JsCmsInternal.UrlInDist}");

        // New in 12.02
        if (Features.Contains(SxcPageFeatures.TurnOn))
            list.Add($"{OqtConstants.UiRoot}/{SxcPageFeatures.TurnOn.UrlInDist}");

            
        return list;
    }

    /// <summary>
    /// The styles to add
    /// </summary>
    /// <returns></returns>
    public IEnumerable<string> Styles()
    {
        var list = new List<string>();
        if (Features.Contains(SxcPageFeatures.ToolbarsInternal))
            list.Add($"{OqtConstants.UiRoot}/{SxcPageFeatures.ToolbarsInternal.UrlInDist}");

        // New 15.01
        if (Features.Contains(SxcPageFeatures.CmsWysiwyg))
            list.Add($"{OqtConstants.UiRoot}/{SxcPageFeatures.CmsWysiwyg.UrlInDist}");
        return list;
    }

    [PrivateApi]
    public static string GetSiteRoot(Alias alias)
        => alias?.Name == null ? OqtConstants.SiteRoot : new Uri($"http://{alias.Name}/").AbsolutePath.SuffixSlash();

    internal IList<IPageFeature> Features => _features ??= RenderResult.Features ?? new List<IPageFeature>();
    private IList<IPageFeature> _features;

}