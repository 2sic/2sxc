using ToSic.Sys.Users;
using ToSic.Sys.Utils;

namespace ToSic.Sxc.Render.Polymorphism.Sys;

/// <summary>
/// Polymorphism resolver for different editions based on user permissions.
/// </summary>
/// <param name="user"></param>
[InternalApi_DoNotUse_MayChangeWithoutNotice]
public class PolymorphismPermissions(IUser user) : IPolymorphismResolver
{
    public static string ResolverNameId = "permissions";

    /// <summary>
    /// BTW: when this is configured, the entire config string is "Permissions?IsSuperUser"
    /// so the parameters are "IsSuperUser"
    /// </summary>
    private const string ModeIsSuperUser = "IsSuperUser";

    public string? Edition(PolymorphismConfigurationModel config, string? overrule, ILog log)
    {
        var l = log.Fn<string>();
        
        // Verify that it's the mode we plan to process
        if (!config.Parameters.EqualsInsensitive(ModeIsSuperUser))
            return l.ReturnNull("unknown param");
        
        // Overrules should only be applied if it's a superuser or the user is whitelisted.
        var isSuper = user.IsSystemAdmin;
        if (overrule.HasValue() && (isSuper || config.UsersWhoMaySwitch.Contains(user.Id))) 
            return l.Return(overrule, $"overruled as: '{overrule}'");

        // Super users should default to `staging`, normal users to `live`.
        var result = isSuper ? "staging" : "live";
        return l.Return(result, $"defaulted as: '{result}'; {(isSuper ? "superuser" : "normal user")}");
    }
}