using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using DotNetNuke.Web.Client;
using DotNetNuke.Web.Client.ClientResourceManagement;
using DotNetNuke.Web.Client.Providers;

namespace ToSic.Sxc.Dnn.Web
{
    public class ClientDependencyManager: SexyContent.Environment.Base.ClientDependencyManager
    {

        public override Tuple<string, bool> Process(string renderedTemplate)
        {
            if (HttpContext.Current == null || HttpContext.Current.CurrentHandler == null || !(HttpContext.Current.CurrentHandler is Page))
                return new Tuple<string, bool>(renderedTemplate, false);

            var page = (HttpContext.Current.CurrentHandler as Page);
            var include2SxcJs = false;
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

                #region Register, then add to remove-queue
                var url = FixUrlWithSpaces(match.Groups["Src"].Value);

                // check special case: the 2sxc.api script. only check the first part of the path
                // because it could be .min, or have versions etc.
                if (url.ToLower()
                    .Replace("\\", "/")
                    .Contains("desktopmodules/tosic_sexycontent/js/2sxc.api"))
                    include2SxcJs = true;
                else
                    ClientResourceManager.RegisterScript(page, url, prio, providerName);
                scriptMatchesToRemove.Add(match);

                

                #endregion
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

            return new Tuple<string, bool>(renderedTemplate, include2SxcJs);
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