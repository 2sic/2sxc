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

    private static (T? typed, object? untyped, bool ok) EvalInterface<T>(object? maybe, T? fallback = default)
        where T: class
        => maybe switch
        {
            null => (fallback, null, true),
            T typed => (typed, maybe, true),
            _ => (null, maybe, false)
        };

    public IEntity? Entity(object maybe, IEntity fallback)
    {
        var (typed, untyped, ok) = EvalInterface(maybe, fallback);
        // Try to convert, in case it's an IEntity or something; could also result in error
        return ok
            ? typed
            : untyped == null
                ? null
                : Cdf.AsEntity(untyped);
    }

    public ITypedItem? Item(object? data, NoParamOrder npo, ITypedItem? fallback)
    {
        var (typed, untyped, ok) = EvalInterface(data, fallback);
        // Try to convert, in case it's an IEntity or something; could also result in error
        // TODO: #ConvertItemSettings
        return ok
            ? typed
            : untyped == null
                ? null
                : Cdf.AsItem(untyped, new() { ItemIsStrict = false });
    }

    public IEnumerable<ITypedItem>? Items(object? maybe, NoParamOrder npo, IEnumerable<ITypedItem>? fallback)
    {
        var (typed, untyped, ok) = EvalInterface(maybe, fallback);
        // Try to convert, in case it's an IEntity or something; could also result in error
        return ok
            ? typed
            : untyped == null
                ? null
                : Cdf.AsItems(untyped, new() { ItemIsStrict = false });
    }

    [return: NotNullIfNotNull(nameof(fallback))]
    public IToolbarBuilder? Toolbar(object? maybe, IToolbarBuilder? fallback)
    {
        var (typed, _, ok) = EvalInterface(maybe, fallback);
        // Try to convert, in case it's an IEntity or something; could also result in error
        return ok
            ? typed
            : fallback;
    }


    [return: NotNullIfNotNull(nameof(fallback))]
    public IFile? File(object? maybe, IFile? fallback)
    {
        var (typed, untyped, ok) = EvalInterface(maybe, fallback);
        if (ok)
            return typed;

        // Flatten list if necessary
        return untyped is IEnumerable<IFile> list
            ? list.First()
            : fallback;
    }

    [return: NotNullIfNotNull(nameof(fallback))]
    public IEnumerable<IFile>? Files(object? maybe, IEnumerable<IFile>? fallback)
    {
        // ReSharper disable PossibleMultipleEnumeration
        var (typed, untyped, ok) = EvalInterface(maybe, fallback);
        if (ok)
            return typed;

        // Wrap into list if necessary
        return untyped is IFile item
            ? new List<IFile> { item }
            : fallback;
        // ReSharper restore PossibleMultipleEnumeration
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
        // ReSharper disable PossibleMultipleEnumeration
        var (typed, untyped, ok) = EvalInterface(maybe, fallback);
        if (ok) return typed;

        // Wrap into list if necessary
        return untyped is IFolder item ? new List<IFolder> { item } : fallback;
        // ReSharper restore PossibleMultipleEnumeration
    }

    [return: NotNullIfNotNull(nameof(fallback))]
    public ITypedStack? Stack(object? maybe, ITypedStack? fallback)
    {
        var (typed, _, ok) = EvalInterface(maybe, fallback);
        return ok ? typed : null;
    }

    [return: NotNullIfNotNull(nameof(fallback))]
    public ITyped? Typed(object? maybe, NoParamOrder npo, ITyped? fallback)
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