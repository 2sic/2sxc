using Oqtane.Repository;
using System;
using Oqtane.Models;
using Oqtane.Shared;
using ToSic.Eav.WebApi.Context;
using ToSic.Lib.Services;
using ToSic.Sxc.Apps.Internal;
using ToSic.Sxc.Blocks.Internal;
using ToSic.Sxc.Internal;

namespace ToSic.Sxc.Oqt.Server.Pages;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
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


    public ViewDto ViewDtoBuilder(IView view, List<BlockConfiguration> blocks, List<Oqtane.Models.PageModule> pageModules, ViewDto dto = null)
    {
        dto ??= new();
        dto.Id = view.Entity.EntityId;
        dto.Guid = view.Entity.EntityGuid;
        dto.Name = view.Name;
        dto.Path = view.Path;
        dto.Blocks = blocks
            .Where(b => b.View.Guid == view.Guid)
            .Select(blWMod => ContentBlockDtoBuilder(blWMod, pageModules.Where(m => m.Module.Settings[ModuleSettingNames.ContentGroup] == blWMod.Guid.ToString()).ToList()));
        return dto;
    }

    private ContentBlockDto ContentBlockDtoBuilder(BlockConfiguration block, List<Oqtane.Models.PageModule> blockModules, ContentBlockDto dto = null)
    {
        dto ??= new();
        dto.Id = block.Id;
        dto.Guid = block.Guid;
        dto.Modules = blockModules.Select(m => InstanceDtoBuilder(m, pageRepository.GetPage(m.PageId)));
        return dto;
    }

    private static InstanceDto InstanceDtoBuilder(Oqtane.Models.PageModule pageModule, Oqtane.Models.Page page, InstanceDto dto = null)
    {
        dto ??= new();
        dto.Id = pageModule.ModuleId;
        dto.ShowOnAllPages = pageModule.Module.AllPages;
        dto.Title = pageModule.Title;
        dto.UsageId = pageModule.PageModuleId;
        dto.IsDeleted = pageModule.IsDeleted || page.IsDeleted;
        dto.Page = PageDtoBuilder(page);
        return dto;
    }

    private static PageDto PageDtoBuilder(Oqtane.Models.Page page, PageDto dto = null)
    {
        dto ??= new();
        dto.Id = page.PageId;
        dto.Url = page.Url;
        dto.Name = page.Name;
        dto.CultureCode = Eav.Constants.NullNameId;
        dto.Visible = !page.IsDeleted;
        dto.Title = page.Title;
        dto.Portal = new(page.SiteId);
        return dto;
    }
}