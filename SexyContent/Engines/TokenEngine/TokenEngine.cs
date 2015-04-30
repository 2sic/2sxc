using DotNetNuke.Entities.Portals;
using DotNetNuke.Services.Tokens;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web.Hosting;
using ToSic.Eav;
using ToSic.SexyContent.DataSources;
using ToSic.SexyContent.EAVExtensions;

namespace ToSic.SexyContent.Engines.TokenEngine
{
    public class TokenEngine : EngineBase
    {
        #region RegExs and internal variables
        private static string RepeatPlaceholder = "<repeat_placeholder>";

        private static string RepeatPattern = @"
# begins with <repeat
<repeat\s
#must contain attribute repeat='...' according to a specific schema with in Data:...
repeat=['|""](?<sourceName>[a-zA-Z0-9$_]+)\sin\sData\:(?<streamName>[a-zA-Z0-9$_]+)['|""]
>
(?<template>.*?)
</repeat>
        "; //<repeat repeat=['|""](?<sourceName>[a-zA-Z0-9$_]+) in Data\:(?<streamName>[a-zA-Z0-9$_]+)['|""]>(?<template>.*?)</repeat>";

        private static Regex RepeatRegex = new Regex(RepeatPattern, 
            RegexOptions.IgnoreCase 
            | RegexOptions.Multiline 
            | RegexOptions.Singleline 
            | RegexOptions.Compiled 
            | RegexOptions.IgnorePatternWhitespace);



        private AppAndDataHelpers dataHelper;

        private TokenReplace tokenReplace;
        // private ToSic.Eav.Tokens.BaseTokenReplace eavToken;//  = new Eav.Tokens.TokenReplace();

        #endregion

        protected override void Init()
        {
            base.Init();
            InitDataHelper();
            InitTokenReplace();
        }

        private void InitDataHelper()
        {
            dataHelper = new AppAndDataHelpers(Sexy, ModuleInfo, (ViewDataSource)DataSource, App);
        }

        private void InitTokenReplace()
        {
            tokenReplace = new TokenReplace(App, ModuleInfo.ModuleID, PortalSettings.Current);           
            tokenReplace.AddPropertySource("listcontent", new DynamicEntityPropertyAccess("listcontent", dataHelper.ListContent));
            var contentProperty = dataHelper.List.FirstOrDefault();
            if (contentProperty != null)
            {// todo: const for strings
                tokenReplace.AddPropertySource("content", new DynamicEntityPropertyAccess("content", contentProperty.Content));
            }
        }



        protected override string RenderTemplate()
        {
            var templateSource = File.ReadAllText(HostingEnvironment.MapPath(TemplatePath));
            // Convert old <repeat> elements to the new ones
            templateSource = templateSource.Replace("[Presentation:", "[Content:Presentation:")
                                           .Replace("[ListPresentation:", "[ListContent:Presentation:")
                                           .Replace("<repeat>", "<repeat repeat=\"Content in Data:Default\">");

            // Render all <repeat>s
            var repeatsMatches = RepeatRegex.Matches(templateSource);       
            var repeatsRendered = new List<string>();
            foreach (Match match in repeatsMatches)
            {
                repeatsRendered.Add(RenderRepeat(match.Groups["sourceName"].Value.ToLower(), match.Groups["streamName"].Value, match.Groups["template"].Value));
            }

            // Render sections between the <repeat>s
            // First remove the <repeat> sections and the sub-templates
            // ...replace with a placeholder which will be filled later on
            var rendered = RepeatRegex.Replace(templateSource, RepeatPlaceholder);
            rendered = RenderSection(rendered, new Dictionary<string, IPropertyAccess>());

            // Re-Insert <repeat>s rendered to the template target
            var repeatsIndexes = FindAllIndexesOfString(rendered, RepeatPlaceholder);
            var templateTargetBuilder = new StringBuilder(rendered);
            for (var i = repeatsIndexes.Count - 1; i >= 0; i--)
            {
                templateTargetBuilder.Remove(repeatsIndexes[i], RepeatPlaceholder.Length)
                                     .Insert(repeatsIndexes[i], repeatsRendered[i]);
            }

            return templateTargetBuilder.ToString();
        }


        private string RenderRepeat(string sourceName, string streamName, string template)
        {
            if (string.IsNullOrEmpty(template))
                return "";

            var builder = new StringBuilder();
            
            var dataItems = DataSource[streamName].List;
            for (var i = 0; i < dataItems.Count; i++)
            {
                var dataItemInfo = new Dictionary<string, string>()  // Information about position of data item in list
                {
                    { "Index",       (i).ToString()     },
                    { "Index1",      (i + 1).ToString() },
                    { "Alternator2", (i % 2).ToString() },
                    { "Alternator3", (i % 3).ToString() },
                    { "Alternator4", (i % 4).ToString() },
                    { "Alternator5", (i % 5).ToString() },  
                    { "IsFirst",     (i == 0) ? "First" : "" },
                    { "IsLast",      (i == dataItems.Count - 1) ? "Last" : "" },
                    { "Count",       dataItems.Count.ToString() }
                };                
              
                var propertySources = new Dictionary<string, IPropertyAccess>();
                propertySources.Add("list", new DictionaryPropertyAccess(dataItemInfo));
                propertySources.Add(sourceName, new DynamicEntityPropertyAccess(sourceName, dataHelper.AsDynamic(dataItems.ElementAt(i).Value)));

                builder.Append(RenderSection(template, propertySources));
            }

            return builder.ToString();
        }

        private string RenderSection(string template, IDictionary<string, IPropertyAccess> propertySource)
        {
            if (string.IsNullOrEmpty(template))
                return "";

            var propertySourcesBackup = new Dictionary<string, IPropertyAccess>();  // Backup sources to restore initial state after rendering
            
            foreach(var src in propertySource)
            {
                propertySourcesBackup.Add(src.Key, tokenReplace.RemovePropertySource(src.Key));
                tokenReplace.AddPropertySource(src.Key, src.Value);
            }

            var sectionRendered = tokenReplace.ReplaceEnvironmentTokens(template);

            foreach (var src in propertySourcesBackup)
            {
                tokenReplace.RemovePropertySource(src.Key);
                if (src.Value != null)
                    tokenReplace.AddPropertySource(src.Key, src.Value);
            }
            
            return sectionRendered;
        }


        public static List<int> FindAllIndexesOfString(string source, string value)
        {
            var indexes = new List<int>();
            for (var index = 0; ; index += value.Length)
            {
                index = source.IndexOf(value, index);
                if (index == -1)
                {   // No more found
                    return indexes;
                }
                indexes.Add(index);
            }
        }
    }
}