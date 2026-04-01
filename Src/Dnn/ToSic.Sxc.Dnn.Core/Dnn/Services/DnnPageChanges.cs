using DotNetNuke.Abstractions.Pages;
using DotNetNuke.Web.Client.ClientResourceManagement;
using DotNetNuke.Web.Client.Providers;
using DotNetNuke.Web.MvcPipeline.ModuleControl.Page;
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
using DnnPageService = DotNetNuke.Abstractions.Pages.IPageService;

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
            count += ApplyHttpHeaders(page?.Response, renderResult);
        }
        catch
        {
            /* ignore BETA feature */
        }

        l.A("Will apply Header Status-Code changes if needed");
        ApplyHttpStatus(page?.Response, renderResult);

        count += headChanges + resourceFeatures;
        return l.Return(count, $"Applied {count} changes");
    }

    public int ApplyMvc(PageConfigurationContext pageContext, IRenderResult renderResult)
    {
        var l = Log.Fn<int>("Will apply MVC PageChanges");
        if (renderResult == null)
            return 0;

        AttachAssetsMvc(renderResult.Assets, pageContext);
        var count = Apply(pageContext.PageService, renderResult.PageChanges);
        var headChanges = ApplyToHead(pageContext.PageService, renderResult.HeadChanges, priorityBase: 100);
        var resourceFeatures = ResourceFeatures(pageContext.PageService, renderResult.FeaturesFromResources, priorityBase: 200);

        try
        {
            count += ApplyHttpHeaders(HttpContext.Current?.Response, renderResult);
        }
        catch
        {
            /* ignore */
        }

        ApplyHttpStatus(HttpContext.Current?.Response, renderResult);
        count += headChanges + resourceFeatures;
        return l.Return(count, $"Applied {count} MVC changes");
    }

    private int Apply(DnnHtmlPage dnnPage, IList<PagePropertyChange> props)
    {
        var l = Log.Fn<int>($"{props.Count} props");
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

        return l.Return(props.Count, $"{props.Count}");
    }

    private int Apply(DnnPageService pageService, IList<PagePropertyChange> props)
    {
        var l = Log.Fn<int>($"{props.Count} props");
        for (var i = 0; i < props.Count; i++)
        {
            var p = props[i];
            switch (p.Property)
            {
                case PageProperties.Base:
                    if (!string.IsNullOrWhiteSpace(p.Value))
                        pageService.AddToHead(new PageTag($"<base href=\"{HttpUtility.HtmlAttributeEncode(p.Value)}\" />", i));
                    break;
                case PageProperties.Title:
                    pageService.SetTitle(Helpers.UpdateProperty(pageService.GetTitle(), p), i);
                    break;
                case PageProperties.Description:
                    pageService.SetDescription(Helpers.UpdateProperty(pageService.GetDescription(), p), i);
                    break;
                case PageProperties.Keywords:
                    pageService.SetKeyWords(Helpers.UpdateProperty(pageService.GetKeyWords(), p), i);
                    break;
            }
        }

        return l.Return(props.Count, $"{props.Count}");
    }

    private static int ResourceFeatures(DnnHtmlPage dnnPage, IList<PageFeatureFromSettings> feats)
    {
        foreach (var f in feats)
            dnnPage.AddToHead(Tag.Custom(f.Html));
        return feats.Count;
    }

    private static int ResourceFeatures(DnnPageService pageService, IList<PageFeatureFromSettings> feats, int priorityBase)
    {
        for (var i = 0; i < feats.Count; i++)
            pageService.AddToHead(new PageTag(feats[i].Html, priorityBase + i));
        return feats.Count;
    }

    private int ApplyToHead(DnnHtmlPage dnnPage, IList<HeadChange> headChanges)
    {
        foreach (var h in headChanges)
            dnnPage.AddToHead(h.Tag as TagBase);
        return headChanges.Count;
    }

    private int ApplyToHead(DnnPageService pageService, IList<HeadChange> headChanges, int priorityBase)
    {
        for (var i = 0; i < headChanges.Count; i++)
            pageService.AddToHead(new PageTag(headChanges[i].Tag?.ToString() ?? string.Empty, priorityBase + i));
        return headChanges.Count;
    }

    private int ApplyHttpHeaders(HttpResponse response, IRenderResult result)
    {
        var l = Log.Fn<int>();
        var httpHeaders = result.HttpHeaders;

        if (result.CspEnabled && featuresService.Value.IsEnabled(SxcFeatures.ContentSecurityPolicy.NameId))
            PageCsp(result.CspEnforced).Add(result.CspParameters ?? []);

        if (response == null)
            return l.Return(0, "error, HttpResponse is null");
        if (response.HeadersWritten)
            return l.Return(0, "error, too late for adding http headers");
        if (httpHeaders.SafeNone())
            return l.Return(0, "ok, no headers to add");

        foreach (var httpHeader in httpHeaders)
        {
            if (string.IsNullOrWhiteSpace(httpHeader.Name))
                continue;
            Log.A($"add http header: {httpHeader.Name}:{httpHeader.Value}");
            response.Headers[httpHeader.Name] = httpHeader.Value;
        }

        return l.ReturnAsOk(httpHeaders.Count);
    }

    private CspOfPage PageCsp(bool enforced)
    {
        const string key = "2sxcPageLevelCsp";

        if (HttpContext.Current.Items.Contains(key))
            return (CspOfPage)HttpContext.Current.Items[key];

        var pageLevelCsp = pageCspGenerator.New();
        HttpContext.Current.Items[key] = pageLevelCsp;

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

    private void ApplyHttpStatus(HttpResponse response, IRenderResult result)
    {
        try
        {
            if (response == null || result?.HttpStatusCode == null)
                return;
        }
        catch (Exception ex)
        {
            Log.Ex(ex);
            return;
        }

        var code = result.HttpStatusCode.Value;
        Log.A($"Custom status code '{code}'. Will set and also {nameof(response.TrySkipIisCustomErrors)}");
        response.StatusCode = code;
        response.TrySkipIisCustomErrors = true;
        if (result.HttpStatusMessage == null)
            return;

        Log.A($"Custom status Description '{result.HttpStatusMessage}'.");
        response.StatusDescription = result.HttpStatusMessage;
    }

    public void AttachAssets(IList<ClientAsset> ass, Page page)
        => ass.ToList().ForEach(a =>
        {
            if (a.IsJs)
                ClientResourceManager.RegisterScript(page, a.Url, a.Priority, DnnProviderName(a.PosInPage), a.HtmlAttributes);
            else
                ClientResourceManager.RegisterStyleSheet(page, a.Url, a.Priority, DnnProviderName(a.PosInPage));
        });

    public void AttachAssetsMvc(IList<ClientAsset> assets, PageConfigurationContext pageContext)
    {
        foreach (var asset in assets ?? [])
        {
            if (string.IsNullOrWhiteSpace(asset.Url))
                continue;

            if (asset.IsJs)
            {
                var script = pageContext.ClientResourceController?.CreateScript(asset.Url);
                if (script == null)
                    continue;

                if (asset.HtmlAttributes?.ContainsKey("async") == true)
                    script.Async = true;
                if (asset.HtmlAttributes?.ContainsKey("defer") == true)
                    script.Defer = true;

                pageContext.ClientResourceController.AddScript(script);
            }
            else
            {
                var stylesheet = pageContext.ClientResourceController?.CreateStylesheet(asset.Url);
                if (stylesheet == null)
                    continue;
                pageContext.ClientResourceController.AddStylesheet(stylesheet);
            }
        }
    }

    private static string DnnProviderName(string position)
        => position.ToLowerInvariant() switch
        {
            AddToBody => DnnBodyProvider.DefaultName,
            AddToHead => DnnPageHeaderProvider.DefaultName,
            AddToBottom => DnnFormBottomProvider.DefaultName,
            _ => ""
        };
}
