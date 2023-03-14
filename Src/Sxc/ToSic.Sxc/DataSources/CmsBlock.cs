using System.Collections.Immutable;
using ToSic.Eav.DataSources;
using ToSic.Eav.DataSources.Queries;
using ToSic.Lib.DI;
using ToSic.Lib.Documentation;
using ToSic.Lib.Services;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Apps.Blocks;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Context;
using IEntity = ToSic.Eav.Data.IEntity;

namespace ToSic.Sxc.DataSources
{
    /// <summary>
    /// This data-source delivers the core data for a CMS Block. <br/>
    /// It will look up the configuration in the CMS (like the Module-Settings in DNN) to determine what data is needed for the block. <br/>
    /// Usually it will then find a reference to a ContentBlock, from which it determines what content-items are assigned. <br/>
    /// It could also find that the template specifies a query, in which case it would retrieve that. <br/>
    /// <em>Was previously called ModuleDataSource</em>
    /// </summary>
    [PublicApi_Stable_ForUseInYourCode]
    [VisualQuery(
        NiceName = "CMS Block",
        UiHint = "Data for this CMS Block (instance/module)",
        Icon = Icons.RecentActor,
        Type = DataSourceType.Source, 
        GlobalName = "ToSic.Sxc.DataSources.CmsBlock, ToSic.Sxc",
        ExpectsDataOfType = "7c2b2bc2-68c6-4bc3-ba18-6e6b5176ba02",
        In = new []{DataSourceConstants.StreamDefaultName},
        HelpLink = "https://docs.2sxc.org/api/dot-net/ToSic.Sxc.DataSources.CmsBlock.html",
        PreviousNames = new []{ "ToSic.SexyContent.DataSources.ModuleDataSource, ToSic.SexyContent" })]
    public sealed partial class CmsBlock : DataSource
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
            get => int.TryParse(Configuration.GetThis(), out var listId) ? listId : new int?();
            set => Configuration.SetThis(value);
        }

        #region Constructor

        public new class MyServices: MyServicesBase<DataSource.MyServices>
        {
            public LazySvc<CmsRuntime> LazyCmsRuntime { get; }
            public LazySvc<IModule> ModuleLazy { get; }
            public LazySvc<IDataSourceFactory> DataSourceFactory { get; }

            public MyServices(DataSource.MyServices parentServices,
                LazySvc<CmsRuntime> lazyCmsRuntime,
                LazySvc<IModule> moduleLazy,
                LazySvc<IDataSourceFactory> dataSourceFactory) : base(parentServices)
            {
                ConnectServices(
                    LazyCmsRuntime = lazyCmsRuntime,
                    ModuleLazy = moduleLazy,
                    DataSourceFactory = dataSourceFactory
                );
            }
        }

        public CmsBlock(MyServices services): base(services, $"SDS.CmsBks")
        {
            _services = services;

            Provide(GetContent);
            Provide(GetHeader, ViewParts.StreamHeader);
            Provide(GetHeader, ViewParts.StreamHeaderOld);
        }
        private readonly MyServices _services;
        #endregion


        private IImmutableList<IEntity> GetContent()
        {
            // First check if BlockConfiguration works - to give better error if not
            var everything = Everything;
            if (everything.IsError)
                return everything.Errors;

            var parts = everything.Result;
            return GetStream(parts.BlockConfiguration, parts.BlockConfiguration.Content, parts.View.ContentItem,
                parts.BlockConfiguration.Presentation, parts.View.PresentationItem, false);
        }

        private IImmutableList<IEntity> GetHeader()
        {
            // First check if BlockConfiguration works - to give better error if not
            var everything = Everything;
            if (everything.IsError)
                return everything.Errors;

            var parts = everything.Result;


            return GetStream(parts.BlockConfiguration, parts.BlockConfiguration.Header, parts.View.HeaderItem,
                parts.BlockConfiguration.HeaderPresentation, parts.View.HeaderPresentationItem, true);
        }
    }
}