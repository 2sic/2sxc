using ToSic.Eav.Data.Sys;
using ToSic.Eav.Data.Sys.ValueConverter;
using ToSic.Sxc.Data.Options;
using ToSic.Sxc.Data.Sys.Decorators;
using ToSic.Sxc.Data.Sys.Factory;
using ToSic.Sxc.Data.Sys.Typed;
using ToSic.Sys.Performance;
using ToSic.Sys.Users;
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

    public IHasPropLookup Parent { get; } = parent;

    public bool Debug => _debug ?? canDebug.Debug;
    private bool? _debug;

    [field: AllowNull, MaybeNull]
    internal SubDataFactory SubDataFactory => field ??= new(Cdf, PropsRequired, canDebug);

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
        var l = logOrNull.Fn<TryGetResult>($"Type: {Parent.GetType().Name}, {nameof(field)}:{field}, {nameof(language)}:{language}, {nameof(lookupLink)}:{lookupLink}");

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
        var resultSet = Parent.PropertyLookup.FindPropertyInternal(specs, path);

        // check Entity is null (in cases where null-objects are asked for properties)
        if (resultSet == null! /* paranoid */)
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



    private readonly Dictionary<string, TryGetResult> _rawValCache = new(InvariantCultureIgnoreCase);

    private object? ValueAutoConverted(PropReqResult original, bool lookupLink, string field, ILog? logOrNull)
    {
        var l = logOrNull.Fn<object?>($"..., {nameof(lookupLink)}: {lookupLink}, {nameof(field)}: {field}");
        var value = original.Result;
        var parent = original.Source as IEntity;

        // If it's a reference like "file:72", try to convert it
        if (lookupLink && value is string strMaybeLink && original.ValueType == ValueTypesWithState.Hyperlink)
        {
            var strMaybeReference = overrider != null
                ? overrider.ProcessString(field, strMaybeLink)
                : strMaybeLink;
            if (ValueConverterBase.CouldBeReference(strMaybeReference))
            {
                l.A("Try to convert value");
                // ReSharper disable once ConstantNullCoalescingCondition - paranoid
                value = Cdf.Services.ValueConverter.ToValue(strMaybeReference, parent?.EntityGuid ?? Guid.Empty) ?? value;
                return l.Return(value, "link-conversion");
            }
        }


        // note 2021-06-07 previously in created sub-entities with modified language-list; I think this is wrong
        // Note 2021-06-08 if the parent is _not_ an IEntity, this will throw an error. Could happen in the DynamicStack, but that should never have such children
        if (value is IEnumerable<IEntity> children)
        {
            if (childrenShouldBeDynamic)
            {
                l.A("Convert entity list as DynamicEntity");
                var dynEnt = Cdf.AsDynamicFromEntities(children.ToArray(), new() { ItemIsStrict = PropsRequired }, parent: parent, field: field);
                if (Debug)
                    dynEnt.Debug = true;
                return l.Return(dynEnt, "ent-list, now dyn");
            }
            l.A($"Convert entity list as {nameof(ITypedItem)}");
            var converted = AsChildrenItems(entities: children, field: field, parentEntity: parent, new());

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

        if (value is string strResult && overrider != null)
        {
            var maybeOverride = overrider.ProcessString(field, strResult);
            return l.Return(maybeOverride, "parsed");
        }

        return l.Return(value, "unmodified");
    }

    #endregion

    #region Parents / Children - ATM still dynamic

    public List<IDynamicEntity?> ParentsDyn(IEntity entity, string? type, string? field)
        => entity.Parents(type, field)
            .Select(SubDataFactory.SubDynEntityOrNull)
            .ToList();

    public List<ITypedItem> ParentsItems(IEntity entity, string? type, string? field, GetRelatedOptions options)
    {
        var list = entity.Parents(type, field);
        var processed = ProcessOptions(list, options, Cdf.Services.User);
        
        var preserveNull = options.ProcessNull == ProcessNull.Preserve;

        return processed
            .Select(ITypedItem (e) => e == null && preserveNull
                ? null!
                : new TypedItemOfEntity(e!, Cdf, PropsRequired)
            )
            .ToList();
    }


    public List<IDynamicEntity?> ChildrenDyn(IEntity entity, string? field, string? type)
        => AsChildrenDyn(entity.Children(field, type), field, parentEntity: entity);

    public List<ITypedItem> ChildrenItems(IEntity entity, string field, string? type, GetRelatedOptions options)
        => AsChildrenItems(entity.Children(field, type), field, parentEntity: entity, options);

    private List<IDynamicEntity?> AsChildrenDyn(IEnumerable<IEntity?> entities, string? field, IEntity? parentEntity)
        => AsChildrenOf(
            ProcessOptions(entities, new(), Cdf.Services.User),
            field,
            parentEntity,
            SubDataFactory.SubDynEntityOrNull,
            new()
        );

    private List<ITypedItem> AsChildrenItems(IEnumerable<IEntity?> entities, string field, IEntity? parentEntity, GetRelatedOptions options)
        => AsChildrenOf(
            ProcessOptions(entities, options, Cdf.Services.User),
            field,
            parentEntity,
            ITypedItem (e) => new TypedItemOfEntity(e, Cdf, PropsRequired),
            options
        );

    private static List<T> AsChildrenOf<T>(
        IEnumerable<IEntity?> entities,
        string? fieldNameForWrapperInfo,
        IEntity? parentEntity,
        Func<IEntity, T> convert,
        GetRelatedOptions options)
        where T : class?, ICanBeEntity?
    {
        var preserveNull = options.ProcessNull == ProcessNull.Preserve;

        var list = entities
            .Select((e, i) =>
            {
                if (e == null && preserveNull)
                    return null!;
                var wrapped = EntityInBlockDecorator.Wrap(entity: e!, fieldName: fieldNameForWrapperInfo, index: i, parent: parentEntity);
                var converted = convert(wrapped);
                return converted;
            })
            .ToList();
        return new ListTypedItems<T>(list, null);
    }

    private static ICollection<IEntity?> ProcessOptions(IEnumerable<IEntity?> entities, GetRelatedOptions options, IUser user)
    {
        // 1. Check if we should remove drafts - default for non-admins
        if (options.ProcessDraft == ProcessDraft.NoDraft ||
            (options.ProcessDraft == ProcessDraft.Auto && !user.IsContentEditor))
            entities = entities
                .Where(e => e?.IsPublished == true);

        // 2. filter out nulls, as the razor code will usually not be able to handle them
        // in future, we should maybe add a trigger to optionally allow nulls,
        // in which case the result should then also be null
        var preserveNull = options.ProcessNull == ProcessNull.Preserve;
        if (!preserveNull)
            entities = entities
                .Where(e => e != null);

        return entities.ToListOpt();
    }

    #endregion
}