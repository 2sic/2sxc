using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Security;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using ToSic.Eav.Documentation;
using ToSic.Eav.Run;

namespace ToSic.Sxc.Dnn.Run
{
    [PrivateApi("should probably be changed once we have IUser<T>")]
    public class DnnUser: IUser<UserInfo>
    {
        public DnnUser(UserInfo user = null)
        {
            UnwrappedContents = user ?? PortalSettings.Current?.UserInfo;
        }

        private static string GetUserIdentityToken ()
        {
            var userId = PortalSettings.Current?.UserId;
            var token = (userId ?? -1) == -1 ? "anonymous" : "dnn:userid=" + userId;
            return token;
        }

        public Guid? Guid => Membership.GetUser()?.ProviderUserKey as Guid?;

        public string IdentityToken => GetUserIdentityToken();

        public List<int> Roles => _roles ?? (_roles = BuildRoleList());
        public bool IsSuperUser => UnwrappedContents?.IsSuperUser ?? false;

        public bool IsAdmin => UnwrappedContents?.IsInRole(PortalSettings.Current?.AdministratorRoleName ?? "dummyrolename-if-no-portal") ?? false;
        public bool IsDesigner => UnwrappedContents?.IsInRole(Settings.SexyContentGroupName) ?? false;

        private static List<int> BuildRoleList()
        {
            var psCurrent = PortalSettings.Current;
            if (psCurrent == null) return new List<int>();

            var portalId = psCurrent.PortalId;
            var user = psCurrent.UserInfo;
            if (user == null) return new List<int>();

            var rc = new DotNetNuke.Security.Roles.RoleController();
            return user.Roles
                .Select(r => rc.GetRoleByName(portalId, r))
                .Where(r => r != null)
                .Select(r => r.RoleID)
                .ToList();
        }

        private List<int> _roles;

        public UserInfo UnwrappedContents { get; }
    }
}