using ToSic.Eav.Context;
using static System.StringComparison;

namespace ToSic.Sxc.Polymorphism.Internal;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class PolymorphismPermissions(IUser user) : IPolymorphismResolver
{
    public string NameId => "Permissions";

    /// <summary>
    /// BTW: when this is configured, the entire config string is "Permissions?IsSuperUser"
    /// so the parameters are "IsSuperUser"
    /// </summary>
    public const string ModeIsSuperUser = "IsSuperUser";

    public string Edition(string parameters, ILog log)
    {
        var l = log.Fn<string>();
        if (!string.Equals(parameters, ModeIsSuperUser, InvariantCultureIgnoreCase))
            return l.ReturnNull("unknown param");
        var isSuper = user.IsSystemAdmin;
        var result = isSuper ? "staging" : "live";
        return l.ReturnAndLog(result);
    }

    public bool IsViable() => true;

    public int Priority => 0;
}