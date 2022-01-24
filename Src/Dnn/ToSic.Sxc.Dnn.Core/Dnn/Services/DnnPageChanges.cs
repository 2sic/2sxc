using System.Collections.Generic;
using System.Web.UI;
using DotNetNuke.Web.Client.ClientResourceManagement;
using DotNetNuke.Web.Client.Providers;
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

        public int Apply(Page page, RenderResult renderResult)
        {
            Log.Add("Will apply PageChanges");
            //var changes = _pageChanges.Apply(_pageChanges.PageServiceShared.GetPropertyChangesAndFlush());

            if (renderResult == null) return 0;

            var dnnPage = new DnnHtmlPage();

            AttachAssetsWIP(renderResult.Assets, page);
            var count = Apply(dnnPage, renderResult.PageChanges);

            var headChanges = ApplyToHead(dnnPage, renderResult.HeadChanges);

            var manualChanges = ManualFeatures(dnnPage, renderResult.ManualChanges);

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
            //var feats = PageServiceShared.Features.ManualFeaturesGetNew();
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

        private void ApplyHttpStatus(Page page, RenderResult result)
        {
            var pageServiceWithInternals = result; // _pageChanges.PageServiceShared; // as Sxc.Web.PageService.PageService;
            if (page?.Response != null && pageServiceWithInternals?.HttpStatusCode != null)
            {
                var code = pageServiceWithInternals.HttpStatusCode.Value;
                Log.Add($"Custom status code '{code}'. Will set and also {nameof(page.Response.TrySkipIisCustomErrors)}");
                page.Response.StatusCode = code;
                // Skip IIS & upstream redirects to a custom 404 so the Dnn page is preserved
                page.Response.TrySkipIisCustomErrors = true;
                if (pageServiceWithInternals.HttpStatusMessage != null)
                {
                    Log.Add($"Custom status Description '{pageServiceWithInternals.HttpStatusMessage}'.");
                    page.Response.StatusDescription = pageServiceWithInternals.HttpStatusMessage;
                }
            }
        }


        public void AttachAssetsWIP(List<ClientAssetInfo> ass, Page page)
        {
            ass.ForEach(a =>
            {
                if (a.IsJs) ClientResourceManager.RegisterScript(page, a.Url, a.Priority, DnnProviderName(a.PosInPage));
                else ClientResourceManager.RegisterStyleSheet(page, a.Url, a.Priority, DnnProviderName(a.PosInPage));
            });
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
