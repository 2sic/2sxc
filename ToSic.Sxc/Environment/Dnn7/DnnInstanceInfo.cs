using DotNetNuke.Entities.Modules;
using ToSic.Eav.Apps.Environment;

namespace ToSic.SexyContent.Environment.Dnn7
{
    public class DnnInstanceInfo: InstanceInfo<ModuleInfo>
    {
        public DnnInstanceInfo(ModuleInfo item) : base(item)
        {
        }

        public override int Id => Info.ModuleID;

        public override int PageId => Info.TabID;
    }
}
