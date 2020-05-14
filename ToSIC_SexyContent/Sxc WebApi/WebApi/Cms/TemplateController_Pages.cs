using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Dnn;

namespace ToSic.Sxc.WebApi.Cms
{
    public partial class TemplateController
    {
        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        public dynamic Usage(int appId, Guid guid)
        {
            var wrapLog = Log.Call<dynamic>($"{appId}, {guid}");
            // todo: extra security to only allow zone change if host user

            var cms = new CmsRuntime(appId, Log, true);
            // treat view as a list - in case future code will want to analyze many views together
            var views = new List<IView> {cms.Views.Get(guid)};

            // get all blocks which have a view assigned
            var blockEntities = cms.Blocks.ContentBlockEntities()
                .Select(b =>
                {
                    var templateGuid = b.Children(ViewParts.ViewFieldInContentBlock)
                        .FirstOrDefault()
                        ?.EntityGuid;
                    return templateGuid != null
                        ? new { Block = b, ViewGuid = templateGuid }
                        : null;
                })
                .Where(b => b != null)
                .ToList();

            Log.Add($"Found {blockEntities.Count} content blocks");

            // create array with all 2sxc modules in this portal
            var allMods = GetAllModulesOfPortal(PortalSettings.PortalId);
            Log.Add($"Found {allMods.Count} modules");

            var modsWithContentGroupLink = allMods.Select(mod => new
                {
                    Module = mod,
                    ContentRef = Guid.TryParse(mod.Module.ModuleSettings[Settings.ContentGroupGuidString].ToString(),
                        out var g)
                        ? g
                        : Guid.Empty
                })
                .Where(set => set.ContentRef != Guid.Empty)
                .ToList();
            Log.Add($"Mods with active content-block reference: {modsWithContentGroupLink.Count}");

            var blocksWithModules = blockEntities.Select(b => new
            {
                b.ViewGuid,
                b.Block,
                Modules = modsWithContentGroupLink
                    .Where(m => m.ContentRef == b.Block.EntityGuid)
                    .Select(m => m.Module)
            });

            var viewsWithBlocks = views.Select(v => new
            {
                View = v,
                Blocks = blocksWithModules.Where(b => b.ViewGuid == v.Entity.EntityGuid)
            });

            var result = viewsWithBlocks.Select(vwb => new
            {
                Id = vwb.View.Entity.EntityId,
                Guid = vwb.View.Entity.EntityGuid,
                vwb.View.Name,
                vwb.View.Path,
                Blocks = vwb.Blocks?.Select(blWMod => new
                {
                    blWMod.Block.EntityId,
                    blWMod.Block.EntityGuid,
                    Modules = blWMod.Modules?.Select(m => new
                    {
                        ModuleId = m.Module.ModuleID,
                        ShowOnAllPages = m.Module.AllTabs,
                        Title = m.Module.ModuleTitle,
                        InstanceId = m.Module.TabModuleID,
                        IsDeleted = m.Module.IsDeleted || m.Page.IsDeleted,
                        Page = new {
                            Id = m.Module.TabID,
                            Url = m.Page.FullUrl,
                            PageName = m.Page.TabName,
                            m.Page.CultureCode,
                            Visible = m.Page.IsVisible,
                            m.Page.Title
                        }
                    })
                })
            });

            return result;
        }


        private List<ModuleWithPage> GetAllModulesOfPortal(int portalId)
        {
            var wrapLog = Log.Call<List<ModuleWithPage>>($"{portalId}");
            var mc = DotNetNuke.Entities.Modules.ModuleController.Instance;
            var tabC = TabController.Instance;

            // create an array with all modules
            var modules2Sxc = mc.GetModulesByDefinition(portalId, DnnConstants.ModuleNameContent).ToArray().Cast<ModuleInfo>().ToList();
            var dnnMod2SxcApp = mc.GetModulesByDefinition(portalId, DnnConstants.ModuleNameApp).ToArray().Cast<ModuleInfo>().ToList();
            var all = modules2Sxc.Union(dnnMod2SxcApp).ToList();
            Log.Add($"Mods for Content: {modules2Sxc.Count}, App: {dnnMod2SxcApp.Count}, Total: {all.Count}");

            // filter the results
            var allMods = all
                .Where(m => m.DefaultLanguageModule == null)
                .Where(m => m.ModuleSettings.ContainsKey(Settings.ContentGroupGuidString))
                .ToList();

            var result = allMods.Select(m => new ModuleWithPage()
            {
                Module = m,
                Page = tabC.GetTab(m.TabID, portalId)
            }).ToList();

            return wrapLog($"{allMods.Count}", result);
        }

        class ModuleWithPage
        {
            public ModuleInfo Module;
            public TabInfo Page;
        }

    }


}
