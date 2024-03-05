using ToSic.Sxc.Apps;
using ToSic.Sxc.Context;
using ToSic.Sxc.Data;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.Code;

/// <summary>
/// Helper object to use on Razor, Code, APIs to create more app-specific helper objects.
/// Eg. the `App` object, `View` object etc.
///
/// It will usually be provided on a protected `Customize` property on RazorTyped etc.
/// </summary>
public interface ICodeCustomizer
{
    /// <summary>
    /// Create (and cache for reuse) a strongly typed App instance for the App object.
    /// </summary>
    /// <typeparam name="TSettings">Type to use for Settings.</typeparam>
    /// <typeparam name="TResources">Type to use for Resources</typeparam>
    IAppTyped<TSettings, TResources> App<TSettings, TResources>()
        where TSettings : class, ITypedItem, ITypedItemWrapper16, new()
        where TResources : class, ITypedItem, ITypedItemWrapper16, new();

    /// <summary>
    /// Create (and cache for reuse) a strongly typed View instance for the MyView object.
    /// </summary>
    /// <typeparam name="TSettings">Type to use for Settings.</typeparam>
    /// <typeparam name="TResources">Type to use for Resources</typeparam>
    ICmsView<TSettings, TResources> View<TSettings, TResources>()
        where TSettings : class, ITypedItem, ITypedItemWrapper16, new()
        where TResources : class, ITypedItem, ITypedItemWrapper16, new();
}