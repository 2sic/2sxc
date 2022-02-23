using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Security;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using ToSic.Eav.Context;
using ToSic.Eav.Documentation;
using ToSic.Sxc.Dnn.Run;

namespace ToSic.Sxc.Dnn.Context
{
    [PrivateApi("this is just internal, external users don't really have anything to do with this")]
    public class DnnUser: IUser<UserInfo>
    {
        public DnnUser() { }

        private string GetUserIdentityToken ()
        {
            var userId = Id;
            var token = userId == -1 ? "anonymous" : "dnn:userid=" + userId;
            return token;
        }

        public Guid? Guid => Membership.GetUser()?.ProviderUserKey as Guid?;

        public string IdentityToken => GetUserIdentityToken();

        public List<int> Roles => _roles ?? (_roles = BuildRoleList());
        private List<int> _roles;

        public bool IsSuperUser => UnwrappedContents?.IsSuperUser ?? false;

        public bool IsAdmin => _isAdmin ?? (_isAdmin = UnwrappedContents?.UserMayAdminThis() ?? false).Value;
        private bool? _isAdmin;

        public bool IsDesigner => IsSuperUser;

        public UserInfo UnwrappedContents => _user ?? (_user = PortalSettings.Current?.UserInfo);
        private UserInfo _user;
        public UserInfo GetContents() => UnwrappedContents;

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

        public int Id => UnwrappedContents?.UserID ?? -1;

        public bool IsAnonymous => Id == -1;
    }
}