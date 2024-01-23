using ToSic.Sxc.Services;

namespace ToSic.Sxc.Data.Internal;

partial class CodeDataFactory
{
    /// <summary>
    /// EXPERIMENTAL
    /// </summary>
    /// <returns></returns>
    public T AsCustom<T>(ICanBeEntity source, ServiceKit16 kit) where T : class, ITypedItemWrapper16, ITypedItem, new()
    {
        var item = source as ITypedItem ?? AsItem(source);
        var wrapper = new T();
        wrapper.Setup(item, kit);
        return wrapper;
    }

    /// <summary>
    /// EXPERIMENTAL
    /// </summary>
    /// <param name="source"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    [PrivateApi("WIP, don't publish yet")]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public IEnumerable<T> AsCustomList<T>(IEnumerable<ICanBeEntity> source, ServiceKit16 kit) where T : class, ITypedItemWrapper16, ITypedItem, new()
    {
        var items = SafeItems().Select(i =>
        {
            var wrapper = new T();
            wrapper.Setup(i, kit);
            return wrapper;
        });
        return items;

        IEnumerable<ITypedItem> SafeItems()
        {
            if (source == null || !source.Any()) return [];
            if (source is IEnumerable<ITypedItem> alreadyOk) return alreadyOk;
            return _CodeApiSvc._Cdf.AsItems(source);
        }
    }

}