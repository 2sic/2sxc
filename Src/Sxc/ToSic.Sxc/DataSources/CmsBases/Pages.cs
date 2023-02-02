using System;
using System.Collections.Immutable;
using System.Linq;
using ToSic.Eav.Data;
using ToSic.Eav.Data.Builder;
using ToSic.Eav.DataSources;
using ToSic.Eav.DataSources.Queries;
using ToSic.Lib.Documentation;
using ToSic.Lib.Logging;

// Important Info to people working with this
// It's an abstract class, and must be overriden in each platform
// In addition, each platform must make sure to register a TryAddTransient with the platform specific implementation
// This is because any constructor DI should be able to target this type, and get the real implementation

namespace ToSic.Sxc.DataSources
{
    /// <summary>
    /// Deliver a list of pages from the current platform (Dnn or Oqtane).
    ///
    /// As of now there are no parameters to set.
    ///
    /// To figure out the properties returned and what they match up to, see <see cref="CmsPageInfo"/>
    /// </summary>
    [PublicApi]
    [VisualQuery(
        ExpectsDataOfType = "",
        GlobalName = "e35031b2-3e99-41fe-a5ac-b79f447d5800",
        HelpLink = "https://r.2sxc.org/ds-pages",
        Icon = Icons.PageFind,
        NiceName = "Pages",
        Type = DataSourceType.Source,
        UiHint = "Pages in this site")]
    public class Pages: ExternalData
    {
        private readonly MultiBuilder _multiBuilder;
        private readonly PagesDataSourceProvider _provider;

        #region Configuration properties

        public bool IncludeHidden
        {
            get => Configuration.GetThis(false);
            set => Configuration.SetThis(value);
        }
        public bool IncludeDeleted
        {
            get => Configuration.GetThis(false);
            set => Configuration.SetThis(value);
        }
        public bool IncludeAdmin
        {
            get => Configuration.GetThis(false);
            set => Configuration.SetThis(value);
        }
        public bool IncludeSystem
        {
            get => Configuration.GetThis(false);
            set => Configuration.SetThis(value);
        }
        public bool IncludeLinks
        {
            get => Configuration.GetThis(true);
            set => Configuration.SetThis(value);
        }
        public bool RequireViewPermissions
        {
            get => Configuration.GetThis(true);
            set => Configuration.SetThis(value);
        }
        public bool RequireEditPermissions
        {
            get => Configuration.GetThis(false);
            set => Configuration.SetThis(value);
        }

        #endregion

        #region Constructor

        [PrivateApi]
        public Pages(Dependencies dependencies, PagesDataSourceProvider provider, MultiBuilder multiBuilder) : base(dependencies, "CDS.Pages")
        {
            ConnectServices(
                _provider = provider,
                _multiBuilder = multiBuilder
            );
            Provide(GetPages);
            ConfigMask(nameof(IncludeHidden));
            ConfigMask(nameof(IncludeDeleted));
            ConfigMask(nameof(IncludeAdmin));
            ConfigMask(nameof(IncludeSystem));
            ConfigMask(nameof(IncludeLinks));
            ConfigMask(nameof(RequireViewPermissions));
            ConfigMask(nameof(RequireEditPermissions));
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
                return (new ImmutableArray<IEntity>(), "null/empty");

            // Convert to Entity-Stream
            var builder = new DataBuilderQuickWIP(DataBuilder, typeName: "Page", titleField: nameof(CmsPageInfo.Name));
            var pages = pagesFromSystem
                .Select(p => builder.Create(p))
                .ToImmutableList();

            // Try to add Navigation properties
            try
            {
                var treeMapper = new TreeMapper<int>(_multiBuilder, Log);
                var asTree =
                    treeMapper.GetEntitiesWithRelationships(pages, "EntityId", "ParentId", "Children", "Parent");
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
