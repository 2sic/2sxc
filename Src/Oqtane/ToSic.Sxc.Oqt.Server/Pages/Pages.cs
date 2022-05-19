using Oqtane.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using Oqtane.Models;
using Oqtane.Shared;
using ToSic.Eav.Logging;
using ToSic.Eav.WebApi.Context;
using ToSic.Sxc.Apps.Blocks;
using ToSic.Sxc.Blocks;

namespace ToSic.Sxc.Oqt.Server.Pages
{
    public class Pages: HasLog
    {
        private readonly IPageModuleRepository _pageModuleRepository;
        private readonly IPageRepository _pageRepository;
        private readonly ISettingRepository _settingRepository;

        public Pages(IPageModuleRepository pageModuleRepository, IPageRepository pageRepository, ISettingRepository settingRepository, ILog parentLog = null) : base("Oqt.Pages", parentLog)
        {
            _pageModuleRepository = pageModuleRepository;
            _pageRepository = pageRepository;
            _settingRepository = settingRepository;
        }

        public List<Oqtane.Models.PageModule> AllModulesWithContent(int siteId)
        {
            var wrapLog = Log.Call<List<Oqtane.Models.PageModule>>($"{siteId}");

            // create an array with all modules
            var sxcContents = _pageModuleRepository.GetPageModules(siteId)
                .Where(pm => pm.Module.ModuleDefinitionName.Contains("ToSic.Sxc.Oqt.Content, ToSic.Sxc.Oqtane.Client")).ToList();

            var sxcApps = _pageModuleRepository.GetPageModules(siteId)
                .Where(pm => pm.Module.ModuleDefinitionName.Contains("ToSic.Sxc.Oqt.App, ToSic.Sxc.Oqtane.Client")).ToList();

            var sxcAll = sxcContents.Union(sxcApps).ToList();

            Log.A($"Mods for Content: {sxcContents.Count}, App: {sxcApps.Count}, Total: {sxcAll.Count}");

            var settings = _settingRepository.GetSettings(EntityNames.Module).ToList();
            foreach (var pageModule in sxcAll)
            {
                pageModule.Module.Settings = settings.Where(item => item.EntityId == pageModule.ModuleId)                   
                        .ToDictionary(setting => setting.SettingName, setting => setting.SettingValue);
            }

            // filter the results
            var allMods = sxcAll
                .Where(m => m.Module.Settings.ContainsKey(Settings.ModuleSettingContentGroup) && m.Module.Settings[Settings.ModuleSettingContentGroup] != Guid.Empty.ToString())
                .ToList();

            return wrapLog($"{allMods.Count}", allMods);
        }


        public ViewDto ViewDtoBuilder(IView view, List<BlockConfiguration> blocks, List<Oqtane.Models.PageModule> pageModules, ViewDto dto = null)
        {
            dto ??= new ViewDto();
            dto.Id = view.Entity.EntityId;
            dto.Guid = view.Entity.EntityGuid;
            dto.Name = view.Name;
            dto.Path = view.Path;
            dto.Blocks = blocks
                .Where(b => b.View.Guid == view.Guid)
                .Select(blWMod => ContentBlockDtoBuilder(blWMod, pageModules.Where(m => m.Module.Settings[Settings.ModuleSettingContentGroup] == blWMod.Guid.ToString()).ToList()));
            return dto;
        }

        private ContentBlockDto ContentBlockDtoBuilder(BlockConfiguration block, List<Oqtane.Models.PageModule> blockModules, ContentBlockDto dto = null)
        {
            dto ??= new ContentBlockDto();
            dto.Id = block.Id;
            dto.Guid = block.Guid;
            dto.Modules = blockModules.Select(m => InstanceDtoBuilder(m, _pageRepository.GetPage(m.PageId)));
            return dto;
        }

        private static InstanceDto InstanceDtoBuilder(Oqtane.Models.PageModule pageModule, Oqtane.Models.Page page, InstanceDto dto = null)
        {
            dto ??= new InstanceDto();
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
            dto ??= new PageDto();
            dto.Id = page.PageId;
            dto.Url = page.Url;
            dto.Name = page.Name;
            dto.CultureCode = "unknown";
            dto.Visible = !page.IsDeleted;
            dto.Title = page.Title;
            dto.Portal = new SiteDto(page.SiteId);
            return dto;
        }
    }
}
