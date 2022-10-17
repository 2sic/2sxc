using DotNetNuke.Entities.Portals;
using DotNetNuke.Security.Roles;
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
        Difficulty = DifficultyBeta.Default
    )]
    public class Roles : CmsBases.RolesDataSourceBase
    {
        protected override IEnumerable<RoleDataSourceInfo> GetRolesInternal()
        {
            var wrapLog = Log.Fn<List<RoleDataSourceInfo>>();
            var siteId = PortalSettings.Current?.PortalId ?? -1;
            Log.A($"Portal Id {siteId}");
            try
            {
                var dnnRoles = RoleController.Instance.GetRoles(portalId: siteId);
                if (!dnnRoles.Any()) return wrapLog.Return(new List<RoleDataSourceInfo>(), "null/empty");

                var result = dnnRoles
                    .Select(r => new RoleDataSourceInfo
                    {
                        Id = r.RoleID,
                        Name = r.RoleName,
                        Created = r.CreatedOnDate,
                        Modified = r.LastModifiedOnDate,
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