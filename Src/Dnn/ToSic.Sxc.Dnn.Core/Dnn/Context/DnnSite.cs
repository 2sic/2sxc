using System.IO;
using System.Web.Hosting;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using ToSic.Eav.Context;
using ToSic.Eav.Plumbing;
using ToSic.Lib.Helpers;
using DotNetNuke.Services.Localization;
using ToSic.Eav.Apps.Internal;
using ToSic.Eav.Integration;
using ToSic.Eav.Internal.Features;
using ToSic.Sxc.Integration.Paths;
using ToSic.Sxc.Web.Internal.Url;
using static ToSic.Eav.Context.IZoneCultureResolverExtensions;
using ISite = ToSic.Eav.Context.ISite;

namespace ToSic.Sxc.Dnn.Context;

/// <summary>
/// This is a DNN implementation of a Tenant-object. 
/// </summary>
internal sealed class DnnSite: Site<PortalSettings>, IZoneCultureResolverProWIP
{

    #region Constructors and DI

    /// <summary>
    /// DI Constructor, will get the current portal settings
    /// #TodoDI not ideal yet, as PortalSettings.current is still retrieved from global
    /// </summary>
    public DnnSite(LazySvc<IZoneMapper> zoneMapperLazy, LazySvc<ILinkPaths> linkPathsLazy, LazySvc<IEavFeaturesService> featuresSvc): base(DnnConstants.LogName)
    {
        ConnectLogs([
            _featuresSvc = featuresSvc,
            _zoneMapperLazy = zoneMapperLazy,
            _linkPathsLazy = linkPathsLazy
        ]);
        TryInitPortal(null);
    }
    private readonly LazySvc<IZoneMapper> _zoneMapperLazy;
    private readonly LazySvc<ILinkPaths> _linkPathsLazy;
    private readonly LazySvc<IEavFeaturesService> _featuresSvc;
    private ILinkPaths LinkPaths => _linkPathsLazy.Value;

    /// <inheritdoc />
    public override ISite Init(int siteId, ILog parentLog) => TryInitPortal(new(siteId), parentLog);

    #endregion

    #region Swap new Portal Settings into this object

    internal DnnSite TryInitPortal(PortalSettings settings, ILog extLogOrNull = default)
    {
        AttachToExternalLog(extLogOrNull);

        var l = Log.Fn<DnnSite>();
        UnwrappedSite = KeepBestPortalSettings(settings);

        // reset language info to be sure to get it from the latest source
        _currentCulture.Reset(Log);
        _currentCodeFallbacks.Reset(Log);
        _defaultLanguage = null;
        _zoneId = null;

        return l.Return(this, $"Site Id {Id}");
    }

    internal DnnSite TryInitModule(ModuleInfo module, ILog extLog)
    {
        AttachToExternalLog(extLog);

        var l = extLog.Fn<DnnSite>($"Owner Site: {module?.OwnerPortalID}, Current Site: {module?.PortalID}");
        if (module == null) return l.Return(this, "no module");
        if (module.OwnerPortalID < 0) return l.Return(this, "no change, owner < 0");

        var modulePortalSettings = new PortalSettings(module.OwnerPortalID);
        TryInitPortal(modulePortalSettings);
        return l.Return(this, "ok");
    }

    private void AttachToExternalLog(ILog extLogOrNull)
    {
        if (extLogOrNull != null && extLogOrNull != Log)
            this.LinkLog(extLogOrNull, forceConnect: true);
    }


    /// <summary>
    /// Very special helper to work around a DNN issue
    /// Reason is that PortalSettings.Current is always "perfect" and also contains root URLs and current Page
    /// Other PortalSettings may not contain this (partially populated objects)
    /// In case we're requesting a DnnTenant with incomplete PortalSettings
    /// we want to correct this here
    /// </summary>
    /// <param name="settings"></param>
    /// <returns></returns>
    private static PortalSettings KeepBestPortalSettings(PortalSettings settings, ILog extLogOrNull = null)
    {
        var l = extLogOrNull.Fn<PortalSettings>();
        // in case we don't have an HTTP Context with current portal settings, don't try anything
        if (PortalSettings.Current == null) return l.Return(settings, "null, use given");

        // If we don't have settings, or they point to the same portal, then use that
        if (settings == null) return l.Return(PortalSettings.Current, "null, use current");
        if (settings == PortalSettings.Current) return l.Return(PortalSettings.Current, "is current, use current");
        if (settings.PortalId == PortalSettings.Current.PortalId) return l.Return(PortalSettings.Current, "id=current, use current");

        // fallback: use supplied settings
        return l.Return(settings, "use new settings");
    }


    #endregion

    #region Culture / Languages

    /// <inheritdoc />
    public override string DefaultCultureCode => _defaultLanguage ??= UnwrappedSite?.DefaultLanguage?.ToLowerInvariant();
    private string _defaultLanguage;


    public override string CurrentCultureCode => _currentCulture.GetM(Log, l =>
    {
        // First check if we know more about the site
        var portal = UnwrappedSite;
        if (portal == null) return (null, "no portal");
        var aliasCulture = portal.PortalAlias?.CultureCode ?? "";

        if (aliasCulture.HasValue())
        {
            var aliasCult = aliasCulture.ToLowerInvariant();
            return (aliasCult, $"{nameof(portal.PortalAlias)}: {aliasCult}");
        }

        // if alias is unknown, then we might be in search mode or something
        var result = portal.CultureCode?.ToLowerInvariant();
        return (result, $"Portal.CultureCode: {result}");
    });
    private readonly GetOnce<string> _currentCulture = new();

    public List<string> CultureCodesWithFallbacks => _currentCodeFallbacks.GetL(Log, l =>
    {
        // 2023-08-31 2dm - new code, as it could contain risks, use try/catch/null to default
        try
        {
            // If the feature is not enabled, return null so up-stream can handle defaults
            if (!_featuresSvc.Value.IsEnabled(BuiltInFeatures.LanguagesAdvancedFallback.Guid))
                return null;

            var lc = LocaleController.Instance;
            if (lc == null) return null;
            var list = new List<string>();

            // Top priority is current and fallbacks of it
            // TODO: verify it uses the one of the current Alias...
            var current = lc.GetCurrentLocale(Id);
            if (current != null)
            {
                var currentCode = current.Code;
                l.A($"{nameof(currentCode)}: {currentCode}");
                ListBuildAddCodeIfNew(list, currentCode);

                // Try to add fallbacks, and fallbacks of fallbacks...
                var fallback = current.FallBackLocale;
                for (var i = 0; i < 3 && fallback != null; i++)
                {
                    ListBuildAddCodeIfNew(list, fallback.Code);
                    fallback = fallback.FallBackLocale;
                }
            }

            // Always add the defaults as well
            var def = lc.GetDefaultLocale(Id);
            if (def != null)
            {
                var defCode = def.Code;
                l.A($"{nameof(defCode)}: {defCode}");
                ListBuildAddCodeIfNew(list, defCode);
                // Default should never have another fallback; it's the default!
            }

            // If the list is empty, return null so upstream can fallback
            return list.Any() ? list : null;
        }
        catch
        {
            return null;
        }
    });
    private readonly GetOnce<List<string>> _currentCodeFallbacks = new();


    #endregion

    // ReSharper disable once InheritdocInvalidUsage
    /// <inheritdoc />
    public override int Id => UnwrappedSite?.PortalId ?? Eav.Constants.NullId;

    /// <inheritdoc />
    public override string Name => UnwrappedSite.PortalName;

    public override string Url
    {
        get
        {
            if (_url != null) return _url;
            // PortalAlias in DNN is without protocol, so we need to add it from current request for consistency
            // also without trailing slash
            var parts = new UrlParts(LinkPaths.GetCurrentRequestUrl());
            _url = $"{parts.Protocol}{UrlRoot}";
            return _url;
        }
    }

    private string _url;

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// Important special case: if the PortalSettings are not from the PortalSettings.Current, then the
    /// PortalAlias are null!!!
    /// I believe this should only matter in very special cases
    /// Like when showing a module from another portal - in which case we don't need that alias
    /// but the current one. Just keep this in mind in case anything ever breaks.
    /// </remarks>
    public override string UrlRoot
        => _urlRoot ??= UnwrappedSite?.PortalAlias?.HTTPAlias
                        ?? PortalSettings.Current?.PortalAlias?.HTTPAlias
                        ?? "err-portal-alias-not-loaded";
    private string _urlRoot;

    [PrivateApi]
    public override string AppsRootPhysical => Path.Combine(UnwrappedSite.HomeDirectory, AppConstants.AppsRootFolder);


    [PrivateApi]
    public override string AppAssetsLinkTemplate => AppsRootPhysical + "/" + AppConstants.AppFolderPlaceholder;
        
    [PrivateApi]
    public override string AppsRootPhysicalFull => HostingEnvironment.MapPath(AppsRootPhysical);

    /// <inheritdoc />
    public override string ContentPath => UnwrappedSite.HomeDirectory;

    public override int ZoneId
    {
        get { 
            if(_zoneId != null) return _zoneId.Value;
            // check if id is negative; 0 is a valid tenant id
            if (Id < 0) return (_zoneId = Eav.Constants.NullId).Value;
            _zoneId = _zoneMapperLazy.Value.GetZoneId(Id);
            return _zoneId.Value;
        }
    }

    private int? _zoneId;
}