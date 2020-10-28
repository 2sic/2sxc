using System;
using System.Web;
using System.Web.UI;
using DotNetNuke.Web.Client;
using DotNetNuke.Web.Client.ClientResourceManagement;
using DotNetNuke.Web.Client.Providers;
using ToSic.Sxc.Web;

namespace ToSic.Sxc.Dnn.Web
{
    public class DnnClientDependencyOptimizer: ClientDependencyOptimizer
    {

        public override Tuple<string, bool> Process(string renderedTemplate)
        {
            if (HttpContext.Current == null || HttpContext.Current.CurrentHandler == null || !(HttpContext.Current.CurrentHandler is Page))
                return new Tuple<string, bool>(renderedTemplate, false);

            JsDefaultPriority = (int)FileOrder.Js.DefaultPriority;
            CssDefaultPriority = (int)FileOrder.Css.DefaultPriority;

            var page = HttpContext.Current.CurrentHandler as Page;
            var include2SxcJs = false;
            
            // Handle Client Dependency injection
            renderedTemplate = ExtractScripts(renderedTemplate, ref include2SxcJs);

            // Handle Scripts
            renderedTemplate = ExtractStyles(renderedTemplate);

            // Add to DNN
            Assets.ForEach(a =>
            {
                if(a.IsJs) ClientResourceManager.RegisterScript(page, a.Url, a.Priority, DnnProviderName(a.PosInPage));
                else ClientResourceManager.RegisterStyleSheet(page, a.Url, a.Priority, DnnProviderName(a.PosInPage));
            });
            
            return new Tuple<string, bool>(renderedTemplate, include2SxcJs);
        }


        private string DnnProviderName(string position/*, string defaultPosition*/)
        {
            position = (/*optMatch.Groups["Position"]?.Value*/position /*?? defaultPosition*/).ToLower();

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