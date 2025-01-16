using System.Collections;
using Microsoft.Extensions.DependencyInjection;
using ToSic.Sxc.Data.Internal.Typed;
using ToSic.Sxc.Models;
using ToSic.Sxc.Models.Attributes;

namespace ToSic.Sxc.Data.Internal;

partial class CodeDataFactory: ICustomModelFactory
{
    /// <summary>
    /// Convert an object to a custom type, if possible.
    /// If the object is an entity-like thing, that will be converted.
    /// If it's a list of entity-like things, the first one will be converted.
    /// </summary>
    public TCustom AsCustom<TCustom>(object source, NoParamOrder protector = default, bool mock = false)
        where TCustom : class, IDataModel, new()
        => source switch
        {
            null when !mock => null,
            TCustom alreadyT => alreadyT,
            IEntity entity => AsCustomFrom2<TCustom, ITypedItem>(AsItem(entity, propsRequired: true)),
            _ => AsCustomFrom2<TCustom, ITypedItem>(source as ITypedItem ?? AsItem(source))
        };

    public TCustom AsCustomFrom2<TCustom, TData>(TData item)
        where TCustom : class, IDataModel
    {
        if (item == null) return null;
        if (item is TCustom t) return t;
        var newT = ActivatorUtilities.CreateInstance<TCustom>(serviceProvider);

        // Should be an ITypedItemWrapper, but not enforced in the signature
        if (newT is IDataModelOf<TData> withMatchingSetup)
            withMatchingSetup.Setup(item, this);
        // DataModelOfEntity can also be filled from Typed (but ATM not the other way around)
        else if (newT is IDataModelOf<IEntity> forEntity && item is ICanBeEntity canBeEntity)
            forEntity.Setup(canBeEntity.Entity, this);
        else
            throw new($"The custom type {typeof(TCustom).Name} does not implement {nameof(IDataModelOf<TData>)}. This is probably a mistake.");
        return newT;
    }

    //internal static TCustom AsCustomFrom<TCustom, TData>(TData item)
    //    where TCustom : class, IDataModel, new()
    //{
    //    if (item == null) return null;
    //    if (item is TCustom t) return t;
    //    var newT = Activator.CreateInstance<TCustom>();

    //    // Should be an ITypedItemWrapper, but not enforced in the signature
    //    if (newT is IDataModelOf<TData> withMatchingSetup)
    //        withMatchingSetup.Setup(item, null);
    //    // DataModelOfEntity can also be filled from Typed (but ATM not the other way around)
    //    else if (newT is IDataModelOf<IEntity> forEntity && item is ICanBeEntity canBeEntity)
    //        forEntity.Setup(canBeEntity.Entity, null);
    //    else
    //        throw new($"The custom type {typeof(TCustom).Name} does not implement {nameof(IDataModelOf<TData>)}. This is probably a mistake.");
    //    return newT;
    //}

    internal TCustom GetOne<TCustom>(Func<IEntity> getItem, object id, bool skipTypeCheck)
        where TCustom : class, IDataModel, new()
    {
        var item = getItem();
        if (item == null)
            return null;

        // Skip Type-Name check
        if (skipTypeCheck)
            return AsCustom<TCustom>(item);

        // Do Type-Name check
        var typeName = DataModelAnalyzer.GetContentTypeNames<TCustom>().Split(',');
        if (typeName.FirstOrDefault() != DataModelAttribute.ForAnyContentType && !typeName.Any(tn => item.Type.Is(tn)))
            throw new($"Item with ID {id} is not a {typeName}. This is probably a mistake, otherwise use {nameof(skipTypeCheck)}: true");
        return AsCustom<TCustom>(item);
    }


    ///// <summary>
    ///// WIP / experimental, would be for types which are not as T, but as a type-object.
    ///// Not in use, so not fully tested.
    /////
    ///// Inspired by https://stackoverflow.com/questions/3702916/is-there-a-typeof-inverse-operation
    ///// </summary>
    ///// <param name="t"></param>
    ///// <param name="source"></param>
    ///// <param name="protector"></param>
    ///// <param name="mock"></param>
    ///// <returns></returns>
    //public object AsCustom(Type t, ICanBeEntity source, NoParamOrder protector = default, bool mock = false)
    //{
    //    if (!mock && source == null) return null;
    //    if (source.GetType() == t) return source;

    //    var item = source as ITypedItem ?? AsItem(source);
    //    var wrapperObj = Activator.CreateInstance(t);
    //    if (wrapperObj is not ITypedItemWrapper16 wrapper) return null;
    //    wrapper.Setup(item);
    //    return wrapper;
    //}

    /// <summary>
    /// Create list of custom-typed ITypedItems
    /// </summary>
    public IEnumerable<TCustom> AsCustomList<TCustom>(object source, NoParamOrder protector, bool nullIfNull)
        where TCustom : class, IDataModel, new()
    {
        return source switch
        {
            null when nullIfNull => null,
            IEnumerable<TCustom> alreadyListT => alreadyListT,
            // special case: empty list, with hidden info about where it's from so the toolbar can adjust and provide new-buttons
            ListTypedItems<ITypedItem> { Count: 0, Entity: not null } list => new ListTypedItems<TCustom>(new List<TCustom>(), list.Entity),
            _ => new ListTypedItems<TCustom>(SafeItems().Select(AsCustomFrom2<TCustom, ITypedItem>), null)
        };

        // Helper function to be called from above to ensure that
        // the source is a list of ITypedItems
        IEnumerable<ITypedItem> SafeItems()
            => source switch
            {
                null => [],
                IEnumerable enumerable when !enumerable.Cast<object>().Any() => [],
                IEnumerable<ITypedItem> alreadyOk => alreadyOk,
                _ => _CodeApiSvc.Cdf.AsItems(source)
            };
    }

}