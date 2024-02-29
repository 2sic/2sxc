namespace ToSic.Sxc.Data.Internal;

partial class CodeDataFactory
{
    /// <summary>
    /// EXPERIMENTAL
    /// </summary>
    public T AsCustom<T>(ICanBeEntity source, NoParamOrder protector = default, bool mock = false)
        where T : class, ITypedItemWrapper16, ITypedItem, new()
    {
        if (!mock && source == null) return null;
        if (source is T alreadyT) return alreadyT;

        var item = source as ITypedItem ?? AsItem(source);
        return AsCustomFromItem<T>(item);
    }

    internal static T AsCustomFromItem<T>(ITypedItem item) where T : class, ITypedItemWrapper16, ITypedItem, new()
    {
        if (item == null) return null;
        if (item is T t) return t;
        var newT = new T();
        newT.Setup(item);
        return newT;
    }
    /// <summary>
    /// WIP / experimental, would be for types which are not as T, but as a type-object.
    /// Not in use, so not fully tested.
    ///
    /// Inspired by https://stackoverflow.com/questions/3702916/is-there-a-typeof-inverse-operation
    /// </summary>
    /// <param name="t"></param>
    /// <param name="source"></param>
    /// <param name="protector"></param>
    /// <param name="mock"></param>
    /// <returns></returns>
    public object AsCustom(Type t, ICanBeEntity source, NoParamOrder protector = default, bool mock = false)
    {
        if (!mock && source == null) return null;
        if (source.GetType() == t) return source;

        var item = source as ITypedItem ?? AsItem(source);
        var wrapperObj = Activator.CreateInstance(t);
        if (wrapperObj is not ITypedItemWrapper16 wrapper) return null;
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

        return SafeItems().Select(AsCustomFromItem<T>);

        IEnumerable<ITypedItem> SafeItems()
        {
            if (source == null || !source.Any()) return [];
            if (source is IEnumerable<ITypedItem> alreadyOk) return alreadyOk;
            return _CodeApiSvc.Cdf.AsItems(source);
        }
    }

}