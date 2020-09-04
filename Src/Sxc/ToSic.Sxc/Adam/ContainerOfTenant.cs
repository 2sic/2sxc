namespace ToSic.Sxc.Adam
{
    /// <summary>
    /// A container for the tenant (top level)
    /// For browsing the tenants content files
    /// </summary>
    public class ContainerOfTenant: ContainerBase
    {
        public ContainerOfTenant(AdamAppContext appContext) : base(appContext)
        {
        }

        protected override string GeneratePath(string subFolder) => (subFolder ?? "").Replace("//", "/");


    }
}