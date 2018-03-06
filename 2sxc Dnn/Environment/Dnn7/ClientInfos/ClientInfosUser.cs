using DotNetNuke.Entities.Users;
using ToSic.SexyContent.Dnn;
using ToSic.SexyContent.Internal;

namespace ToSic.SexyContent.Environment.Dnn7
{
    public class ClientInfosUser
    {
        public bool CanDesign;
        public bool CanDevelop;

        public ClientInfosUser(UserInfo uinfo)
        {
            CanDesign = SecurityHelpers.IsInSexyContentDesignersGroup(uinfo);
            CanDevelop = uinfo.IsSuperUser;
        }
    }
}
