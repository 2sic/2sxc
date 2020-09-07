namespace ToSic.Sxc.WebApi.Context
{
    public class TenantDto
    {
        public int Id;

        public TenantDto(int portalId)
        {
            Id = portalId;
        }
    }
}
