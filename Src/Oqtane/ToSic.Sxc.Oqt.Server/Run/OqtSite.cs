using System;
using System.IO;
using Oqtane.Models;
using Oqtane.Repository;
using ToSic.Eav.Documentation;
using ToSic.Eav.Run;
using ToSic.Sxc.Oqt.Shared;
using ToSic.Sxc.Oqt.Shared.Dev;

// todo: #Oqtane
// - url 


namespace ToSic.Sxc.Oqt.Server.Run
{
    /// <summary>
    /// This is a Mvc implementation of a Tenant-object. 
    /// </summary>
    [InternalApi_DoNotUse_MayChangeWithoutNotice("this is just fyi")]
    public class OqtSite: Site<Site>
    {
        /// <summary>
        /// Constructor for DI
        /// </summary>
        public OqtSite(Lazy<ISiteRepository> siteRepository, Lazy<ITenantResolver> tenantResolver, Lazy<IServerPaths> serverPaths, Lazy<OqtZoneMapper> zoneMapper) : base(null)
        {
            _siteRepository = siteRepository;
            _tenantResolver = tenantResolver;
            _serverPaths = serverPaths;
            _zoneMapper = zoneMapper;
        }
        private readonly Lazy<ISiteRepository> _siteRepository;
        private readonly Lazy<ITenantResolver> _tenantResolver;
        private readonly Lazy<IServerPaths> _serverPaths;
        private readonly Lazy<OqtZoneMapper> _zoneMapper;

        public OqtSite Init(Site settings)
        {
            UnwrappedContents = settings;
            return this;
        }

        public override Site UnwrappedContents
        {
            get => _unwrapped ??= _siteRepository.Value.GetSite(_tenantResolver.Value.GetAlias().SiteId);
            protected set => _unwrapped = value;
        }
        private Site _unwrapped;


        public override ISite Init(int siteId)
        {
            UnwrappedContents = _siteRepository.Value.GetSite(siteId);
            return this;
        }

        /// <inheritdoc />
        public override string DefaultLanguage => WipConstants.DefaultLanguage;

        /// <inheritdoc />
        public override int Id => UnwrappedContents.SiteId;

        // Todo: #Oqtane
        public override string Url => WipConstants.HttpUrlRoot;

        /// <inheritdoc />
        public override string Name => UnwrappedContents.Name;

        [PrivateApi]
        public override string AppsRootPhysical => AppsRootPartial();

        private string AppsRootPartial()
        {
            var site = UnwrappedContents;
            return Path.Combine(OqtConstants.ContentSubfolder, string.Format(OqtConstants.AppRootPublicBase, site.TenantId, site.SiteId), Settings.AppsRootFolder);
        }

        [PrivateApi]
        public override string AppsRootLink => Path.Combine(string.Format(OqtConstants.AppAssetsLinkRoot, Id.ToString()));

        [PrivateApi] public override string AppsRootPhysicalFull => _serverPaths.Value.FullAppPath(AppsRootPartial());

        [PrivateApi]
        public override bool RefactorUserIsAdmin => WipConstants.IsAdmin;

        /// <inheritdoc />
        public override string ContentPath
        {
            get
            {
                var site = UnwrappedContents;
                return Path.Combine(OqtConstants.ContentSubfolder, string.Format(OqtConstants.AppRootPublicBase, site.TenantId, site.SiteId));
            }
        }

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
