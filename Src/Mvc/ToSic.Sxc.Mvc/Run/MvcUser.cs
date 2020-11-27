using ToSic.Eav.Context;
using ToSic.Eav.Run;

namespace ToSic.Sxc.Mvc.Run
{
    public class MvcUser: UnknownUser, IUserLight
    {
        public new string IdentityToken => "mvcuser:1";
    }
}
