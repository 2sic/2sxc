using ToSic.Eav.Context;
using ToSic.Eav.Run;
using ToSic.Sxc.Context;

namespace ToSic.Sxc.Mvc.Run
{
    public class MvcUser: UnknownUser, ICmsUser
    {
        public new string IdentityToken => "mvcuser:1";
    }
}
