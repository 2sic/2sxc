using System.Data.Objects.DataClasses;
using System.Text.RegularExpressions;
using System.Web.UI;
using DotNetNuke.Web.Client.ClientResourceManagement;
using ToSic.SexyContent;
using System;
using DotNetNuke.UI.Modules;
using DotNetNuke.Entities.Portals;
using System.Web;
using System.Reflection;
using ToSic.SexyContent.Engines;
using System.Collections.Generic;
using System.Linq;

namespace ToSic.SexyContent
{
    public partial class Template
    {
        ///// <summary>
        ///// Returns the path to the current template file
        ///// </summary>
        ///// <param name="PortalSettings"></param>
        ///// <returns></returns>
        //public string GetTemplatePath(App app)
        //{
        //    return System.IO.Path.Combine(SexyContent.GetTemplatePathRoot(Location), Path);
        //}

        /// <summary>
        /// Returns true if the current template uses Razor
        /// </summary>
        public bool IsRazor
        {
            get
            {
                return (Type == "C# Razor" || Type == "VB Razor");
            }
        }

        /// <summary>
        /// Renders the given elements with Razor or TokenReplace and returns the string representation.
        /// </summary>
        /// <param name="Server"></param>
        /// <param name="PortalSettings"></param>
        /// <param name="ModuleContext"></param>
        /// <param name="LocalResourceFile"></param>
        /// <param name="Entity"></param>
        /// <param name="Elements"></param>
        /// <returns></returns>
        public string RenderTemplate(Page Page, HttpServerUtility Server, PortalSettings PortalSettings, ModuleInstanceContext ModuleContext, string LocalResourceFile, List<Element> Elements, Element ListElement, System.Web.UI.Control Control, ToSic.Eav.DataSources.IDataSource DataSource, SexyContent sexy, int appId)
        {
            string templatePath = System.IO.Path.Combine(sexy.GetTemplatePathRoot(this.Location), this.Path);

            // Throw Exception if Template does not exist
            if (!System.IO.File.Exists(Server.MapPath(templatePath)))
                throw new SexyContentException("The template file '" + templatePath + "' does not exist.");

            Type EngineType;

            if (IsRazor)
            {
                // Load the Razor Engine via Reflection
                Assembly EngineAssembly = Assembly.Load("ToSic.SexyContent.Razor");
                EngineType = EngineAssembly.GetType("ToSic.SexyContent.Engines.RazorEngine");
            }
            else
            {
                // Load Token Engine
                EngineType = typeof(ToSic.SexyContent.Engines.TokenEngine.TokenEngine);
            }

            if (EngineType == null)
                throw new Exception("Error: Could not find the template engine to parse this template.");

            IEngine Engine = (IEngine)Activator.CreateInstance(EngineType, null);

            var app = SexyContent.GetApp(SexyContent.GetZoneID(PortalSettings.Current.PortalId).Value, appId);

            // Render elements that have a content (by DemoEntityID or real Entity ID)
            string RenderedTemplate = Engine.Render(this, templatePath, app, Elements.Where(p => p.Content != null).ToList(), ListElement, ModuleContext, LocalResourceFile, DataSource);

            #region  Handle Client Dependency injection

            var EnableClientDependencyRegex = "\\sdata-enableoptimizations=('|\")true('|\")(>|\\s)";

            #region Scripts
            var ScriptMatches = Regex.Matches(RenderedTemplate, "<script\\s([^>]*)src=('|\")(?<Src>.*?)('|\")(([^>]*/>)|[^>]*(>.*?</script>))", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            var ScriptMatchesToRemove = new List<Match>();

            foreach (Match Match in ScriptMatches)
            {
                if (!Regex.IsMatch(Match.Value, EnableClientDependencyRegex, RegexOptions.IgnoreCase))
                    continue;

                var Path = Match.Groups["Src"].Value;

                ClientResourceManager.RegisterScript(Page, Path);

                // Remove the script tag from the Rendered Template
                ScriptMatchesToRemove.Add(Match);
            }

            ScriptMatchesToRemove.Reverse();
            ScriptMatchesToRemove.ForEach(p => RenderedTemplate = RenderedTemplate.Remove(p.Index, p.Length));
            #endregion

            #region Styles

            var StyleMatches = Regex.Matches(RenderedTemplate, "<link\\s([^>]*)href=('|\")(?<Src>.*?)('|\")([^>]*)(>.*?</link>|/>)", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            var StyleMatchesToRemove = new List<Match>();

            foreach (Match Match in StyleMatches)
            {
                if (!Regex.IsMatch(Match.Value, EnableClientDependencyRegex, RegexOptions.IgnoreCase))
                    continue;

                // Break If the Rel attribute is not stylesheet
                if (!Regex.IsMatch(Match.Value, "('|\"|\\s)rel=('|\")stylesheet('|\")", RegexOptions.IgnoreCase))
                    continue;

                var Path = Match.Groups["Src"].Value;

                ClientResourceManager.RegisterStyleSheet(Page, Path);

                // Remove the script tag from the Rendered Template
                StyleMatchesToRemove.Add(Match);
            }

            StyleMatchesToRemove.Reverse();
            StyleMatchesToRemove.ForEach(p => RenderedTemplate = RenderedTemplate.Remove(p.Index, p.Length));

            #endregion
            #endregion

            return RenderedTemplate;
        }
    }
}