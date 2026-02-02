using Oqtane.Models;
using Oqtane.Services;
using ToSic.Eav.Apps.Sys;
using ToSic.Eav.Context;
using ToSic.Eav.Context.Sys.Site;
using ToSic.Eav.Context.Sys.ZoneMapper;
using ToSic.Eav.Environment.Sys.ServerPaths;
using ToSic.Sxc.Oqt.Server.Plumbing;
using ToSic.Sxc.Oqt.Server.WebApi;
using ToSic.Sxc.Oqt.Shared;
using ToSic.Sxc.Sys.Integration.Paths;
using OqtPageOutput = ToSic.Sxc.Oqt.Server.Blocks.Output.OqtPageOutput;
using UrlParts = ToSic.Sxc.Web.Sys.Url.UrlParts;

namespace ToSic.Sxc.Oqt.Server.Context;

/// <summary>
/// This is a Mvc implementation of a Tenant-object.
/// </summary>
[PrivateApi]
internal sealed class OqtSite(
    AliasResolver aliasResolver,
    LazySvc<ISiteService> siteService,
    LazySvc<IServerPaths> serverPaths,
    LazySvc<IZoneMapper> zoneMapper,
    LazySvc<OqtCulture> oqtCulture,
    LazySvc<ILinkPaths> linkPathsLazy)
    : Site<Site>(OqtConstants.OqtLogPrefix,
        connect: [aliasResolver, siteService, serverPaths, zoneMapper, oqtCulture, linkPathsLazy])
{
    private ILinkPaths LinkPaths => linkPathsLazy.Value;


    public OqtSite Init(Site site)
    {
        UnwrappedSite = site;
        return this;
    }

    public override ISite Init(int siteId, ILog? parentLogOrNull)
    {
        UnwrappedSite = siteService.Value.GetSiteAsync(siteId).GetAwaiter().GetResult();
        return this;
    }

    protected override Site UnwrappedSite => base.UnwrappedSite ??= siteService.Value.GetSiteAsync(Alias.SiteId).GetAwaiter().GetResult();
    private Alias Alias => aliasResolver.Alias;
    public override Site GetContents() => UnwrappedSite;

    /// <inheritdoc />
    public override string DefaultCultureCode => field ??= oqtCulture.Value.DefaultCultureCode;

    public string DefaultLanguageCode => field ??= oqtCulture.Value.DefaultLanguageCode(Alias.SiteId).ToLowerInvariant();

    /// <inheritdoc />
    public override string CurrentCultureCode => field ??= oqtCulture.Value.CurrentCultureCode.ToLowerInvariant();

    /// <inheritdoc />
    public override int Id => UnwrappedSite.SiteId;

    public override string Url
    {
        get
        {
            if (field != null) return field;
            // Site Alias in Oqtane is without protocol, so we need to add it from current request for consistency
            // also without trailing slash
            var parts = new UrlParts(LinkPaths.GetCurrentRequestUrl());
            field = $"{parts.Protocol}{Alias.Name}";
            return field;
        }
    }

    public override string UrlRoot => Alias.Name;

    /// <inheritdoc />
    public override string Name => UnwrappedSite.Name;

    [PrivateApi]
    public override string AppsRootPhysical
        => string.Format(OqtConstants.AppRootTenantSiteBase, Alias.TenantId, Id);

    [PrivateApi]
    public override string AppAssetsLinkTemplate => OqtPageOutput.GetSiteRoot(aliasResolver.Alias)
                                                    + OqtWebApiConstants.AppRootNoLanguage + "/" + AppConstants.AppFolderPlaceholder + "/assets";

    [PrivateApi] public override string AppsRootPhysicalFull => serverPaths.Value.FullAppPath(AppsRootPhysical);


    /// <inheritdoc />
    public override string ContentPath => string.Format(OqtConstants.ContentRootPublicBase, Alias.TenantId, Id);

    public override int ZoneId
    {
        get
        {
            if (_zoneId != null) return _zoneId.Value;
            // check if id is negative; 0 is a valid tenant id
            if (Id < 0) return (_zoneId = Eav.Sys.EavConstants.NullId).Value;
            _zoneId = zoneMapper.Value.GetZoneId(Id);
            return _zoneId.Value;
        }
    }
    private int? _zoneId;
}
