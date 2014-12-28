using System.Globalization;
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
		private IEnumerable<DynamicEntity> GetEntitiesFromStream(IDictionary<string, IDataStream> streams, string streamName, string[] dimensions)
		{
			return streams.ContainsKey(streamName)
				? streams[streamName].List.Select(e => new DynamicEntity(e.Value, dimensions, Sexy))
				: new DynamicEntity[0];
		}

		/// <summary>
		/// Renders a Token Template
		/// </summary>
		/// <returns>Rendered template as string</returns>
		protected override string RenderTemplate()
		{
			// ToDo: Get rid of ModuleDataSource, use In-Streams instead
			var dimensionIds = new[] { System.Threading.Thread.CurrentThread.CurrentCulture.Name };
			var inStreams = ((IDataTarget)DataSource).In;

			//var moduleDataSource = DataPipelineFactory.FindDataSource<ModuleDataSource>((IDataTarget)DataSource);
			//listContent = moduleDataSource.ListElement != null ? moduleDataSource.ListElement.Content : null;
			var listContent = GetEntitiesFromStream(inStreams, "ListContent", dimensionIds).FirstOrDefault();
			//listPresentation = moduleDataSource.ListElement != null ? moduleDataSource.ListElement.Presentation : null;
			var listPresentation = GetEntitiesFromStream(inStreams, "ListPresentation", dimensionIds).FirstOrDefault();
			//var elements = moduleDataSource.ContentElements.Where(p => p.Content != null).ToList();
			var elements = GetEntitiesFromStream(inStreams, "Default", dimensionIds);
			var presentation = GetEntitiesFromStream(inStreams, "Presentation", dimensionIds).FirstOrDefault();


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

			var elementsCount = elements.Count();
			list["Count"] = elementsCount.ToString(CultureInfo.InvariantCulture);

			// If the SourceText contains a <repeat>, define Repeating Part. Else take SourceText as repeating part.
			var containsRepeat = sourceText.Contains("<repeat>") && sourceText.Contains("</repeat>");
			if (containsRepeat)
				repeatingPart = Regex.Match(sourceText, @"<repeat>(.*?)</repeat>", RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.Singleline).Groups[1].Captures[0].Value;
			else
				repeatingPart = "";

			var renderedTemplate = "";

			var elementIndex = 0;
			foreach (var element in elements)
			{
				// Modify List Object
				list["Index"] = elementIndex.ToString();
				list["Index1"] = (elementIndex + 1).ToString();
				list["IsFirst"] = elementIndex == 0 ? "First" : "";
				list["IsLast"] = elementsCount == elementsCount - 1 ? "Last" : "";
				list["Alternator2"] = (elementIndex % 2).ToString();
				list["Alternator3"] = (elementIndex % 3).ToString();
				list["Alternator4"] = (elementIndex % 4).ToString();
				list["Alternator5"] = (elementIndex % 5).ToString();

				// Replace Tokens
				DynamicEntity listItemPresentation = null; // ToDo: get correct Presentation-Entity
				var tokenReplace = new TokenReplace(element, listItemPresentation, listContent, listPresentation, list, App)
				{
					ModuleId = ModuleInfo.ModuleID,
					PortalSettings = PortalSettings.Current
				};
				renderedTemplate += tokenReplace.ReplaceEnvironmentTokens(repeatingPart);

				elementIndex++;
			}

			// Replace repeating part
			renderedTemplate = Regex.Replace(sourceText, "<repeat>.*?</repeat>", renderedTemplate, RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.Singleline);

			// Replace tokens outside the repeating part
			var tr2 = new TokenReplace(elements.Any() ? elements.First() : null, elements.Any() ? presentation : null, listContent, listPresentation, list, App)
			{
				ModuleId = ModuleInfo.ModuleID,
				PortalSettings = PortalSettings.Current
			};
			renderedTemplate = tr2.ReplaceEnvironmentTokens(renderedTemplate);

			return renderedTemplate;
		}


	}
}