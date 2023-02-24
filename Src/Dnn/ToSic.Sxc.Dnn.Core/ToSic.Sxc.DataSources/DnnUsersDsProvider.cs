using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Lib.DI;
using ToSic.Lib.Logging;
using ToSic.Sxc.Context.Raw;
using ToSic.Sxc.Dnn.Run;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.DataSources
{

    public class DnnUsersDsProvider : UsersDataSourceProvider
    {
        private readonly LazySvc<DnnSecurity> _dnnSecurity;

        public DnnUsersDsProvider(LazySvc<DnnSecurity> dnnSecurity) : base("Dnn.Users")
        {
            ConnectServices(
                _dnnSecurity = dnnSecurity
            );
        }

        public override IEnumerable<CmsUserNew> GetUsersInternal() => Log.Func(l =>
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
                    if (!dnnUsers.Any()) return (new List<CmsUserNew>(), "null/empty");

                    var result = dnnUsers
                        //.Where(user => !user.IsDeleted)
                        .Select(u => _dnnSecurity.Value.CmsUserBuilder(u, siteId)).ToList();
                    return (result, "found");
                }
                catch (Exception ex)
                {
                    l.Ex(ex);
                    return (new List<CmsUserNew>(), "error");
                }
            });
    }
}