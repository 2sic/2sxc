using ToSic.Eav.Documentation;

namespace ToSic.Sxc.Context
{
    [PublicApi]
    public interface ICmsModule
    {
        /// <summary>
        /// The module id on the page. Corresponds to the Dnn ModuleId or the Oqtane Module Id.
        /// </summary>
        /// <remarks>
        /// In some systems a module can be re-used on multiple pages, and possibly have different settings for re-used modules.
        /// 2sxc doesn't use that, so the module id corresponds to the Dnn ModuleId and not the PageModuleId.
        /// </remarks>
        int Id { get; }
    }
}
