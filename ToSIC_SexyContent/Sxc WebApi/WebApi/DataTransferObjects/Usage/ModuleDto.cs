using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Tabs;

namespace ToSic.Sxc.WebApi.DataTransferObjects.Usage
{
    public class ModuleDto
    {
        public int Id;
        public int ModuleId;
        public bool ShowOnAllPages;
        public string Title;
        public bool IsDeleted;
        public PageDto Page;

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
