using System;
using System.Collections.Immutable;
using System.Linq;
using ToSic.Eav.Data;
using ToSic.Eav.Data.Build;
using ToSic.Eav.DataSources;
using ToSic.Eav.DataSources.Queries;
using ToSic.Lib.Documentation;
using ToSic.Lib.Logging;
using static ToSic.Eav.DataSources.DataSourceConstants;

// Important Info to people working with this
// It depends on abstract provider, that must be overriden in each platform
// In addition, each platform must make sure to register a TryAddTransient with the platform specific provider implementation
// This is because any constructor DI should be able to target this type, and get the real provider implementation

namespace ToSic.Sxc.DataSources
{
    /// <summary>
    /// Deliver a list of pages from the current platform (Dnn or Oqtane).
    ///
    /// To figure out the properties returned and what they match up to, see <see cref="PageDataRaw"/>
    /// </summary>
    [PublicApi]
    [VisualQuery(
        ExpectsDataOfType = "3d970d2b-32cb-4ecb-aeaf-c49fbcc678a5",
        GlobalName = "e35031b2-3e99-41fe-a5ac-b79f447d5800",
        HelpLink = "https://r.2sxc.org/ds-pages",
        Icon = Icons.PageFind,
        NiceName = "Pages",
        Type = DataSourceType.Source,
        UiHint = "Pages in this site")]
    public class Pages: CustomDataSource
    {
        private readonly ITreeMapper _treeMapper;
        private readonly IDataFactory _pageFactory;
        private readonly PagesDataSourceProvider _provider;

        #region Configuration properties

        /// <summary>
        /// Include hidden pages.
        /// Default is `false`
        /// </summary>
        [Configuration]
        public bool IncludeHidden
        {
            get => Configuration.GetThis(false);
            set => Configuration.SetThis(value);
        }
        /// <summary>
        /// Include deleted pages in the recycle bin.
        /// Default is `false`
        /// </summary>
        [Configuration]
        public bool IncludeDeleted
        {
            get => Configuration.GetThis(false);
            set => Configuration.SetThis(value);
        }
        /// <summary>
        /// Include admin pages such as site files.
        /// Default is `false`
        /// </summary>
        [Configuration]
        public bool IncludeAdmin
        {
            get => Configuration.GetThis(false);
            set => Configuration.SetThis(value);
        }
        /// <summary>
        /// Include system pages such as modules management.
        /// Default is `false`
        /// </summary>
        [Configuration]
        public bool IncludeSystem
        {
            get => Configuration.GetThis(false);
            set => Configuration.SetThis(value);
        }
        /// <summary>
        /// Include link-reference pages (which are usually used in menus, and not themselves a real page).
        /// Default is `true`
        /// </summary>
        [Configuration]
        public bool IncludeLinks
        {
            get => Configuration.GetThis(true);
            set => Configuration.SetThis(value);
        }
        /// <summary>
        /// Require that the current user has view permissions on all pages.
        /// Default is `true`
        /// </summary>
        [Configuration]
        public bool RequireViewPermissions
        {
            get => Configuration.GetThis(true);
            set => Configuration.SetThis(value);
        }
        /// <summary>
        /// Require that the current user has edit permissions on all pages.
        /// Default is `false`
        /// </summary>
        [Configuration]
        public bool RequireEditPermissions
        {
            get => Configuration.GetThis(false);
            set => Configuration.SetThis(value);
        }
        #endregion

        #region Constructor

        [PrivateApi]
        public Pages(MyServices services, PagesDataSourceProvider provider, IDataFactory dataFactory, ITreeMapper treeMapper) : base(services, "CDS.Pages")
        {
            ConnectServices(
                _provider = provider,
                _pageFactory = dataFactory.New(settings: PageDataRaw.Setting),
                _treeMapper = treeMapper
            );
            Provide(GetPages);
        }
        #endregion

        [PrivateApi]
        public IImmutableList<IEntity> GetPages() => Log.Func(l =>
        {
            Configuration.Parse();

            // Get pages from underlying system/provider
            var pagesFromSystem = _provider.GetPagesInternal(
                includeHidden: IncludeHidden,
                includeDeleted: IncludeDeleted,
                includeAdmin: IncludeAdmin,
                includeSystem: IncludeSystem,
                includeLinks: IncludeLinks,
                requireViewPermissions: RequireViewPermissions,
                requireEditPermissions: RequireEditPermissions
            );
            if (pagesFromSystem == null || !pagesFromSystem.Any())
                return (EmptyList, "null/empty");

            // Convert to Entity-Stream
            var pages = _pageFactory.Create(pagesFromSystem);

            // Try to add Navigation properties
            try
            {
                var asTree = ((TreeMapper)_treeMapper).AddParentChild(pages, "EntityId", "ParentId");
                return (asTree, $"As Tree: {asTree.Count}");
            }
            catch (Exception ex)
            {
                l.Ex(ex);
                return (pages, $"Just pages (tree had error): {pages.Count}");
            }
        });
    }
}
