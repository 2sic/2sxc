using ToSic.Eav.Context;

namespace ToSic.Sxc.Web.JsContext
{
    public class JsContextUser
    {
        // 2022-02-23 2dm remove this, believe it's not used any more
        //public bool CanDesign { get; }
        public bool CanDevelop { get; }

        public bool CanAdmin { get; }

        public JsContextUser(IUser user, bool? overrideDesign = null)
        {
            CanAdmin = user.IsAdmin;
            // 2022-02-23 2dm remove this, believe it's not used any more
            //CanDesign = overrideDesign ?? user.IsDesigner;
            CanDevelop = user.IsSuperUser;
        }
    }
}
