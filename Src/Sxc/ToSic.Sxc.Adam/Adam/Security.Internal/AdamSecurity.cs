using ToSic.Eav.Identity;

namespace ToSic.Sxc.Adam.Security.Internal;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class AdamSecurity
{
    public static bool PathIsInItemAdam(Guid guid, string? field, string? path)
    {
        if (string.IsNullOrWhiteSpace(field) || string.IsNullOrWhiteSpace(path))
            return false;

        var shortGuid = guid.GuidCompress();
        // will do check, case-sensitive because the compressed guid is case-sensitive
        return path!.Replace('\\', '/').Contains(shortGuid + "/" + field);
    }
}