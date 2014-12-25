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
		private DynamicEntity GetEntityFromStream(IDictionary<string, IDataStream> streams, string streamName, string[] dimensions)
		{
			return streams.ContainsKey(streamName)
				? streams[streamName].List.Select(e => new DynamicEntity(e.Value, dimensions, Sexy)).FirstOrDefault()
				: null;
		}

		/// <summary>
		/// Renders a Token Template
		/// </summary>
		/// <returns>Rendered template as string</returns>
		protected override string RenderTemplate()
		{
			// ToDo: Get rid of ModuleDataSource, use In-Streams instead
			var dimensions = new[] { "" };
			var inStreams = ((IDataTarget)DataSource).In;

			//var moduleDataSource = DataPipelineFactory.FindDataSource<ModuleDataSource>((IDataTarget)DataSource);
			//listContent = moduleDataSource.ListElement != null ? moduleDataSource.ListElement.Content : null;
			var listContent = GetEntityFromStream(inStreams, "ListContent", dimensions);
			//listPresentation = moduleDataSource.ListElement != null ? moduleDataSource.ListElement.Presentation : null;
			var listPresentation = GetEntityFromStream(inStreams, "ListPresentation", dimensions);
			//var elements = moduleDataSource.ContentElements.Where(p => p.Content != null).ToList();
			var elements = "";

			// Prepare Source Text
			string sourceText = System.IO.File.ReadAllText(HostingEnvironment.MapPath(TemplatePath));
			string repeatingPart;

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
			bool containsRepeat = sourceText.Contains("<repeat>") && sourceText.Contains("</repeat>");
			if (containsRepeat)
				repeatingPart = Regex.Match(sourceText, @"<repeat>(.*?)</repeat>", RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.Singleline).Groups[1].Captures[0].Value;
			else
				repeatingPart = "";

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
				var tokenReplace = new TokenReplace(element.Content, element.Presentation, listContent, listPresentation, list, App);
				tokenReplace.ModuleId = ModuleInfo.ModuleID;
				tokenReplace.PortalSettings = PortalSettings.Current;
				renderedTemplate += tokenReplace.ReplaceEnvironmentTokens(repeatingPart);
			}

			// Replace repeating part
			renderedTemplate = Regex.Replace(sourceText, "<repeat>.*?</repeat>", renderedTemplate, RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.Singleline);

			// Replace tokens outside the repeating part
			var tr2 = new TokenReplace(elements.Any() ? elements.First().Content : null, elements.Any() ? elements.First().Presentation : null, listContent, listPresentation, list, App);
			tr2.ModuleId = ModuleInfo.ModuleID;
			tr2.PortalSettings = PortalSettings.Current;
			renderedTemplate = tr2.ReplaceEnvironmentTokens(renderedTemplate);

			return renderedTemplate;
		}


	}
}