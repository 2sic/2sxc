using DotNetNuke.Entities.Portals;
using DotNetNuke.Services.Tokens;
using System;
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
        #region Regular Expressions / String Constants
        
        private static readonly dynamic SourcePropertyName =  new {
            Content     = "content",
            ListContent = "listcontent"
        };

        private static readonly dynamic RegexToken = new {
            SourceName = "sourceName",
            StreamName = "streamName",
            Template   = "template"
        };

        private static string RepeatPlaceholder = "<repeat_placeholder>";

        private static string RepeatPattern = @"
                # Begins with <repeat
                <repeat\s
                    # Must contain the attribute repeat='...' according to the schema with 'in Data:...'
                    repeat=['|""](?<sourceName>[a-zA-Z0-9$_]+)\sin\sData\:(?<streamName>[a-zA-Z0-9$_]+)['|""]
                    >
                    (?<template>.*?)
                </repeat>";

        private static Regex RepeatRegex = new Regex(RepeatPattern, 
                RegexOptions.IgnoreCase 
                | RegexOptions.Multiline 
                | RegexOptions.Singleline 
                | RegexOptions.Compiled 
                | RegexOptions.IgnorePatternWhitespace);
        #endregion


        private AppAndDataHelpers dataHelper;

        private TokenReplace tokenReplace;



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
            
            // Add the Content and ListContent property sources used always
            tokenReplace.AddPropertySource(SourcePropertyName.ListContent, new DynamicEntityPropertyAccess(SourcePropertyName.ListContent, dataHelper.ListContent));
            var contentProperty = dataHelper.List.FirstOrDefault();
            if (contentProperty != null)
            {
                tokenReplace.AddPropertySource(SourcePropertyName.Content, new DynamicEntityPropertyAccess(SourcePropertyName.Content, contentProperty.Content));
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
                repeatsRendered.Add(RenderRepeat(match.Groups[RegexToken.SourceName].Value.ToLower(), match.Groups[RegexToken.StreamName].Value, match.Groups[RegexToken.Template].Value));
            }

            // Render sections between the <repeat>s (but before replace the <repeat>s and 
            // the tempates contained with placeholders, so the templates in the <reapeat>s 
            // are not rendered twice)
            var template = RepeatRegex.Replace(templateSource, RepeatPlaceholder);
            var rendered = RenderSection(template, new Dictionary<string, IPropertyAccess>());

            // Insert <repeat>s rendered to the template target
            var repeatsIndexes = FindAllIndexesOfString(rendered, RepeatPlaceholder);
            var renderedBuilder = new StringBuilder(rendered);
            for (var i = repeatsIndexes.Count - 1; i >= 0; i--) // Reversed
            {
                renderedBuilder.Remove(repeatsIndexes[i], RepeatPlaceholder.Length)
                               .Insert(repeatsIndexes[i], repeatsRendered[i]);
            }

            return renderedBuilder.ToString();
        }


        private string RenderRepeat(string sourceName, string streamName, string template)
        {
            if (string.IsNullOrEmpty(template))
                return "";

            var builder = new StringBuilder();

            if (!DataSource.Out.ContainsKey(streamName))
                throw new ArgumentException("Was not able to implement REPEAT because I could not find Data:" + streamName + ". Please check spelling the pipeline delivering data to this template.");

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
              
                // Create property sources for the current data item (for the current data item and its list information)
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

            var propertySourcesBackup = new Dictionary<string, IPropertyAccess>();
            
            // Replace old property sources with the new ones, and backup the old ones so that 
            // they can be restored after rendering the section
            foreach(var src in propertySource)
            {
                propertySourcesBackup.Add(src.Key, tokenReplace.RemovePropertySource(src.Key));
                tokenReplace.AddPropertySource(src.Key, src.Value);
            }

            // Render
            var sectionRendered = tokenReplace.ReplaceEnvironmentTokens(template);

            // Restore
            foreach (var src in propertySourcesBackup)
            {
                tokenReplace.RemovePropertySource(src.Key);
                if (src.Value != null)
                    tokenReplace.AddPropertySource(src.Key, src.Value);
            }
            
            return sectionRendered;
        }

        // Find all indexes of a string in a source string
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