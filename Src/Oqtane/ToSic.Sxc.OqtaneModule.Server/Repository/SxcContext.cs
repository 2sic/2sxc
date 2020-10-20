using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Oqtane.Modules;
using Oqtane.Repository;
using ToSic.Sxc.OqtaneModule.Models;

namespace ToSic.Sxc.OqtaneModule.Repository
{
    public class SxcContext : DBContextBase, IService
    {
        public virtual DbSet<Models.Sxc> Sxc { get; set; }

        public SxcContext(ITenantResolver tenantResolver, IHttpContextAccessor accessor) : base(tenantResolver, accessor)
        {
            // ContextBase handles multi-tenant database connections
        }
    }
}
