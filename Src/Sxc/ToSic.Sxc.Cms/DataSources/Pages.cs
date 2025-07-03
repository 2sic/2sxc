using System.Collections.Immutable;
using ToSic.Eav.DataSource;
using ToSic.Eav.DataSource.Internal;
using ToSic.Eav.DataSource.VisualQuery;
using ToSic.Sxc.Cms.Pages;
using ToSic.Sxc.Cms.Pages.Sys;
using ToSic.Sxc.Cms.Users;
using ToSic.Sxc.DataSources.Sys.Pages;

// Important Info to people working with this
// It depends on abstract provider, that must be overriden in each platform
// In addition, each platform must make sure to register a TryAddTransient with the platform specific provider implementation
// This is because any constructor DI should be able to target this type, and get the real provider implementation

namespace ToSic.Sxc.DataSources;

/// <summary>
/// Get a list of pages from the current platform (Dnn or Oqtane).
/// </summary>
/// <remarks>
/// You can cast the result to <see cref="IPageModel"/> for typed use in your code.
/// To figure out the returned properties, best also consult the <see cref="IPageModel"/>.
/// 
/// 
/// History
/// 
/// * Created ca. v.16 early 2023 but not officially communicated
/// * Models <see cref="IUserModel"/> and <see cref="IUserRoleModel"/> created in v19.01 and officially released
/// </remarks>
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
    public Pages(Dependencies services, PagesDataSourceProvider provider) : base(services, "CDS.Pages", connect: [provider])
    {
        _provider = provider;

        ProvideOut(GetPages);
    }
    #endregion

    private IImmutableList<IEntity> GetPages()
    {
        var l = Log.Fn<IImmutableList<IEntity>>();
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

        if (pagesFromSystem == null || pagesFromSystem.Count == 0)
            return l.Return([], "null/empty");

        // Convert to Entity-Stream
        var pageFactory = DataFactory.SpawnNew(options: PageModelRaw.Option);

        var pages = pageFactory.Create(pagesFromSystem);

        return l.Return(pages, $"{pages.Count}");
        //// Try to add Navigation properties
        //try
        //{
        //    var asTree = _treeMapper.AddParentChild(pages, Attributes.EntityIdPascalCase, "ParentId");
        //    return l.Return(asTree, $"As Tree: {asTree.Count}");
        //}
        //catch (Exception ex)
        //{
        //    l.Ex(ex);
        //    return l.Return(pages, $"Just pages (tree had error): {pages.Count}");
        //}
    }
}