using Oqtane.Repository;
using Oqtane.Shared;
using System;
using ToSic.Sxc.DataSources.Internal;
using ToSic.Sxc.Models.Internal;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.DataSources;

/// <summary>
/// Deliver a list of roles from the Oqtane
/// </summary>
internal class OqtRolesDsProvider(IRoleRepository roles, SiteState siteState)
    : RolesDataSourceProvider("Oqt.Roles", connect: [roles, siteState])
{
    [PrivateApi]
    public override IEnumerable<UserRoleRaw> GetRolesInternal()
    {
        var l = Log.Fn<IEnumerable<UserRoleRaw>>();
        var siteId = siteState.Alias.SiteId;
        l.A($"Portal Id {siteId}");
        try
        {
            var roles1 = roles.GetRoles(siteId, includeGlobalRoles: true).ToList();
            if (!roles1.Any())
                return l.Return(new List<UserRoleRaw>(), "null/empty");

            var result = roles1
                .Select(r => new UserRoleRaw
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
            return l.Return(new List<UserRoleRaw>(), "error");
        }
    }
}