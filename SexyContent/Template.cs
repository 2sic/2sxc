using System.Data;
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

        public IEngine RenderEngine
        {
            get
            {
                Type engineType;

                if (IsRazor)
                {
                    // Load the Razor Engine via Reflection
                    var engineAssembly = Assembly.Load("ToSic.SexyContent.Razor");
                    engineType = engineAssembly.GetType("ToSic.SexyContent.Engines.RazorEngine");
                }
                else
                {
                    // Load Token Engine
                    engineType = typeof (Engines.TokenEngine.TokenEngine);
                }

                if (engineType == null)
                    throw new Exception("Error: Could not find the template engine to parse this template.");

                return (IEngine) Activator.CreateInstance(engineType, null);
            }
        }

        /// <summary>
        /// Renders the given elements with Razor or TokenReplace and returns the string representation.
        /// </summary>
        /// <param name="Server"></param>
        /// <param name="portalSettings"></param>
        /// <param name="ModuleContext"></param>
        /// <param name="LocalResourceFile"></param>
        /// <param name="Entity"></param>
        /// <param name="Elements"></param>
        /// <returns></returns>
        public string RenderTemplate(Page Page, PortalSettings portalSettings, ModuleInstanceContext ModuleContext, string LocalResourceFile, ToSic.Eav.DataSources.IDataSource DataSource, SexyContent sexy)
        {
            var templatePath = VirtualPathUtility.Combine(sexy.GetTemplatePathRoot(this.Location) + "/", this.Path);

            // Throw Exception if Template does not exist
            if (!System.IO.File.Exists(HttpContext.Current.Server.MapPath(templatePath)))
                throw new SexyContentException("The template file '" + templatePath + "' does not exist.");

            var app = SexyContent.GetApp(SexyContent.GetZoneID(PortalSettings.Current.PortalId).Value, sexy.AppId.Value);

            // Render elements that have a content (by DemoEntityId or real Entity Id)
            var renderedTemplate = RenderEngine.Render(this, templatePath, app, ModuleContext, LocalResourceFile, DataSource);

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
                    ClientResourceManager.RegisterScript(Page, path);
                else
                    ClientResourceManager.RegisterScript(Page, path, int.Parse(priority));

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

                if(priority == "true")
                    ClientResourceManager.RegisterStyleSheet(Page, path);
                else
                    ClientResourceManager.RegisterStyleSheet(Page, path, int.Parse(priority));

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