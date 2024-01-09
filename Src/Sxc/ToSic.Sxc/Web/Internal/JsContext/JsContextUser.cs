using ToSic.Eav.Context;

namespace ToSic.Sxc.Web.JsContext;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class JsContextUser
{
    public bool CanDevelop { get; }

    public bool CanAdmin { get; }

    public JsContextUser(IUser user)
    {
        CanAdmin = user.IsSiteAdmin;
        CanDevelop = user.IsSystemAdmin;
    }
}