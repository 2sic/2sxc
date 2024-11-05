using ToSic.Sxc.Apps;
using ToSic.Sxc.Context;
using ToSic.Sxc.Data;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.Code;

/// <summary>
/// Helper object to use on Razor, Code, APIs to create more app-specific helper objects.
/// Like the `App` object, `View` object etc.
///
/// It will usually be provided on a protected `Customize` property on RazorTyped etc.
/// </summary>
/// <remarks>
/// New v17.03 (BETA!)
/// </remarks>
[PublicApi]
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
    ICmsView<TSettings, TResources> MyView<TSettings, TResources>()
        where TSettings : class, ITypedItem, ITypedItemWrapper16, new()
        where TResources : class, ITypedItem, ITypedItemWrapper16, new();

    /// <summary>
    /// Create (and cache for reuse) a strongly typed Item instance for the MyItem object.
    /// </summary>
    /// <typeparam name="TCustomType">Type to use for MyItem.</typeparam>
    /// <returns></returns>
    public TCustomType MyItem<TCustomType>()
        where TCustomType : class, ITypedItem, ITypedItemWrapper16, new();

    /// <summary>
    /// Create (and cache for reuse) a strongly typed Items instance for the MyItems object.
    /// </summary>
    /// <typeparam name="TCustomType">Type to use for MyItems.</typeparam>
    /// <returns></returns>
    public IEnumerable<TCustomType> MyItems<TCustomType>()
        where TCustomType : class, ITypedItem, ITypedItemWrapper16, new();

    /// <summary>
    /// Create (and cache for reuse) a strongly typed Header instance for the MyHeader object.
    /// </summary>
    /// <typeparam name="TCustomType">Type to use for MyHeader.</typeparam>
    TCustomType MyHeader<TCustomType>()
        where TCustomType : class, ITypedItem, ITypedItemWrapper16, new();
}