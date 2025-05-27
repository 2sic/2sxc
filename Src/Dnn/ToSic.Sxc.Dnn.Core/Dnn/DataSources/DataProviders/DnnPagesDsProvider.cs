using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Security.Permissions;
using ToSic.Eav.Plumbing;
using ToSic.Lib.Coding;
using ToSic.Sxc.Cms.Pages.Internal;
using ToSic.Sxc.DataSources.Internal;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.DataSources;

[PrivateApi]
internal class DnnPagesDsProvider() : PagesDataSourceProvider("Dnn.Pages")
{
    private const int DnnNoParent = -1;
    private const int DnnLevelOffset = 1;

    public override List<PageModelRaw> GetPagesInternal(
        NoParamOrder noParamOrder = default,
        bool includeHidden = default,
        bool includeDeleted = default,
        bool includeAdmin = default,
        bool includeSystem = default,
        bool includeLinks = default,
        bool requireViewPermissions = true,
        bool requireEditPermissions = true)
    {
        var l = Log.Fn<List<PageModelRaw>>($"PortalId: {PortalSettings.Current?.PortalId ?? -1}");
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
            return l.Return([], "error");
        }

        if (pages == null || !pages.Any()) return l.Return([], "null/empty");

        try
        {
            IEnumerable<TabInfo> filtered = pages;

            // Apply filters as needed
            if (!includeAdmin)
                filtered = filtered.Where(x => !x.IsSystem);
            if (!includeSystem)
                filtered = filtered.Where(x => !x.IsSuperTab);
            if (requireViewPermissions)
                filtered = filtered.Where(TabPermissionController.CanViewPage);
            if (requireEditPermissions)
                filtered = filtered.Where(TabPermissionController.CanAdminPage);

            var final = filtered.ToList();

            var result = final
                .Select(p => new PageModelRaw
                {
                    Id = p.TabID,
                    Guid = p.UniqueId,
                    Title = p.Title.UseFallbackIfNoValue(p.TabName),
                    Name = p.TabName,
                    ParentId = p.ParentId == DnnNoParent ? NoParent : p.ParentId,
                    Path = p.TabPath.FlattenMultipleForwardSlashes(),
                    Url = p.FullUrl.TrimLastSlash(),
                    Created = p.CreatedOnDate,
                    Modified = p.LastModifiedOnDate,
                    IsNavigation = p.IsVisible, // note: renamed in 19.01 to `IsNavigation` from `Visible`

                    // New 15.01
                    IsClickable = !p.DisableLink,
                    HasChildren = p.HasChildren,
                    IsDeleted = p.IsDeleted,
                    Level = p.Level + DnnLevelOffset,
                    Order = p.TabOrder,
                    // New in 15.02
                    LinkTarget = (string)p.TabSettings["LinkNewWindow"] == true.ToString() ? "_blank": "",
                })
                .ToList();
            return l.Return(result, $"found {result.Count}");
        }
        catch (Exception ex)
        {
            l.Ex(ex);
            return l.Return([], "error");
        }
    }

}