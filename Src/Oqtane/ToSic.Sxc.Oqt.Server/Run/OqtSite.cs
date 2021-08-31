using Oqtane.Models;
using Oqtane.Repository;
using System;
using ToSic.Eav.Context;
using ToSic.Eav.Documentation;
using ToSic.Eav.Run;
using ToSic.Sxc.Context;
using ToSic.Sxc.Oqt.Server.Block;
using ToSic.Sxc.Oqt.Server.Plumbing;
using ToSic.Sxc.Oqt.Shared;
using ToSic.Sxc.Run;

namespace ToSic.Sxc.Oqt.Server.Run
{
    /// <summary>
    /// This is a Mvc implementation of a Tenant-object.
    /// </summary>
    [InternalApi_DoNotUse_MayChangeWithoutNotice("this is just fyi")]
    public sealed class OqtSite: Site<Site>, ICmsSite
    {
        /// <summary>
        /// Constructor for DI
        /// </summary>
        public OqtSite(SiteStateInitializer siteStateInitializer,
            Lazy<ISiteRepository> siteRepository,
            Lazy<IServerPaths> serverPaths,
            Lazy<OqtZoneMapper> zoneMapper,
            Lazy<OqtCulture> oqtCulture)
        {
            _siteStateInitializer = siteStateInitializer;
            _siteRepository = siteRepository;
            _serverPaths = serverPaths;
            _zoneMapper = zoneMapper;
            _oqtCulture = oqtCulture;
        }

        private readonly SiteStateInitializer _siteStateInitializer;
        private readonly Lazy<ISiteRepository> _siteRepository;
        private readonly Lazy<IServerPaths> _serverPaths;
        private readonly Lazy<OqtZoneMapper> _zoneMapper;
        private readonly Lazy<OqtCulture> _oqtCulture;


        public OqtSite Init(Site site)
        {
            UnwrappedContents = site;
            return this;
        }

        public override ISite Init(int siteId)
        {
            UnwrappedContents = _siteRepository.Value.GetSite(siteId);
            return this;
        }

        public override Site UnwrappedContents
        {
            get => _unwrapped ??= _siteRepository.Value.GetSite(Alias.SiteId);
            protected set => _unwrapped = value;
        }
        private Site _unwrapped;
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

        public override string Url => Alias.Name;

        /// <inheritdoc />
        public override string Name => UnwrappedContents.Name;

        [PrivateApi]
        public override string AppsRootPhysical => string.Format(OqtConstants.AppRootPublicBase, Id);

        [PrivateApi]
        public override string AppAssetsLinkTemplate => OqtAssetsAndHeaders.GetSiteRoot(_siteStateInitializer.InitializedState)
                                                        + WebApiConstants.AppRoot + "/" + LinkPaths.AppFolderPlaceholder + "/assets";

        [PrivateApi] public override string AppsRootPhysicalFull => _serverPaths.Value.FullAppPath(AppsRootPhysical);


        /// <inheritdoc />
        public override string ContentPath => string.Format(OqtConstants.ContentRootPublicBase, UnwrappedContents.TenantId, Id);

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
