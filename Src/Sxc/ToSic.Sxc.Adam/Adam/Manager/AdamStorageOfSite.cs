namespace ToSic.Sxc.Adam.Internal;

/// <summary>
/// A container for the tenant (top level)
/// For browsing the tenants content files
/// </summary>
[ShowApiWhenReleased(ShowApiMode.Never)]
public class AdamStorageOfSite(): AdamStorage("Adm.OfSite")
{

    protected override string GeneratePath(string subFolder) => (subFolder ?? "").Replace("//", "/");

}