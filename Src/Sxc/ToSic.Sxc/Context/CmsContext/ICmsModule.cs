using System.Text.Json.Serialization;
using ToSic.Eav.Metadata;
using ToSic.Sxc.Data;

namespace ToSic.Sxc.Context;

/// <summary>
/// Information about the module context the code is running in.
/// 
/// 🪒 In [Dynamic Razor](xref:Custom.Hybrid.Razor14) it's found on `CmsContext.Module`  
/// 🪒 In [Typed Razor](xref:Custom.Hybrid.RazorTyped) it's found on `MyContext.Module`
/// </summary>
/// <remarks>
/// Note that the module context is the module for which the code is currently running.
/// In some scenarios (like Web-API scenarios) the code is running _for_ this module but _not on_ this module,
/// as it would then be running on a WebApi.
/// </remarks>
[PublicApi]
public interface ICmsModule: IHasMetadata
{
    /// <summary>
    /// The module id on the page. 
    /// 
    /// 🪒 Use in Dynamic Razor: `CmsContext.Module.Id`  
    /// 🪒 Use in Typed Razor: `MyContext.Module.Id`
    ///
    /// > [!TIP]
    /// > This Module ID is often used to give DOM elements a unique name.
    /// > For example: `id="my-app-wrapper-@CmsContext.Module.Id"`.
    /// > But since v16.04 there is a new property `UniqueKey` which is better suited for this.
    /// </summary>
    /// <remarks>
    /// Corresponds to the Dnn ModuleId or the Oqtane Module Id.
    /// 
    /// In some systems a module can be re-used on multiple pages, and possibly have different settings for re-used modules.
    /// 2sxc doesn't use that, so the module id corresponds to the Dnn ModuleId and not the PageModuleId.  
    /// </remarks>
    /// <returns>The ID, unless unknown, in which case it's a negative number</returns>
    int Id { get; }

    /// <summary>
    /// Information about the root block in the module.
    /// </summary>
    /// <remarks>
    /// Added ca. v13, but not documented/published till 2sxc 17.
    /// </remarks>
    ICmsBlock Block { get; }

    [PrivateApi("WIP")]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    [JsonIgnore] // prevent serialization as it's not a normal property
#pragma warning disable CS0108, CS0114
    IMetadata Metadata { get; }
#pragma warning restore CS0108, CS0114
}