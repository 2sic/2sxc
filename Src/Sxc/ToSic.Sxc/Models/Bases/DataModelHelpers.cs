using ToSic.Sxc.Data;
using ToSic.Sxc.Data.Internal;

namespace ToSic.Sxc.Models;

internal class DataModelHelpers
{
    /// <summary>
    /// Convert an Entity or TypedItem into a strongly typed object.
    /// Typically, the type will be from your `AppCode.Data`.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="item"></param>
    /// <returns></returns>
    internal static T As<T>(object item)
        where T : class, IDataModel, new()
        => item switch
        {
            null => null,
            ITypedItem typedItem => CodeDataFactory.AsCustomFrom<T, ITypedItem>(typedItem),
            IEntity entity => CodeDataFactory.AsCustomFrom<T, IEntity>(entity),
            _ => throw new($"Type {typeof(T).Name} not supported, only {typeof(IEntity)} and {nameof(ITypedItem)} are allowed as data"),
        };


    /// <summary>
    /// Convert a list of Entities or TypedItems into a strongly typed list.
    /// Typically, the type will be from your `AppCode.Data`.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="protector"></param>
    /// <param name="nullIfNull"></param>
    /// <returns></returns>
    internal static IEnumerable<T> AsList<T>(object source, NoParamOrder protector = default, bool nullIfNull = false)
        where T : class, IDataModel, new()
        => source switch
        {
            null => (nullIfNull ? null : []),
            IEnumerable<ITypedItem> typedItems => typedItems.Select(CodeDataFactory.AsCustomFrom<T, ITypedItem>).ToList(),
            IEnumerable<IEntity> entities => entities.Select(CodeDataFactory.AsCustomFrom<T, IEntity>).ToList(),
            _ => throw new($"Type {typeof(T).Name} not supported, only {typeof(IEntity)} and {nameof(ITypedItem)} are allowed as data"),
        };

}