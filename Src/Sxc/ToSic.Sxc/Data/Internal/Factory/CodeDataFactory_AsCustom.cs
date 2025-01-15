using System.Collections;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Data.Internal.Factory;
using ToSic.Sxc.Data.Internal.Typed;
using ToSic.Sxc.Models;
using ToSic.Sxc.Models.Attributes;

namespace ToSic.Sxc.Data.Internal;

partial class CodeDataFactory
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
            IEntity entity => AsCustomFrom<TCustom, ITypedItem>(AsItem(entity, propsRequired: true)),
            _ => AsCustomFrom<TCustom, ITypedItem>(source as ITypedItem ?? AsItem(source))
        };

    //internal static TCustom AsCustomFromItem<TCustom>(ITypedItem item)
    //    where TCustom : class, IDataModel, new()
    //{
    //    if (item == null) return null;
    //    if (item is TCustom t) return t;
    //    var newT = new TCustom();

    //    // Should be an ITypedItemWrapper, but not enforced in the signature
    //    if (newT is IDataModelOf<ITypedItem> ofTypedItem)
    //        ofTypedItem.Setup(item);
    //    else
    //        throw new($"The custom type {typeof(TCustom).Name} does not implement {nameof(IDataModelOf<ITypedItem>)}. This is probably a mistake.");
    //    return newT;
    //}

    internal static TCustom AsCustomFrom<TCustom, TData>(TData item)
        where TCustom : class, IDataModel, new()
    {
        if (item == null) return null;
        if (item is TCustom t) return t;
        var newT = new TCustom();

        // Should be an ITypedItemWrapper, but not enforced in the signature
        if (newT is IDataModelOf<TData> withMatchingSetup)
            withMatchingSetup.Setup(item);
        // DataModelOfEntity can also be filled from Typed (but ATM not the other way around)
        else if (newT is IDataModelOf<IEntity> forEntity && item is ICanBeEntity canBeEntity)
            forEntity.Setup(canBeEntity.Entity);
        else
            throw new($"The custom type {typeof(TCustom).Name} does not implement {nameof(IDataModelOf<TData>)}. This is probably a mistake.");
        return newT;
    }

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
        var typeName = GetContentTypeName<TCustom>();
        if (!item.Type.Is(typeName) &&  typeName != DataModelAttribute.ForAnyContentType)
            throw new($"Item with ID {id} is not a {typeName}. This is probably a mistake, otherwise use {nameof(skipTypeCheck)}: true");
        return AsCustom<TCustom>(item);
    }

    /// <summary>
    /// Figure out the expected ContentTypeName of a DataWrapper type.
    /// If it is decorated with <see cref="DataModelAttribute"/> then use the information it provides, otherwise
    /// use the type name.
    /// </summary>
    /// <typeparam name="TCustom"></typeparam>
    /// <returns></returns>
    internal static string GetContentTypeName<TCustom>() where TCustom : IDataModel =>
        ContentTypeNames.Get<TCustom, DataModelAttribute>(a =>
        {
            // If we have an attribute, use the value provided (unless not specified)
            if (a?.ForContentTypes != null)
                return a.ForContentTypes;

            // If no attribute, use name of type
            var type = typeof(TCustom);
            var typeName = type.Name;
            // If type is Interface: drop the "I" as this can't be a content-type
            // TODO: not sure if this is a good idea
            return typeName.StartsWith("I") && type.IsInterface
                ? typeName.Substring(1)
                : typeName;
        });
    private static readonly ClassAttributeLookup<string> ContentTypeNames = new();

    internal static string GetStreamName<TCustom>() where TCustom : IDataModel =>
        StreamNames.Get<TCustom, DataModelAttribute>(a =>
            // if we have the attribute, use that
            a?.StreamNames.Split(',').First().Trim()
            // If no attribute, use name of type
            ?? typeof(TCustom).Name);
    private static readonly ClassAttributeLookup<string> StreamNames = new();

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
            _ => new ListTypedItems<TCustom>(SafeItems().Select(AsCustomFrom<TCustom, ITypedItem>), null)
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