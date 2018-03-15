using System;
using System.Web.Security;
using DotNetNuke.Entities.Portals;
using ToSic.Eav.Apps.Interfaces;
using ToSic.Eav.Interfaces;

namespace ToSic.SexyContent.Environment.Dnn7
{
    public class DnnUser: IUser
    {
        private static string GetUserIdentityToken ()
        {
            var userId = PortalSettings.Current?.UserId;
            var token = (userId ?? -1) == -1 ? "anonymous" : "dnn:userid=" + userId;
            return token;
        }

        public Guid? Guid => Membership.GetUser()?.ProviderUserKey as Guid?;

        public string IdentityToken => GetUserIdentityToken();
    }
}