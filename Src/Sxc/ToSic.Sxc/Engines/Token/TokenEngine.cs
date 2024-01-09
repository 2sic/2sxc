using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ToSic.Eav.LookUp;
using ToSic.Lib.DI;
using ToSic.Lib.Documentation;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Blocks.Internal;
using ToSic.Sxc.Code;
using ToSic.Sxc.Code.Internal;
using ToSic.Sxc.Internal;
using ToSic.Sxc.LookUp;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.Engines;

/// <summary>
/// Rendering Engine for Token based templates (html using [Content:Title] kind of placeholders. 
/// </summary>
[InternalApi_DoNotUse_MayChangeWithoutNotice("this is just fyi")]
[EngineDefinition(Name = "Token")]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class TokenEngine : EngineBase
{

    #region Replacement List to still support old Tokens
    // Version 6 to 7
    /// <summary>
    /// Token translation table - to auto-convert tokens as they were written
    /// pre v7 to be treated as they should in the new convention
    /// </summary>
    private readonly string[,] _upgrade6To7 = {
        {"[Presentation:", "[Content:Presentation:"},           // replaces all old direct references to presentation
        {"[ListPresentation:", "[ListContent:Presentation:"},   // Replaces all old references to ListPresentation
        {"[AppSettings:", "[App:Settings:"},                    // Replaces all old Settings
        {"[AppResources:", "[App:Resources:"},                  // Replaces all old Resources
        {"<repeat>", "<repeat repeat=\"Content in Data:Default\">"},    // Replace all old repeat-tags
        {"[List:", "[Content:Repeater:"},                       // Replaces all old List:Index, List:Index1, List:Alternator, IsFirst/Last etc.
    };
    #endregion

    #region Regular Expressions / String Constants

    private static readonly dynamic SourcePropertyName = new
    {
        Content = ViewParts.ContentLower,
        ListContent = ViewParts.ListContentLower
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

    private static Regex RepeatRegex = new(RepeatPattern, 
        RegexOptions.IgnoreCase 
        | RegexOptions.Multiline 
        | RegexOptions.Singleline 
        | RegexOptions.Compiled 
        | RegexOptions.IgnorePatternWhitespace);
    #endregion

    #region Constructor / DI

    private readonly LazySvc<CodeRootFactory> _codeRootFactory;
    private readonly Generator<AppConfigDelegate> _appConfigDelegateGenerator;

    public TokenEngine(MyServices services, LazySvc<CodeRootFactory> codeRootFactory, Generator<AppConfigDelegate> appConfigDelegateGenerator) : base(services) =>
        ConnectServices(
            _codeRootFactory = codeRootFactory,
            _appConfigDelegateGenerator = appConfigDelegateGenerator
        );

    #endregion

    private IDynamicCodeRoot _data;

    private TokenReplace _tokenReplace;

    [PrivateApi]
    public override void Init(IBlock block)
    {
        base.Init(block);
        InitDataHelper();
        InitTokenReplace();
    }

    private void InitDataHelper() => _data = _codeRootFactory.Value
        .BuildCodeRoot(null, Block, Log, CompatibilityLevels.CompatibilityLevel9Old);

    private void InitTokenReplace()
    {
        var confProv = _appConfigDelegateGenerator.New().GetLookupEngineForContext(Block.Context, Block.App, Block);
        _tokenReplace = new TokenReplace(confProv);
            
        // Add the Content and ListContent property sources used always
        confProv.Add(new LookUpForTokenTemplate(SourcePropertyName.ListContent, _data.Header));
        confProv.Add(new LookUpForTokenTemplate(SourcePropertyName.Content, _data.Content));
    }

    [PrivateApi]
    protected override (string, List<Exception>) RenderImplementation(object data)
    {
        var templateSource = File.ReadAllText(Services.ServerPaths.FullAppPath(TemplatePath));
        // Convert old <repeat> elements to the new ones
        for (var upgrade = 0; upgrade < _upgrade6To7.Length/2; upgrade++)
            templateSource = templateSource.Replace(_upgrade6To7[upgrade, 0], _upgrade6To7[upgrade, 1]);

        // Render all <repeat>s
        var repeatsMatches = RepeatRegex.Matches(templateSource);       
        var repeatsRendered = new List<string>();
        foreach (Match match in repeatsMatches)
            repeatsRendered.Add(RenderRepeat(match.Groups[RegexToken.SourceName].Value.ToLowerInvariant(),
                match.Groups[RegexToken.StreamName].Value,
                match.Groups[RegexToken.Template].Value));

        // Render sections between the <repeat>s (but before replace the <repeat>s and 
        // the templates contained with placeholders, so the templates in the <reapeat>s 
        // are not rendered twice)
        var template = RepeatRegex.Replace(templateSource, RepeatPlaceholder);
        var rendered = RenderSection(template, new Dictionary<string, ILookUp>());

        // Insert <repeat>s rendered to the template target
        var repeatsIndexes = FindAllIndexesOfString(rendered, RepeatPlaceholder);
        var renderedBuilder = new StringBuilder(rendered);
        for (var i = repeatsIndexes.Count - 1; i >= 0; i--) // Reversed
        {
            renderedBuilder.Remove(repeatsIndexes[i], RepeatPlaceholder.Length)
                .Insert(repeatsIndexes[i], repeatsRendered[i]);
        }

        return (renderedBuilder.ToString(), null);
    }


    private string RenderRepeat(string sourceName, string streamName, string template)
    {
        if (string.IsNullOrEmpty(template))
            return "";

        var builder = new StringBuilder();

        if (!DataSource.Out.ContainsKey(streamName))
            throw new ArgumentException("Was not able to implement REPEAT because I could not find Data:" + streamName + ". Please check spelling the pipeline delivering data to this template.");

        var dataItems = DataSource[streamName].List.ToImmutableList();
        var itemsCount = dataItems.Count;
        for (var i = 0; i < itemsCount; i++)
        {
            // Create property sources for the current data item (for the current data item and its list information)
            var propertySources = new Dictionary<string, ILookUp>();
            propertySources.Add(sourceName, new LookUpForTokenTemplate(sourceName, _data.Cdf.AsDynamic(dataItems.ElementAt(i), propsRequired: false), i, itemsCount));
            builder.Append(RenderSection(template, propertySources));
        }

        return builder.ToString();
    }

    private string RenderSection(string template, IDictionary<string, ILookUp> valuesForThisInstanceOnly)
    {
        if (string.IsNullOrEmpty(template))
            return "";

        var propertySourcesBackup = new Dictionary<string, ILookUp>();
            
        // Replace old property sources with the new ones, and backup the old ones so that 
        // they can be restored after rendering the section
        foreach(var src in valuesForThisInstanceOnly)
        {
            if (_tokenReplace.LookupEngine.Sources.ContainsKey(src.Key))
            {
                var oldSource = _tokenReplace.LookupEngine.Sources[src.Key];
                propertySourcesBackup.Add(src.Key, oldSource);
                if (oldSource != null)
                    _tokenReplace.LookupEngine.Sources.Remove(src.Key);
            }
            _tokenReplace.LookupEngine.Sources[src.Key] = src.Value;
        }

        // Render
        var sectionRendered = _tokenReplace.ReplaceTokens(template, 0);

        // Restore values list to original state
        foreach (var src in valuesForThisInstanceOnly)
        {
            _tokenReplace.LookupEngine.Sources.Remove(src.Key);
            if(propertySourcesBackup.ContainsKey(src.Key))
                _tokenReplace.LookupEngine.Sources.Add(src.Key, src.Value);
        }
            
        return sectionRendered;
    }

    // Find all indexes of a string in a source string
    private static List<int> FindAllIndexesOfString(string source, string value)
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