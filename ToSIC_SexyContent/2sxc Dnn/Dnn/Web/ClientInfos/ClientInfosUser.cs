using DotNetNuke.Entities.Users;
using ToSic.Sxc.Dnn.Run;

namespace ToSic.Sxc.Dnn.Web.ClientInfos
{
    public class ClientInfosUser
    {
        public bool CanDesign;
        public bool CanDevelop;

        public ClientInfosUser(UserInfo user)
        {
            CanDesign = DnnSecurity.IsInSexyContentDesignersGroup(user);
            CanDevelop = user.IsSuperUser;
        }
    }
}
