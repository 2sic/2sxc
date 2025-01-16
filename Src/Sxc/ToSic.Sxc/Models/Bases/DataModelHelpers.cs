using ToSic.Sxc.Data;
using ToSic.Sxc.Data.Internal;

namespace ToSic.Sxc.Models;

internal class DataModelHelpers
{
    /// <summary>
    /// Convert an Entity or TypedItem into a strongly typed object.
    /// Typically, the type will be from your `AppCode.Data`.
    /// </summary>
    /// <returns></returns>
    internal static T As<T>(ICustomModelFactory modelFactory, object item)
        where T : class, IDataModel
        => item switch
        {
            null => null,
            ITypedItem typedItem => modelFactory.AsCustomFrom2<T, ITypedItem>(typedItem),
            IEntity entity => modelFactory.AsCustomFrom2<T, IEntity>(entity),
            _ => throw new($"Type {typeof(T).Name} not supported, only {typeof(IEntity)} and {nameof(ITypedItem)} are allowed as data"),
        };


    /// <summary>
    /// Convert a list of Entities or TypedItems into a strongly typed list.
    /// Typically, the type will be from your `AppCode.Data`.
    /// </summary>
    /// <returns></returns>
    internal static IEnumerable<T> AsList<T>(ICustomModelFactory modelFactory, object source, NoParamOrder protector = default, bool nullIfNull = false)
        where T : class, IDataModel
        => source switch
        {
            null => (nullIfNull ? null : []),
            IEnumerable<ITypedItem> typedItems => typedItems.Select(modelFactory.AsCustomFrom2<T, ITypedItem>).ToList(),
            IEnumerable<IEntity> entities => entities.Select(modelFactory.AsCustomFrom2<T, IEntity>).ToList(),
            _ => throw new($"Type {typeof(T).Name} not supported, only {typeof(IEntity)} and {nameof(ITypedItem)} are allowed as data"),
        };

}