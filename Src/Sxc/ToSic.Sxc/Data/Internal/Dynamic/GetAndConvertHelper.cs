using ToSic.Eav.Data.PropertyLookup;
using ToSic.Eav.Plumbing;
using ToSic.Lib.Helpers;
using ToSic.Sxc.Data.Internal.Decorators;
using ToSic.Sxc.Data.Internal.Typed;
using static System.StringComparer;

namespace ToSic.Sxc.Data.Internal.Dynamic;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
internal class GetAndConvertHelper(
    IHasPropLookup parent,
    CodeDataFactory cdf,
    bool propsRequired,
    bool childrenShouldBeDynamic,
    ICanDebug canDebug)
{
    #region Setup and Log

    public CodeDataFactory Cdf { get; } = cdf;

    public bool PropsRequired { get; } = propsRequired;

    public IHasPropLookup Parent { get; } = parent;

    public bool Debug => _debug ?? canDebug.Debug;
    private bool? _debug;

    internal SubDataFactory SubDataFactory => _subData ??= new(Cdf, PropsRequired, canDebug);
    private SubDataFactory _subData;

    public ILog LogOrNull => _logOrNull.Get(() => Cdf?.Log?.SubLogOrNull("Sxc.GetCnv", Debug));
    private readonly GetOnce<ILog> _logOrNull = new();

    #endregion

    #region Get Implementations 1:1 - names must be identical with caller, so the exceptions have the right names

    public object Get(string name) => GetInternal(name, lookupLink: true).Result;

    public object Get(string name, NoParamOrder noParamOrder = default, string language = null, bool convertLinks = true, bool? debug = null)
    {
        _debug = debug;
        var result = GetInternal(name, language, convertLinks).Result;
        _debug = null;

        return result;
    }

    public TValue Get<TValue>(string name)
        => TryGet(name).Result.ConvertOrDefault<TValue>();

    public TValue Get<TValue>(string name, NoParamOrder noParamOrder = default, TValue fallback = default)
        => TryGet(name).Result.ConvertOrFallback(fallback);

    #endregion

    #region Get Values

    public TryGetResult TryGet(string field, string language)
        => GetInternal(field, language, lookupLink: false);

    public TryGetResult TryGet(string field)
        => GetInternal(field, null, lookupLink: false);

    public TryGetResult GetInternal(string field, string language = null, bool lookupLink = true)
    {
        var logOrNull = LogOrNull.SubLogOrNull("GnC.GetInt", Debug);
        var l = logOrNull.Fn<TryGetResult>($"Type: {Parent.GetType().Name}, {nameof(field)}:{field}, {nameof(language)}:{language}, {nameof(lookupLink)}:{lookupLink}");

        if (!field.HasValue())
            return l.Return(new(false, null), "field null/empty");

        // This determines if we should access & store in cache
        // check if we already have it in the cache - but only in default case (no language, lookup=true)
        var cacheKey = (field + "$" + lookupLink + "-" + language).ToLowerInvariant();
        if (_rawValCache.TryGetValue(cacheKey, out var cached))
            return l.Return(cached, "cached");

        // use the standard dimensions or overload
        var languages = language == null
            ? Cdf.Dimensions
            : GetFinalLanguagesList(language, Cdf.SiteCultures, Cdf.Dimensions);

        l.A($"cache-key: {cacheKey}, {nameof(languages)}:{languages.Length}");

        // check if we have an explicitly set language resulting in an empty language list - then exit now
        if (!languages.Any())
            return l.Return(new(false, null), "no languages to look-up, exit");

        // Get the field or the path if it has one
        // Classic field case
        var specs = new PropReqSpecs(field, languages, true, logOrNull);
        var path = new PropertyLookupPath().Add("DynEntStart", field);
        var resultSet = Parent.PropertyLookup.FindPropertyInternal(specs, path);

        // check Entity is null (in cases where null-objects are asked for properties)
        if (resultSet == null)
            return l.Return(new(false, null), "result null");

        l.A($"Result... IsFinal: {resultSet.IsFinal}, Source Name: {resultSet.Name}, SourceIndex: {resultSet.SourceIndex}, FieldType: {resultSet.ValueType}");

        var result = ValueAutoConverted(resultSet, lookupLink, field, logOrNull);

        // cache result, but only if using default languages
        l.A("add to cache");
        var found = resultSet.ValueType != ValueTypesWithState.NotFound;
        var final = new TryGetResult(found, result);
        if (found)
            _rawValCache.Add(cacheKey, final);

        return l.Return(final, "ok");
    }

    /// <summary>
    /// Full logic, as static, testable method
    /// </summary>
    /// <param name="language"></param>
    /// <param name="possibleDims"></param>
    /// <param name="defaultDims"></param>
    /// <returns></returns>
    internal static string[] GetFinalLanguagesList(string language, List<string> possibleDims, string[] defaultDims)
    {
        // if nothing specified, use default
        if (language == null)
            return defaultDims;

        var languages = language.ToLowerInvariant()
            .Split(',')
            .Select(s => s.Trim())
            .ToArray();

        // expand language codes, e.g.
        // - "en" should become "en-us" if available
        // - "" should become null to signal fallback to default
        var final = languages
            .Select(l =>
            {
                if (l == "") return null;
                // note: availableDims usually has a null-entry at the end
                // note: both l and availableDims are lowerInvariant
                var found = possibleDims.FirstOrDefault(ad => ad?.StartsWith(l) == true);
                return found ?? "not-found";
            })
            .Where(s => s != "not-found")
            .ToArray();

        return final;
    }

    private readonly Dictionary<string, TryGetResult> _rawValCache = new(InvariantCultureIgnoreCase);

    private object ValueAutoConverted(PropReqResult original, bool lookupLink, string field, ILog logOrNull)
    {
        var l = logOrNull.Fn<object>($"..., {nameof(lookupLink)}: {lookupLink}, {nameof(field)}: {field}");
        var value = original.Result;
        var parent = original.Source as IEntity;
        // New mechanism to not use resolve-hyperlink
        if (lookupLink && value is string strResult
                       && original.ValueType == ValueTypesWithState.Hyperlink
                       && ValueConverterBase.CouldBeReference(strResult))
        {
            l.A($"Try to convert value - HasValueConverter: {Cdf.Services.ValueConverterOrNull != null}");
            value = Cdf.Services.ValueConverterOrNull?.ToValue(strResult, parent?.EntityGuid ?? Guid.Empty) ?? value;
            return l.Return(value, "link-conversion");
        }

        // note 2021-06-07 previously in created sub-entities with modified language-list; I think this is wrong
        // Note 2021-06-08 if the parent is _not_ an IEntity, this will throw an error. Could happen in the DynamicStack, but that should never have such children
        if (value is IEnumerable<IEntity> children)
        {
            if (childrenShouldBeDynamic)
            {
                l.A($"Convert entity list as {nameof(DynamicEntity)}");
                var dynEnt = new DynamicEntity(children.ToArray(), parent, field, null, propsRequired: PropsRequired, Cdf);
                if (Debug) dynEnt.Debug = true;
                return l.Return(dynEnt, "ent-list, now dyn");
            }
            l.A($"Convert entity list as {nameof(ITypedItem)}");
            var converted = AsChildrenItems(entities: children, field: field, parentEntity: parent);

            // if (Debug) converted.ForEach(c => c.Debug = true);
            return l.Return(converted, "ent-list, now dyn");
        }

        // special debug of path if possible
        if (canDebug.Debug)
            try
            {
                    
                var finalPath = string.Join(" > ", original.Path?.Parts?.ToArray() ?? []);
                l.A($"Debug path: {finalPath}");
            }
            catch {/* ignore */}

        return l.Return(value, "unmodified");
    }

    #endregion

    #region Parents / Children - ATM still dynamic

    public List<IDynamicEntity> ParentsDyn(IEntity entity, string type, string field)
        => entity.Parents(type, field)
            .Select(SubDataFactory.SubDynEntityOrNull)
            .ToList();

    public List<ITypedItem> ParentsItems(IEntity entity, string type, string field)
        => entity.Parents(type, field)
            .Select(e => new TypedItemOfEntity(null, e, Cdf, PropsRequired) as ITypedItem)
            .ToList();


    public List<IDynamicEntity> ChildrenDyn(IEntity entity, string field, string type)
        => AsChildrenDyn(entity.Children(field, type), field, parentEntity: entity);

    public List<ITypedItem> ChildrenItems(IEntity entity, string field, string type)
        => AsChildrenItems(entity.Children(field, type), field, parentEntity: entity);

    private List<IDynamicEntity> AsChildrenDyn(IEnumerable<IEntity> entities, string field, IEntity parentEntity)
        => AsChildrenOf(entities, field, parentEntity, SubDataFactory.SubDynEntityOrNull);

    private List<ITypedItem> AsChildrenItems(IEnumerable<IEntity> entities, string field, IEntity parentEntity)
        => AsChildrenOf(entities, field, parentEntity,e => new TypedItemOfEntity(null, e, Cdf, PropsRequired) as ITypedItem);

    private static List<T> AsChildrenOf<T>(IEnumerable<IEntity> entities, string field, IEntity parentEntity, Func<IEntity, T> convert)
        where T : class, ICanBeEntity
    {
        var list = entities
            .Select((e, i) => EntityInBlockDecorator.Wrap(entity: e, field: field, index: i, parent: parentEntity) as IEntity)
            .Select(convert)
            .ToList();
        return new ListTypedItems<T>(list, null);
    }

    #endregion


}