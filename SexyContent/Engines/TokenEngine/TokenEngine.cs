using System.Web;
using DotNetNuke.UI.Modules;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text.RegularExpressions;
using ToSic.Eav.DataSources;
using ToSic.SexyContent.DataSources;

namespace ToSic.SexyContent.Engines.TokenEngine
{
    public class TokenEngine : IEngine
    {
        /// <summary>
        /// Renders a Token Template
        /// </summary>
        /// <param name="TemplatePath">Path to the template</param>
        /// <param name="TemplateText">Text of the template</param>
        /// <param name="HostingModule">Module that hosts this instance</param>
        /// <param name="LocalResourcesPath">Resource path</param>
        /// <param name="Elements">The SexyContent Element list</param>
        /// <returns></returns>
        public string Render(Template template, string templatePath, App app, ModuleInstanceContext HostingModule, string LocalResourcesPath, IDataSource DataSource)
        {
            List<Element> Elements;
            DynamicEntity ListContent = null;
            DynamicEntity ListPresentation = null;

            var moduleDataSource = (ModuleDataSource) DataSource;
            ListContent = moduleDataSource.ListElement != null ? moduleDataSource.ListElement.Content : null;
            ListPresentation = moduleDataSource.ListElement != null ? moduleDataSource.ListElement.Presentation : null;
            Elements = moduleDataSource.ContentElements;

            // Prepare Source Text
            string SourceText = System.IO.File.ReadAllText(HttpContext.Current.Server.MapPath(templatePath));
            string RepeatingPart;
            bool ContainsRepeat = SourceText.Contains("<repeat>") && SourceText.Contains("</repeat>");
            

            // Prepare List Object
            Dictionary<string, string> List = new Dictionary<string, string>();
            List.Add("Index", "0");
            List.Add("Index1", "1");
            List.Add("Count", "1");
            List.Add("IsFirst", "First");
            List.Add("IsLast", "Last");
            List.Add("Alternator2", "0");
            List.Add("Alternator3", "0");
            List.Add("Alternator4", "0");
            List.Add("Alternator5", "0");

            List["Count"] = Elements.Count.ToString();

            // If the SourceText contains a <repeat>, define Repeating Part. Else take SourceText as repeating part.
            if (ContainsRepeat)
            {
                RepeatingPart = Regex.Match(SourceText, @"<repeat>(.*?)</repeat>", RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.Singleline).Groups[1].Captures[0].Value;
                var tr = new TokenReplace(null, null, ListContent, ListPresentation, List, app);
                tr.ModuleId = HostingModule.ModuleId;
                tr.PortalSettings = HostingModule.PortalSettings;
                SourceText = tr.ReplaceEnvironmentTokens(SourceText);
            }
            else
                RepeatingPart = SourceText;

            TokenReplace TokenReplace;
            string RenderedTemplate = "";

            foreach (Element Element in Elements)
            {
                // Modify List Object
                List["Index"] = Elements.IndexOf(Element).ToString();
                List["Index1"] = (Elements.IndexOf(Element) + 1).ToString();
                List["IsFirst"] = Elements.First() == Element ? "First" : "";
                List["IsLast"] = Elements.Last() == Element ? "Last" : "";
                List["Alternator2"] = (Elements.IndexOf(Element) % 2).ToString();
                List["Alternator3"] = (Elements.IndexOf(Element) % 3).ToString();
                List["Alternator4"] = (Elements.IndexOf(Element) % 4).ToString();
                List["Alternator5"] = (Elements.IndexOf(Element) % 5).ToString();

                // Replace Tokens
                TokenReplace = new TokenReplace(Element.Content, Element.Presentation, ListContent, ListPresentation, List, app);
                TokenReplace.ModuleId = HostingModule.ModuleId;
                TokenReplace.PortalSettings = HostingModule.PortalSettings;
                RenderedTemplate += TokenReplace.ReplaceEnvironmentTokens(RepeatingPart);
            }

            // Return Rendered Template
            RenderedTemplate = ContainsRepeat ? Regex.Replace(SourceText, "<repeat>.*?</repeat>", RenderedTemplate, RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.Singleline) : RenderedTemplate;

            return RenderedTemplate;
        }

        public void PrepareViewData() {}
    }
}