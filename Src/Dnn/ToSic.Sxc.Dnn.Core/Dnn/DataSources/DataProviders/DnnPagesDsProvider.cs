using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Security.Permissions;
using ToSic.Eav.Helpers;
using ToSic.Lib.Coding;
using ToSic.Sxc.DataSources.Internal;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.DataSources;

[PrivateApi]
internal class DnnPagesDsProvider: PagesDataSourceProvider
{
    private const int DnnNoParent = -1;
    private const int DnnLevelOffset = 1;
    public DnnPagesDsProvider() : base("Dnn.Pages")
    {
    }


    public override List<PageDataRaw> GetPagesInternal(
        NoParamOrder noParamOrder = default,
        bool includeHidden = default,
        bool includeDeleted = default,
        bool includeAdmin = default,
        bool includeSystem = default,
        bool includeLinks = default,
        bool requireViewPermissions = true,
        bool requireEditPermissions = true)
    {
        var l = Log.Fn<List<PageDataRaw>>($"PortalId: {PortalSettings.Current?.PortalId ?? -1}");
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
            return l.Return(result, $"found {result.Count}");
        }
        catch (Exception ex)
        {
            l.Ex(ex);
            return l.Return([], "error");
        }
    }

}