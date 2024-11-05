using System.Text.Json.Serialization;
using ToSic.Eav.Metadata;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Data;

namespace ToSic.Sxc.Context;

/// <summary>
/// View context information.
///
/// 🪒 In [Dynamic Razor](xref:Custom.Hybrid.Razor14) it's found on `CmsContext.View`  
/// 🪒 In [Typed Razor](xref:Custom.Hybrid.RazorTyped) it's found on `MyView`
/// </summary>
/// <remarks>
/// Added in 2sxc 12.02
/// </remarks>
[PublicApi]
public interface ICmsView: IHasMetadata
{
    /// <summary>
    /// View configuration ID
    /// 
    /// 🪒 Use in Dynamic Razor: `CmsContext.View.Id`  
    /// 🪒 Use in Typed Razor: `MyView.Id`
    /// </summary>
    int Id { get; }

    /// <summary>
    /// Name of the view as configured - note that because of i18n it could be different depending on the language.
    /// To clearly identify a view, use the <see cref="Identifier"/> or use `Settings`
    /// 
    /// 🪒 Use in Dynamic Razor: `CmsContext.View.Name`  
    /// 🪒 Use in Typed Razor: `MyView.Name`
    /// </summary>
    string Name { get; }

    /// <summary>
    /// An optional identifier which the View configuration can provide.
    /// Use this when you want to use the same template but make minor changes based on the View selected (like change the number of columns).
    /// Usually you will use either this OR the `Settings`
    /// 
    /// 🪒 Use in Razor: `CmsContext.View.Identifier`  
    /// 🪒 Use in Typed Razor: `MyView.Identifier`
    /// </summary>
    string Identifier { get; }

    /// <summary>
    /// Edition used - if any. Otherwise empty string. 
    /// 
    /// 🪒 Use in Razor: `CmsContext.View.Edition`  
    /// 🪒 Use in Typed Razor: `MyView.Edition`
    /// </summary>
    string Edition { get; }

    /// <summary>
    /// Metadata of the current view
    /// </summary>
    /// <remarks>
    /// Added in v13.12
    /// </remarks>
    // actually Added in v12.10; changed from IMetadataOf to IDynamicMetadata in 13.12
    [JsonIgnore] // prevent serialization as it's not a normal property
    new IMetadata Metadata { get; }

    /// <summary>
    /// The path to this view.
    /// For URLs which should load js/css from a path beneath the view.
    ///
    /// This is different from the `App.Path`, because it will also contain the edition (if there is an edition)
    /// </summary>
    /// <remarks>
    /// Added in v15.04
    /// </remarks>
    [Obsolete("Obsolete in v16, pls use Folder.Url instead")]
    [PrivateApi("Hidden in 16.04, because we want people to use the Folder. Can't remove it though, because there are many apps that already published this.")]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    [JsonIgnore] // old property
    string Path { get; }

    /// <summary>
    /// This is the preferred way to get the Url or Path to the current view.
    ///
    /// This is different from the `App.Folder`, because it will also contain the edition (if there is an edition)
    /// 
    /// 🪒 Use in Razor: `CmsContext.View.Folder` - like `CmsContext.View.Folder.Url`  
    /// 🪒 Use in Typed Razor: `MyView.Edition` - like `MyView.Folder.Url`
    /// </summary>
    /// <remarks>
    /// Added in v16.04
    /// </remarks>
    IFolder Folder { get; }

    /// <summary>
    /// Settings of this view.
    /// This property only works in the new typed code.
    ///
    /// Note that many views don't have their own settings, so this would be empty = `null`.
    /// </summary>
    ITypedItem Settings { get; }

    /// <summary>
    /// Settings of this view.
    /// This property only works in the new typed code.
    ///
    /// Note that many views don't have their own settings, so this would be empty = `null`.
    /// </summary>
    /// <remarks>Resources added to API in v17.04</remarks>
    ITypedItem Resources { get; }
}