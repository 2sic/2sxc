using Oqtane.Repository;
using Oqtane.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.DataSources.Queries;
using ToSic.Eav.Documentation;
using ToSic.Eav.Logging;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.DataSources
{
    /// <summary>
    /// Deliver a list of roles from the current platform (Dnn or Oqtane)
    /// </summary>
    [InternalApi_DoNotUse_MayChangeWithoutNotice("Still BETA, many changes expected")]
    [VisualQuery(
        NiceName = VqNiceName,
        Icon = VqIcon,
        UiHint = VqUiHint,
        HelpLink = VqHelpLink,
        GlobalName = VqGlobalName,
        Type = VqType,
        ExpectsDataOfType = VqExpectsDataOfType,
        Difficulty = DifficultyBeta.Admin
    )]
    public class RolesDataSource : CmsBases.RolesDataSourceBase
    {
        private readonly IRoleRepository _roles;
        private readonly SiteState _siteState;

        public RolesDataSource(IRoleRepository roles, SiteState siteState)
        {
            _roles = roles;
            _siteState = siteState;
        }
        protected override IEnumerable<RoleDataSourceInfo> GetRolesInternal()
        {
            var wrapLog = Log.Fn<List<RoleDataSourceInfo>>();
            var siteId = _siteState.Alias.SiteId;
            Log.A($"Portal Id {siteId}");
            try
            {
                var roles = _roles.GetRoles(siteId, includeGlobalRoles: true).ToList();
                if (!roles.Any()) return wrapLog.Return(new List<RoleDataSourceInfo>(), "null/empty");

                var result = roles
                    .Select(r => new RoleDataSourceInfo
                    {
                        Id = r.RoleId,
                        Name = r.Name,
                        Created = r.CreatedOn,
                        Modified = r.ModifiedOn,
                    })
                    .ToList();
                return wrapLog.Return(result, "found");
            }
            catch (Exception ex)
            {
                Log.Ex(ex);
                return wrapLog.Return(new List<RoleDataSourceInfo>(), "error");
            }
        }
    }
}