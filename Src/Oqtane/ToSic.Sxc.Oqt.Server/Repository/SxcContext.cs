using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Oqtane.Modules;
using Oqtane.Repository;

namespace ToSic.Sxc.Oqt.Server.Repository
{
    public class SxcContext : DBContextBase, IService
    {
        public virtual DbSet<Shared.Models.Sxc> Sxc { get; set; }

        public SxcContext(ITenantResolver tenantResolver, IHttpContextAccessor accessor) : base(tenantResolver, accessor)
        {
            // ContextBase handles multi-tenant database connections
        }
    }
}
