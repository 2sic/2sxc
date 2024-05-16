using System.Collections.Immutable;
using ToSic.Eav.Apps.Internal.Work;
using ToSic.Eav.DataSource;
using ToSic.Eav.DataSource.Internal;
using ToSic.Eav.DataSource.VisualQuery;
using ToSic.Eav.Services;
using ToSic.Lib.DI;
using ToSic.Lib.Helpers;
using ToSic.Lib.Services;
using ToSic.Sxc.Apps.Internal.Work;
using ToSic.Sxc.Blocks.Internal;
using ToSic.Sxc.Context;
using IEntity = ToSic.Eav.Data.IEntity;

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
    NameId = "ToSic.Sxc.DataSources.CmsBlock, ToSic.Sxc",
    ConfigurationType = "7c2b2bc2-68c6-4bc3-ba18-6e6b5176ba02",
    In = [DataSourceConstants.StreamDefaultName],
    HelpLink = "https://docs.2sxc.org/api/dot-net/ToSic.Sxc.DataSources.CmsBlock.html",
    NameIds = ["ToSic.SexyContent.DataSources.ModuleDataSource, ToSic.SexyContent"])]
public sealed partial class CmsBlock : DataSourceBase
{
    [PrivateApi] internal const string InstanceLookupName = "module";
    [PrivateApi] internal const string ModuleIdKey = "Id";
    [PrivateApi] internal const string FieldInstanceId = "InstanceId";

    /// <summary>
    /// The instance-id of the CmsBlock (2sxc instance, DNN ModId). <br/>
    /// It's named Instance-Id to be more neutral as we're opening it to other platforms
    /// </summary>
    [Configuration(Field = FieldInstanceId, Fallback = "[" + InstanceLookupName + ":" + ModuleIdKey + "]")]
    public int? ModuleId
    {
        get => _moduleId ?? (int.TryParse(Configuration.GetThis(), out var listId) ? listId : new int?());
        set => _moduleId = value;
    }
    private int? _moduleId;

    #region Constructor

    public new class MyServices: MyServicesBase<DataSourceBase.MyServices>
    {
        public GenWorkPlus<WorkBlocks> AppBlocks { get; }
        public LazySvc<IModule> ModuleLazy { get; }
        public LazySvc<IDataSourcesService> DataSourceFactory { get; }

        public MyServices(DataSourceBase.MyServices parentServices,
            LazySvc<IModule> moduleLazy,
            LazySvc<IDataSourcesService> dataSourceFactory,
            GenWorkPlus<WorkBlocks> appBlocks
        ) : base(parentServices)
        {
            ConnectLogs([
                ModuleLazy = moduleLazy,
                DataSourceFactory = dataSourceFactory,
                AppBlocks = appBlocks
            ]);
        }
    }

    public CmsBlock(MyServices services): base(services, $"SDS.CmsBks")
    {
        _services = services;

        ProvideOut(GetContent);
        ProvideOut(GetHeader, ViewParts.StreamHeader);
        ProvideOut(GetHeader, ViewParts.StreamHeaderOld);
    }
    private readonly MyServices _services;
    #endregion

    public override IDataSourceLink Link => _link.Get(() => BreachExtensions.CreateEmptyLink(this) // new DataSourceLink(null, dataSource: this)
        .AddStream(name: ViewParts.StreamHeader)
        .AddStream(name: ViewParts.StreamHeaderOld));
    private readonly GetOnce<IDataSourceLink> _link = new();


    private IImmutableList<IEntity> GetContent()
    {
        // First check if BlockConfiguration works - to give better error if not
        var blockSpecsAndErrors = ConfigAndViewOrErrors;
        if (blockSpecsAndErrors.IsError)
            return blockSpecsAndErrors.Errors;

        var parts = blockSpecsAndErrors.Result;
        return GetStream(parts.View, parts.BlockConfiguration.Content, parts.View.ContentItem,
            parts.BlockConfiguration.Presentation, parts.View.PresentationItem, false);
    }

    private IImmutableList<IEntity> GetHeader()
    {
        // First check if BlockConfiguration works - to give better error if not
        var blockSpecsAndErrors = ConfigAndViewOrErrors;
        if (blockSpecsAndErrors.IsError)
            return blockSpecsAndErrors.Errors;

        var parts = blockSpecsAndErrors.Result;


        return GetStream(parts.View, parts.BlockConfiguration.Header, parts.View.HeaderItem,
            parts.BlockConfiguration.HeaderPresentation, parts.View.HeaderPresentationItem, true);
    }
}