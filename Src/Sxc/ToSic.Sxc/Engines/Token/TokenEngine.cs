using System.Collections.Immutable;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using ToSic.Eav.Apps.Internal;
using ToSic.Eav.LookUp;
using ToSic.Lib.DI;
using ToSic.Sxc.Blocks.Internal;
using ToSic.Sxc.Code.Internal;
using ToSic.Sxc.Internal;
using ToSic.Sxc.LookUp;
using ToSic.Sxc.LookUp.Internal;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.Engines;

/// <summary>
/// Rendering Engine for Token based templates (html using [Content:Title] kind of placeholders. 
/// </summary>
[PrivateApi("used to be InternalApi_DoNotUse_MayChangeWithoutNotice, hidden in 17.08")]
[EngineDefinition(Name = "Token")]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class TokenEngine(
    EngineBase.MyServices services,
    LazySvc<CodeApiServiceFactory> codeRootFactory,
    Generator<IAppDataConfigProvider> tokenEngineWithContext)
    : EngineBase(services, connect: [codeRootFactory, tokenEngineWithContext])
{
    #region Replacement List to still support old Tokens

    /// <summary>
    /// Token translation table - to auto-convert tokens as they were written
    /// pre v7 to be treated as they should in the new convention
    /// </summary>
    private readonly Dictionary<string, string> _upgrade6To7Dict = new()
    {
        { "[Presentation:", "[Content:Presentation:" },
        { "[ListPresentation:", "[ListContent:Presentation:" },
        { "[AppSettings:", "[App:Settings:" },
        { "[AppResources:", "[App:Resources:" },
        { "<repeat>", "<repeat repeat=\"Content in Data:Default\">" },
        { "[List:", "[Content:Repeater:" }
    };

    #endregion

    #region Regular Expressions / String Constants

    private static readonly dynamic RegexToken = new {
        SourceName = "sourceName",
        StreamName = "streamName",
        Template   = "template"
    };

    private static readonly string RepeatPlaceholder = "<repeat_placeholder>";

    private static readonly string RepeatPattern = @"
                # Begins with <repeat
                <repeat\s
                    # Must contain the attribute repeat='...' according to the schema with 'in Data:...'
                    repeat=['|""](?<sourceName>[a-zA-Z0-9$_]+)\sin\sData\:(?<streamName>[a-zA-Z0-9$_]+)['|""]
                    >
                    (?<template>.*?)
                </repeat>";

    private static readonly Regex RepeatRegex = new(RepeatPattern, 
        RegexOptions.IgnoreCase 
        | RegexOptions.Multiline 
        | RegexOptions.Singleline 
        | RegexOptions.Compiled 
        | RegexOptions.IgnorePatternWhitespace);
    #endregion

    private ICodeApiService _codeApiSvc;

    private TokenReplace _tokenReplace;

    [PrivateApi]
    public override void Init(IBlock block)
    {
        base.Init(block);
        InitDataHelper();
        InitTokenReplace();
    }

    private void InitDataHelper() => _codeApiSvc = codeRootFactory.Value
        .BuildCodeRoot(null, Block, Log, CompatibilityLevels.CompatibilityLevel9Old);

    private void InitTokenReplace()
    {
        var specs = new SxcAppDataConfigSpecs { BlockForLookupOrNull = Block };
        var appDataConfig = tokenEngineWithContext.New().GetDataConfiguration(Block.App as EavApp, specs);

        var lookUpEngine = new LookUpEngine(appDataConfig.Configuration, Log, sources: [
            new LookUpForTokenTemplate(ViewParts.ListContentLower, _codeApiSvc.Header, _codeApiSvc),
            new LookUpForTokenTemplate(ViewParts.ContentLower, _codeApiSvc.Content, _codeApiSvc),
        ]);

        _tokenReplace = new(lookUpEngine);
    }

    [PrivateApi]
    protected override (string Contents, List<Exception> Exception) RenderEntryRazor(RenderSpecs specs)
    {
        var templateSource = File.ReadAllText(Services.ServerPaths.FullAppPath(TemplatePath));
        // Convert old <repeat> elements to the new ones
        templateSource = _upgrade6To7Dict.Aggregate(templateSource,
            (current, var) => current.Replace(var.Key, var.Value)
        );

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

        // Replace the parts in reversed order, so the indexes don't shift
        for (var i = repeatsIndexes.Count - 1; i >= 0; i--)
            renderedBuilder
                .Remove(repeatsIndexes[i], RepeatPlaceholder.Length)
                .Insert(repeatsIndexes[i], repeatsRendered[i]);

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
            var dynEntity = _codeApiSvc.Cdf.AsDynamic(dataItems.ElementAt(i), propsRequired: false);
            var propertySources = new Dictionary<string, ILookUp>
            {
                { sourceName, new LookUpForTokenTemplate(sourceName, dynEntity, _codeApiSvc, i, itemsCount) }
            };
            builder.Append(RenderSection(template, propertySources));
        }

        return builder.ToString();
    }

    private string RenderSection(string template, IDictionary<string, ILookUp> valuesForThisInstanceOnly)
    {
        var l = Log.Fn<string>($"{nameof(valuesForThisInstanceOnly)}: {valuesForThisInstanceOnly.Count}");
        if (string.IsNullOrEmpty(template))
            return l.Return("", "empty");

        // Get the existing sources and remove the ones which would be duplicate
        var sources = _tokenReplace.LookupEngine.Sources.ToList();
        foreach (var src in valuesForThisInstanceOnly) 
            sources.Remove(sources.GetSource(src.Key));

        // Create a new lookup engine with the new sources; but skip the original sources; only use the downstream
        var newEngine = new LookUpEngine(_tokenReplace.LookupEngine, Log, sources: [..valuesForThisInstanceOnly.Values, ..sources], skipOriginalSource: true);
        var tokenReplace = new TokenReplace(newEngine);

        // Render
        var section = tokenReplace.ReplaceTokens(template);
        return l.Return(section, "new render");

    }

    // Find all indexes of a string in a source string
    private static List<int> FindAllIndexesOfString(string source, string value)
    {
        var indexes = new List<int>();
        for (var index = 0; ; index += value.Length)
        {
            index = source.IndexOf(value, index, StringComparison.Ordinal);
            if (index == -1) // No more found
                return indexes;
            indexes.Add(index);
        }
    }
}