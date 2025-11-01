using ToSic.Eav.Data.Sys;
using ToSic.Eav.Data.Sys.PropertyLookup;
using ToSic.Sxc.Data.Sys.Factory;
using ToSic.Sxc.Data.Sys.Typed;
using static System.StringComparer;

namespace ToSic.Sxc.Data.Sys.Dynamic;

/// <param name="overrider">Optional helper for templating scenarios, which can replace the source with something else - typically for replacing "file:72" with something from a template</param>
[ShowApiWhenReleased(ShowApiMode.Never)]
internal class GetAndConvertHelper(
    IHasPropLookup parent,
    ICodeDataFactory cdf,
    bool propsRequired,
    bool childrenShouldBeDynamic,
    ICanDebug canDebug,
    IValueOverrider? overrider = default)
{
    #region Setup and Log

    public ICodeDataFactory Cdf { get; } = cdf;

    public bool PropsRequired { get; } = propsRequired;

    public bool Debug => _debug ?? canDebug.Debug;
    private bool? _debug;

    [field: AllowNull, MaybeNull]
    internal GetAndConvertConverter Converter => field ??= new(Cdf, PropsRequired, childrenShouldBeDynamic, canDebug, overrider);

    public ILog? LogOrNull => _logOrNull.Get(() => Cdf.Log.SubLogOrNull("Sxc.GetCnv", Debug));
    private readonly GetOnce<ILog?> _logOrNull = new();

    #endregion

    #region Get Implementations 1:1 - names must be identical with caller, so the exceptions have the right names

    public object? Get(string name) => TryGet(name, lookupLink: true).Result;

    // ReSharper disable once MethodOverloadWithOptionalParameter
    public object? Get(string name, NoParamOrder noParamOrder = default, string? language = null, bool convertLinks = true, bool? debug = null)
    {
        _debug = debug;
        var result = TryGet(name, language: language, lookupLink: convertLinks).Result;
        _debug = null;

        return result;
    }

    public TValue? Get<TValue>(string name)
        => TryGet(name).Result.ConvertOrDefault<TValue>();

    // ReSharper disable once MethodOverloadWithOptionalParameter
    public TValue? Get<TValue>(string name, NoParamOrder noParamOrder = default, TValue? fallback = default)
        => TryGet(name).Result.ConvertOrFallback(fallback);

    #endregion

    #region Try-Get Values

    /// <summary>
    /// 
    /// </summary>
    /// <param name="field"></param>
    /// <param name="language"></param>
    /// <param name="lookupLink"></param>
    /// <returns></returns>
    public TryGetResult TryGet(string? field, string? language = null, bool lookupLink = false)
    {
        var logOrNull = LogOrNull.SubLogOrNull("GnC.GetInt", Debug);
        var l = logOrNull.Fn<TryGetResult>($"Type: {parent.GetType().Name}, {nameof(field)}:{field}, {nameof(language)}:{language}, {nameof(lookupLink)}:{lookupLink}");

        if (!field.HasValue())
            return l.Return(new(false, null), "field null/empty");

        // This determines if we should access & store in cache
        // check if we already have it in the cache - but only in default case (no language, lookup=true)
        var cacheKey = (field + "$" + lookupLink + "-" + language).ToLowerInvariant();
        if (_rawValCache.TryGetValue(cacheKey, out var cached))
            return l.Return(cached, "cached");

        // Figure out best order of languages to look up
        var languages = LanguagePreprocessor.GetLookupLanguages(language, Cdf);

        l.A($"cache-key: {cacheKey}, {nameof(languages)}:{languages.Length}");

        // check if we have an explicitly set language resulting in an empty language list - then exit now
        if (!languages.Any())
            return l.Return(new(false, null), "no languages to look-up, exit");

        // Get the field or the path if it has one
        // Classic field case
        var specs = new PropReqSpecs(field, languages, true, logOrNull);
        var path = new PropertyLookupPath().Add("DynEntStart", field);
        var resultSet = parent.PropertyLookup.FindPropertyInternal(specs, path);

        // check Entity is null (in cases where null-objects are asked for properties)
        if (resultSet == null! /* paranoid */)
            return l.Return(new(false, null), "result null");

        l.A($"Result... IsFinal: {resultSet.IsFinal}, Source Name: {resultSet.Name}, SourceIndex: {resultSet.SourceIndex}, FieldType: {resultSet.ValueType}");

        var result = Converter.ValueAutoConverted(resultSet, lookupLink, field, logOrNull);

        // cache result, but only if using default languages
        l.A("add to cache");
        var found = resultSet.ValueType != ValueTypesWithState.NotFound;
        var final = new TryGetResult(found, result);
        if (found)
            _rawValCache.Add(cacheKey, final);

        return l.Return(final, "ok");
    }

    private readonly Dictionary<string, TryGetResult> _rawValCache = new(InvariantCultureIgnoreCase);

    #endregion
}