using DotNetNuke.Entities.Portals;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Hosting;
using ToSic.Eav.Apps;
using ToSic.Eav.ValueProvider;
using ToSic.SexyContent.DataSources;

namespace ToSic.SexyContent.Engines.TokenEngine
{
    public class TokenEngine : EngineBase
    {
        #region Replacement List to still support old Tokens
        // Version 6 to 7
        private string[,] upgrade6to7 = {
            {"[Presentation:", "[Content:Presentation:"},           // replaces all old direct references to presentation
            {"[ListPresentation:", "[ListContent:Presentation:"},   // Replaces all old references to ListPresentation
            {"[AppSettings:", "[App:Settings:"},                    // Replaces all old Settings
            {"[AppResources:", "[App:Resources:"},                  // Replaces all old Resources
            {"<repeat>", "<repeat repeat=\"Content in Data:Default\">"},    // Replace all old repeat-tags
            {"[List:", "[Content:Repeater:"},                       // Replaces all old List:Index, List:Index1, List:Alternator, IsFirst/Last etc.
        };
        #endregion

        #region Regular Expressions / String Constants

        private static readonly dynamic SourcePropertyName =  new {
            Content     = AppConstants.ContentLower,
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


        private AppAndDataHelpers _dataHelper;

        private TokenReplaceEav _tokenReplace;


        protected override void Init()
        {
            base.Init();
            InitDataHelper();
            InitTokenReplace();
        }

        private void InitDataHelper()
        {
            _dataHelper = new AppAndDataHelpers(Sexy);
        }

        private void InitTokenReplace()
        {
            var confProv = ConfigurationProvider.GetConfigProviderForModule(ModuleInfo.ModuleID, Sexy.App, Sexy);
            _tokenReplace = new TokenReplaceEav(App, ModuleInfo.ModuleID, PortalSettings.Current, confProv);
            
            // Add the Content and ListContent property sources used always
            _tokenReplace.ValueSources.Add(SourcePropertyName.ListContent, new DynamicEntityPropertyAccess(SourcePropertyName.ListContent, _dataHelper.ListContent));
            var contentProperty = _dataHelper.List.FirstOrDefault();
            if (contentProperty != null)
            {
                _tokenReplace.ValueSources.Add(SourcePropertyName.Content, new DynamicEntityPropertyAccess(SourcePropertyName.Content, contentProperty.Content));
            }
        }



        protected override string RenderTemplate()
        {
            var templateSource = File.ReadAllText(HostingEnvironment.MapPath(TemplatePath));
            // Convert old <repeat> elements to the new ones
            for (var upgrade = 0; upgrade < upgrade6to7.Length/2; upgrade++)
                templateSource = templateSource.Replace(upgrade6to7[upgrade, 0], upgrade6to7[upgrade, 1]);

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
            var rendered = RenderSection(template, new Dictionary<string, IValueProvider>());

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
            var itemsCount = dataItems.Count;
            for (var i = 0; i < itemsCount; i++)
            {
                // Create property sources for the current data item (for the current data item and its list information)
                var propertySources = new Dictionary<string, IValueProvider>();
                propertySources.Add(sourceName, new DynamicEntityPropertyAccess(sourceName, _dataHelper.AsDynamic(dataItems.ElementAt(i).Value), i, itemsCount));
                builder.Append(RenderSection(template, propertySources));
            }

            return builder.ToString();
        }

        private string RenderSection(string template, IDictionary<string, IValueProvider> valuesForThisInstanceOnly)
        {
            if (string.IsNullOrEmpty(template))
                return "";

            var propertySourcesBackup = new Dictionary<string, IValueProvider>();
            
            // Replace old property sources with the new ones, and backup the old ones so that 
            // they can be restored after rendering the section
            foreach(var src in valuesForThisInstanceOnly)
            {
                if (_tokenReplace.ValueSources.ContainsKey(src.Key))
                {
                    var oldSource = _tokenReplace.ValueSources[src.Key];
                    propertySourcesBackup.Add(src.Key, oldSource); // tokenReplace.RemovePropertySource(src.Key));
                    if (oldSource != null)
                        _tokenReplace.ValueSources.Remove(src.Key);
                }
                _tokenReplace.ValueSources[src.Key] = src.Value;
            }

            // Render
            var sectionRendered = _tokenReplace.ReplaceTokens(template, 0);

            // Restore values list to original state
            foreach (var src in valuesForThisInstanceOnly)
            {
                _tokenReplace.ValueSources.Remove(src.Key);
                if(propertySourcesBackup.ContainsKey(src.Key))
                    _tokenReplace.ValueSources.Add(src.Key, src.Value);
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