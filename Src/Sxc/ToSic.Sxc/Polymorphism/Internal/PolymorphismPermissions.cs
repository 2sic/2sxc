using ToSic.Eav.Context;
using ToSic.Eav.Plumbing;
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

    public string Edition(PolymorphismConfiguration config, string overrule, ILog log)
    {
        var l = log.Fn<string>();
        if (!string.Equals(config.Parameters, ModeIsSuperUser, InvariantCultureIgnoreCase))
            return l.ReturnNull("unknown param");
        var isSuper = user.IsSystemAdmin;

        // TEMP: for now, site admins can overrule this
        // They won't see the button, but if the button is added on purpose using .Button("edition") it will work.
        if (overrule.HasValue() && (isSuper || config.UsersWhoMaySwitch.Contains(user.Id))) 
            return l.Return(overrule, $"overruled as: '{overrule}'");

        var result = isSuper ? "staging" : "live";
        return l.ReturnAndLog(result);
    }

    public bool IsViable() => true;

    public int Priority => 0;
}