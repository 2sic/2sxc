using DotNetNuke.Entities.Portals;
using DotNetNuke.Security.Roles;
using ToSic.Lib.Services;
using ToSic.Sxc.Cms.Users.Sys;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.DataSources;

/// <summary>
/// Deliver a list of roles from the Dnn.
/// </summary>
internal class DnnRolesDsProvider() : ServiceBase("Dnn.Roles"), IUserRolesProvider
{
    public IEnumerable<UserRoleModel> GetRoles()
    {
        var l = Log.Fn<IEnumerable<UserRoleModel>>();
        var siteId = PortalSettings.Current?.PortalId ?? -1;
        l.A($"Portal Id {siteId}");
        try
        {
            var dnnRoles = RoleController.Instance.GetRoles(portalId: siteId);
            if (!dnnRoles.Any())
                return l.Return(new List<UserRoleModel>(), "null/empty");

            var result = dnnRoles
                .Select(r => new UserRoleModel
                {
                    Id = r.RoleID,
                    // Guid = r.
                    Name = r.RoleName,
                    Created = r.CreatedOnDate,
                    Modified = r.LastModifiedOnDate,
                })
                .ToList();
            return l.Return(result, "found");
        }
        catch (Exception ex)
        {
            l.Ex(ex);
            return l.Return(new List<UserRoleModel>(), "error");
        }
    }
}