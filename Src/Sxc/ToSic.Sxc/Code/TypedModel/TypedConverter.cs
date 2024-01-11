using ToSic.Razor.Blade;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Data;
using ToSic.Sxc.Edit.Toolbar;
using CodeDataFactory = ToSic.Sxc.Data.Internal.CodeDataFactory;

// ReSharper disable PossibleMultipleEnumeration

namespace ToSic.Sxc.Code;

/// <summary>
/// Helper to convert some unknown object into the possible result.
/// </summary>
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
internal class TypedConverter(CodeDataFactory cdf)
{
    public CodeDataFactory Cdf { get; } = cdf;

    public (T typed, object untyped, bool ok) EvalInterface<T>(object maybe, T fallback = default) where T: class 
    {
        if (maybe == null) return (fallback, null, true);
        if (maybe is T typed) return (typed, maybe, true);
        return (null, maybe, false);
    }

    public IEntity Entity(object maybe, IEntity fallback)
    {
        var (typed, untyped, ok) = EvalInterface(maybe, fallback);
        // Try to convert, in case it's an IEntity or something; could also result in error
        return ok ? typed : Cdf.AsEntity(untyped);
    }

    public ITypedItem Item(object data, NoParamOrder noParamOrder, ITypedItem fallback)
    {
        var (typed, untyped, ok) = EvalInterface(data, fallback);
        // Try to convert, in case it's an IEntity or something; could also result in error
        return ok ? typed : Cdf.AsItem(untyped);
    }

    public IEnumerable<ITypedItem> Items(object maybe, NoParamOrder noParamOrder, IEnumerable<ITypedItem> fallback)
    {
        var (typed, untyped, ok) = EvalInterface(maybe, fallback);
        // Try to convert, in case it's an IEntity or something; could also result in error
        return ok ? typed : Cdf.AsItems(untyped);
    }

    public IToolbarBuilder Toolbar(object maybe, IToolbarBuilder fallback)
    {
        var (typed, _, ok) = EvalInterface(maybe, fallback);
        // Try to convert, in case it's an IEntity or something; could also result in error
        return ok ? typed : fallback;
    }


    public IFile File(object maybe, IFile fallback)
    {
        var (typed, untyped, ok) = EvalInterface(maybe, fallback);
        if (ok) return typed;

        // Flatten list if necessary
        return untyped is IEnumerable<IFile> list ? list.First() : fallback;
    }

    public IEnumerable<IFile> Files(object maybe, IEnumerable<IFile> fallback)
    {
        var (typed, untyped, ok) = EvalInterface(maybe, fallback);
        if (ok) return typed;

        // Wrap into list if necessary
        return untyped is IFile item ? new List<IFile> { item } : fallback;
    }

    public IFolder Folder(object maybe, IFolder fallback)
    {
        var (typed, untyped, ok) = EvalInterface(maybe, fallback);
        if (ok) return typed;

        // Flatten list if necessary
        return untyped is IEnumerable<IFolder> list ? list.First() : fallback;
    }

    public IEnumerable<IFolder> Folders(object maybe, IEnumerable<IFolder> fallback)
    {
        var (typed, untyped, ok) = EvalInterface(maybe, fallback);
        if (ok) return typed;

        // Wrap into list if necessary
        return untyped is IFolder item ? new List<IFolder> { item } : fallback;
    }

    public ITypedStack Stack(object maybe, ITypedStack fallback)
    {
        var (typed, _, ok) = EvalInterface(maybe, fallback);
        return ok ? typed : null;
    }

    public ITyped Typed(object maybe, NoParamOrder noParamOrder, ITyped fallback)
    {
        var (typed, untyped, ok) = EvalInterface(maybe, fallback);
        // Try to convert, in case it's an IEntity or something; could also result in error
        return ok ? typed : Cdf.AsItem(untyped);
    }

    #region Tags

    public IHtmlTag HtmlTag(object maybe, IHtmlTag fallback)
    {
        var (typed, _, ok) = EvalInterface(maybe, fallback);
        return ok ? typed : null;
    }
        
    public IEnumerable<IHtmlTag> HtmlTags(object maybe, IEnumerable<IHtmlTag> fallback)
    {
        var (typed, _, ok) = EvalInterface(maybe, fallback);
        return ok ? typed : null;
    }

    #endregion
}