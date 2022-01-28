using System;
using Oqtane.Models;
using Oqtane.Repository;
using ToSic.Eav.Apps;
using ToSic.Eav.Context;
using ToSic.Eav.Documentation;
using ToSic.Eav.Logging;
using ToSic.Eav.Run;
using ToSic.Sxc.Oqt.Server.Plumbing;
using ToSic.Sxc.Oqt.Server.Run;
using ToSic.Sxc.Oqt.Shared;
using ToSic.Sxc.Web;
using OqtPageOutput = ToSic.Sxc.Oqt.Server.Blocks.Output.OqtPageOutput;

namespace ToSic.Sxc.Oqt.Server.Context
{
    /// <summary>
    /// This is a Mvc implementation of a Tenant-object.
    /// </summary>
    [PrivateApi]
    public sealed class OqtSite: Site<Site>
    {
        /// <summary>
        /// Constructor for DI
        /// </summary>
        public OqtSite(SiteStateInitializer siteStateInitializer,
            Lazy<ISiteRepository> siteRepository,
            Lazy<IServerPaths> serverPaths,
            Lazy<OqtZoneMapper> zoneMapper,
            Lazy<OqtCulture> oqtCulture,
            Lazy<ILinkHelper> linkHelperLazy): base(OqtConstants.OqtLogPrefix)
        {
            _siteStateInitializer = siteStateInitializer;
            _siteRepository = siteRepository;
            _serverPaths = serverPaths;
            _zoneMapper = zoneMapper;
            _oqtCulture = oqtCulture;
            _linkHelperLazy = linkHelperLazy;
        }

        private readonly SiteStateInitializer _siteStateInitializer;
        private readonly Lazy<ISiteRepository> _siteRepository;
        private readonly Lazy<IServerPaths> _serverPaths;
        private readonly Lazy<OqtZoneMapper> _zoneMapper;
        private readonly Lazy<OqtCulture> _oqtCulture;
        private readonly Lazy<ILinkHelper> _linkHelperLazy;


        public OqtSite Init(Site site)
        {
            _contents = site;
            return this;
        }

        public override ISite Init(int siteId, ILog parentLog)
        {
            _contents = _siteRepository.Value.GetSite(siteId);
            return this;
        }

        public override Site UnwrappedContents => _contents ??= _siteRepository.Value.GetSite(Alias.SiteId);
        private Alias Alias => _siteStateInitializer.InitializedState.Alias;

        /// <inheritdoc />
        public override string DefaultCultureCode => _defaultCultureCode ??= _oqtCulture.Value.DefaultCultureCode;
        private string _defaultCultureCode;

        public string DefaultLanguageCode => _defaultLanguageCode ??= _oqtCulture.Value.DefaultLanguageCode(Alias.SiteId).ToLowerInvariant();
        private string _defaultLanguageCode;

        /// <inheritdoc />
        public override string CurrentCultureCode => _currentCultureCode ??= _oqtCulture.Value.CurrentCultureCode.ToLowerInvariant();
        private string _currentCultureCode;

        /// <inheritdoc />
        public override int Id => UnwrappedContents.SiteId;

        public override string Url
        {
            get
            {
                if (_url != null) return _url;
                // Site Alias in Oqtane is without protocol, so we need to add it from current request for consistency
                // also without trailing slash
                var parts = new UrlParts(_linkHelperLazy.Value.GetCurrentRequestUrl());
                _url = $"{parts.Protocol}{Alias.Name}";
                return _url;
            }
        }
        private string _url;

        public override string UrlRoot => Alias.Name;

        /// <inheritdoc />
        public override string Name => _contents.Name;

        [PrivateApi]
        public override string AppsRootPhysical => string.Format(OqtConstants.AppRootPublicBase, Id);

        [PrivateApi]
        public override string AppAssetsLinkTemplate => OqtPageOutput.GetSiteRoot(_siteStateInitializer.InitializedState)
                                                        + WebApiConstants.AppRoot + "/" + AppConstants.AppFolderPlaceholder + "/assets";

        [PrivateApi] public override string AppsRootPhysicalFull => _serverPaths.Value.FullAppPath(AppsRootPhysical);


        /// <inheritdoc />
        public override string ContentPath => string.Format(OqtConstants.ContentRootPublicBase, _contents.TenantId, Id);

        public override int ZoneId
        {
            get
            {
                if (_zoneId != null) return _zoneId.Value;
                // check if id is negative; 0 is a valid tenant id
                if (Id < 0) return (_zoneId = Eav.Constants.NullId).Value;
                _zoneId = _zoneMapper.Value.Init(null).GetZoneId(Id);
                return _zoneId.Value;
            }
        }
        private int? _zoneId;
    }
}
