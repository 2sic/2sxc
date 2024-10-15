using Microsoft.AspNetCore.Http;
using Oqtane.Infrastructure;
using Oqtane.Models;
using Oqtane.Repository;
using Oqtane.Shared;
using System;
using ToSic.Lib.DI;
using ToSic.Lib.Services;
using ToSic.Sxc.Oqt.Shared;

namespace ToSic.Sxc.Oqt.Server.Plumbing;

/// <summary>
/// AliasResolver will provide correct Alias
/// in addition of taking a care to set not mapped properties like BaseUrl, Protocol, etc.
/// It will use HttpContext to cache correct Alias for the request.
/// </summary>
/// <param name="siteState"></param>
/// <param name="httpContextAccessor"></param>
/// <param name="aliasAccessor"></param>
/// <param name="aliasRepository"></param>
/// <param name="tenantManager"></param>
/// <remarks>
/// in general to get Alias use AliasResolver, instead of working directly with IAliasRepository, 
/// to ensure that not mapped properties are also set
/// </remarks>
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class AliasResolver(
    SiteState siteState,
    IHttpContextAccessor httpContextAccessor,
    IAliasAccessor aliasAccessor,
    LazySvc<IAliasRepository> aliasRepository,
    LazySvc<ITenantManager> tenantManager)
    : ServiceBase($"{OqtConstants.OqtLogPrefix}.SSInit",
        connect: [])
{
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
    /// Alias has not mapped properties like BaseUrl, Protocol, etc. so we need to do that here
    /// </summary>
    /// <param name="aliasId"></param>
    /// <returns></returns>
    internal Alias GetAndStoreAlias(int aliasId)
    {
        var alias = aliasRepository.Value.GetAlias(aliasId);
        AddMissingProperties(alias, httpContextAccessor?.HttpContext?.Request);
        
        // Store Alias in SiteState for background processing.
        siteState.Alias = alias;
        UpdateHttpContextItem(Constants.HttpContextAliasKey, siteState.Alias);

        return siteState.Alias;
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

        // Check if alias already set and stored in HttpContext and if is for provided siteId (if provided), in which case we skip this
        if (aliasAccessor.Alias != null && (!siteId.HasValue || siteId.Value == aliasAccessor.Alias.SiteId))
        {
            siteState.Alias = aliasAccessor.Alias;
            return l.ReturnTrue($"Alias from IAliasAccessor:'{siteState.Alias?.Name}'");
        }

        // For anything else we need the HttpContext, otherwise skip
        var request = httpContextAccessor?.HttpContext?.Request;
        if (request == null) return l.ReturnFalse("request is NULL");

        // Find and build Alias
        siteState.Alias = FindAndBuildAlias(siteId, request);

        // Store it
        UpdateHttpContextItem(Constants.HttpContextAliasKey, siteState.Alias);

        return l.Return(siteState.Alias != null, $"resolved to siteState.Alias:'{siteState.Alias?.Name}'");
    }

    /// <summary>
    /// Because Site can have many Aliases, default one is not always the right one,
    /// so we try to find the right one based on the request.
    /// </summary>
    /// <param name="siteId"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    private Alias FindAndBuildAlias(int? siteId, HttpRequest request)
    {
        var l = Log.Fn<Alias>($"{nameof(siteId)}:{siteId}");

        Alias alias;

        if (request == null) return l.ReturnNull("request is NULL");

        // Try to get alias with info for HttpRequest and eventual SiteId.
        if (siteId.HasValue || (request.Path is { HasValue: true, Value: not null } /*&& request.Path.Value.Contains("/_blazor")*/))
        {
            var url = $"{request.Host}";
            l.A($"url:{url}");

            var aliases = aliasRepository.Value.GetAliases().ToList(); // cached by Oqtane
            l.A($"aliases:{aliases.Count}");

            if (siteId.HasValue) // acceptable solution
            {
                alias = aliases
                    .OrderByDescending(a => /*a.IsDefault*/ a.Name.Length) // TODO: a.IsDefault DESC after upgrade to Oqt v3.0.3+
                    //.ThenByDescending(a => a.Name.Length)
                    .ThenBy(a => a.Name)
                    .FirstOrDefault(a =>
                        a.SiteId == siteId.Value &&
                        a.Name.StartsWith(url, StringComparison.InvariantCultureIgnoreCase));
                AddMissingProperties(alias, request);
                l.A($"siteId:{siteId} => siteState.Alias:'{siteState.Alias?.Name}'");
            }
            else // fallback solution, wrong site is possible
            {
                alias = tenantManager.Value.GetAlias(); // get alias (note that this also sets SiteState.Alias)
                l.A($"siteId is NULL => siteState.Alias:'{siteState.Alias?.Name}'");
            }
        }
        else // great solution
        {
            var url = $"{request.Host}{request.Path}";
            l.A($"url:{url}");
            var aliases = aliasRepository.Value.GetAliases().ToList(); // cached by Oqtane
            alias = aliases.OrderByDescending(a => a.Name.Length)
                .ThenBy(a => a.Name)
                .FirstOrDefault(a => url.StartsWith(a.Name, StringComparison.InvariantCultureIgnoreCase));
            AddMissingProperties(alias, request);
            l.A($"siteId is NULL and path is NULL => siteState.Alias:'{siteState.Alias?.Name}'");
        }

        return l.ReturnAsOk(alias);
    }

    /// <summary>
    /// Oqtane v5.1+ builds BaseUrl for Alias, in "Oqtane.Server\Infrastructure\TenantManager.cs", line 52,
    /// we need to repeat that functionality here
    /// </summary>
    /// <param name="alias"></param>
    /// <param name="request"></param>
    private void AddMissingProperties(Alias alias, HttpRequest request)
    {
        if (alias is not { BaseUrl: null }) return;
        alias.BaseUrl = "";
        alias.Protocol = (request?.IsHttps == true) ? "https://" : "http://";
        if (request?.Headers.ContainsKey("User-Agent") == true && request.Headers["User-Agent"] == Constants.MauiUserAgent)
            alias.BaseUrl = siteState.Alias.Protocol + siteState.Alias.Name.Replace("/" + siteState.Alias.Path, "");
    }

    private void UpdateHttpContextItem(string key, Alias alias)
    {
        if (httpContextAccessor?.HttpContext != null && alias != null)
            httpContextAccessor.HttpContext.Items[key] ??= alias;
    }
}