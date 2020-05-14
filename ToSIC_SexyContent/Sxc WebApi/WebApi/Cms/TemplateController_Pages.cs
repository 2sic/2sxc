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

            var blocks = cms.Blocks.AllWithView();

            Log.Add($"Found {blocks.Count} content blocks");

            // create array with all 2sxc modules in this portal
            var allMods = new Pages(Log).AllModulesWithContent(PortalSettings.PortalId);
            Log.Add($"Found {allMods.Count} modules");

            var result = views.Select(vwb => new
            {
                Id = vwb.Entity.EntityId,
                Guid = vwb.Entity.EntityGuid,
                vwb.Name,
                vwb.Path,
                Blocks = blocks
                    .Where(b => b.View.Guid == vwb.Entity.EntityGuid)
                    .Select(blWMod => new
                    {
                        blWMod.Id,
                        blWMod.Guid,
                        Modules = allMods
                            .Where(m => m.ContentGroup == blWMod.Guid)
                            .Select(m => new
                            {
                                ModuleId = m.Module.ModuleID,
                                ShowOnAllPages = m.Module.AllTabs,
                                Title = m.Module.ModuleTitle,
                                Id = m.Module.TabModuleID,
                                IsDeleted = m.Module.IsDeleted || m.Page.IsDeleted,
                                Page = new
                                {
                                    Id = m.Module.TabID,
                                    Url = m.Page.FullUrl,
                                    Name = m.Page.TabName,
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
