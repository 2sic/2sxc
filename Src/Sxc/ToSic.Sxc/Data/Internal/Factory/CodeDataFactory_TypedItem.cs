using System.Collections;
using ToSic.Eav.DataSource;
using ToSic.Eav.Plumbing;
using ToSic.Lib.DI;
using ToSic.Sxc.Data.Internal.Typed;
using ToSic.Sxc.Data.Internal.Wrapper;

namespace ToSic.Sxc.Data.Internal;

partial class CodeDataFactory
{
    public const int MaxRecursions = 3;

    #region AsTyped Implementations

    public ITypedItem AsItem(object data, NoParamOrder noParamOrder = default, bool? required = default, ITypedItem fallback = default, bool? propsRequired = default, bool? mock = default)
    {
        // If we need mock data, return a fake object
        if (mock == true)
            return codeDataWrapper.Value.TypedItemFromObject(data,
                WrapperSettings.Typed(true, true, propsRequired ?? true),
                new LazyLike<CodeDataFactory>(this));

        return AsItemInternal(data, MaxRecursions, propsRequired: propsRequired ?? false) ?? fallback;
    }

    /// <summary>
    /// Quick convert an entity to item - if not null, otherwise return null.
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="propsRequired"></param>
    /// <returns></returns>
    public ITypedItem AsItem(IEntity entity, bool propsRequired)
        => entity == null ? null : new TypedItemOfEntity(null, entity, this, propsRequired: propsRequired);

    private LogFilter AsItemLogFilter => _asItemLogFilter ??= new(Log, logFirstMax: 25, reLogIteration: 100);
    private LogFilter _asItemLogFilter;

    internal ITypedItem AsItemInternal(object data, int recursions, bool propsRequired)
    {
        // Only log the first 25 calls to this method, then stop logging
        var l = AsItemLogFilter.FnOrNull<ITypedItem>();

        if (recursions <= 0)
            throw l.Done(new Exception($"Conversion with {nameof(AsItem)} failed, max recursions reached"));

        switch (data)
        {
            case null:
                return l.ReturnNull("null");
            case string:
                throw l.Done(new ArgumentException($"Type '{data.GetType()}' cannot be converted to {nameof(ITypedItem)}"));
            case ITypedItem alreadyItem:
                return ToItemOrNullAndLog(alreadyItem.Entity, nameof(ITypedItem));
            case IEntity entity:
                return ToItemOrNullAndLog(entity, nameof(IEntity));
            // Dynamic Entity is also an ICanBeEntity
            //case IDynamicEntity dynEnt:
            //    return l.Return(new TypedItem(dynEnt), nameof(IDynamicEntity));
            case ICanBeEntity canBeEntity:
                return ToItemOrNullAndLog(canBeEntity.Entity, nameof(ICanBeEntity));
            case IDataSource ds:
                return ToItemOrNullAndLog(ds.List.FirstOrDefault(), nameof(IDataSource));
            case IDataStream ds:
                return ToItemOrNullAndLog(ds.List.FirstOrDefault(), nameof(IDataStream));
            case IEnumerable<IEntity> entList:
                return ToItemOrNullAndLog(entList.FirstOrDefault(), nameof(IEnumerable<IEntity>));
            case IEnumerable enumerable:
                var enumFirst = enumerable.Cast<object>().FirstOrDefault();
                if (enumFirst is null) return l.ReturnNull($"{nameof(IEnumerable)} with null object");
                // retry conversion
                return l.Return(AsItemInternal(enumFirst, recursions - 1, propsRequired: propsRequired));
            default:
                throw l.Done(new ArgumentException($"Type '{data.GetType()}' cannot be converted to {nameof(ITypedItem)}. " +
                                                   $"If you are trying to create mock/fake/fallback data, try using \", mock: true\""));
        }

        ITypedItem ToItemOrNullAndLog(IEntity e, string typeName) => e == null
            ? l.ReturnNull($"empty {typeName}")
            : l.Return(new TypedItemOfEntity(null, e, this, propsRequired: propsRequired), typeName);
    }

    public IEnumerable<ITypedItem> EntitiesToItems(IEnumerable<IEntity> entities, bool propsRequired = false)
        => entities?.Select(e => AsItem(e, propsRequired: propsRequired)).ToList() ?? [];

    public IEnumerable<ITypedItem> AsItems(object list, NoParamOrder noParamOrder = default, bool? required = default, IEnumerable<ITypedItem> fallback = default, bool? propsRequired = default) 
        => AsItemList(list, required ?? true, fallback, MaxRecursions, propsRequired: propsRequired ?? false);

    private IEnumerable<ITypedItem> AsItemList(object list, bool required, IEnumerable<ITypedItem> fallback, int recursions, bool propsRequired)
    {
        var l = Log.Fn<IEnumerable<ITypedItem>>($"{nameof(list)}: '{list}'; {nameof(required)}: {required}; {nameof(recursions)}: {recursions}");

        if (recursions <= 0)
            return FallbackOrErrorAndLog("max recursions", $"Max recursions {MaxRecursions} reached.");

        switch (list)
        {
            case null:
                return FallbackOrErrorAndLog("null", "Got null.");
            // String check must come early because strings are enumerable
            case string:
                return FallbackOrErrorAndLog("string", "Got a string.");
            // List of ITypedItem
            case IEnumerable<ITypedItem> alreadyOk:
                return l.Return(alreadyOk.Select(e => AsItem(e.Entity, propsRequired: propsRequired)), nameof(IEnumerable<ITypedItem>));
            //return l.Return(alreadyOk, "already matches type");
            //case IEnumerable<IDynamicEntity> dynIDynEnt:
            //    return l.Return(dynIDynEnt.Select(e => AsTyped(e, services, MaxRecursions, log)), "IEnum<DynEnt>");
            case IDataSource dsEntities:
                return l.Return(EntitiesToItems(dsEntities.List, propsRequired), "DataSource - convert list");
            case IDataStream dataStream:
                return l.Return(EntitiesToItems(dataStream.List, propsRequired), "DataStream - convert list");
            case IEnumerable<IEntity> iEntities:
                return l.Return(iEntities.Select(e => AsItem(e, propsRequired: propsRequired)), nameof(IEnumerable<IEntity>));
            case IEnumerable<dynamic> dynEntities:
                return l.Return(dynEntities.Select(e => AsItemInternal(e as object, MaxRecursions, propsRequired: propsRequired)), nameof(IEnumerable<dynamic>));
            // Variations of single items - should be converted to list
            // All of the commented out variants are ICanBeEntity
            //case IEntity ent:
            //    return l.Return(AsItemFromEntity(ent, propsRequired).ToListOfOneOrNone(), nameof(IEntity));
            //case IDynamicEntity dynEnt:
            //    return l.Return(AsItemFromEntity(dynEnt.Entity, propsRequired).ToListOfOneOrNone(), nameof(IDynamicEntity));
            //case ITypedItem oneTyped:
            //    return l.Return(AsItemFromEntity(oneTyped.Entity, propsRequired).ToListOfOneOrNone(), "already typed");
            case ICanBeEntity canBeEntity:
                var converted = AsItem(canBeEntity.Entity, propsRequired: propsRequired);
                return l.Return(converted.ToListOfOneOrNone(), nameof(ICanBeEntity) + (converted == null ? " - null" : ""));
            // Check for IEnumerable but make sure it's not a string (so that should come before)
            // Should come fairly late, because some things like DynamicEntities can also be enumerated
            case IEnumerable asEnumerable:
                return l.Return(asEnumerable.Cast<object>().Select(e => AsItemInternal(e, MaxRecursions, propsRequired: propsRequired)), "IEnumerable");
            default:
                return FallbackOrErrorAndLog($"can't convert '{list.GetType()}'", $"Type '{list.GetType()}' cannot be converted.");


        }

        // Inner call to complete scenarios where the data couldn't be created
        IEnumerable<ITypedItem> FallbackOrErrorAndLog(string fallbackMsg, string exMsg)
            => fallback != null
                ? l.Return(fallback, fallbackMsg + ", fallback")
                : required
                    ? throw l.Done(new ArgumentException($@"Conversion with {nameof(AsItems)} failed, {nameof(required)}=true. {exMsg}", nameof(list)))
                    : l.Return([], "no fallback, not required, empty list");
    }

    #endregion
}