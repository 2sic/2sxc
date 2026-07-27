using Oqtane.Repository;
using Oqtane.Shared;
using ToSic.Sxc.Cms.Users.Sys;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.DataSources;

/// <summary>
/// Deliver a list of roles from the Oqtane
/// </summary>
internal class OqtRolesDsProvider(IRoleRepository roles, SiteState siteState)
    : ServiceBase("Oqt.Roles", connect: [roles, siteState]),
        IUserRolesProvider
{
    [PrivateApi]
    public IEnumerable<UserRoleModelRaw> GetRoles()
    {
        var l = Log.Fn<IEnumerable<UserRoleModelRaw>>();
        var siteId = siteState.Alias.SiteId;
        l.A($"Portal Id {siteId}");
        try
        {
            var roles1 = roles.GetRoles(siteId, includeGlobalRoles: true).ToList();
            if (!roles1.Any())
                return l.Return(new List<UserRoleModelRaw>(), "null/empty");

            var result = roles1
                .Select(r => new UserRoleModelRaw
                {
                    Id = r.RoleId,
                    // Guid = r.
                    Name = r.Name,
                    Created = r.CreatedOn,
                    Modified = r.ModifiedOn,
                })
                .ToList();
            return l.Return(result, "found");
        }
        catch (Exception ex)
        {
            l.Ex(ex);
            return l.Return(new List<UserRoleModelRaw>(), "error");
        }
    }
}