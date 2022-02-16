using System;
using System.IO;
using System.Web.Hosting;
using DotNetNuke.Common;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using ToSic.Eav.Apps;
using ToSic.Eav.Context;
using ToSic.Eav.Documentation;
using ToSic.Eav.Logging;
using ToSic.Eav.Run;
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
        public DnnSite(Lazy<IZoneMapper> zoneMapperLazy, Lazy<ILinkPaths> linkPathsLazy): base(DnnConstants.LogName)
        {
            _zoneMapperLazy = zoneMapperLazy;
            _linkPathsLazy = linkPathsLazy;
            Swap(null, null);
        }
        private readonly Lazy<IZoneMapper> _zoneMapperLazy;
        private readonly Lazy<ILinkPaths> _linkPathsLazy;
        private ILinkPaths LinkPaths => _linkPathsLazy.Value;

        /// <inheritdoc />
        public override ISite Init(int siteId, ILog parentLog) => Swap(new PortalSettings(siteId), parentLog);

        #endregion

        #region Swap new Portal Settings into this object

        public DnnSite Swap(PortalSettings settings, ILog extLogOrNull)
        {
            var wrapLog = extLogOrNull.SafeCall<DnnSite>();
            _contents = KeepBestPortalSettings(settings);

            // reset language info to be sure to get it from the latest source
            _currentCulture = null;
            _defaultLanguage = null;
            _zoneId = null;

            return wrapLog($"Site Id {Id}", this);
        }

        public DnnSite TrySwap(ModuleInfo module, ILog extLog)
        {
            var wrapLog = extLog.Call<DnnSite>($"Owner Site: {module?.OwnerPortalID}, Current Site: {module?.PortalID}");

            if (module == null) return wrapLog("no module", this);
            if (module.OwnerPortalID < 0) return wrapLog("no change, owner < 0", this);

            var modulePortalSettings = new PortalSettings(module.OwnerPortalID);
            Swap(modulePortalSettings, extLog);
            return wrapLog("", this);
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
            var safeWrap = extLogOrNull.SafeCall<PortalSettings>();

            // in case we don't have an HTTP Context with current portal settings, don't try anything
            if (PortalSettings.Current == null) return safeWrap("null", settings);

            // If we don't have settings, or they point to the same portal, then use that
            if (settings == null) return safeWrap("null, use current", PortalSettings.Current);
            if (settings == PortalSettings.Current) return safeWrap("is current, use current", PortalSettings.Current);
            if (settings.PortalId == PortalSettings.Current.PortalId) return safeWrap("id=current, use current", PortalSettings.Current);

            // fallback: use supplied settings
            return safeWrap("use new settings", settings);
        }


        #endregion

        #region Culture / Languages

        /// <inheritdoc />
        public override string DefaultCultureCode => _defaultLanguage ?? (_defaultLanguage = UnwrappedContents?.DefaultLanguage?.ToLowerInvariant());
        private string _defaultLanguage;


        public override string CurrentCultureCode
        {
            get
            {
                if (_currentCulture != null) return _currentCulture;

                // First check if we know more about the site
                var portal = UnwrappedContents;
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
        public override int Id => UnwrappedContents?.PortalId ?? Eav.Constants.NullId;

        /// <inheritdoc />
        public override string Name => UnwrappedContents.PortalName;

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
            => _urlRoot ?? (_urlRoot = UnwrappedContents?.PortalAlias?.HTTPAlias
                                       ?? PortalSettings.Current?.PortalAlias?.HTTPAlias
                                       ?? "err-portal-alias-not-loaded");
        private string _urlRoot;

        [PrivateApi]
        public override string AppsRootPhysical => AppsRootRelative;


        [PrivateApi]
        public override string AppAssetsLinkTemplate => AppsRootPhysical + "/" + AppConstants.AppFolderPlaceholder;
        
        internal string AppsRootRelative => Path.Combine(UnwrappedContents.HomeDirectory, AppConstants.AppsRootFolder);
        internal string SharedAppsRootRelative => Path.Combine(Globals.HostPath, AppConstants.AppsRootFolder);

        [PrivateApi]
        public override string AppsRootPhysicalFull => HostingEnvironment.MapPath(AppsRootRelative);

        /// <inheritdoc />
        public override string ContentPath => UnwrappedContents.HomeDirectory;

        public override int ZoneId
        {
            get { 
                if(_zoneId != null) return _zoneId.Value;
                // check if id is negative; 0 is a valid tenant id
                if (Id < 0) return (_zoneId = Eav.Constants.NullId).Value;
                _zoneId = _zoneMapperLazy.Value.Init(null).GetZoneId(Id);
                return _zoneId.Value;
            }
        }

        private int? _zoneId;
    }
}