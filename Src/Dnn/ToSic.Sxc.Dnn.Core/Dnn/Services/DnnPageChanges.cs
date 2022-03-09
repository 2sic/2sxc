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

            Log.Add("Will apply Header Status-Code changes if needed");
            ApplyHttpStatus(page, renderResult);

            count += headChanges + manualChanges;
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
        /// Our implementation is necessary because it is not possible to provide
        /// additional html attributes with DNN ClientResourceManager.RegisterScript.
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
                // Used to set the HtmlAttributes on DnnJsInclude class via a string which is parsed.
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
