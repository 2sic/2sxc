using ToSic.Eav.Run.Basic;
using ToSic.Sxc.Context;

namespace ToSic.Sxc.Mvc.Run
{
    public class MvcUser: BasicUser, ICmsUser
    {
        public new string IdentityToken => "mvcuser:1";
    }
}
