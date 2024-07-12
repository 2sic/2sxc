using Microsoft.AspNetCore.Http;
using Oqtane.Infrastructure;
using Oqtane.Models;
using Oqtane.Repository;
using Oqtane.Shared;
using System;
using System.Linq;
using ToSic.Lib.DI;
using ToSic.Lib.Logging;
using ToSic.Lib.Services;
using ToSic.Sxc.Oqt.Shared;

namespace ToSic.Sxc.Oqt.Server.Plumbing;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class AliasResolver(
    SiteState siteState,
    IHttpContextAccessor httpContextAccessor,
    LazySvc<IAliasRepository> aliasRepository,
    LazySvc<ITenantManager> tenantManager)
    : ServiceBase($"{OqtConstants.OqtLogPrefix}.SSInit",
        connect: [])
{
    public const string AliasFor2Sxc = "AliasFor2sxc";

    /// <summary>
    /// Use this from inner code, which must always have an initialized state.
    /// Usually this has been ensured at the very top - when razor starts or when WebApi start
    /// </summary>
    public Alias Alias
    {
        get
        {
            if (siteState.Alias != null && siteState.Alias.AliasId != -1) return siteState.Alias;
            InitIfEmpty();
            return siteState.Alias;
        }
    }

    /// <summary>
    /// Will initialize the SiteState if it has not been initialized yet
    /// </summary>
    /// <returns></returns>
    internal bool InitIfEmpty(int? siteId = null)
    {
        var l = Log.Fn<bool>($"{nameof(siteId)}:{siteId}");

        // This would indicate it was called improperly, because we need the shared SiteState variable to work properly
        if (siteState == null) throw l.Ex(new ArgumentNullException(nameof(siteState)));

        // Check if alias already set and if is for provided siteId (if provided), in which case we skip this
        if (siteState.Alias != null && (!siteId.HasValue || siteId.Value == siteState.Alias.SiteId))
            return l.ReturnTrue($"siteState.Alias:'{siteState.Alias?.Name}'");

        // For anything else we need the httpContext, otherwise skip
        var context = httpContextAccessor?.HttpContext;
        var items = context?.Items;

        // Try 2sxc HACK
        if ((items?.TryGetValue(AliasFor2Sxc, out var alias2Sxc) ?? false) && alias2Sxc != null)
        {
            siteState.Alias ??= (Alias)alias2Sxc;
            return l.ReturnFalse($"from AliasFor2Sxc (siteState.Alias:'{siteState.Alias?.Name}')");
        }

        var request = context?.Request;
        if (request == null) return l.ReturnFalse("request is NULL");

        // Try to get alias with info for HttpRequest and eventual SiteId.
        if (siteId.HasValue || (request.Path is { HasValue: true, Value: not null } /*&& request.Path.Value.Contains("/_blazor")*/))
        {
            var url = $"{request.Host}";
            l.A($"url:{url}");

            var aliases = aliasRepository.Value.GetAliases().ToList(); // cached by Oqtane
            l.A($"aliases:{aliases.Count}");

            if (siteId.HasValue) // acceptable solution
            {
                siteState.Alias = aliases
                    .OrderByDescending(a => /*a.IsDefault*/ a.Name.Length) // TODO: a.IsDefault DESC after upgrade to Oqt v3.0.3+
                                                                           //.ThenByDescending(a => a.Name.Length)
                    .ThenBy(a => a.Name)
                    .FirstOrDefault(a =>
                        a.SiteId == siteId.Value &&
                        a.Name.StartsWith(url, StringComparison.InvariantCultureIgnoreCase));
                l.A($"siteId:{siteId} => siteState.Alias:'{siteState.Alias?.Name}'");
            }
            else // fallback solution, wrong site is possible
            {
                siteState.Alias = tenantManager.Value.GetAlias(); // get alias (note that this also sets SiteState.Alias)
                l.A($"siteId is NULL => siteState.Alias:'{siteState.Alias?.Name}'");
            }
        }
        else // great solution
        {
            var url = $"{request.Host}{request.Path}";
            l.A($"url:{url}");
            var aliases = aliasRepository.Value.GetAliases().ToList(); // cached by Oqtane
            siteState.Alias = aliases.OrderByDescending(a => a.Name.Length)
                .ThenBy(a => a.Name)
                .FirstOrDefault(a => url.StartsWith(a.Name, StringComparison.InvariantCultureIgnoreCase));
            l.A($"siteId is NULL and path is NULL => siteState.Alias:'{siteState.Alias?.Name}'");
        }

        //UpdateHttpContextItem(Constants.HttpContextAliasKey, siteState.Alias);
        //UpdateHttpContextItem(AliasFor2Sxc, siteState.Alias);

        return l.Return(siteState.Alias != null, $"resolved to siteState.Alias:'{siteState.Alias?.Name}'");
    }

    private void UpdateHttpContextItem(string key, Alias alias)
    {
        if (httpContextAccessor?.HttpContext != null && alias != null)
            httpContextAccessor.HttpContext.Items[key] ??= alias;
    }
}