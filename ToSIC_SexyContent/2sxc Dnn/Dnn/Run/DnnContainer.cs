using DotNetNuke.Entities.Modules;
using ToSic.Eav.Documentation;
using ToSic.Eav.Run;

namespace ToSic.Sxc.Dnn
{
    /// <summary>
    /// The DNN implementation of a Block Container (a Module).
    /// </summary>
    [PublicApi]
    public class DnnContainer: Container<ModuleInfo>
    {
        public DnnContainer(ModuleInfo item) : base(item)
        {
        }

        /// <inheritdoc />
        public override int Id => UnwrappedContents.ModuleID;

        /// <inheritdoc />
        public override int PageId => UnwrappedContents.TabID;

        /// <inheritdoc />
        public override int TenantId => UnwrappedContents.PortalID;

        /// <inheritdoc />
        public override bool IsPrimary => UnwrappedContents.DesktopModule.ModuleName == "2sxc";
    }
}
