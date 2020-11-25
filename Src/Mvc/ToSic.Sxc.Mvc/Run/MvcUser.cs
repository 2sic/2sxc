using ToSic.Eav.Run;

namespace ToSic.Sxc.Mvc.Run
{
    public class MvcUser: UnknownUser
    {
        public new string IdentityToken => "mvcuser:1";
    }
}
