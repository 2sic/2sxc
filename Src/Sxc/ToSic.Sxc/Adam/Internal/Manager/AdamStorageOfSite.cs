namespace ToSic.Sxc.Adam.Internal;

/// <summary>
/// A container for the tenant (top level)
/// For browsing the tenants content files
/// </summary>
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class AdamStorageOfSite<TFolderId, TFileId>: AdamStorage<TFolderId, TFileId>
{

    protected override string GeneratePath(string subFolder) => (subFolder ?? "").Replace("//", "/");

}