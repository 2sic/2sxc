using DotNetNuke.Entities.Modules;
using ToSic.Eav.Documentation;
using ToSic.Eav.Run;

namespace ToSic.Sxc.Dnn.Run
{
    /// <summary>
    /// The DNN implementation of a Block Container (a Module).
    /// </summary>
    [InternalApi_DoNotUse_MayChangeWithoutNotice("this is just fyi")]
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
