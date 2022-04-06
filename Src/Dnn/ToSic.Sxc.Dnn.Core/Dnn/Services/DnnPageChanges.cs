using DotNetNuke.Web.Client.ClientResourceManagement;
using DotNetNuke.Web.Client.Providers;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using ToSic.Eav.Documentation;
using ToSic.Eav.Logging;
using ToSic.Razor.Blade;
using ToSic.Razor.Dnn;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Web;
using ToSic.Sxc.Web.PageFeatures;
using ToSic.Sxc.Web.PageService;

namespace ToSic.Sxc.Dnn.Services
{
    [PrivateApi]
    public class DnnPageChanges : HasLog<DnnPageChanges>
    {
        public PageServiceShared PageServiceShared { get; }

        public DnnPageChanges(PageServiceShared pageServiceShared): base($"{DnnConstants.LogName}.PgeCng")
        {
            PageServiceShared = pageServiceShared;
        }

        public int Apply(Page page, IRenderResult renderResult)
        {
            Log.Add("Will apply PageChanges");

            if (renderResult == null) return 0;

            var dnnPage = new DnnHtmlPage();

            AttachAssets(renderResult.Assets, page);
            var count = Apply(dnnPage, renderResult.PageChanges);

            var headChanges = ApplyToHead(dnnPage, renderResult.HeadChanges);

            var manualChanges = ManualFeatures(dnnPage, renderResult.FeaturesFromSettings);

            var httpHeaderChanges = ApplayToHttpHeaders(page, renderResult.HttpHeaders);

            Log.Add("Will apply Header Status-Code changes if needed");
            ApplyHttpStatus(page, renderResult);

            count += headChanges + manualChanges + httpHeaderChanges;
            Log.Add($"Applied {count} changes");
            return count;
        }

        public int Apply(DnnHtmlPage dnnPage, IList<PagePropertyChange> props)
        {
            var wrapLog = Log.Call<int>();

            props = props ?? PageServiceShared.GetPropertyChangesAndFlush(Log);
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
                        //default:
                        //    throw new ArgumentOutOfRangeException();
                }

            var count = props.Count;



            return wrapLog($"{count}", count);
        }

        private int ManualFeatures(DnnHtmlPage dnnPage, IList<IPageFeature> feats)
        {
            // New in 12.04 - Add features which have HTML only
            // In the page the code would be like this:
            // var pageService = GetService<ToSic.Sxc.Web.IPageService>();
            // pageService.Activate("fancybox4");
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
                dnnPage.AddToHead(h.Tag);
            return headChanges.Count;
        }
        
        private int ApplayToHttpHeaders(Page page, IList<string> httpHeaders)
        {
            var wrapLog = Log.Call<int>();

            if (page?.Response == null) return wrapLog("error, HttpResponse is null", 0);
            if (page.Response.HeadersWritten) return wrapLog("error, to late for adding http headers", 0);
            if (httpHeaders == null || httpHeaders.Count == 0) return wrapLog("ok, no headers to add", 0);

            foreach (var httpHeader in httpHeaders)
            {
                if (string.IsNullOrWhiteSpace(httpHeader)) continue;
                string key;
                string value;
                var s = httpHeader.IndexOf(":");
                if (s < 1)
                {
                    key = httpHeader;
                    value = string.Empty;
                }
                else
                {
                    key = httpHeader.Substring(0, s);
                    value = httpHeader.Substring(s + 1);
                }
                Log.Add($"add http header: {key}:{value}");
                page.Response.Headers[key] = value;
            }
            return wrapLog("ok", httpHeaders.Count);
        }


        private void ApplyHttpStatus(Page page, IRenderResult result)
        {
            if (page?.Response == null || result?.HttpStatusCode == null) return;

            var code = result.HttpStatusCode.Value;
            Log.Add($"Custom status code '{code}'. Will set and also {nameof(page.Response.TrySkipIisCustomErrors)}");
            page.Response.StatusCode = code;
            // Skip IIS & upstream redirects to a custom 404 so the Dnn page is preserved
            page.Response.TrySkipIisCustomErrors = true;
            if (result.HttpStatusMessage == null) return;

            Log.Add($"Custom status Description '{result.HttpStatusMessage}'.");
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
            position = position.ToLowerInvariant();

            switch (position)
            {
                case "body": return DnnBodyProvider.DefaultName;
                case "head": return DnnPageHeaderProvider.DefaultName;
                case "bottom": return DnnFormBottomProvider.DefaultName;
            }
            return "";
        }

    }
}
