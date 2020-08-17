using ToSic.Eav.Run;

namespace ToSic.Sxc.Dnn.Web.ClientInfos
{
    public class ClientInfosUser
    {
        public bool CanDesign;
        public bool CanDevelop;

        public ClientInfosUser(IUser user)
        {
            CanDesign = user.IsDesigner;
            CanDevelop = user.IsSuperUser;
        }
    }
}
