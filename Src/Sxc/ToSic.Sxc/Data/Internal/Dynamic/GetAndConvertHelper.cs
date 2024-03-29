﻿using ToSic.Eav.Data.PropertyLookup;
using ToSic.Eav.Plumbing;
using ToSic.Lib.Helpers;
using ToSic.Sxc.Data.Internal.Decorators;
using ToSic.Sxc.Data.Internal.Typed;
using static System.StringComparer;

namespace ToSic.Sxc.Data.Internal.Dynamic;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
internal class GetAndConvertHelper(
    IHasPropLookup parent,
    Internal.CodeDataFactory cdf,
    bool propsRequired,
    bool childrenShouldBeDynamic,
    ICanDebug canDebug)
{
    #region Setup and Log


    public Internal.CodeDataFactory Cdf { get; } = cdf;

    public bool PropsRequired { get; } = propsRequired;

    public bool Debug => _debug ?? canDebug.Debug;
    private bool? _debug;

    internal SubDataFactory SubDataFactory => _subData ??= new(Cdf, PropsRequired, canDebug);
    private SubDataFactory _subData;


    public IHasPropLookup Parent { get; } = parent;

    public ILog LogOrNull => _logOrNull.Get(() => Cdf?.Log?.SubLogOrNull("DynEnt", Debug));
    private readonly GetOnce<ILog> _logOrNull = new();

    #endregion

    #region Get Implementations 1:1 - names must be identical with caller, so the exceptions have the right names

    public dynamic Get(string name) => GetInternal(name, lookupLink: true).Result;

    public object Get(string name, NoParamOrder noParamOrder = default, string language = null, bool convertLinks = true, bool? debug = null)
    {
        _debug = debug;
        var result = GetInternal(name, language, convertLinks).Result;
        _debug = null;

        return result;
    }

    public TValue Get<TValue>(string name) => TryGet(name).Result.ConvertOrDefault<TValue>();

    public TValue Get<TValue>(string name, NoParamOrder noParamOrder = default, TValue fallback = default)
        => TryGet(name).Result.ConvertOrFallback(fallback);

    #endregion

    #region Get Values

    public TryGetResult TryGet(string field, string language = null) => GetInternal(field, language, lookupLink: false);

    public TryGetResult GetInternal(string field, string language = null, bool lookupLink = true)
    {
        var logOrNull = LogOrNull.SubLogOrNull("Dyn.EntBas", Debug);
        var l = logOrNull.Fn<TryGetResult>($"Type: {Parent.GetType().Name}, {nameof(field)}:{field}, {nameof(language)}:{language}, {nameof(lookupLink)}:{lookupLink}");

        if (!field.HasValue())
            return l.Return(new(false, null), "field null/empty");

        // This determines if we should access & store in cache
        // check if we already have it in the cache - but only in default case (no language, lookup=true)
        var cacheKey = (field + "$" + lookupLink + "-" + language).ToLowerInvariant();
        if (_rawValCache.TryGetValue(cacheKey, out var cached))
            return l.Return(cached, "cached");

        // use the standard dimensions or overload
        var languages = language == null ? Cdf.Dimensions : [language];
        l.A($"cache-key: {cacheKey}, {nameof(languages)}:{languages}");

        // Get the field or the path if it has one
        // Classic field case
        var specs = new PropReqSpecs(field, languages, logOrNull);
        var path = new PropertyLookupPath().Add("DynEntStart", field);
        var resultSet = Parent.PropertyLookup.FindPropertyInternal(specs, path);

        // check Entity is null (in cases where null-objects are asked for properties)
        if (resultSet == null)
            return l.Return(new(false, null), "result null");

        l.A($"Result... IsFinal: {resultSet.IsFinal}, Source Name: {resultSet.Name}, SourceIndex: {resultSet.SourceIndex}, FieldType: {resultSet.FieldType}");

        var result = ValueAutoConverted(resultSet, lookupLink, field, logOrNull);

        // cache result, but only if using default languages
        l.A("add to cache");
        var found = resultSet.FieldType != Attributes.FieldIsNotFound;
        var final = new TryGetResult(found, result);
        if (found)
            _rawValCache.Add(cacheKey, final);

        return l.Return(final, "ok");
    }
    private readonly Dictionary<string, TryGetResult> _rawValCache = new(InvariantCultureIgnoreCase);

    private object ValueAutoConverted(PropReqResult original, bool lookupLink, string field, ILog logOrNull)
    {
        var l = logOrNull.Fn<object>($"..., {nameof(lookupLink)}: {lookupLink}, {nameof(field)}: {field}");
        var value = original.Result;
        var parent = original.Source as IEntity;
        // New mechanism to not use resolve-hyperlink
        if (lookupLink && value is string strResult
                       && original.FieldType == DataTypes.Hyperlink
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
            var converted = Entity2Children(entities: children, field: field, parentEntity: parent)
                .Cast<DynamicEntity>()
                .Select(de => de.TypedItem)
                .ToList();
            // if (Debug) converted.ForEach(c => c.Debug = true);
            return l.Return(converted, "ent-list, now dyn");
        }

        // special debug of path if possible
        if (canDebug.Debug)
            try
            {
                    
                var finalPath = string.Join(" > ", original.Path?.Parts?.ToArray() ?? Array.Empty<string>());
                l.A($"Debug path: {finalPath}");
            }
            catch {/* ignore */}

        return l.Return(value, "unmodified");
    }

    #endregion

    #region Parents / Children - ATM still dynamic

    public List<IDynamicEntity> Parents(IEntity entity, string type = null, string field = null)
        => entity.Parents(type, field).Select(e => SubDataFactory.SubDynEntityOrNull(e)).ToList();


    public List<IDynamicEntity> Children(IEntity entity, string field = null, string type = null)
        => Entity2Children( entity.Children(field, type), field, parentEntity: entity);

    private List<IDynamicEntity> Entity2Children(IEnumerable<IEntity> entities, string field = null, IEntity parentEntity = default)
        => entities
            .Select((e, i) => EntityInBlockDecorator.Wrap(entity: e, field: field, index: i, parent: parentEntity))
            .Select(e => SubDataFactory.SubDynEntityOrNull(e))
            .ToList();

    #endregion


}