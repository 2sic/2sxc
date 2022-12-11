using ToSic.Eav.Context;

namespace ToSic.Sxc.Web.JsContext
{
    public class JsContextUser
    {
        public bool CanDevelop { get; }

        public bool CanAdmin { get; }

        public JsContextUser(IUser user, bool? overrideDesign = null)
        {
            CanAdmin = user.IsSiteAdmin;
            CanDevelop = user.IsSystemAdmin;
        }
    }
}
