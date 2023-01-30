using System;
using System.Collections.Generic;
using System.Linq;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Security.Permissions;
using ToSic.Lib.Documentation;
using ToSic.Eav.Helpers;
using ToSic.Lib.Logging;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.DataSources
{
    [PrivateApi]
    public class DnnPagesDsProvider: PagesDataSourceProvider
    {
        private const int DnnNoParent = -1;
        private const int DnnLevelOffset = 1;
        public DnnPagesDsProvider() : base("Dnn.Pages")
        {
        }


        public override List<CmsPageInfo> GetPagesInternal() => Log.Func(l =>
        {
            var siteId = PortalSettings.Current?.PortalId ?? -1;
            l.A($"Portal Id {siteId}");
            List<TabInfo> pages;
            try
            {
                pages = TabController.GetPortalTabs(siteId, 0, false, true);
            }
            catch (Exception ex)
            {
                l.Ex(ex);
                return (new List<CmsPageInfo>(), "error");
            }

            if (pages == null || !pages.Any()) return (new List<CmsPageInfo>(), "null/empty");

            try
            {
                var result = pages
                    .Where(p => !p.IsSuperTab && !p.IsDeleted && !p.IsSystem)
                    .Where(TabPermissionController.CanViewPage)
                    .Select(p => new CmsPageInfo
                    {
                        Id = p.TabID,
                        Guid = p.UniqueId,
                        Title = p.Title,
                        Name = p.TabName,
                        ParentId = p.ParentId == DnnNoParent ? NoParent : p.ParentId,
                        Path = p.TabPath,
                        Url = p.FullUrl.TrimLastSlash(),
                        Created = p.CreatedOnDate,
                        Modified = p.LastModifiedOnDate,
                        // Existed it v14 but should be deprecated
                        Visible = p.IsVisible,

                        // New 15.01
                        Clickable = !p.DisableLink,
                        HasChildren = p.HasChildren,
                        IsDeleted = p.IsDeleted,
                        Level = p.Level + DnnLevelOffset,
                        Order = p.TabOrder,
                    })
                    .ToList();
                return (result, $"found {result.Count}");
            }
            catch (Exception ex)
            {
                l.Ex(ex);
                return (new List<CmsPageInfo>(), "error");
            }
        });

    }
}
