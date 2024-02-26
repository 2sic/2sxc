using ToSic.Sxc.Services;

namespace ToSic.Sxc.Data.Internal;

partial class CodeDataFactory
{
    /// <summary>
    /// EXPERIMENTAL
    /// </summary>
    public T AsCustom<T>(ICanBeEntity source, NoParamOrder protector, bool mock)
        where T : class, ITypedItemWrapper16, ITypedItem, new()
    {
        if (!mock && source == null) return null;
        if (source is T alreadyT) return alreadyT;

        var item = source as ITypedItem ?? AsItem(source);
        var wrapper = new T();
        wrapper.Setup(item);
        return wrapper;
    }

    /// <summary>
    /// EXPERIMENTAL
    /// </summary>
    public IEnumerable<T> AsCustomList<T>(IEnumerable<ICanBeEntity> source, NoParamOrder protector, bool nullIfNull)
        where T : class, ITypedItemWrapper16, ITypedItem, new()
    {
        if (nullIfNull && source == null) return null;
        if (source is IEnumerable<T> alreadyListT) return alreadyListT;

        var items = SafeItems().Select(i =>
        {
            var wrapper = new T();
            wrapper.Setup(i);
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