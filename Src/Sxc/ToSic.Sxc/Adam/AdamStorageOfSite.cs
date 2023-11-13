using System.ComponentModel;

namespace ToSic.Sxc.Adam
{
    /// <summary>
    /// A container for the tenant (top level)
    /// For browsing the tenants content files
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class AdamStorageOfSite<TFolderId, TFileId>: AdamStorage<TFolderId, TFileId>
    {

        protected override string GeneratePath(string subFolder) => (subFolder ?? "").Replace("//", "/");

    }
}