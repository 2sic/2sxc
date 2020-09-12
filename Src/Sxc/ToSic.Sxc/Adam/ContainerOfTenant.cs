namespace ToSic.Sxc.Adam
{
    /// <summary>
    /// A container for the tenant (top level)
    /// For browsing the tenants content files
    /// </summary>
    public class AdamOfTenant<TFolderId, TFileId>: AdamOfBase<TFolderId, TFileId>
    {
        public AdamOfTenant(AdamAppContext<TFolderId, TFileId> appContext) : base(appContext)
        {
        }

        protected override string GeneratePath(string subFolder) => (subFolder ?? "").Replace("//", "/");


    }
}