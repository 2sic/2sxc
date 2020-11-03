using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ToSic.Eav;
using ToSic.Eav.DataSources;
using ToSic.Eav.Documentation;
using ToSic.Eav.Logging;
using ToSic.Eav.LookUp;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Data;
using ToSic.Sxc.DataSources;
using ToSic.Sxc.Edit.InPageEditingSystem;
using ToSic.Sxc.Run.Context;
using ToSic.Sxc.Web;
using DynamicJacket = ToSic.Sxc.Data.DynamicJacket;
using IApp = ToSic.Sxc.Apps.IApp;
using IEntity = ToSic.Eav.Data.IEntity;
using IFolder = ToSic.Sxc.Adam.IFolder;

namespace ToSic.Sxc.Code
{
    /// <summary>
    /// Base class for any dynamic code root objects. <br/>
    /// Root objects are the ones compiled by 2sxc - like the RazorComponent or ApiController. <br/>
    /// If you create code for dynamic compilation, you'll always inherit from ToSic.Sxc.Dnn.DynamicCode. 
    /// Note that other DynamicCode objects like RazorComponent or ApiController reference this object for all the interface methods of <see cref="IDynamicCode"/>.
    /// </summary>
    [PublicApi_Stable_ForUseInYourCode]
    public /*abstract*/ class DynamicCodeRoot : HasLog, IDynamicCode
    {
        public IBlock Block { get; private set; }

        public DynamicCodeRoot(): base("Sxc.DynCdR") { }
        protected DynamicCodeRoot(string logName): base(logName) { }

        [PrivateApi]
        public DynamicCodeRoot Init(IBlock block, ILog parentLog, int compatibility = 10)
        {
            Log.LinkTo(parentLog ?? block?.Log);
            if (block == null)
                return this;

            CompatibilityLevel = compatibility;
            Block = block;
            App = Block.App;
            Data = Block.Data;
            Edit = new InPageEditingHelper(Block, Log);
            return this;
        }

        [PrivateApi]
        internal void LateAttachApp(IApp app) => App = app;

        [PrivateApi]
        public int CompatibilityLevel { get; private set; }

        /// <inheritdoc />
        public IApp App { get; private set; }

        /// <inheritdoc />
        public IBlockDataSource Data { get; private set; }

        /// <inheritdoc />
        public ILinkHelper Link { get; protected set; }


        #region AsDynamic Implementations

        /// <inheritdoc />
        public dynamic AsDynamic(string json, string fallback = DynamicJacket.EmptyJson) => DynamicJacket.AsDynamicJacket(json, fallback);

        /// <inheritdoc />
        public dynamic AsDynamic(IEntity entity) 
            => new DynamicEntity(entity, new[] { Thread.CurrentThread.CurrentCulture.Name }, CompatibilityLevel, Block);

        /// <inheritdoc />
        public dynamic AsDynamic(dynamic dynamicEntity) => dynamicEntity;


        /// <inheritdoc />
        public IEntity AsEntity(dynamic dynamicEntity) => ((IDynamicEntity) dynamicEntity).Entity;

        #endregion

        #region AsList

        /// <inheritdoc />
        public IEnumerable<dynamic> AsList(dynamic list)
        {
            switch (list)
            {
                case null:
                    return new List<dynamic>();
                case IDataSource dsEntities:
                    return AsList(dsEntities[Eav.Constants.DefaultStreamName]);
                case IEnumerable<IEntity> iEntities:
                    return iEntities.Select(e => AsDynamic(e));
                case IEnumerable<dynamic> dynEntities:
                    return dynEntities;
                default:
                    return null;
            }
        }

        #endregion

        #region DataSource and ConfigurationProvider (for DS) section
        private ILookUpEngine _configurationProvider;

        internal ILookUpEngine ConfigurationProvider
            => _configurationProvider ??
               (_configurationProvider = Data.Configuration.LookUps);

        internal DataSourceFactory DataSourceFactory => _dataSourceFactory ??
                                                        (_dataSourceFactory = Block?.Context?.ServiceProvider
                                                            .Build<DataSourceFactory>().Init(Log));// new DataSource(Log));
        private DataSourceFactory _dataSourceFactory;



        /// <inheritdoc />
        public T CreateSource<T>(IDataSource inSource = null, ILookUpEngine configurationProvider = null) where T : IDataSource
        {
            if (configurationProvider == null)
                configurationProvider = ConfigurationProvider;

            if (inSource != null)
                return DataSourceFactory.GetDataSource<T>(inSource, inSource, configurationProvider);

            var userMayEdit = Block.EditAllowed;

            var initialSource = DataSourceFactory.GetPublishing(
                App, userMayEdit, ConfigurationProvider as LookUpEngine);
            return DataSourceFactory.GetDataSource<T>(initialSource, initialSource, configurationProvider);
        }

        /// <inheritdoc />
        public T CreateSource<T>(IDataStream inStream) where T : IDataSource
        {
            // if it has a source, then use this, otherwise it's null and that works too. Reason: some sources like DataTable or SQL won't have an upstream source
            var src = CreateSource<T>(inStream.Source);

            var srcDs = (IDataTarget)src;
            srcDs.In.Clear();
            srcDs.In.Add(Eav.Constants.DefaultStreamName, inStream);
            return src;
        }
        #endregion

        #region basic properties like Content, Presentation, ListContent, ListPresentation

        /// <inheritdoc />
        public dynamic Content
        {
            get
            {
                if(_content == null) TryToBuildContent();
                return _content;
            } 
        }
        private dynamic _content;


        /// <inheritdoc />
		public dynamic Header 
        {
            get
            {
                if(_header == null) TryToBuildHeaderObject();
                return _header;
            } 
        }
        private dynamic _header;

        /// <remarks>
        /// This must be lazy-loaded, otherwise initializing the AppAndDataHelper will break when the Data-object fails 
        /// - this would break API even though the List etc. are never accessed
        /// </remarks>
        private void TryToBuildHeaderObject()
        {
            Log.Add("try to build ListContent (header) object");
            if (Data == null || Block.View == null) return;
            if (!Data.Out.ContainsKey(ViewParts.ListContent)) return;

            var listEntity = Data[ViewParts.ListContent].Immutable.FirstOrDefault();
            _header = listEntity == null ? null : AsDynamic(listEntity);
        }

#pragma warning disable 618
        //[PrivateApi]
        //[Obsolete("This is an old way used to loop things - shouldn't be used any more - will be removed in 2sxc v10")]
        //public List<Element> List {
        //    get
        //    {
        //        if(_list == null) TryToBuildContentAndList();
        //        return _list;
        //    } 
        //}
        //private List<Element> _list;

        /// <remarks>
        /// This must be lazy-loaded, otherwise initializing the AppAndDataHelper will break when the Data-object fails 
        /// - this would break API even though the List etc. are never accessed
        /// </remarks>
        [PrivateApi]
        internal void TryToBuildContent()
        {
            Log.Add("try to build Content objects");

            if (Data == null || Block.View == null) return;
            if (!Data.Out.ContainsKey(Eav.Constants.DefaultStreamName)) return;

            var entities = Data.Immutable; //.ToList();
            if (entities.Any()) _content = AsDynamic(entities.First());

        }
#pragma warning restore 618

        #endregion

        #region Adam

        /// <inheritdoc />
        public IFolder AsAdam(IDynamicEntity entity, string fieldName)
            => AsAdam(AsEntity(entity), fieldName);


        /// <inheritdoc />
        public IFolder AsAdam(IEntity entity, string fieldName)
        {
            if (_adamAppContext == null) 
                _adamAppContext = Factory.Resolve<AdamAppContext>()
                    .Init(Block.Context.Tenant, App, Block, CompatibilityLevel, Log);
            return _adamAppContext.FolderOfField(entity.EntityGuid, fieldName);
        }
        private AdamAppContext _adamAppContext;

        #endregion
        
        #region Edit

        /// <inheritdoc />
        public IInPageEditingSystem Edit { get; private set; }
        #endregion

        #region SharedCode Compiler

        /// <inheritdoc />
        public virtual dynamic CreateInstance(string virtualPath,
            string dontRelyOnParameterOrder = Eav.Constants.RandomProtectionParameter,
            string name = null,
            string relativePath = null,
            bool throwOnError = true)
        {
            var wrap = Log.Call<dynamic>($"{virtualPath}, {name}, {relativePath}, {throwOnError}");
            Eav.Constants.ProtectAgainstMissingParameterNames(dontRelyOnParameterOrder, "CreateInstance",
                $"{nameof(name)},{nameof(throwOnError)}");

            // Compile
            var instance = new CodeCompiler(Log)
                .InstantiateClass(virtualPath, name, relativePath, throwOnError);

            // if it supports all our known context properties, attach them
            if (instance is ICoupledDynamicCode isShared)
            {
                isShared.DynamicCodeCoupling(this);
            }

            return wrap((instance != null).ToString(), instance);
        }

        /// <inheritdoc />
        public string CreateInstancePath { get; set; }

        #endregion

        #region Context WIP

        [PrivateApi] public RunContext RunContext => _runContext ?? (_runContext = Factory.Resolve<RunContext>().Init(this));
        private RunContext _runContext;

        #endregion
    }
}