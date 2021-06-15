using System;
using System.Web;
using DotNetNuke.Web.Client;
using DotNetNuke.Web.Client.ClientResourceManagement;
using DotNetNuke.Web.Client.Providers;
using ToSic.Sxc.Dnn.Services;
using ToSic.Sxc.Web;
using Page = System.Web.UI.Page;

namespace ToSic.Sxc.Dnn.Web
{
    public class DnnClientDependencyOptimizer: ClientDependencyOptimizer
    {
        private readonly DnnPageChanges _pageChanges;

        public DnnClientDependencyOptimizer(DnnPageChanges pageChanges)
        {
            _pageChanges = pageChanges.Init(Log);
        }

        public override Tuple<string, bool> Process(string renderedTemplate)
        {
            var wrapLog = Log.Call<Tuple<string, bool>>();
            if (HttpContext.Current == null || HttpContext.Current.CurrentHandler == null || !(HttpContext.Current.CurrentHandler is Page))
                return wrapLog("no context", new Tuple<string, bool>(renderedTemplate, false));

            JsDefaultPriority = (int)FileOrder.Js.DefaultPriority;
            CssDefaultPriority = (int)FileOrder.Css.DefaultPriority;

            var page = HttpContext.Current.CurrentHandler as Page;
            var include2SxcJs = false;
            
            // Handle Client Dependency injection
            renderedTemplate = ExtractExternalScripts(renderedTemplate, ref include2SxcJs);

            // Handle Scripts
            renderedTemplate = ExtractStyles(renderedTemplate);

            // Add to DNN
            Assets.ForEach(a =>
            {
                if(a.IsJs) ClientResourceManager.RegisterScript(page, a.Url, a.Priority, DnnProviderName(a.PosInPage));
                else ClientResourceManager.RegisterStyleSheet(page, a.Url, a.Priority, DnnProviderName(a.PosInPage));
            });

            Log.Add("Will apply PageChanges");
            var changes = _pageChanges.Apply();
            Log.Add($"Applied {changes} changes");

            Log.Add("Will apply Header Status-Code changes if needed");
            var pageServiceWithInternals = _pageChanges.PageService as Sxc.Web.PageService.PageService;
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
            
            return wrapLog("ok", new Tuple<string, bool>(renderedTemplate, include2SxcJs));
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