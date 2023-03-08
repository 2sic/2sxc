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


        public override List<PageDataRaw> GetPagesInternal(
            string noParamOrder = Eav.Parameters.Protector,
            bool includeHidden = default,
            bool includeDeleted = default,
            bool includeAdmin = default,
            bool includeSystem = default,
            bool includeLinks = default,
            bool requireViewPermissions = true,
            bool requireEditPermissions = true
        ) => Log.Func($"PortalId: {PortalSettings.Current?.PortalId ?? -1}", l =>
        {
            List<TabInfo> pages;
            try
            {
                var siteId = PortalSettings.Current?.PortalId ?? -1;
                const bool addDummyNoneSpecifiedPage = false;
                pages = TabController.GetPortalTabs(siteId, 0, addDummyNoneSpecifiedPage,
                    includeHidden, includeDeleted, includeLinks);
            }
            catch (Exception ex)
            {
                l.Ex(ex);
                return (new List<PageDataRaw>(), "error");
            }

            if (pages == null || !pages.Any()) return (new List<PageDataRaw>(), "null/empty");

            try
            {
                var filteredPages = (IEnumerable<TabInfo>)pages;

                // Apply filters as needed
                if (!includeAdmin) filteredPages = filteredPages.Where(x => !x.IsSystem);
                if (!includeSystem) filteredPages = filteredPages.Where(x => !x.IsSuperTab);
                if (requireViewPermissions) filteredPages = filteredPages.Where(TabPermissionController.CanViewPage);
                if (requireEditPermissions) filteredPages = filteredPages.Where(TabPermissionController.CanAdminPage);

                var final = filteredPages.ToList();

                var result = final
                    .Select(p => new PageDataRaw
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
                        // New in 15.02
                        LinkTarget = (string)p.TabSettings["LinkNewWindow"] == true.ToString() ? "_blank": "",
                    })
                    .ToList();
                return (result, $"found {result.Count}");
            }
            catch (Exception ex)
            {
                l.Ex(ex);
                return (new List<PageDataRaw>(), "error");
            }
        });

    }
}
