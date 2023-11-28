using Microsoft.AspNetCore.Http;
using Oqtane.Models;
using Oqtane.Repository;
using Oqtane.Shared;
using System;
using System.Linq;
using ToSic.Lib.DI;
using ToSic.Lib.Services;
using ToSic.Sxc.Oqt.Shared;

namespace ToSic.Sxc.Oqt.Server.Plumbing;

public class SiteStateInitializer: ServiceBase
{
    public LazySvc<SiteState> SiteStateLazy { get; }
    public LazySvc<Oqtane.Infrastructure.SiteState> SiteState2Lazy { get; }
    public IHttpContextAccessor HttpContextAccessor { get; }
    public LazySvc<IAliasRepository> AliasRepositoryLazy { get; }

    public SiteStateInitializer(LazySvc<SiteState> siteStateLazy, LazySvc<Oqtane.Infrastructure.SiteState> siteState2Lazy, IHttpContextAccessor httpContextAccessor,
        LazySvc<IAliasRepository> aliasRepositoryLazy): base($"{OqtConstants.OqtLogPrefix}.SSInit")
    {
        ConnectServices(
            SiteStateLazy = siteStateLazy,
            SiteState2Lazy = siteState2Lazy,
            HttpContextAccessor = httpContextAccessor,
            AliasRepositoryLazy = aliasRepositoryLazy
        );
    }

    /// <summary>
    /// Use this from inner code, which must always have an initialized state.
    /// Usually this has been ensured at the very top - when razor starts or when WebApi start
    /// </summary>
    public SiteState InitializedState
    {
        get
        {
            if (SiteStateLazy.Value?.Alias != null && SiteState2Lazy.Value?.Alias != null) return SiteStateLazy.Value;
            InitIfEmpty();
            return SiteStateLazy.Value;
        }
    }

    /// <summary>
    /// Will initialize the SiteState if it has not been initialized yet
    /// </summary>
    /// <returns></returns>
    internal bool InitIfEmpty(int? siteId = null)
    {
        var siteState = SiteStateLazy.Value;
        var siteState2 = SiteState2Lazy.Value; // In Oqtane 3.1 SiteState is preserved in 2 places.

        // This would indicate it was called improperly, because we need the shared SiteState variable to work properly
        if (siteState == null) throw new ArgumentNullException(nameof(siteState));

        // This would indicate it was called improperly, because we need the shared SiteState variable to work properly
        if (siteState2 == null) throw new ArgumentNullException(nameof(siteState2));

        // Check if alias already set and if is for provided siteId (if provided), in which case we skip this
        if (siteState.Alias != null && siteState2.Alias != null
                                    && (!siteId.HasValue || siteId.Value == siteState?.Alias?.SiteId || siteId.Value == siteState2?.Alias?.SiteId)) return true;

        // For anything else we need the httpContext, otherwise skip
        var request = HttpContextAccessor?.HttpContext?.Request;
        if (request == null) return false;

        // Try HACK
        if ((HttpContextAccessor?.HttpContext?.Items.TryGetValue("AliasFor2sxc", out var alias2sxc) ?? false) && alias2sxc != null)
        {
            siteState.Alias ??= (Alias)alias2sxc;
            siteState2.Alias ??= (Alias)alias2sxc;
            return false;
        }

        // Oqtane cache on request
        if ((HttpContextAccessor?.HttpContext?.Items.TryGetValue(Oqtane.Shared.Constants.HttpContextAliasKey, out var alias) ?? false) && alias != null)
        {
            siteState.Alias ??= (Alias)alias;
            siteState2.Alias ??= (Alias)alias;
            return false;
        }

        // Try get alias with info for HttpRequest and eventual SiteId.
        if (siteId.HasValue || (request.Path.HasValue && request.Path.Value != null && request.Path.Value.Contains("/_blazor")))
        {
            var url = $"{request.Host}";

            var aliases = AliasRepositoryLazy.Value.GetAliases().ToList(); // cached by Oqtane

            if (siteId.HasValue) // acceptable solution
                siteState.Alias = aliases.OrderByDescending(a => /*a.IsDefault*/  a.Name.Length) // TODO: a.IsDefault DESC after upgrade to Oqt v3.0.3+
                    //.ThenByDescending(a => a.Name.Length)
                    .ThenBy(a => a.Name)
                    .FirstOrDefault(a => a.SiteId == siteId.Value && a.Name.StartsWith(url, StringComparison.InvariantCultureIgnoreCase));
            else // fallback solution, wrong site is possible
                siteState.Alias = aliases.OrderByDescending(a => a.Name)
                    .FirstOrDefault(a => a.Name.StartsWith(url, StringComparison.InvariantCultureIgnoreCase));
        }
        else // great solution
        {
            var url = $"{request.Host}{request.Path}";

            var aliases = AliasRepositoryLazy.Value.GetAliases().ToList(); // cached by Oqtane
            siteState.Alias = aliases.OrderByDescending(a => a.Name.Length)
                .ThenBy(a => a.Name)
                .FirstOrDefault(a => url.StartsWith(a.Name, StringComparison.InvariantCultureIgnoreCase));
        }

        siteState2.Alias ??= siteState.Alias;

        return siteState.Alias != null;
    }

}