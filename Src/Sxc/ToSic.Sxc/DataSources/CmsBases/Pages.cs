using System.Collections.Immutable;
using ToSic.Eav.Data.Build;
using ToSic.Eav.DataSource;
using ToSic.Eav.DataSource.Internal;
using ToSic.Eav.DataSource.VisualQuery;
using ToSic.Eav.DataSources.Internal;
using ToSic.Sxc.DataSources.Internal;
using static ToSic.Eav.DataSource.Internal.DataSourceConstants;

// Important Info to people working with this
// It depends on abstract provider, that must be overriden in each platform
// In addition, each platform must make sure to register a TryAddTransient with the platform specific provider implementation
// This is because any constructor DI should be able to target this type, and get the real provider implementation

namespace ToSic.Sxc.DataSources;

/// <summary>
/// Deliver a list of pages from the current platform (Dnn or Oqtane).
///
/// To figure out the properties returned and what they match up to, see <see cref="PageDataRaw"/>
/// </summary>
[PublicApi]
[VisualQuery(
    ConfigurationType = "3d970d2b-32cb-4ecb-aeaf-c49fbcc678a5",
    NameId = "e35031b2-3e99-41fe-a5ac-b79f447d5800",
    HelpLink = "https://go.2sxc.org/ds-pages",
    Icon = DataSourceIcons.PageFind,
    NiceName = "Pages",
    Type = DataSourceType.Source,
    UiHint = "Pages in this site")]
public class Pages: CustomDataSourceAdvanced
{
    private readonly ITreeMapper _treeMapper;
    private readonly IDataFactory _pageFactory;
    private readonly PagesDataSourceProvider _provider;

    #region Configuration properties

    /// <summary>
    /// Include hidden pages.
    /// Default is `false`
    /// </summary>
    /// <remarks>
    /// * new in 15.04
    /// * uses the [immutable convention](xref:NetCode.Conventions.Immutable)
    /// </remarks>
    [Configuration]
    public bool IncludeHidden => Configuration.GetThis(false);

    /// <summary>
    /// Include deleted pages in the recycle bin.
    /// Default is `false`
    /// </summary>
    /// <remarks>
    /// * new in 15.04
    /// * uses the [immutable convention](xref:NetCode.Conventions.Immutable)
    /// </remarks>
    [Configuration]
    public bool IncludeDeleted => Configuration.GetThis(false);

    /// <summary>
    /// Include admin pages such as site files.
    /// Default is `false`
    /// </summary>
    /// <remarks>
    /// * new in 15.04
    /// * uses the [immutable convention](xref:NetCode.Conventions.Immutable)
    /// </remarks>
    [Configuration]
    public bool IncludeAdmin => Configuration.GetThis(false);

    /// <summary>
    /// Include system pages such as modules management.
    /// Default is `false`
    /// </summary>
    /// <remarks>
    /// * new in 15.04
    /// * uses the [immutable convention](xref:NetCode.Conventions.Immutable)
    /// </remarks>
    [Configuration]
    public bool IncludeSystem => Configuration.GetThis(false);

    /// <summary>
    /// Include link-reference pages (which are usually used in menus, and not themselves a real page).
    /// Default is `true`
    /// </summary>
    /// <remarks>
    /// * new in 15.04
    /// * uses the [immutable convention](xref:NetCode.Conventions.Immutable)
    /// </remarks>
    [Configuration]
    public bool IncludeLinks => Configuration.GetThis(true);

    /// <summary>
    /// Require that the current user has view permissions on all pages.
    /// Default is `true`
    /// </summary>
    /// <remarks>
    /// * new in 15.04
    /// * uses the [immutable convention](xref:NetCode.Conventions.Immutable)
    /// </remarks>
    [Configuration]
    public bool RequireViewPermissions => Configuration.GetThis(true);

    /// <summary>
    /// Require that the current user has edit permissions on all pages.
    /// Default is `false`
    /// </summary>
    /// <remarks>
    /// * new in 15.04
    /// * uses the [immutable convention](xref:NetCode.Conventions.Immutable)
    /// </remarks>
    [Configuration]
    public bool RequireEditPermissions => Configuration.GetThis(false);

    #endregion

    #region Constructor

    [PrivateApi]
    public Pages(MyServices services, PagesDataSourceProvider provider, IDataFactory dataFactory, ITreeMapper treeMapper) : base(services, "CDS.Pages")
    {
        ConnectLogs([
            _provider = provider,
            _pageFactory = dataFactory.New(options: PageDataRaw.Option),
            _treeMapper = treeMapper
        ]);
        ProvideOut(GetPages);
    }
    #endregion

    private IImmutableList<IEntity> GetPages() => Log.Func(l =>
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
            var asTree = _treeMapper.AddParentChild(pages, Attributes.EntityIdPascalCase, "ParentId");
            return (asTree, $"As Tree: {asTree.Count}");
        }
        catch (Exception ex)
        {
            l.Ex(ex);
            return (pages, $"Just pages (tree had error): {pages.Count}");
        }
    });
}