using DotNetNuke.Web.Client.ClientResourceManagement;
using DotNetNuke.Web.Client.Providers;
using System.Web;
using System.Web.UI;
using ToSic.Eav.Plumbing;
using ToSic.Lib.Services;
using ToSic.Razor.Blade;
using ToSic.Razor.Dnn;
using ToSic.Razor.Markup;
using ToSic.Sxc.Blocks.Internal.Render;
using ToSic.Sxc.Configuration.Internal;
using ToSic.Sxc.Services;
using ToSic.Sxc.Web.Internal.ClientAssets;
using ToSic.Sxc.Web.Internal.ContentSecurityPolicy;
using ToSic.Sxc.Web.Internal.PageFeatures;
using ToSic.Sxc.Web.Internal.PageService;
using static ToSic.Sxc.Web.Internal.ClientAssets.ClientAssetConstants;

namespace ToSic.Sxc.Dnn.Services;

internal class DnnPageChanges(LazySvc<IFeaturesService> featuresService, Generator<CspOfPage> pageCspGenerator)
    : ServiceBase($"{DnnConstants.LogName}.PgeCng", connect: [featuresService, pageCspGenerator])
{
    public int Apply(Page page, IRenderResult renderResult)
    {
        var l = Log.Fn<int>("Will apply PageChanges");

        if (renderResult == null) return 0;

        var dnnPage = new DnnHtmlPage();

        AttachAssets(renderResult.Assets, page);
        var count = Apply(dnnPage, renderResult.PageChanges);

        var headChanges = ApplyToHead(dnnPage, renderResult.HeadChanges);

        var manualChanges = ManualFeatures(dnnPage, renderResult.FeaturesFromSettings);

        try{
            var httpHeaderChanges = ApplyHttpHeaders(page, renderResult);
            count += httpHeaderChanges;
        }
        catch { /* ignore BETA feature */ }

        l.A("Will apply Header Status-Code changes if needed");
        ApplyHttpStatus(page, renderResult);

        count += headChanges + manualChanges;
        return l.Return(count, $"Applied {count} changes");
    }

    private int Apply(DnnHtmlPage dnnPage, IList<PagePropertyChange> props)
    {
        var l = Log.Fn<int>($"{props.Count} props");
        // 2022-05-03 2dm - don't think the props are ever null, requiring access to the shared data
        // props = props ?? PageServiceShared.GetPropertyChangesAndFlush(Log);
        foreach (var p in props)
            switch (p.Property)
            {
                case PageProperties.Base:
                    dnnPage.AddBase(p.Value);
                    break;
                case PageProperties.Title:
                    dnnPage.Title = Helpers.UpdateProperty(dnnPage.Title, p);
                    break;
                case PageProperties.Description:
                    dnnPage.Description = Helpers.UpdateProperty(dnnPage.Description, p);
                    break;
                case PageProperties.Keywords:
                    dnnPage.Keywords = Helpers.UpdateProperty(dnnPage.Keywords, p);
                    break;
            }

        var count = props.Count;

        return l.Return(count, $"{count}");
    }

    private int ManualFeatures(DnnHtmlPage dnnPage, IList<IPageFeature> feats)
    {
        // New in 12.04 - Add features which have HTML only
        // In the page the code would be like this (v14):
        // Kit.Page.Activate("fancybox4");
        // This will add a header for the sources of these features
        foreach (var f in feats) dnnPage.AddToHead(Tag.Custom(f.Html));
        return feats.Count;
    }

    private int ApplyToHead(DnnHtmlPage dnnPage, IList<HeadChange> headChanges)
    {
        // Note: we're not implementing replace etc. in DNN
        // ATM there's no reason to, maybe some other time
        //var headChanges = PageServiceShared.GetHeadChangesAndFlush();
        foreach (var h in headChanges)
            dnnPage.AddToHead(h.Tag as TagBase);
        return headChanges.Count;
    }

    private int ApplyHttpHeaders(Page page, IRenderResult result)
    {
        var l = Log.Fn<int>();
        var httpHeaders = result.HttpHeaders;

        // Register CSP changes for applying once all modules have been prepared
        // Note that in cached scenarios, CspEnabled is true, but it may have been turned off since
        if (result.CspEnabled && featuresService.Value.IsEnabled(SxcFeatures.ContentSecurityPolicy.NameId))
            PageCsp(result.CspEnforced).Add(result.CspParameters);

        if (page?.Response == null) return l.Return(0, "error, HttpResponse is null");
        if (page.Response.HeadersWritten) return l.Return(0, "error, to late for adding http headers");
        if (httpHeaders.SafeNone()) return l.Return(0, "ok, no headers to add");

        foreach (var httpHeader in httpHeaders)
        {
            if (string.IsNullOrWhiteSpace(httpHeader.Name)) continue;
            Log.A($"add http header: {httpHeader.Name}:{httpHeader.Value}");
            // TODO: The CSP header can only exist once
            // So to do this well, we'll need to merge them in future, 
            // Ideally combining the existing one with any additional ones added here
            page.Response.Headers[httpHeader.Name] = httpHeader.Value;
        }

        return l.ReturnAsOk(httpHeaders.Count);
    }

    private CspOfPage PageCsp(bool enforced)
    {
        var key = "2sxcPageLevelCsp";

        // If it's already registered, then the add-on-sending has already been added too
        // So we shouldn't repeat it, just return the cache which will be used later
        if (HttpContext.Current.Items.Contains(key))
            return (CspOfPage)HttpContext.Current.Items[key];

        // Not yet registered. Create, and register for on-end of request
        var pageLevelCsp = pageCspGenerator.New();// new CspOfPage();
        HttpContext.Current.Items[key] = pageLevelCsp;

        // Register event to attach headers once the request is done and all Apps have registered their Csp
        HttpContext.Current.Response.AddOnSendingHeaders(context =>
        {
            try
            {
                var headers = pageLevelCsp.CspHttpHeader();
                if (headers != null)
                    context.Response.Headers[pageLevelCsp.HeaderName(enforced)] = headers;
            }
            catch
            {
                /* ignore */
            }
        });
        return pageLevelCsp;
    }


    private void ApplyHttpStatus(Page page, IRenderResult result)
    {
        try
        {
            if (page?.Response == null || result?.HttpStatusCode == null) return;
        }
        catch (Exception ex)
        {
            // FIX: handles "Response is not available in this context." in edge case for 404 page in DNN
            // https://github.com/2sic/2sxc/issues/2986
            Log.Ex(ex);
            return;
        }

        var code = result.HttpStatusCode.Value;
        Log.A($"Custom status code '{code}'. Will set and also {nameof(page.Response.TrySkipIisCustomErrors)}");
        page.Response.StatusCode = code;
        // Skip IIS & upstream redirects to a custom 404 so the Dnn page is preserved
        page.Response.TrySkipIisCustomErrors = true;
        if (result.HttpStatusMessage == null) return;

        Log.A($"Custom status Description '{result.HttpStatusMessage}'.");
        page.Response.StatusDescription = result.HttpStatusMessage;
    }


    public void AttachAssets(IList<IClientAsset> ass, Page page)
    {
        ass.ToList().ForEach(a =>
        {
            if (a.IsJs) RegisterJsScript(page, a);
            else ClientResourceManager.RegisterStyleSheet(page, a.Url, a.Priority, DnnProviderName(a.PosInPage));
        });
    }

    /// <summary>
    /// Register JS script with additional html attributes.
    /// </summary>
    /// <remarks>
    /// Our implementation that is almost exactly the same as DNN ClientResourceManager.RegisterScript
    /// https://github.com/dnnsoftware/Dnn.Platform/blob/62b82997fbd5338fc9468ad82f3eb7191433b542/DNN%20Platform/DotNetNuke.Web.Client/ClientResourceManager.cs#L231
    /// is necessary because we provide additional html attributes.
    /// As usual The Client Resource Management framework will automatically
    /// minify and combine JS files (when enabled in DNN and DNN is not in debug mode)
    /// because we still use DotNetNuke.Web.Client.DnnJsInclude class
    /// to register our js script with additional attributes. 
    /// </remarks>
    /// <param name="page"></param>
    /// <param name="clientAsset"></param>
    private void RegisterJsScript(Page page, IClientAsset clientAsset)
    {
        var include = new DnnJsInclude 
        {
            ForceProvider = DnnProviderName(clientAsset.PosInPage), 
            Priority = clientAsset.Priority, 
            FilePath = clientAsset.Url, 
            AddTag = false
        }; // direct dependency on ClientDependency.Core.dll (included in default DNN installation)
        if (clientAsset.HtmlAttributes.Count > 0)
        {
            // Convert HtmlAttributes dictionary to string.
            // The syntax for the string must be: key1:value1, key2:value2   etc...
            // Used to set the HtmlAttributes on DnnJsInclude class via a string.
            // This is DNN (and ClientDependency) supported way to provide additional HtmlAttributes
            // https://github.com/Shazwazza/ClientDependency/wiki/Html-Attributes
            var list = clientAsset.HtmlAttributes.Select(a => $"{a.Key}:{(!string.IsNullOrEmpty(a.Value) ? a.Value : a.Key)}").ToList();
            var htmlAttributesAsString = string.Join(",", list);
            include.HtmlAttributesAsString = htmlAttributesAsString;
        }
        page.FindControl("ClientResourceIncludes")?.Controls.Add(include);
    }

    private string DnnProviderName(string position)
    {
        switch (position.ToLowerInvariant())
        {
            case AddToBody: return DnnBodyProvider.DefaultName;
            case AddToHead: return DnnPageHeaderProvider.DefaultName;
            case AddToBottom: return DnnFormBottomProvider.DefaultName;
            default: return "";
        }
    }

}