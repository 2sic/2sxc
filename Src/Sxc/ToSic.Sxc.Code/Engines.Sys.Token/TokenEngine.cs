using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using ToSic.Eav.Environment.Sys.ServerPaths;
using ToSic.Eav.LookUp;
using ToSic.Eav.LookUp.Sys;
using ToSic.Eav.LookUp.Sys.Engines;
using ToSic.Sxc.Apps.Sys;
using ToSic.Sxc.Blocks.Sys;
using ToSic.Sxc.Blocks.Sys.Views;
using ToSic.Sxc.Code.Sys;
using ToSic.Sxc.Code.Sys.CodeApi;
using ToSic.Sxc.Data.Sys.Factory;
using ToSic.Sxc.Engines.Sys;
using ToSic.Sxc.Engines.Sys.Token;
using ToSic.Sxc.LookUp.Sys;
using ToSic.Sxc.Render.Sys.Specs;
using ToSic.Sxc.Sys.ExecutionContext;
using ToSic.Sys.Utils.Culture;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.Engines;

/// <summary>
/// Rendering Engine for Token based templates (html using [Content:Title] kind of placeholders. 
/// </summary>
[PrivateApi("used to be InternalApi_DoNotUse_MayChangeWithoutNotice, hidden in 17.08")]
[EngineDefinition(Name = "Token")]
[ShowApiWhenReleased(ShowApiMode.Never)]
public class TokenEngine(
    EngineBase.Dependencies services,
    IServerPaths serverPaths,
    LazySvc<IExecutionContextFactory> codeRootFactory,
    Generator<IAppDataConfigProvider> tokenEngineWithContext)
    : EngineBase(services, connect: [codeRootFactory, tokenEngineWithContext]),
        ITokenEngine
        
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

    private IExecutionContext _executionContext = null!;
    private ICodeDynamicApiHelper _dynamicApiSvc = null!;

    private TokenReplace _tokenReplace = null!;

    [PrivateApi]
    public override void Init(IBlock block)
    {
        base.Init(block);
        _executionContext = codeRootFactory.Value
            .New(null, EngineSpecs.Block, Log, CompatibilityLevels.CompatibilityLevel9Old);
        _dynamicApiSvc = _executionContext.GetDynamicApi();
        InitTokenReplace();
    }

    private void InitTokenReplace()
    {
        var specs = new SxcAppDataConfigSpecs { BlockForLookupOrNull = EngineSpecs.Block };
        var appDataConfig = tokenEngineWithContext.New().GetDataConfiguration((EngineSpecs.App as SxcAppBase)!, specs);

        var lookUpEngine = new LookUpEngine(appDataConfig.LookUpEngine, Log, sources: [
            new LookUpForTokenTemplate(ViewParts.ListContentLower, _dynamicApiSvc.Header, CultureInfo),
            new LookUpForTokenTemplate(ViewParts.ContentLower, _dynamicApiSvc.Content, CultureInfo),
        ]);

        _tokenReplace = new(lookUpEngine);
    }

    [field: AllowNull, MaybeNull]
    private ICodeDataFactory Cdf => field ??= _executionContext.GetCdf();

    [field: AllowNull, MaybeNull]
    private CultureInfo CultureInfo => field ??= CultureHelpers.SafeCultureInfo(Cdf.Dimensions);


    [PrivateApi]
    protected override (string Contents, List<Exception>? Exception) RenderEntryRazor(EngineSpecs engineSpecs, RenderSpecs specs)
    {
        var templateSource = File.ReadAllText(serverPaths.FullAppPath(engineSpecs.TemplatePath));
        // Convert old <repeat> elements to the new ones
        templateSource = _upgrade6To7Dict.Aggregate(templateSource,
            (current, var) => current.Replace(var.Key, var.Value)
        );

        // Render all <repeat>s
        var repeatsMatches = RepeatRegex.Matches(templateSource);       
        var repeatsRendered = new List<string>();
        foreach (Match match in repeatsMatches)
            repeatsRendered.Add(RenderRepeat(engineSpecs,
                match.Groups[RegexToken.SourceName].Value.ToLowerInvariant(),
                match.Groups[RegexToken.StreamName].Value,
                match.Groups[RegexToken.Template].Value));

        // Render sections between the <repeat>s (but before replace the <repeat>s and 
        // the templates contained with placeholders, so the templates in the <repeat>s 
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


    private string RenderRepeat(EngineSpecs engineSpecs, string sourceName, string streamName, string template)
    {
        if (string.IsNullOrEmpty(template))
            return "";

        var builder = new StringBuilder();

        if (!engineSpecs.DataSource.Out.ContainsKey(streamName))
            throw new ArgumentException("Was not able to implement REPEAT because I could not find Data:" + streamName + ". Please check spelling the pipeline delivering data to this template.");

        var dataItems = engineSpecs.DataSource[streamName]!.List.ToImmutableOpt();
        var itemsCount = dataItems.Count;
        for (var i = 0; i < itemsCount; i++)
        {
            // Create property sources for the current data item (for the current data item and its list information)
            var dynEntity = Cdf.AsDynamic(dataItems.ElementAt(i), new() { ItemIsStrict = false });
            var propertySources = new Dictionary<string, ILookUp>
            {
                { sourceName, new LookUpForTokenTemplate(sourceName, dynEntity, CultureInfo, i, itemsCount) }
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
        {
            var lookup = sources.GetSource(src.Key);
            if (lookup != null)
                sources.Remove(lookup);
        }

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