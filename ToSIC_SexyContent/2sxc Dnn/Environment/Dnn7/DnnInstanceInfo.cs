using DotNetNuke.Entities.Modules;
using ToSic.Eav.Apps.Environment;

namespace ToSic.SexyContent.Environment.Dnn7
{
    public class DnnInstanceInfo: EnvironmentInstance<ModuleInfo>
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
