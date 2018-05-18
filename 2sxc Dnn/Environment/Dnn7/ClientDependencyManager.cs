using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using DotNetNuke.Web.Client;
using DotNetNuke.Web.Client.ClientResourceManagement;
using DotNetNuke.Web.Client.Providers;

namespace ToSic.SexyContent.Environment.Dnn7
{
    public class ClientDependencyManager: Base.ClientDepedencyManager
    {

        public override string Process(string renderedTemplate)
        {
            if (HttpContext.Current == null || HttpContext.Current.CurrentHandler == null || !(HttpContext.Current.CurrentHandler is Page))
                return renderedTemplate;

            var page = (HttpContext.Current.CurrentHandler as Page);

            #region  Handle Client Dependency injection

            #region Scripts

            var scriptMatches = ScriptDetection.Matches(renderedTemplate);
            var scriptMatchesToRemove = new List<Match>();

            foreach (Match match in scriptMatches)
            {
                var optMatch = OptimizeDetection.Match(match.Value);
                if (!optMatch.Success)
                    continue;

                var providerName = GetProviderName(optMatch, "body");

                var prio = GetPriority(optMatch, (int)FileOrder.Js.DefaultPriority);

                if (prio <= 0) continue;    // don't register/remove if not within specs

                // Register, then remember to remove later on
                var url = FixUrlWithSpaces(match.Groups["Src"].Value);
                ClientResourceManager.RegisterScript(page, url, prio, providerName);
                scriptMatchesToRemove.Add(match);
            }

            scriptMatchesToRemove.Reverse();
            scriptMatchesToRemove.ForEach(p => renderedTemplate = renderedTemplate.Remove(p.Index, p.Length));
            #endregion

            #region Styles

            var styleMatches = StyleDetection.Matches(renderedTemplate);
            var styleMatchesToRemove = new List<Match>();

            foreach (Match match in styleMatches)
            {
                var optMatch = OptimizeDetection.Match(match.Value);
                if (!optMatch.Success)
                    continue;

                // skip If the Rel attribute is not stylesheet
                if (!StyleRelDetect.IsMatch(match.Value))
                    continue;

                var providerName = GetProviderName(optMatch, "head");

                var prio = GetPriority(optMatch, (int)FileOrder.Css.DefaultPriority);

                if (prio <= 0) continue;    // don't register/remove if not within specs

                // Register, then remember to remove later on
                var url = FixUrlWithSpaces(match.Groups["Src"].Value);
                ClientResourceManager.RegisterStyleSheet(page, url, prio, providerName);
                styleMatchesToRemove.Add(match);
            }

            styleMatchesToRemove.Reverse();
            styleMatchesToRemove.ForEach(p => renderedTemplate = renderedTemplate.Remove(p.Index, p.Length));

            #endregion
            #endregion

            return renderedTemplate;
        }

        /// <summary>
        /// Because of an issue with spaces, prepend tilde to urls that start at root
        /// and contain spaces: https://github.com/2sic/2sxc/issues/1566
        /// </summary>
        /// <returns></returns>
        private string FixUrlWithSpaces(string url)
        {
            if (!url.Contains(" "))
                return url;
            if (!url.StartsWith("/") || url.StartsWith("//"))
                return url;
            return "~" + url;
        }

        private int GetPriority(Match optMatch, int defValue)
        {
            var priority = (optMatch.Groups["Priority"]?.Value ?? "true").ToLower();
            var prio = (priority == "true" || priority == "")
                ? defValue
                : int.Parse(priority);
            return prio;
        }

        private string GetProviderName(Match optMatch, string defaultPosition)
        {
            var position = (optMatch.Groups["Position"]?.Value ?? defaultPosition).ToLower();

            switch (position)
            {
                case "body":
                    return DnnBodyProvider.DefaultName;
                case "head":
                    return DnnPageHeaderProvider.DefaultName;
                case "bottom":
                    return DnnFormBottomProvider.DefaultName;
            }
            return "";
        }
    }
}