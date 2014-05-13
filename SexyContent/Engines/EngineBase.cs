using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Hosting;
using System.Web.UI;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Web.Client.ClientResourceManagement;
using IDataSource = ToSic.Eav.DataSources.IDataSource;

namespace ToSic.SexyContent.Engines
{
    public abstract class EngineBase : IEngine
    {
        protected Template Template;
        protected string TemplatePath;
        protected App App;
        protected ModuleInfo ModuleInfo;
        protected IDataSource DataSource;

        public void Init(Template template, App app, ModuleInfo hostingModule, IDataSource dataSource)
        {
            var templatePath = VirtualPathUtility.Combine(SexyContent.GetTemplatePathRoot(template.Location, app, new PortalSettings(hostingModule.PortalID)) + "/", template.Path);

            // Throw Exception if Template does not exist
            if (!System.IO.File.Exists(HostingEnvironment.MapPath(templatePath)))
                throw new SexyContentException("The template file '" + templatePath + "' does not exist.");

            Template = template;
            TemplatePath = templatePath;
            App = app;
            ModuleInfo = hostingModule;
            DataSource = dataSource;

            this.Init();
        }

        public abstract string Render();

        protected virtual void Init() {}

        public virtual bool PrepareViewData()
        {
            return true;
        }

        /// <summary>
        /// Renders the given elements with Razor or TokenReplace and returns the string representation.
        /// </summary>
        /// <returns></returns>
        public string RenderTemplate(SexyContent sexy)
        {
            var renderedTemplate = Render();
            return HandleClientDependencyInjection(renderedTemplate);
        }

        private string HandleClientDependencyInjection(string renderedTemplate)
        {
            if (HttpContext.Current == null || HttpContext.Current.CurrentHandler == null)
                return renderedTemplate;

            var page = (HttpContext.Current.CurrentHandler as Page);

            #region  Handle Client Dependency injection

            var clientDependencyRegex = "\\sdata-enableoptimizations=('|\")(?<Priority>true|[0-9]+)('|\")(>|\\s)";

            #region Scripts
            var scriptMatches = Regex.Matches(renderedTemplate, "<script\\s([^>]*)src=('|\")(?<Src>.*?)('|\")(([^>]*/>)|[^>]*(>.*?</script>))", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            var scriptMatchesToRemove = new List<Match>();

            foreach (Match match in scriptMatches)
            {
                var clientDependencyMatch = Regex.Match(match.Value, clientDependencyRegex, RegexOptions.IgnoreCase);
                if (!clientDependencyMatch.Success)
                    continue;

                var path = match.Groups["Src"].Value;
                var priority = clientDependencyMatch.Groups["Priority"].Value;

                if (priority == "true")
                    ClientResourceManager.RegisterScript(page, path);
                else
                    ClientResourceManager.RegisterScript(page, path, int.Parse(priority));

                // Remove the script tag from the Rendered Template
                scriptMatchesToRemove.Add(match);
            }

            scriptMatchesToRemove.Reverse();
            scriptMatchesToRemove.ForEach(p => renderedTemplate = renderedTemplate.Remove(p.Index, p.Length));
            #endregion

            #region Styles

            var styleMatches = Regex.Matches(renderedTemplate, "<link\\s([^>]*)href=('|\")(?<Src>.*?)('|\")([^>]*)(>.*?</link>|/>)", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            var styleMatchesToRemove = new List<Match>();

            foreach (Match match in styleMatches)
            {
                var clientDependencyMatch = Regex.Match(match.Value, clientDependencyRegex, RegexOptions.IgnoreCase);
                if (!clientDependencyMatch.Success)
                    continue;

                // Break If the Rel attribute is not stylesheet
                if (!Regex.IsMatch(match.Value, "('|\"|\\s)rel=('|\")stylesheet('|\")", RegexOptions.IgnoreCase))
                    continue;

                var path = match.Groups["Src"].Value;
                var priority = clientDependencyMatch.Groups["Priority"].Value;

                if (priority == "true")
                    ClientResourceManager.RegisterStyleSheet(page, path);
                else
                    ClientResourceManager.RegisterStyleSheet(page, path, int.Parse(priority));

                // Remove the script tag from the Rendered Template
                styleMatchesToRemove.Add(match);
            }

            styleMatchesToRemove.Reverse();
            styleMatchesToRemove.ForEach(p => renderedTemplate = renderedTemplate.Remove(p.Index, p.Length));

            #endregion
            #endregion

            return renderedTemplate;
        }
    }
}