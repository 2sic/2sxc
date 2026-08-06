using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Services.Localization;
using System.Web;
using System.Web.Hosting;
using Microsoft.EntityFrameworkCore.Internal;
using ToSic.Eav.Apps.Sys;
using ToSic.Eav.Context.Sys.Site;
using ToSic.Eav.Context.Sys.ZoneCulture;
using ToSic.Eav.Context.Sys.ZoneMapper;
using ToSic.Eav.Sys;
using ToSic.Sxc.Sys.Integration.Paths;
using ToSic.Sxc.Web.Sys.Url;
using ToSic.Sys.Capabilities.Features;
using static ToSic.Eav.Context.Sys.ZoneCulture.IZoneCultureResolverExtensions;
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
    /// #TodoDI not ideal yet, as PortalSettings.Current is still retrieved from global
    /// </summary>
    public DnnSite(LazySvc<IZoneMapper> zoneMapperLazy, LazySvc<ILinkPaths> linkPathsLazy, LazySvc<ISysFeaturesService> featuresSvc)
        : base(DnnConstants.LogName, connect: [featuresSvc, zoneMapperLazy, linkPathsLazy])
    {
        _featuresSvc = featuresSvc;
        _zoneMapperLazy = zoneMapperLazy;
        _linkPathsLazy = linkPathsLazy;
        TryInitPortal(null);
    }
    private readonly LazySvc<IZoneMapper> _zoneMapperLazy;
    private readonly LazySvc<ILinkPaths> _linkPathsLazy;
    private readonly LazySvc<ISysFeaturesService> _featuresSvc;
    private ILinkPaths LinkPaths => _linkPathsLazy.Value;

    /// <inheritdoc />
    public override ISite Init(int siteId, ILog parentLogOrNull)
        => TryInitPortal(new(siteId), parentLogOrNull);

    #endregion

    #region Swap new Portal Settings into this object

    internal DnnSite TryInitPortal(PortalSettings settings, ILog parentLogOrNull = default)
    {
        AttachToExternalLog(parentLogOrNull);

        var l = Log.Fn<DnnSite>();
        UnwrappedSite = KeepBestPortalSettings(settings, parentLogOrNull);

        // reset language info to be sure to get it from the latest source
        _currentCulture.Reset();
        CultureCodesWithFallbacks = null!;
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
    /// <returns></returns>
    private static PortalSettings KeepBestPortalSettings(PortalSettings settings, ILog logOrNull)
    {
        var l = logOrNull.Fn<PortalSettings>();
        // in case we don't have an HTTP Context with current portal settings, don't try anything
        var current = PortalSettings.Current;
        if (current == null)
            return l.Return(settings, "null, use given");

        // If we don't have settings, or they point to the same portal, then use that
        var msgKeepCurrent = settings switch
        {
            null => "null, use current",
            _ when settings == current => "is current, use current",
            _ when settings.PortalId == current.PortalId => "id=current, use current",
            _ => null,
        };

        // fallback: use supplied settings
        return l.Return(msgKeepCurrent != null ? current : settings, msgKeepCurrent ?? "use new settings");
    }


    #endregion

    #region Culture / Languages

    /// <inheritdoc />
    public override string DefaultCultureCode => (_defaultLanguage ??= UnwrappedSite?.DefaultLanguage?.ToLowerInvariant()) ?? string.Empty;
    private string _defaultLanguage;


    public override string CurrentCultureCode => _currentCulture.Get(GetCurrentCultureCode);
    private readonly LazyGetAndReset<string> _currentCulture = new();

    private string GetCurrentCultureCode()
    {
        var l = Log.Fn<string>();
        // First check if we know more about the site
        var portal = UnwrappedSite;
        if (portal == null! /* paranoid */)
            return l.ReturnNull("no portal");
        var aliasCulture = portal.PortalAlias?.CultureCode ?? "";

        if (aliasCulture.HasValue())
        {
            var aliasCult = aliasCulture.ToLowerInvariant();
            return l.Return(aliasCult, $"{nameof(portal.PortalAlias)}: {aliasCult}");
        }

        // if alias is unknown, then we might be in search mode or something
        var result = portal.CultureCode?.ToLowerInvariant();
        return l.Return(result, $"Portal.CultureCode: {result}");
    }

    public List<string>? CultureCodesWithFallbacks
    {
        get => field ??= GetCultureCodesWithFallbacks();
        private set;
    }

    private List<string>? GetCultureCodesWithFallbacks()
    {
        var l = Log.Fn<List<string>>();
        // 2023-08-31 2dm - new code, as it could contain risks, use try/catch/null to default
        try
        {
            // If the feature is not enabled, return null so up-stream can handle defaults
            if (!_featuresSvc.Value.IsEnabled(BuiltInFeatures.LanguagesAdvancedFallback.Guid))
                return l.ReturnNull("feature not enabled");

            var lc = LocaleController.Instance;
            if (lc == null)
                return l.ReturnNull("no locale controller");
            var list = new List<string>();

            // Top priority is current and fallbacks of it
            // 2025-09-15 2dm: Dnn seems to do something wrong during WebApi calls.
            // Internally it wants to prefer the `language` querystring param,
            // but if it doesn't have it, then in a WebApi call it seems to not correctly take the current portal alias culture.
            // So we can't use `GetCurrentLocale` but need to use `GetLocaleOrCurrent` with the current culture code.
            var languageQuery = HttpContext.Current?.Request?.QueryString?["language"]; // null-safe when no HTTP context
            var current = !string.IsNullOrEmpty(languageQuery)
                ? lc.GetCurrentLocale(Id) // This will use the querystring param if available, or work in Razor, but not in WebApi
                : lc.GetLocaleOrCurrent(Id, CurrentCultureCode ?? DefaultCultureCode ?? System.Globalization.CultureInfo.CurrentCulture.Name); // Use known codes or thread culture when no HTTP context
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
            return list.Any()
                ? l.Return(list, $"got: {list.Count}")
                : l.ReturnNull("no list");
        }
        catch
        {
            return l.ReturnAsError(null);
        }
    }

    #endregion

    // ReSharper disable once InheritdocInvalidUsage
    /// <inheritdoc />
    public override int Id
        => UnwrappedSite?.PortalId ?? EavConstants.NullId;

    /// <inheritdoc />
    public override string Name
        => UnwrappedSite.PortalName;

    public override string Url
    {
        get
        {
            if (field != null)
                return field;
            // PortalAlias in DNN is without protocol, so we need to add it from current request for consistency
            // also without trailing slash
            var parts = new UrlParts(LinkPaths.GetCurrentRequestUrl());
            return field = $"{parts.Protocol}{UrlRoot}";
        }
    }

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
    public override string AppsRootPhysical
        => Path.Combine(UnwrappedSite.HomeDirectory, AppConstants.AppsRootFolder);


    [PrivateApi]
    public override string AppAssetsLinkTemplate
        => AppsRootPhysical + "/" + AppConstants.AppFolderPlaceholder;
        
    [PrivateApi]
    public override string AppsRootPhysicalFull
        => HostingEnvironment.MapPath(AppsRootPhysical);

    /// <inheritdoc />
    public override string ContentPath
        => UnwrappedSite.HomeDirectory;

    public override int ZoneId
    {
        get { 
            if(_zoneId != null)
                return _zoneId.Value;
            // check if id is negative; 0 is a valid tenant id
            if (Id < 0)
                return (_zoneId = EavConstants.NullId).Value;
            _zoneId = _zoneMapperLazy.Value.GetZoneId(Id);
            return _zoneId.Value;
        }
    }
    private int? _zoneId;
}