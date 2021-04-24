using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Oqtane.Repository;
using Oqtane.Shared;

namespace ToSic.Sxc.Oqt.Server.Plumbing
{
    public class SiteStateInitializer
    {
        public SiteState SiteState { get; }
        public IHttpContextAccessor HttpContextAccessor { get; }
        public Lazy<IAliasRepository> AliasRepositoryLazy { get; }

        public SiteStateInitializer(SiteState siteState, IHttpContextAccessor httpContextAccessor,
            Lazy<IAliasRepository> aliasRepositoryLazy)
        {
            SiteState = siteState;
            HttpContextAccessor = httpContextAccessor;
            AliasRepositoryLazy = aliasRepositoryLazy;
        }
        
        /// <summary>
        /// Will initialize the SiteState if it has not been initialized yet
        /// </summary>
        /// <returns></returns>
        internal bool InitIfEmpty()
        {
            // This would indicate it was called improperly, because we need the shared SiteState variable to work properly
            if (SiteState == null) throw new ArgumentNullException(nameof(SiteState));

            // Check if alias already set, in which case we skip this
            if (SiteState.Alias != null) return true;

            // For anything else we need the httpContext, otherwise skip
            var request = HttpContextAccessor?.HttpContext?.Request;
            if (request == null) return false;
            var url = $"{request.Host}{request.Path}";

            var aliases = AliasRepositoryLazy.Value.GetAliases().ToList(); // cached by Oqtane
            SiteState.Alias = aliases.OrderByDescending(a => a.Name.Length)
                .ThenBy(a => a.Name)
                .FirstOrDefault(a => url.StartsWith(a.Name, StringComparison.InvariantCultureIgnoreCase));
            return SiteState.Alias != null;
        }

    }
}
