﻿using ToSic.Sxc.Data.Internal;

namespace ToSic.Sxc.Data.Models;

internal class DataModelHelpers
{
    /// <summary>
    /// Convert an Entity or TypedItem into a strongly typed object.
    /// Typically, the type will be from your `AppCode.Data`.
    /// </summary>
    /// <returns></returns>
    internal static TCustom As<TCustom>(IModelFactory modelFactory, object item)
        where TCustom : class, ICanWrapData
        => item switch
        {
            null => null,
            ITypedItem typedItem => modelFactory.AsCustomFrom<TCustom, ITypedItem>(typedItem),
            IEntity entity => modelFactory.AsCustomFrom<TCustom, IEntity>(entity),
            ICanBeItem canBeItem => modelFactory.AsCustomFrom<TCustom, ICanBeItem>(canBeItem.Item),
            ICanBeEntity canBeEntity => modelFactory.AsCustomFrom<TCustom, ICanBeEntity>(canBeEntity.Entity),
            _ => throw new(
                $"Type {typeof(TCustom).Name} not supported. " +
                $"Only {typeof(IEntity)}, {nameof(ITypedItem)}, {nameof(ICanBeEntity)} and {nameof(ICanBeItem)} are allowed as data"),
        };


    /// <summary>
    /// Convert a list of Entities or TypedItems into a strongly typed list.
    /// Typically, the type will be from your `AppCode.Data`.
    /// </summary>
    /// <returns></returns>
    internal static IEnumerable<TCustom> AsList<TCustom>(IModelFactory modelFactory, object source, NoParamOrder protector = default, bool nullIfNull = false)
        where TCustom : class, ICanWrapData
        => source switch
        {
            null => nullIfNull ? null : [],
            IEnumerable<ITypedItem> typedItems => typedItems
                .Select(modelFactory.AsCustomFrom<TCustom, ITypedItem>)
                .ToList(),
            IEnumerable<IEntity> entities => entities
                .Select(modelFactory.AsCustomFrom<TCustom, IEntity>)
                .ToList(),
            _ => throw new($"Type {typeof(TCustom).Name} not supported, only {typeof(IEntity)} and {nameof(ITypedItem)} are allowed as data"),
        };
}