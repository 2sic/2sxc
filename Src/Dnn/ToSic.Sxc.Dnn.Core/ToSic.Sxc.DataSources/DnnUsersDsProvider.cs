using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Lib.Logging;
using ToSic.Sxc.Context.Raw;
using ToSic.Sxc.Dnn.Run;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.DataSources
{

    public class DnnUsersDsProvider : UsersDataSourceProvider
    {
        public DnnUsersDsProvider() : base("Dnn.Users")
        { }

        public override IEnumerable<CmsUserRaw> GetUsersInternal() => Log.Func(l =>
            {
                var siteId = PortalSettings.Current?.PortalId ?? -1;
                l.A($"Portal Id {siteId}");
                try
                {
                    // take all portal users (this should include superusers, but superusers are missing)
                    var dnnAllUsers =
                        UserController.GetUsers(portalId: siteId, includeDeleted: false, superUsersOnly: false);

                    // append all superusers
                    dnnAllUsers.AddRange(UserController.GetUsers(portalId: -1, includeDeleted: false,
                        superUsersOnly: true));

                    var dnnUsers = dnnAllUsers.Cast<UserInfo>().ToList();
                    if (!dnnUsers.Any()) return (new List<CmsUserRaw>(), "null/empty");

                    var result = dnnUsers
                        //.Where(user => !user.IsDeleted)
                        .Select(u => u.CmsUserBuilder(siteId)).ToList();
                    return (result, "found");
                }
                catch (Exception ex)
                {
                    l.Ex(ex);
                    return (new List<CmsUserRaw>(), "error");
                }
            });
    }
}