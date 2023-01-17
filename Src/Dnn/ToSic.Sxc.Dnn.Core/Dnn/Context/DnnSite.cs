using System;
using System.IO;
using System.Web.Hosting;
using DotNetNuke.Common;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using ToSic.Eav.Apps;
using ToSic.Eav.Context;
using ToSic.Lib.Documentation;
using ToSic.Lib.Logging;
using ToSic.Eav.Run;
using ToSic.Lib.DI;
using ToSic.Sxc.Run;
using ToSic.Sxc.Web;

namespace ToSic.Sxc.Dnn.Context
{
    /// <summary>
    /// This is a DNN implementation of a Tenant-object. 
    /// </summary>
    [PrivateApi("this is just internal, external users don't really have anything to do with this")]
    public sealed class DnnSite: Site<PortalSettings>
    {
        #region Constructors and DI

        /// <summary>
        /// DI Constructor, will get the current portal settings
        /// #TodoDI not ideal yet, as PortalSettings.current is still retrieved from global
        /// </summary>
        public DnnSite(LazySvc<IZoneMapper> zoneMapperLazy, LazySvc<ILinkPaths> linkPathsLazy): base(DnnConstants.LogName)
        {
            this.ConnectServices(
                _zoneMapperLazy = zoneMapperLazy,
                _linkPathsLazy = linkPathsLazy
            );
            Swap(null, null);
        }
        private readonly ILazySvc<IZoneMapper> _zoneMapperLazy;
        private readonly ILazySvc<ILinkPaths> _linkPathsLazy;
        private ILinkPaths LinkPaths => _linkPathsLazy.Value;

        /// <inheritdoc />
        public override ISite Init(int siteId, ILog parentLog) => Swap(new PortalSettings(siteId), parentLog);

        #endregion

        #region Swap new Portal Settings into this object

        public DnnSite Swap(PortalSettings settings, ILog extLogOrNull) => Log.Func(() =>
        {
            UnwrappedSite = KeepBestPortalSettings(settings);

            // reset language info to be sure to get it from the latest source
            _currentCulture = null;
            _defaultLanguage = null;
            _zoneId = null;

            return (this, $"Site Id {Id}");
        });

        public DnnSite TrySwap(ModuleInfo module, ILog extLog) => Log.Func($"Owner Site: {module?.OwnerPortalID}, Current Site: {module?.PortalID}", () =>
        {
            if (module == null) return (this, "no module");
            if (module.OwnerPortalID < 0) return (this, "no change, owner < 0");

            var modulePortalSettings = new PortalSettings(module.OwnerPortalID);
            Swap(modulePortalSettings, extLog);
            return (this, "ok");
        });

        /// <summary>
        /// Very special helper to work around a DNN issue
        /// Reason is that PortalSettings.Current is always "perfect" and also contains root URLs and current Page
        /// Other PortalSettings may not contain this (partially populated objects)
        /// In case we're requesting a DnnTenant with incomplete PortalSettings
        /// we want to correct this here
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        private static PortalSettings KeepBestPortalSettings(PortalSettings settings, ILog extLogOrNull = null) => extLogOrNull.Func(() =>
        {
            // in case we don't have an HTTP Context with current portal settings, don't try anything
            if (PortalSettings.Current == null) return (settings, "null, use given");

            // If we don't have settings, or they point to the same portal, then use that
            if (settings == null) return (PortalSettings.Current, "null, use current");
            if (settings == PortalSettings.Current) return (PortalSettings.Current, "is current, use current");
            if (settings.PortalId == PortalSettings.Current.PortalId) return (PortalSettings.Current, "id=current, use current");

            // fallback: use supplied settings
            return (settings, "use new settings");
        });


        #endregion

        #region Culture / Languages

        /// <inheritdoc />
        public override string DefaultCultureCode => _defaultLanguage ?? (_defaultLanguage = UnwrappedSite?.DefaultLanguage?.ToLowerInvariant());
        private string _defaultLanguage;


        public override string CurrentCultureCode
        {
            get
            {
                if (_currentCulture != null) return _currentCulture;

                // First check if we know more about the site
                var portal = UnwrappedSite;
                var aliasCulture = portal?.PortalAlias?.CultureCode;
                if (!string.IsNullOrWhiteSpace(aliasCulture)) return _currentCulture = aliasCulture.ToLowerInvariant();

                // if alias is unknown, then we might be in search mode or something
                return _currentCulture = portal?.CultureCode?.ToLowerInvariant();
            }
        }
        private string _currentCulture;

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
            => _urlRoot ?? (_urlRoot = UnwrappedSite?.PortalAlias?.HTTPAlias
                                       ?? PortalSettings.Current?.PortalAlias?.HTTPAlias
                                       ?? "err-portal-alias-not-loaded");
        private string _urlRoot;

        [PrivateApi]
        public override string AppsRootPhysical => AppsRootRelative;


        [PrivateApi]
        public override string AppAssetsLinkTemplate => AppsRootPhysical + "/" + AppConstants.AppFolderPlaceholder;
        
        internal string AppsRootRelative => Path.Combine(UnwrappedSite.HomeDirectory, AppConstants.AppsRootFolder);
        internal string SharedAppsRootRelative => Path.Combine(Globals.HostPath, AppConstants.AppsRootFolder);

        [PrivateApi]
        public override string AppsRootPhysicalFull => HostingEnvironment.MapPath(AppsRootRelative);

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
}