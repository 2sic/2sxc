using Oqtane.Repository;
using Oqtane.Models;
using Oqtane.Shared;
using ToSic.Lib.Services;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Blocks.Sys;
using ToSic.Sxc.Blocks.Sys.Views;

namespace ToSic.Sxc.Oqt.Server.Pages;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class Pages(
    IPageModuleRepository pageModuleRepository,
    IPageRepository pageRepository,
    ISettingRepository settingRepository)
    : ServiceBase("Oqt.Pages")
{
    public List<PageModule> AllModulesWithContent(int siteId)
    {
        var l = Log.Fn<List<PageModule>>($"{siteId}");

        // create an array with all modules
        var sxcContents = pageModuleRepository.GetPageModules(siteId)
            .Where(pm => pm.Module.ModuleDefinitionName.Contains("ToSic.Sxc.Oqt.Content, ToSic.Sxc.Oqtane.Client")).ToList();

        var sxcApps = pageModuleRepository.GetPageModules(siteId)
            .Where(pm => pm.Module.ModuleDefinitionName.Contains("ToSic.Sxc.Oqt.App, ToSic.Sxc.Oqtane.Client")).ToList();

        var sxcAll = sxcContents.Union(sxcApps).ToList();

        Log.A($"Mods for Content: {sxcContents.Count}, App: {sxcApps.Count}, Total: {sxcAll.Count}");

        var settings = settingRepository.GetSettings(EntityNames.Module).ToList();
        foreach (var pageModule in sxcAll)
        {
            pageModule.Module.Settings = settings.Where(item => item.EntityId == pageModule.ModuleId)                   
                .ToDictionary(setting => setting.SettingName, setting => setting.SettingValue);
        }

        // filter the results
        var allMods = sxcAll
            .Where(m => m.Module.Settings.ContainsKey(ModuleSettingNames.ContentGroup) && m.Module.Settings[ModuleSettingNames.ContentGroup] != Guid.Empty.ToString())
            .ToList();

        return l.Return(allMods, $"{allMods.Count}");
    }


    public ViewDto ViewDtoBuilder(IView view, ICollection<BlockConfiguration> blocks, List<PageModule> pageModules)
    {
        var dto = new ViewDto
        {
            Id = view.Entity.EntityId,
            Guid = view.Entity.EntityGuid,
            Name = view.Name,
            Path = view.Path,
            Blocks = blocks
                .Where(b => b.View.Guid == view.Guid)
                .Select(blWMod => ContentBlockDtoBuilder(blWMod,
                    pageModules.Where(m => m.Module.Settings[ModuleSettingNames.ContentGroup] == blWMod.Guid.ToString())
                        .ToList()))
        };
        return dto;
    }

    private ContentBlockDto ContentBlockDtoBuilder(BlockConfiguration block, List<PageModule> blockModules)
        => new()
        {
            Id = block.Id,
            Guid = block.Guid,
            Modules = blockModules.Select(m => InstanceDtoBuilder(m, pageRepository.GetPage(m.PageId))),
        };

    private static InstanceDto InstanceDtoBuilder(PageModule pageModule, Page page)
        => new()
        {
            Id = pageModule.ModuleId,
            ShowOnAllPages = pageModule.Module.AllPages,
            Title = pageModule.Title,
            UsageId = pageModule.PageModuleId,
            IsDeleted = pageModule.IsDeleted || page.IsDeleted,
            Page = PageDtoBuilder(page),
        };

    private static PageDto PageDtoBuilder(Page page)
        => new()
        {
            Id = page.PageId,
            Url = page.Url,
            Name = page.Name,
            CultureCode = Eav.Sys.EavConstants.NullNameId,
            Visible = !page.IsDeleted,
            Title = page.Title,
            Portal = new(page.SiteId),
        };
}