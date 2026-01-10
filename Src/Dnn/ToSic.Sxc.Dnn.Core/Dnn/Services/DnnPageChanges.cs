using DotNetNuke.Web.Client.ClientResourceManagement;
using DotNetNuke.Web.Client.Providers;
using System.Web;
using System.Web.UI;
using ToSic.Razor.Blade;
using ToSic.Razor.Dnn;
using ToSic.Razor.Markup;
using ToSic.Sxc.Render.Sys;
using ToSic.Sxc.Services;
using ToSic.Sxc.Sys.Configuration;
using ToSic.Sxc.Sys.Render.PageContext;
using ToSic.Sxc.Sys.Render.PageFeatures;
using ToSic.Sxc.Web.Sys.ClientAssets;
using ToSic.Sxc.Web.Sys.ContentSecurityPolicy;
using static ToSic.Sxc.Render.Sys.Output.ClientAssetConstants;

namespace ToSic.Sxc.Dnn.Services;

internal class DnnPageChanges(LazySvc<IFeaturesService> featuresService, Generator<CspOfPage> pageCspGenerator)
    : ServiceBase($"{DnnConstants.LogName}.PgeCng", connect: [featuresService, pageCspGenerator])
{
    public int Apply(Page page, IRenderResult renderResult)
    {
        var l = Log.Fn<int>("Will apply PageChanges");

        if (renderResult == null)
            return 0;

        var dnnPage = new DnnHtmlPage();

        AttachAssets(renderResult.Assets, page);
        var count = Apply(dnnPage, renderResult.PageChanges);

        var headChanges = ApplyToHead(dnnPage, renderResult.HeadChanges);

        var resourceFeatures = ResourceFeatures(dnnPage, renderResult.FeaturesFromResources);

        try
        {
            var httpHeaderChanges = ApplyHttpHeaders(page, renderResult);
            count += httpHeaderChanges;
        }
        catch { /* ignore BETA feature */ }

        l.A("Will apply Header Status-Code changes if needed");
        ApplyHttpStatus(page, renderResult);

        count += headChanges + resourceFeatures;
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

    private static int ResourceFeatures(DnnHtmlPage dnnPage, IList<PageFeatureFromSettings> feats)
    {
        // New in 12.04 - Add features which have HTML only
        // In the page the code would be like this (v14):
        // Kit.Page.Activate("fancybox4");
        // This will add a header for the sources of these features
        foreach (var f in feats)
            dnnPage.AddToHead(Tag.Custom(f.Html));
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
            PageCsp(result.CspEnforced).Add(result.CspParameters ?? []);

        if (page?.Response == null)
            return l.Return(0, "error, HttpResponse is null");
        if (page.Response.HeadersWritten)
            return l.Return(0, "error, to late for adding http headers");
        if (httpHeaders.SafeNone())
            return l.Return(0, "ok, no headers to add");

        foreach (var httpHeader in httpHeaders)
        {
            if (string.IsNullOrWhiteSpace(httpHeader.Name))
                continue;
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
            if (page?.Response == null || result?.HttpStatusCode == null)
                return;
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
        if (result.HttpStatusMessage == null)
            return;

        Log.A($"Custom status Description '{result.HttpStatusMessage}'.");
        page.Response.StatusDescription = result.HttpStatusMessage;
    }

    public void AttachAssets(IList<ClientAsset> ass, Page page)
        => ass.ToList().ForEach(a =>
        {
            if (a.IsJs)
                ClientResourceManager.RegisterScript(page, a.Url, a.Priority, DnnProviderName(a.PosInPage), a.HtmlAttributes);
            else
                ClientResourceManager.RegisterStyleSheet(page, a.Url, a.Priority, DnnProviderName(a.PosInPage));
        });
    
    private static string DnnProviderName(string position)
        => position.ToLowerInvariant() switch
        {
            AddToBody => DnnBodyProvider.DefaultName,
            AddToHead => DnnPageHeaderProvider.DefaultName,
            AddToBottom => DnnFormBottomProvider.DefaultName,
            _ => ""
        };
}