using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Tabs;
using ToSic.Sxc.WebApi.Context;

namespace ToSic.Sxc.WebApi.DataTransferObjects.Usage
{
    public class ModuleDto: InstanceDto
    {
        public ModuleDto(ModuleInfo module, TabInfo page)
        {
            ModuleId = module.ModuleID;
            ShowOnAllPages = module.AllTabs;
            Title = module.ModuleTitle;
            Id = module.TabModuleID;
            IsDeleted = module.IsDeleted || page.IsDeleted;
            Page = new PageDto(page);
        }
    }
}
