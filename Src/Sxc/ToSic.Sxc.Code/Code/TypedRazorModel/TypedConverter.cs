using ToSic.Razor.Blade;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Data;
using ToSic.Sxc.Data.Sys.Factory;
using ToSic.Sxc.Edit.Toolbar;

namespace ToSic.Sxc.Code;

/// <summary>
/// Helper to convert some unknown object into the possible result.
/// </summary>
[ShowApiWhenReleased(ShowApiMode.Never)]
internal class TypedConverter(ICodeDataFactory cdf)
{
    public ICodeDataFactory Cdf { get; } = cdf;

    public (T? typed, object? untyped, bool ok) EvalInterface<T>(object? maybe, T? fallback = default) where T: class 
    {
        if (maybe == null)
            return (fallback, null, true);
        if (maybe is T typed) return (typed, maybe, true);
        return (null, maybe, false);
    }

    public IEntity Entity(object maybe, IEntity fallback)
    {
        var (typed, untyped, ok) = EvalInterface(maybe, fallback);
        // Try to convert, in case it's an IEntity or something; could also result in error
        return ok ? typed : Cdf.AsEntity(untyped);
    }

    public ITypedItem Item(object? data, NoParamOrder noParamOrder, ITypedItem? fallback)
    {
        var (typed, untyped, ok) = EvalInterface(data, fallback);
        // Try to convert, in case it's an IEntity or something; could also result in error
        // TODO: #ConvertItemSettings
        return ok ? typed : Cdf.AsItem(untyped, new() { ItemIsStrict = false });
    }

    public IEnumerable<ITypedItem> Items(object? maybe, NoParamOrder noParamOrder, IEnumerable<ITypedItem>? fallback)
    {
        var (typed, untyped, ok) = EvalInterface(maybe, fallback);
        // Try to convert, in case it's an IEntity or something; could also result in error
        return ok
            ? typed
            : Cdf.AsItems(untyped, new() { ItemIsStrict = false });
    }

    [return: NotNullIfNotNull(nameof(fallback))]
    public IToolbarBuilder? Toolbar(object? maybe, IToolbarBuilder? fallback)
    {
        var (typed, _, ok) = EvalInterface(maybe, fallback);
        // Try to convert, in case it's an IEntity or something; could also result in error
        return ok ? typed : fallback;
    }


    [return: NotNullIfNotNull(nameof(fallback))]
    public IFile? File(object? maybe, IFile? fallback)
    {
        var (typed, untyped, ok) = EvalInterface(maybe, fallback);
        if (ok)
            return typed;

        // Flatten list if necessary
        return untyped is IEnumerable<IFile> list ? list.First() : fallback;
    }

    [return: NotNullIfNotNull(nameof(fallback))]
    public IEnumerable<IFile>? Files(object maybe, IEnumerable<IFile>? fallback)
    {
        var (typed, untyped, ok) = EvalInterface(maybe, fallback);
        if (ok)
            return typed;

        // Wrap into list if necessary
        return untyped is IFile item ? new List<IFile> { item } : fallback;
    }

    [return: NotNullIfNotNull(nameof(fallback))]
    public IFolder? Folder(object? maybe, IFolder? fallback)
    {
        var (typed, untyped, ok) = EvalInterface(maybe, fallback);
        if (ok)
            return typed;

        // Flatten list if necessary
        return untyped is IEnumerable<IFolder> list ? list.First() : fallback;
    }

    [return: NotNullIfNotNull(nameof(fallback))]
    public IEnumerable<IFolder>? Folders(object? maybe, IEnumerable<IFolder>? fallback)
    {
        var (typed, untyped, ok) = EvalInterface(maybe, fallback);
        if (ok) return typed;

        // Wrap into list if necessary
        return untyped is IFolder item ? new List<IFolder> { item } : fallback;
    }

    [return: NotNullIfNotNull(nameof(fallback))]
    public ITypedStack? Stack(object? maybe, ITypedStack? fallback)
    {
        var (typed, _, ok) = EvalInterface(maybe, fallback);
        return ok ? typed : null;
    }

    [return: NotNullIfNotNull(nameof(fallback))]
    public ITyped? Typed(object? maybe, NoParamOrder noParamOrder, ITyped? fallback)
    {
        var (typed, untyped, ok) = EvalInterface(maybe, fallback);
        // Try to convert, in case it's an IEntity or something; could also result in error
        // TODO: #ConvertItemSettings
        return ok ? typed : Cdf.AsItem(untyped, new() { ItemIsStrict = false });
    }

    #region Tags

    [return: NotNullIfNotNull(nameof(fallback))]
    public IHtmlTag? HtmlTag(object? maybe, IHtmlTag? fallback)
    {
        var (typed, _, ok) = EvalInterface(maybe, fallback);
        return ok ? typed : null;
    }

    [return: NotNullIfNotNull(nameof(fallback))]
    public IEnumerable<IHtmlTag>? HtmlTags(object? maybe, IEnumerable<IHtmlTag>? fallback)
    {
        var (typed, _, ok) = EvalInterface(maybe, fallback);
        return ok ? typed : null;
    }

    #endregion
}