using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Tabs;
using ToSic.Eav.WebApi.Sys.Context;
using ToSic.Sxc.Blocks.Internal;
using ToSic.Sxc.Dnn.Pages;

namespace ToSic.Sxc.Dnn.Backend.Context;

/// <summary>
/// This contains constructor like initializer calls for ContextDto objects.
/// They are DNN specific
/// </summary>
internal static class DnnContextDtoExtensions
{
    internal static ContentBlockDto ToDto(this BlockConfiguration block, IEnumerable<ModuleWithContent> blockModules)
        => new()
        {
            Id = block.Id,
            Guid = block.Guid,
            Modules = blockModules.Select(m => m.Module.ToDto(m.Page)),
        };

    internal static InstanceDto ToDto(this ModuleInfo module, TabInfo page)
        => new()
        {
            Id = module.ModuleID,
            ShowOnAllPages = module.AllTabs,
            Title = module.ModuleTitle,
            UsageId = module.TabModuleID,
            IsDeleted = module.IsDeleted || page.IsDeleted,
            Page = page.ToDto(),
        };

    internal static PageDto ToDto(this TabInfo page)
        => new()
        {
            Id = page.TabID,
            Url = page.FullUrl,
            Name = page.TabName,
            CultureCode = page.CultureCode,
            Visible = page.IsVisible,
            Title = page.Title,
            Portal = new(page.PortalID),
        };

    internal static ViewDto Init(this IView view, ICollection<BlockConfiguration> blocks, List<ModuleWithContent> modules)
        => new()
        {
            Id = view.Entity.EntityId,
            Guid = view.Entity.EntityGuid,
            Name = view.Name,
            Path = view.Path,
            Blocks = blocks
                .Where(b => b.View.Guid == view.Guid)
                .Select(blWMod => blWMod.ToDto(modules.Where(m => m.ContentGroup == blWMod.Guid))),
        };
}