using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Dnn.Pages;

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
            //var blockEntities = cms.Blocks.Entities()
            //    .Select(b =>
            //    {
            //        var templateGuid = b.Children(ViewParts.ViewFieldInContentBlock)
            //            .FirstOrDefault()
            //            ?.EntityGuid;
            //        return templateGuid != null
            //            ? new { Block = b, ViewGuid = templateGuid }
            //            : null;
            //    })
            //    .Where(b => b != null)
            //    .ToList();
            var blocks = cms.Blocks.AllWithView();

            Log.Add($"Found {blocks.Count} content blocks");

            // create array with all 2sxc modules in this portal
            var allMods = new Pages(Log).AllModulesWithContent(PortalSettings.PortalId);
            Log.Add($"Found {allMods.Count} modules");

            var blocksWithModules = blocks.Select(b => new
            {
                //b.ViewGuid,
                Block = b,
                Modules = allMods.Where(m => m.ContentGroup == b.Guid)
            });

            var result = views.Select(vwb => new
            {
                Id = vwb.Entity.EntityId,
                Guid = vwb.Entity.EntityGuid,
                vwb.Name,
                vwb.Path,
                Blocks = blocksWithModules
                    .Where(b => b.Block.View.Guid == vwb.Entity.EntityGuid)
                    .Select(blWMod => new
                    {
                        ContentGroupId = blWMod.Block.Id,
                        ContentGroupGuid = blWMod.Block.Guid,
                        Modules = blWMod.Modules?.Select(m => new
                        {
                            ModuleId = m.Module.ModuleID,
                            ShowOnAllPages = m.Module.AllTabs,
                            Title = m.Module.ModuleTitle,
                            InstanceId = m.Module.TabModuleID,
                            IsDeleted = m.Module.IsDeleted || m.Page.IsDeleted,
                            Page = new
                            {
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

            return wrapLog("ok", result);
        }

    }


}
