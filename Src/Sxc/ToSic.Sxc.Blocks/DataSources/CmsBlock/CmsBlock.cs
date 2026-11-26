using System.Collections.Immutable;
using ToSic.Eav.DataSource;
using ToSic.Eav.DataSource.Sys;
using ToSic.Eav.DataSource.VisualQuery;
using ToSic.Eav.Services;
using ToSic.Sxc.Blocks.Sys;
using ToSic.Sxc.Blocks.Sys.Views;
using ToSic.Sxc.Blocks.Sys.Work;
using ToSic.Sxc.Context;


namespace ToSic.Sxc.DataSources;

/// <summary>
/// This data-source delivers the core data for a CMS Block. <br/>
/// It will look up the configuration in the CMS (like the Module-Settings in DNN) to determine what data is needed for the block. <br/>
/// Usually it will then find a reference to a ContentBlock, from which it determines what content-items are assigned. <br/>
/// It could also find that the template specifies a query, in which case it would retrieve that. <br/>
/// <em>Was previously called ModuleDataSource</em>
/// </summary>
[PublicApi]
[VisualQuery(
    NiceName = "CMS Block",
    UiHint = "Data for this CMS Block (instance/module)",
    Icon = DataSourceIcons.RecentActor,
    Type = DataSourceType.Source, 
    NameId = "ce46fcac-e531-45a2-adc5-181164e742f6",
    NameIds =
    [
        "ToSic.Sxc.DataSources.CmsBlock, ToSic.Sxc",
        "ToSic.SexyContent.DataSources.ModuleDataSource, ToSic.SexyContent"
    ],
    ConfigurationType = "7c2b2bc2-68c6-4bc3-ba18-6e6b5176ba02",
    In = [DataSourceConstants.StreamDefaultName],
    HelpLink = "https://docs.2sxc.org/api/dot-net/ToSic.Sxc.DataSources.CmsBlock.html")]
public sealed partial class CmsBlock(
    DataSourceBase.Dependencies parentServices,
    LazySvc<IModuleFactory> moduleLazy,
    LazySvc<IDataSourcesService> dataSourceFactory,
    AppWorkQuick<WorkBlocks> appBlocks)
    : DataSourceBase(parentServices, $"SDS.CmsBks", callSetup: true, connect: [moduleLazy, dataSourceFactory, appBlocks]), IDataSourceLinkable
{
    #region Constructor / Setup (called back)

    protected override void SetupProvideOut()
    {
        ProvideOut(GetContent);
        ProvideOut(GetHeader, ViewParts.StreamHeader);
        ProvideOut(GetHeader, ViewParts.StreamHeaderOld);
    }

    #endregion

    /// <summary>
    /// The instance-id of the CmsBlock (2sxc instance, DNN ModId). <br/>
    /// It's named Instance-Id to be more neutral as we're opening it to other platforms
    /// </summary>
    [Configuration(Field = BlockInstanceConstants.FieldInstanceId,
        Fallback = $"[{BlockInstanceConstants.InstanceLookupName}:{BlockInstanceConstants.ModuleIdKey}]")]
    public int? ModuleId
    {
        get => field ?? (int.TryParse(Configuration.GetThis(), out var listId)
            ? listId
            : new int?());
        set;
    }


    /// <summary>
    /// Special link which when connecting will connect all the names incl. the old header name 
    /// </summary>
    /// <remarks>
    /// 2026-11-26 2dm: this has been here for a long time, I'm not 100% sure for what.
    /// My guess is that the CmsBlock is passed through PassThrough data sources, which needs to get the full list of links?
    /// But that may also happen automatically, so I'm not sure if we need this.
    /// </remarks>
    /// <returns></returns>
    IDataSourceLink IDataSourceLinkable.GetLink() => _link
        ??= BreachExtensions.CreateEmptyLink(this)
            .WithAnotherStream(name: ViewParts.StreamHeader)
            .WithAnotherStream(name: ViewParts.StreamHeaderOld);

    private IDataSourceLink? _link;


    private IImmutableList<IEntity> GetContent()
    {
        // First check if BlockConfiguration works - to give better error if not
        var blockSpecsAndErrors = ConfigAndViewOrErrors;
        if (!blockSpecsAndErrors.IsOk)
            return blockSpecsAndErrors.ErrorsSafe();

        var (blockConfig, view) = blockSpecsAndErrors.Result;
        return GetStream(view,
            blockConfig.Content,
            view.ContentItem,
            blockConfig.Presentation,
            view.PresentationItem,
            false
        );
    }

    private IImmutableList<IEntity> GetHeader()
    {
        // First check if BlockConfiguration works - to give better error if not
        var blockSpecsAndErrors = ConfigAndViewOrErrors;
        if (!blockSpecsAndErrors.IsOk)
            return blockSpecsAndErrors.ErrorsSafe();

        var (blockConfig, view) = blockSpecsAndErrors.Result;
        
        return GetStream(view,
            blockConfig.Header,
            view.HeaderItem,
            blockConfig.HeaderPresentation,
            view.HeaderPresentationItem,
            true
        );
    }
}