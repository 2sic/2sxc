using Oqtane.Repository;
using Oqtane.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Lib.Documentation;
using ToSic.Lib.Logging;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.DataSources
{
    /// <summary>
    /// Deliver a list of roles from the Oqtane
    /// </summary>
    public class OqtRolesDsProvider : RolesDataSourceProvider
    {
        private readonly IRoleRepository _roles;
        private readonly SiteState _siteState;

        public OqtRolesDsProvider(IRoleRepository roles, SiteState siteState): base("Oqt.Roles")
        {
            ConnectServices(
                _roles = roles,
                _siteState = siteState
            );
        }

        [PrivateApi]
        public override IEnumerable<CmsRoleInfo> GetRolesInternal() => Log.Func(l =>
        {
            var siteId = _siteState.Alias.SiteId;
            l.A($"Portal Id {siteId}");
            try
            {
                var roles = _roles.GetRoles(siteId, includeGlobalRoles: true).ToList();
                if (!roles.Any()) return (new(), "null/empty");

                var result = roles
                    .Select(r => new CmsRoleInfo
                    {
                        Id = r.RoleId,
                        // Guid = r.
                        Name = r.Name,
                        Created = r.CreatedOn,
                        Modified = r.ModifiedOn,
                    })
                    .ToList();
                return (result, "found");
            }
            catch (Exception ex)
            {
                l.Ex(ex);
                return (new(), "error");
            }
        });
    }
}