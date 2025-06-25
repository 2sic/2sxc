using Oqtane.Repository;
using Oqtane.Shared;
using ToSic.Lib.Services;
using ToSic.Sxc.Cms.Users.Sys;
using ToSic.Sxc.DataSources.Internal;

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
    public IEnumerable<UserRoleModel> GetRoles()
    {
        var l = Log.Fn<IEnumerable<UserRoleModel>>();
        var siteId = siteState.Alias.SiteId;
        l.A($"Portal Id {siteId}");
        try
        {
            var roles1 = roles.GetRoles(siteId, includeGlobalRoles: true).ToList();
            if (!roles1.Any())
                return l.Return(new List<UserRoleModel>(), "null/empty");

            var result = roles1
                .Select(r => new UserRoleModel
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
            return l.Return(new List<UserRoleModel>(), "error");
        }
    }
}