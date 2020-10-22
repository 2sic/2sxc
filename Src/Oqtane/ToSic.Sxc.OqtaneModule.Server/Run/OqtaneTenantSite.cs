using System.IO;
using ToSic.Eav.Documentation;
using ToSic.Eav.Run;
using Oqtane.Models;
using Oqtane.Repository;
using ToSic.Sxc.OqtaneModule.Shared.Dev;

// todo: #Oqtane
// - url 


namespace ToSic.Sxc.OqtaneModule.Server.Run
{
    /// <summary>
    /// This is a Mvc implementation of a Tenant-object. 
    /// </summary>
    [InternalApi_DoNotUse_MayChangeWithoutNotice("this is just fyi")]
    public class OqtaneTenantSite: Tenant<Site>
    {
        /// <summary>
        /// Constructor for DI
        /// </summary>
        public OqtaneTenantSite(ISiteRepository siteRepository, ITenantResolver tenantResolver) : base(null)
        {
            _siteRepository = siteRepository;
            _tenantResolver = tenantResolver;
        }
        private readonly ISiteRepository _siteRepository;
        private readonly ITenantResolver _tenantResolver;

        public OqtaneTenantSite(Site settings) : base(settings)
        {
        }

        public override Site UnwrappedContents
        {
            get => _unwrapped ??= _siteRepository.GetSite(_tenantResolver.GetAlias().SiteId);
            protected set => _unwrapped = value;
        }
        private Site _unwrapped;


        public override ITenant Init(int siteId)
        {
            UnwrappedContents = _siteRepository.GetSite(siteId);
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
        public override string AppsRoot => Path.Combine(WipConstants.AppRootPublicBase, Settings.AppsRootFolder);

        [PrivateApi]
        public override bool RefactorUserIsAdmin => WipConstants.IsAdmin;

        /// <inheritdoc />
        public override string ContentPath => WipConstants.ContentRoot;

        public override int ZoneId => TestIds.PrimaryZone;

    }
}
