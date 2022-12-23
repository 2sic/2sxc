using Oqtane.Models;
using Oqtane.Repository;
using ToSic.Eav.Apps;
using ToSic.Eav.Context;
using ToSic.Lib.Logging;
using ToSic.Eav.Run;
using ToSic.Lib.DI;
using ToSic.Lib.Documentation;
using ToSic.Sxc.Oqt.Server.Plumbing;
using ToSic.Sxc.Oqt.Server.WebApi;
using ToSic.Sxc.Oqt.Shared;
using ToSic.Sxc.Run;
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
            ILazySvc<ISiteRepository> siteRepository,
            ILazySvc<IServerPaths> serverPaths,
            ILazySvc<IZoneMapper> zoneMapper,
            ILazySvc<OqtCulture> oqtCulture,
            ILazySvc<ILinkPaths> linkPathsLazy): base(OqtConstants.OqtLogPrefix)
        {
            this.ConnectServices(
                _siteStateInitializer = siteStateInitializer,
                _siteRepository = siteRepository,
                _serverPaths = serverPaths,
                _zoneMapper = zoneMapper,
                _oqtCulture = oqtCulture,
                _linkPathsLazy = linkPathsLazy
            );
        }

        private readonly SiteStateInitializer _siteStateInitializer;
        private readonly ILazySvc<ISiteRepository> _siteRepository;
        private readonly ILazySvc<IServerPaths> _serverPaths;
        private readonly ILazySvc<IZoneMapper> _zoneMapper;
        private readonly ILazySvc<OqtCulture> _oqtCulture;
        private readonly ILazySvc<ILinkPaths> _linkPathsLazy;

        private ILinkPaths LinkPaths => _linkPathsLazy.Value;


        public OqtSite Init(Site site)
        {
            UnwrappedSite = site;
            return this;
        }

        public override ISite Init(int siteId, ILog parentLog)
        {
            UnwrappedSite = _siteRepository.Value.GetSite(siteId);
            return this;
        }

        protected override Site UnwrappedSite => base.UnwrappedSite ??= _siteRepository.Value.GetSite(Alias.SiteId);
        private Alias Alias => _siteStateInitializer.InitializedState.Alias;
        public override Site GetContents() => UnwrappedSite;

        /// <inheritdoc />
        public override string DefaultCultureCode => _defaultCultureCode ??= _oqtCulture.Value.DefaultCultureCode;
        private string _defaultCultureCode;

        public string DefaultLanguageCode => _defaultLanguageCode ??= _oqtCulture.Value.DefaultLanguageCode(Alias.SiteId).ToLowerInvariant();
        private string _defaultLanguageCode;

        /// <inheritdoc />
        public override string CurrentCultureCode => _currentCultureCode ??= _oqtCulture.Value.CurrentCultureCode.ToLowerInvariant();
        private string _currentCultureCode;

        /// <inheritdoc />
        public override int Id => UnwrappedSite.SiteId;

        public override string Url
        {
            get
            {
                if (_url != null) return _url;
                // Site Alias in Oqtane is without protocol, so we need to add it from current request for consistency
                // also without trailing slash
                var parts = new UrlParts(LinkPaths.GetCurrentRequestUrl());
                _url = $"{parts.Protocol}{Alias.Name}";
                return _url;
            }
        }
        private string _url;

        public override string UrlRoot => Alias.Name;

        /// <inheritdoc />
        public override string Name => UnwrappedSite.Name;

        [PrivateApi]
        public override string AppsRootPhysical => string.Format(OqtConstants.AppRootPublicBase, Id);

        [PrivateApi]
        public override string AppAssetsLinkTemplate => OqtPageOutput.GetSiteRoot(_siteStateInitializer.InitializedState)
                                                        + WebApiConstants.AppRootNoLanguage + "/" + AppConstants.AppFolderPlaceholder + "/assets";

        [PrivateApi] public override string AppsRootPhysicalFull => _serverPaths.Value.FullAppPath(AppsRootPhysical);


        /// <inheritdoc />
        public override string ContentPath => string.Format(OqtConstants.ContentRootPublicBase, UnwrappedSite.TenantId, Id);

        public override int ZoneId
        {
            get
            {
                if (_zoneId != null) return _zoneId.Value;
                // check if id is negative; 0 is a valid tenant id
                if (Id < 0) return (_zoneId = Eav.Constants.NullId).Value;
                _zoneId = _zoneMapper.Value.GetZoneId(Id);
                return _zoneId.Value;
            }
        }
        private int? _zoneId;
    }
}
