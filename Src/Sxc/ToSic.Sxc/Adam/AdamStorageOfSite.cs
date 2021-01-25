namespace ToSic.Sxc.Adam
{
    /// <summary>
    /// A container for the tenant (top level)
    /// For browsing the tenants content files
    /// </summary>
    public class AdamOfSite<TFolderId, TFileId>: AdamStorage<TFolderId, TFileId>
    {
        public AdamOfSite(AdamManager<TFolderId, TFileId> manager) : base(manager)
        {
        }

        protected override string GeneratePath(string subFolder) => (subFolder ?? "").Replace("//", "/");


    }
}