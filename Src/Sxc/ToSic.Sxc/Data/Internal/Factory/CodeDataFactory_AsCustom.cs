using System.Collections;
using Microsoft.Extensions.DependencyInjection;
using ToSic.Sxc.Data.Internal.Typed;
using ToSic.Sxc.Data.Model;
using ToSic.Sxc.Models;

namespace ToSic.Sxc.Data.Internal;

partial class CodeDataFactory: IModelFactory
{
    /// <summary>
    /// Convert an object to a custom type, if possible.
    /// If the object is an entity-like thing, that will be converted.
    /// If it's a list of entity-like things, the first one will be converted.
    /// </summary>
    public TCustom AsCustom<TCustom>(object source, NoParamOrder protector = default, bool mock = false)
        where TCustom : class, ICanWrapData
        => source switch
        {
            null when !mock => null,
            TCustom alreadyT => alreadyT,
            IEntity entity => AsCustomFrom<TCustom, ITypedItem>(AsItem(entity, propsRequired: true)),
            _ => AsCustomFrom<TCustom, ITypedItem>(source as ITypedItem ?? AsItem(source))
        };

    public TCustom AsCustomFrom<TCustom, TData>(TData item)
        where TCustom : class, ICanWrapData
    {
        if (item == null) return null;
        if (item is TCustom t) return t;

        var bestType = DataModelAnalyzer.GetTargetType<TCustom>();
        var newT = ActivatorUtilities.CreateInstance(serviceProvider, bestType) as TCustom;

        // Should be an ITypedItemWrapper, but not enforced in the signature
        if (newT is ICanWrap<TData> withMatchingSetup)
            withMatchingSetup.Setup(item, this);
        // DataModelOfEntity can also be filled from Typed (but ATM not the other way around)
        else if (newT is ICanWrap<IEntity> forEntity && item is ICanBeEntity canBeEntity)
            forEntity.Setup(canBeEntity.Entity, this);
        else
            throw new($"The custom type {typeof(TCustom).Name} does not implement {nameof(ICanWrap<TData>)}. This is probably a mistake.");
        return newT;
    }

    internal TCustom GetOne<TCustom>(Func<IEntity> getItem, object id, bool skipTypeCheck)
        where TCustom : class, ICanWrapData, new()
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


    /// <summary>
    /// Create list of custom-typed ITypedItems
    /// </summary>
    public IEnumerable<TCustom> AsCustomList<TCustom>(object source, NoParamOrder protector, bool nullIfNull)
        where TCustom : class, ICanWrapData
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