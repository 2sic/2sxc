using ToSic.Sxc.Data;

// ReSharper disable once CheckNamespace
namespace Custom.Data;

/// <summary>
/// Base class for custom data objects, which are typed and can be used in Razor Components
/// </summary>
[PrivateApi("WIP, don't publish yet")]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
// ReSharper disable once UnusedMember.Global
public abstract class Item16<TPresentation> : Item16Experimental
    where TPresentation : class, ITypedItemWrapper16, ITypedItem, new()
{
    public new TPresentation Presentation => _presentation ??= Kit.Convert.As<TPresentation>(base.Presentation);
    private TPresentation _presentation;
}