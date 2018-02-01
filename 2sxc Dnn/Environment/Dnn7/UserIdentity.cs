using DotNetNuke.Entities.Portals;
using ToSic.Eav.Apps.Interfaces;

namespace ToSic.SexyContent.Environment.Dnn7
{
    public class UserIdentity: IUser
    {
        public static string CurrentUserIdentityToken
        {
            get
            {
                var userId = PortalSettings.Current?.UserId;
                var token = ((userId ?? -1) == -1) ? "anonymous" : "dnn:userid=" + userId;
                return token;
            }
        }

        string IUser.CurrentUserIdentityToken => CurrentUserIdentityToken;
    }
}