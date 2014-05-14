using System.Web;
using System.Web.Hosting;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.UI.Modules;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text.RegularExpressions;
using ToSic.Eav.DataSources;
using ToSic.SexyContent.DataSources;

namespace ToSic.SexyContent.Engines.TokenEngine
{
    public class TokenEngine : EngineBase
    {

        /// <summary>
        /// Renders a Token Template
        /// </summary>
        /// <returns>Rendered template as string</returns>
        protected override string RenderTemplate()
        {
            DynamicEntity listContent = null;
            DynamicEntity listPresentation = null;

            var moduleDataSource = (ModuleDataSource)((IDataTarget)DataSource).In["Default"].Source;
            listContent = moduleDataSource.ListElement != null ? moduleDataSource.ListElement.Content : null;
            listPresentation = moduleDataSource.ListElement != null ? moduleDataSource.ListElement.Presentation : null;
            var elements = moduleDataSource.ContentElements.Where(p => p.Content != null).ToList();

            // Prepare Source Text
            string sourceText = System.IO.File.ReadAllText(HostingEnvironment.MapPath(TemplatePath));
            string repeatingPart;
            bool containsRepeat = sourceText.Contains("<repeat>") && sourceText.Contains("</repeat>");
            
            // Prepare List Object
            var list = new Dictionary<string, string>
            {
                {"Index", "0"},
                {"Index1", "1"},
                {"Count", "1"},
                {"IsFirst", "First"},
                {"IsLast", "Last"},
                {"Alternator2", "0"},
                {"Alternator3", "0"},
                {"Alternator4", "0"},
                {"Alternator5", "0"}
            };

            list["Count"] = elements.Count.ToString();

            // If the SourceText contains a <repeat>, define Repeating Part. Else take SourceText as repeating part.
            if (containsRepeat)
            {
                repeatingPart = Regex.Match(sourceText, @"<repeat>(.*?)</repeat>", RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.Singleline).Groups[1].Captures[0].Value;
                var tr = new TokenReplace(null, null, listContent, listPresentation, list, App);
                tr.ModuleId = ModuleInfo.ModuleID;
                tr.PortalSettings = PortalSettings.Current;
                sourceText = tr.ReplaceEnvironmentTokens(sourceText);
            }
            else
                repeatingPart = sourceText;

            TokenReplace tokenReplace;
            string renderedTemplate = "";

            foreach (Element element in elements)
            {
                // Modify List Object
                list["Index"] = elements.IndexOf(element).ToString();
                list["Index1"] = (elements.IndexOf(element) + 1).ToString();
                list["IsFirst"] = elements.First() == element ? "First" : "";
                list["IsLast"] = elements.Last() == element ? "Last" : "";
                list["Alternator2"] = (elements.IndexOf(element) % 2).ToString();
                list["Alternator3"] = (elements.IndexOf(element) % 3).ToString();
                list["Alternator4"] = (elements.IndexOf(element) % 4).ToString();
                list["Alternator5"] = (elements.IndexOf(element) % 5).ToString();

                // Replace Tokens
                tokenReplace = new TokenReplace(element.Content, element.Presentation, listContent, listPresentation, list, App);
                tokenReplace.ModuleId = ModuleInfo.ModuleID;
                tokenReplace.PortalSettings = PortalSettings.Current;
                renderedTemplate += tokenReplace.ReplaceEnvironmentTokens(repeatingPart);
            }

            // Return Rendered Template
            renderedTemplate = containsRepeat ? Regex.Replace(sourceText, "<repeat>.*?</repeat>", renderedTemplate, RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.Singleline) : renderedTemplate;

            return renderedTemplate;
        }

        /// <summary>
        /// The template engine does not support PrepareViewData yet, so do nothing
        /// </summary>
        public bool PrepareViewData()
        {
            return false;
        }
    }
}