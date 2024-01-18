using ToSic.Eav.Context;

namespace ToSic.Sxc.Web.Internal.JsContext;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class JsContextUser(IUser user)
{
    public bool CanDevelop { get; } = user.IsSystemAdmin;

    public bool CanAdmin { get; } = user.IsSiteAdmin;
}