using ToSic.Eav.Documentation;
using ToSic.Eav.Metadata;
using ToSic.Sxc.Data;

namespace ToSic.Sxc.Context
{
    /// <summary>
    /// Information about the module context the code is running in.
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
        /// 🪒 Use in Razor: `CmsContext.Module.Id`
        /// </summary>
        /// <remarks>
        /// Corresponds to the Dnn ModuleId or the Oqtane Module Id.
        /// 
        /// In some systems a module can be re-used on multiple pages, and possibly have different settings for re-used modules.
        /// 2sxc doesn't use that, so the module id corresponds to the Dnn ModuleId and not the PageModuleId.  
        /// </remarks>
        /// <returns>The ID, unless unknown, in which case it's a negative number</returns>
        int Id { get; }

        [PrivateApi("WIP v13")]
        ICmsBlock Block { get; }

        [PrivateApi("WIP")]
#pragma warning disable CS0108, CS0114
        IDynamicMetadata Metadata { get; }
#pragma warning restore CS0108, CS0114
    }
}
