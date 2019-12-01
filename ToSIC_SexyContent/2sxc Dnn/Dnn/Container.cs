using DotNetNuke.Entities.Modules;
using ToSic.Eav.Documentation;
using ToSic.Eav.Environment;

namespace ToSic.Sxc.Dnn
{
    /// <summary>
    /// The DNN implementation of a Block Container (a Module).
    /// </summary>
    [PublicApi]
    public class Container: Container<ModuleInfo>
    {
        public Container(ModuleInfo item) : base(item)
        {
        }

        /// <inheritdoc />
        public override int Id => Original.ModuleID;

        /// <inheritdoc />
        public override int PageId => Original.TabID;

        /// <inheritdoc />
        public override int TenantId => Original.PortalID;

        /// <inheritdoc />
        public override bool IsPrimary => Original.DesktopModule.ModuleName == "2sxc";
    }
}
