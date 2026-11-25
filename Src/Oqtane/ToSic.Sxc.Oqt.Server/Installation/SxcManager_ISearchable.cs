using Microsoft.Extensions.DependencyInjection;
using Oqtane.Infrastructure;
using Oqtane.Interfaces;
using Oqtane.Models;
using ToSic.Sxc.Oqt.Server.Search;
using ToSic.Sxc.Oqt.Server.Context;

namespace ToSic.Sxc.Oqt.Server.Installation;

public partial class SxcManager : ISearchable
{
    /// <summary>Provides 2sxc content to Oqtane's background search indexer.</summary>
    /// <remarks>
    /// The native job has already selected the tenant and hydrated the page/module model.
    /// Returns modified entities and explicit deletion records without retaining index state.
    /// Oqtane saves the batch after this callback returns, without a write acknowledgement.
    /// Items no longer enumerated cannot be deleted here; a site reset and rebuild clears
    /// those stale results. See Search/README.md for retry and reactivation limitations.
    /// </remarks>
    public async Task<List<SearchContent>> GetSearchContentsAsync(PageModule pageModule, DateTime lastIndexedOn)
    {
        // SearchIndexJob selects the alias by tenant and site before loading the page
        // modules. Pass that alias directly so the child scope needs no request lookup.
        var alias = siteState.Alias;

        // The caller indexes multiple sites/modules in one scope. A fresh scope owns all
        // 2sxc context and caches; initialize its tenant before resolving any 2sxc services.
        // Keep the tenant marker alive until the child services have been disposed.
        using var tenantScope = new OqtBackgroundTenantScope(alias.TenantId);
        await using var scope = serviceScopeFactory.CreateAsyncScope();
        scope.GetSysServiceProvider().GetRequiredService<ITenantManager>().SetAlias(alias);

        return scope.GetSysServiceProvider().GetRequiredService<OqtSearchController>()
            .GetSearchContents(pageModule, lastIndexedOn);
    }
}
