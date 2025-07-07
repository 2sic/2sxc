using ToSic.Sxc.Cms.Users;
using ToSic.Sys.Utils;

namespace ToSic.Sxc.Edit.Toolbar.Sys.ToolbarBuilder;

internal class ToolbarConfigurationShowHelper
{
    public bool? OverrideShowBecauseOfRoles(ToolbarBuilderConfiguration config, IUserModel user)
    {
        if (config == null || user == null)
            return null;

        // 99% never have this configured
        if (config.ShowForRoles == null && config.ShowDenyRoles == null)
            return null;

        var enabledFor = config.ShowForRoles?.ToArray().TrimmedAndWithoutEmpty() ?? [];
        var denyFor = config.ShowDenyRoles?.ToArray().TrimmedAndWithoutEmpty() ?? [];
        var roles = user.Roles.Select(r => r.Name);

        // Check allow
        if (enabledFor.Any(eg => roles.Contains(eg, StringComparer.InvariantCultureIgnoreCase)))
            return true;

        // Check deny
        if (denyFor.Any(eg => roles.Contains(eg, StringComparer.InvariantCultureIgnoreCase)))
            return false;

        return null;
    }
}