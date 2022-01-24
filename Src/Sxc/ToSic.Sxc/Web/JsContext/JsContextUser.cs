using ToSic.Eav.Context;

namespace ToSic.Sxc.Web.JsContext
{
    public class JsContextUser
    {
        public bool CanDesign { get; }
        public bool CanDevelop { get; }

        public bool CanAdmin { get; }

        public JsContextUser(IUser user, bool? overrideDesign = null)
        {
            CanAdmin = user.IsAdmin;
            CanDesign = overrideDesign ?? user.IsDesigner;
            CanDevelop = user.IsSuperUser;
        }
    }
}
