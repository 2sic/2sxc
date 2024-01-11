using System.Linq;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Tabs;
using ToSic.Eav.WebApi.Context;
using ToSic.Sxc.Apps.Internal;
using ToSic.Sxc.Blocks.Internal;
using ToSic.Sxc.Dnn.Pages;

namespace ToSic.Sxc.Dnn.Backend.Context;

/// <summary>
/// This contains constructor like initializer calls for ContextDto objects.
/// They are DNN specific
/// </summary>
internal static class DnnContextDtoExtensions
{
    internal static ContentBlockDto Init(this ContentBlockDto dto, BlockConfiguration block, IEnumerable<ModuleWithContent> blockModules)
    {
        dto.Id = block.Id;
        dto.Guid = block.Guid;
        dto.Modules = blockModules.Select(m => new InstanceDto().Init(m.Module, m.Page));
        return dto;
    }

    internal static InstanceDto Init(this InstanceDto dto, ModuleInfo module, TabInfo page)
    {
        dto.Id = module.ModuleID;
        dto.ShowOnAllPages = module.AllTabs;
        dto.Title = module.ModuleTitle;
        dto.UsageId = module.TabModuleID;
        dto.IsDeleted = module.IsDeleted || page.IsDeleted;
        dto.Page = new PageDto().Init(page);
        return dto;
    }

    internal static PageDto Init(this PageDto dto, TabInfo page)
    {
        dto.Id = page.TabID;
        dto.Url = page.FullUrl;
        dto.Name = page.TabName;
        dto.CultureCode = page.CultureCode;
        dto.Visible = page.IsVisible;
        dto.Title = page.Title;
        dto.Portal = new(page.PortalID);
        return dto;
    }

    internal static ViewDto Init(this ViewDto dto, IView view, List<BlockConfiguration> blocks, List<ModuleWithContent> modules)
    {
        dto.Id = view.Entity.EntityId;
        dto.Guid = view.Entity.EntityGuid;
        dto.Name = view.Name;
        dto.Path = view.Path;
        dto.Blocks = blocks
            .Where(b => b.View.Guid == view.Guid)
            .Select(blWMod => new ContentBlockDto().Init(blWMod,
                modules.Where(m => m.ContentGroup == blWMod.Guid)));
        return dto;
    }


}