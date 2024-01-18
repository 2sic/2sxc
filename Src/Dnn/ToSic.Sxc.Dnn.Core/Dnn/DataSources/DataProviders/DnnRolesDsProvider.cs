using DotNetNuke.Entities.Portals;
using DotNetNuke.Security.Roles;
using ToSic.Sxc.DataSources.Internal;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.DataSources;

/// <summary>
/// Deliver a list of roles from the Dnn.
/// </summary>
internal class DnnRolesDsProvider : RolesDataSourceProvider
{
    public DnnRolesDsProvider() : base("Dnn.Roles")
    { }

    [PrivateApi]
    public override IEnumerable<RoleDataRaw> GetRolesInternal()
    {
        var l = Log.Fn<IEnumerable<RoleDataRaw>>();
        var siteId = PortalSettings.Current?.PortalId ?? -1;
        l.A($"Portal Id {siteId}");
        try
        {
            var dnnRoles = RoleController.Instance.GetRoles(portalId: siteId);
            if (!dnnRoles.Any()) return l.Return(new List<RoleDataRaw>(), "null/empty");

            var result = dnnRoles
                .Select(r => new RoleDataRaw
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
            return l.Return(new List<RoleDataRaw>(), "error");
        }
    }
}