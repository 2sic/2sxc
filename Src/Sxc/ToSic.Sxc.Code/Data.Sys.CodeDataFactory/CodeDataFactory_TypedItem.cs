using System.Collections;
using ToSic.Eav.DataSource;
using ToSic.Sxc.Data.Sys.Factory;
using ToSic.Sxc.Data.Sys.Typed;
using ToSic.Sxc.Data.Sys.Wrappers;

namespace ToSic.Sxc.Data.Sys.CodeDataFactory;

partial class CodeDataFactory
{
    public const int MaxRecursions = 3;

    #region AsTyped Implementations

    [return: NotNullIfNotNull(nameof(data)), NotNullIfNotNull(nameof(fallback))]
    public ITypedItem? AsItem(object? data, Factory.ConvertItemSettings settings, NoParamOrder noParamOrder = default, ITypedItem? fallback = default)
    {
        // If we need mock data, return a fake object
        if (settings.UseMock)
            return codeDataWrapper.Value.TypedItemFromObject(data,
                WrapperSettings.Typed(true, true, settings.ItemIsStrict),
                new LazyLike<ICodeDataFactory>(this));

        return AsItemInternal(data, settings, MaxRecursions)
               ?? fallback;
    }

    /// <summary>
    /// Quick convert an entity to item - if not null, otherwise return null.
    /// </summary>
    /// <returns></returns>
    [return: NotNullIfNotNull(nameof(entity))]
    public ITypedItem? AsItem(IEntity? entity, Factory.ConvertItemSettings settings)
        => entity == null
            ? null
            : new TypedItemOfEntity(null, entity, this, propsRequired: settings.ItemIsStrict);

    [field: AllowNull, MaybeNull]
    private LogFilter AsItemLogFilter
        => field ??= new(Log, logFirstMax: 25, reLogIteration: 100);

    [return: NotNullIfNotNull(nameof(data))]
    internal ITypedItem? AsItemInternal(object? data, Factory.ConvertItemSettings settings, int recursions)
    {
        // Only log the first 25 calls to this method, then stop logging
        var l = AsItemLogFilter.FnOrNull<ITypedItem?>();

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
                return ToItemOrNullAndLog(ds.List.FirstOrDefault(), nameof(IDataSource))!;
            case IDataStream ds:
                return ToItemOrNullAndLog(ds.List.FirstOrDefault(), nameof(IDataStream))!;
            case IEnumerable<IEntity> entList:
                return ToItemOrNullAndLog(entList.FirstOrDefault(), "IEnumerable<IEntity>")!;
            case IEnumerable enumerable:
                var enumFirst = enumerable.Cast<object>().FirstOrDefault();
                if (enumFirst is null)
                    return l.ReturnNull($"{nameof(IEnumerable)} with null object")!;
                // retry conversion
                return l.Return(AsItemInternal(enumFirst, settings, recursions - 1));
            default:
                throw l.Done(new ArgumentException($"Type '{data.GetType()}' cannot be converted to {nameof(ITypedItem)}. " +
                                                   $"If you are trying to create mock/fake/fallback data, try using \", mock: true\""));
        }

        [return: NotNullIfNotNull(nameof(e))]
        ITypedItem? ToItemOrNullAndLog(IEntity? e, string typeName)
            => e == null
                ? l.ReturnNull($"empty {typeName}")
                : l.Return(new TypedItemOfEntity(null, e, this, propsRequired: settings.ItemIsStrict), typeName);
    }

    public IEnumerable<ITypedItem> EntitiesToItems(IEnumerable<IEntity>? entities, Factory.ConvertItemSettings settings)
    {
        if (entities == null)
            return [];
        var result = entities
            .Select(e => AsItem(e, settings))
            .ToList();

        if (settings.DropNullItems)
            result = result
                .Where(e => e != null! /* rare cases do have null */)
                .ToList();

        // Ignore that some items could contain null, since the default behavior is no-nulls
        return result;
    }

    public IEnumerable<ITypedItem> AsItems(object list, Factory.ConvertItemSettings settings, NoParamOrder noParamOrder = default, IEnumerable<ITypedItem>? fallback = null) 
        => AsItemList(list, fallback, MaxRecursions, settings);

    private IEnumerable<ITypedItem> AsItemList(object list, IEnumerable<ITypedItem>? fallback, int recursions, Factory.ConvertItemSettings settings)
    {
        var l = Log.Fn<IEnumerable<ITypedItem>>($"{nameof(list)}: '{list}'; Settings: {settings}");

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
                return l.Return(alreadyOk.Select(e => AsItem(e.Entity, settings)), nameof(IEnumerable<ITypedItem>));
            //return l.Return(alreadyOk, "already matches type");
            //case IEnumerable<IDynamicEntity> dynIDynEnt:
            //    return l.Return(dynIDynEnt.Select(e => AsTyped(e, services, MaxRecursions, log)), "IEnum<DynEnt>");
            case IDataSource source:
                return l.Return(EntitiesToItems(source.List, settings), "DataSource - convert list");
            case IDataStream stream:
                return l.Return(EntitiesToItems(stream.List, settings), "DataStream - convert list");
            case IEnumerable<IEntity> entList:
                return l.Return(EntitiesToItems(entList, settings), "IEnumerable<IEntity>");
            case IEnumerable<dynamic> dynList:
                var fromDynList = dynList.Select(e => AsItemInternal(e as object, settings, MaxRecursions));
                if (settings.DropNullItems)
                    fromDynList = fromDynList.Where(e => e != null!).ToListOpt();
                return l.Return(fromDynList, "IEnumerable<dynamic>");
            // Variations of single items - should be converted to list
            // All of the commented out variants are ICanBeEntity
            //case IEntity ent:
            //    return l.Return(AsItemFromEntity(ent, propsRequired).ToListOfOneOrNone(), nameof(IEntity));
            //case IDynamicEntity dynEnt:
            //    return l.Return(AsItemFromEntity(dynEnt.Entity, propsRequired).ToListOfOneOrNone(), nameof(IDynamicEntity));
            //case ITypedItem oneTyped:
            //    return l.Return(AsItemFromEntity(oneTyped.Entity, propsRequired).ToListOfOneOrNone(), "already typed");
            case ICanBeEntity canBeEntity:
                var converted = AsItem(canBeEntity.Entity, settings);
                return l.Return(converted.ToListOfOneOrNone(), nameof(ICanBeEntity) + (converted == null ? " - null" : ""));
            // Check for IEnumerable but make sure it's not a string (so that should come before)
            // Should come fairly late, because some things like DynamicEntities can also be enumerated
            case IEnumerable asEnumerable:
                return l.Return(asEnumerable.Cast<object>().Select(e => AsItemInternal(e, settings, MaxRecursions)), "IEnumerable");
            default:
                return FallbackOrErrorAndLog($"can't convert '{list.GetType()}'", $"Type '{list.GetType()}' cannot be converted.");


        }

        // Inner call to complete scenarios where the data couldn't be created
        IEnumerable<ITypedItem> FallbackOrErrorAndLog(string fallbackMsg, string exMsg)
            => fallback != null
                ? l.Return(fallback, fallbackMsg + ", fallback")
                : settings.FirstIsRequired
                    ? throw l.Done(new ArgumentException($@"Conversion with {nameof(AsItems)} failed, {nameof(settings.FirstIsRequired)}=true. {exMsg}", nameof(list)))
                    : l.Return([], "no fallback, not required, empty list");
    }

    #endregion
}