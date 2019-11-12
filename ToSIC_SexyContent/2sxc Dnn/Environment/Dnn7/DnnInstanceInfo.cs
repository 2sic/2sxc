using DotNetNuke.Entities.Modules;
using ToSic.Eav.Environment;

namespace ToSic.SexyContent.Environment.Dnn7
{
    public class DnnInstanceInfo: Container<ModuleInfo>
    {
        public DnnInstanceInfo(ModuleInfo item) : base(item)
        {
        }

        public override int Id => Original.ModuleID;

        public override int PageId => Original.TabID;

        public override int TenantId => Original.PortalID;

        public override bool IsPrimary => Original.DesktopModule.ModuleName == "2sxc";
    }
}
