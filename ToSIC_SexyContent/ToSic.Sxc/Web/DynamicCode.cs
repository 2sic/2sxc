using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ToSic.Eav;
using ToSic.Eav.Data;
using ToSic.Eav.DataSources;
using ToSic.Eav.Documentation;
using ToSic.Eav.Logging;
using ToSic.Eav.LookUp;
using ToSic.Eav.Run;
using ToSic.SexyContent;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Code;
using ToSic.Sxc.Data;
using ToSic.Sxc.DataSources;
using ToSic.Sxc.Edit.InPageEditingSystem;
using DynamicJacket = ToSic.Sxc.Data.DynamicJacket;
using IApp = ToSic.Sxc.Apps.IApp;
using ICmsBlock = ToSic.Sxc.Blocks.ICmsBlock;
using IEntity = ToSic.Eav.Data.IEntity;
using IFolder = ToSic.Sxc.Adam.IFolder;

namespace ToSic.Sxc.Web
{
    /// <summary>
    /// Base class for any dynamic code.
    /// Note that other DynamicCode objects like RazorComponent or ApiController reference this object for all the interface methods of <see cref="IDynamicCode"/>.
    /// </summary>
    [PublicApi]
    public abstract class DynamicCode : HasLog, IDynamicCode
    {
        [PrivateApi]
        protected readonly ICmsBlock CmsInstance;

        private readonly ITenant _tenant;
        [PrivateApi]
        protected DynamicCode(ICmsBlock cmsInstance, ITenant tenant, ILog parentLog): base("Sxc.AppHlp", parentLog ?? cmsInstance?.Log)
        {
            if (cmsInstance == null)
                return;

            CmsInstance = cmsInstance;
            _tenant = tenant;
            App = cmsInstance.App;
            Data = cmsInstance.Block.Data;
			Sxc = new SxcHelper(cmsInstance);
            Edit = new InPageEditingHelper(cmsInstance, Log);
        }

        [PrivateApi]
        internal void LateAttachApp(IApp app) => App = app;


        /// <inheritdoc />
        public IApp App { get; private set; }

        /// <inheritdoc />
        public IBlockDataSource Data { get; }

        [PrivateApi]
		public SxcHelper Sxc { get; }

        /// <inheritdoc />
        public ILinkHelper Link { get; protected set; }


        #region AsDynamic Implementations

        /// <inheritdoc />
        public dynamic AsDynamic(string json, string fallback = DynamicJacket.EmptyJson) => DynamicJacket.AsDynamicJacket(json, fallback);

        /// <inheritdoc />
        public dynamic AsDynamic(IEntity entity) => new DynamicEntity(entity, new[] { Thread.CurrentThread.CurrentCulture.Name }, CmsInstance);

        [PrivateApi]
        [Obsolete("for compatibility only, avoid using this and cast your entities to ToSic.Eav.Data.IEntity")]
        public dynamic AsDynamic(Eav.Interfaces.IEntity entity) => AsDynamic(entity as IEntity);


        /// <inheritdoc />
        public dynamic AsDynamic(dynamic dynamicEntity) => dynamicEntity;

        [PrivateApi]
        [Obsolete("for compatibility only, avoid using this and cast your entities to ToSic.Eav.Data.IEntity")]
        public dynamic AsDynamic(KeyValuePair<int, Eav.Interfaces.IEntity> entityKeyValuePair) => AsDynamic(entityKeyValuePair.Value);

        /// <inheritdoc />
        public IEntity AsEntity(dynamic dynamicEntity) => ((IDynamicEntity) dynamicEntity).Entity;

        /// <inheritdoc />
        public IEnumerable<dynamic> AsDynamic(IEnumerable<IEntity> entities) => entities.Select(e => AsDynamic(e));


        [PrivateApi]
        [Obsolete("for compatibility only, avoid using this and cast your entities to ToSic.Eav.Data.IEntity")]
        public IEnumerable<dynamic> AsDynamic(IEnumerable<Eav.Interfaces.IEntity> entities) => entities.Select(e => AsDynamic(e));

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
                    return AsDynamic(iEntities);
                case IEnumerable<dynamic> dynEntities:
                    return dynEntities;
                //case IDynamicEntity iDynamicEntity:
                //    if(name == null)
                //        throw new Exception("AsList got a DynamicEntity but not a name. You must either provide list of DynamicEntities or add a name to access that property.");
                //    return AsList(iDynamicEntity.Get(name));
                //case IEntity iEntity:
                //    if(name == null)
                //        throw new Exception("AsList got an IEntity but not a name. You must either provide list of DynamicEntities or add a name to access that property.");
                //    return AsList(iEntity.Children(name));
                default:
                    return null;
            }
        }

        #endregion

        #region DataSource and ConfigurationProvider (for DS) section
        private ILookUpEngine _configurationProvider;

        private ILookUpEngine ConfigurationProvider
            => _configurationProvider ??
               (_configurationProvider = Data.In[Eav.Constants.DefaultStreamName].Source.Configuration.LookUps);

        private DataSource DataSourceFactory => _dataSourceFactory ?? (_dataSourceFactory = new DataSource(Log));
        private DataSource _dataSourceFactory;


        /// <inheritdoc />
        [PrivateApi("obsolete")]
        [Obsolete("you should use the CreateSource<T> instead")]
        public IDataSource CreateSource(string typeName = "", IDataSource inSource = null, ILookUpEngine lookUpEngine = null)
        {
            if (lookUpEngine == null)
                lookUpEngine = ConfigurationProvider;

            if (inSource != null)
                return DataSourceFactory.GetDataSource(typeName, inSource, inSource, lookUpEngine);

            var userMayEdit = CmsInstance.UserMayEdit;

            var initialSource = DataSourceFactory.GetPublishing(
                App, userMayEdit, ConfigurationProvider as LookUpEngine);
            return typeName != "" ? DataSourceFactory.GetDataSource(typeName, initialSource, initialSource, lookUpEngine) : initialSource;
        }

        /// <inheritdoc />
        public T CreateSource<T>(IDataSource inSource = null, ILookUpEngine configurationProvider = null) where T : IDataSource
        {
            if (configurationProvider == null)
                configurationProvider = ConfigurationProvider;

            if (inSource != null)
                return DataSourceFactory.GetDataSource<T>(inSource, inSource, configurationProvider);

            var userMayEdit = CmsInstance.UserMayEdit;

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
        public dynamic Content {
            get
            {
                if(_content == null) TryToBuildContentAndList();
                return _content;
            } 
        }
        private dynamic _content;


        /// <inheritdoc />
		public dynamic Header {
            get
            {
                if(_header == null) TryToBuildHeaderObject();
                return _header;
            } 
        }
        private dynamic _header;

        [PrivateApi]
        [Obsolete("use Header instead")]
        public dynamic ListContent => Header;

        /// <remarks>
        /// This must be lazy-loaded, otherwise initializing the AppAndDataHelper will break when the Data-object fails 
        /// - this would break API even though the List etc. are never accessed
        /// </remarks>
        private void TryToBuildHeaderObject()
        {
            Log.Add("try to build ListContent (header) object");
            if (Data == null || CmsInstance.View == null) return;
            if (!Data.Out.ContainsKey(ViewParts.ListContent)) return;

            var listEntity = Data[ViewParts.ListContent].List.FirstOrDefault();
            _header = listEntity == null ? null : AsDynamic(listEntity);
        }

#pragma warning disable 618
        [PrivateApi]
        [Obsolete("This is an old way used to loop things - shouldn't be used any more - will be removed in 2sxc v10")]
        public List<Element> List {
            get
            {
                if(_list == null) TryToBuildContentAndList();
                return _list;
            } 
        }
        private List<Element> _list;

        /// <remarks>
        /// This must be lazy-loaded, otherwise initializing the AppAndDataHelper will break when the Data-object fails 
        /// - this would break API even though the List etc. are never accessed
        /// </remarks>
        private void TryToBuildContentAndList()
        {
            Log.Add("try to build List and Content objects");
            _list = new List<Element>();

            if (Data == null || CmsInstance.View == null) return;
            if (!Data.Out.ContainsKey(Eav.Constants.DefaultStreamName)) return;

            var entities = Data.List.ToList();
            if (entities.Any()) _content = AsDynamic(entities.First());

            _list = entities.Select(GetElementFromEntity).ToList();

            Element GetElementFromEntity(IEntity e)
            {
                var el = new Element
                {
                    EntityId = e.EntityId,
                    Content = AsDynamic(e)
                };

                if (e is EntityInBlock c)
                {
                    el.GroupId = c.GroupId;
                    el.Presentation = c.Presentation == null ? null : AsDynamic(c.Presentation);
                    el.SortOrder = c.SortOrder;
                }

                return el;
            }
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
            var envFs = Factory.Resolve<IEnvironmentFileSystem>();
            if (_adamAppContext == null)
                _adamAppContext = new AdamAppContext(_tenant, App, CmsInstance, Log);
            return new FolderOfField(envFs, _adamAppContext, entity.EntityGuid, fieldName);
        }
        private AdamAppContext _adamAppContext;

        #endregion


        #region Edit

        /// <inheritdoc />
        public IInPageEditingSystem Edit { get; }
        #endregion

        #region SharedCode Compiler
        /// <inheritdoc />
        public dynamic CreateInstance(string virtualPath,
            string dontRelyOnParameterOrder = Eav.Constants.RandomProtectionParameter,
            string name = null,
            string relativePath = null,
            bool throwOnError = true)
        {
            Eav.Constants.ProtectAgainstMissingParameterNames(dontRelyOnParameterOrder, "CreateInstance",
                $"{nameof(name)},{nameof(throwOnError)}");

            // Compile
            var instance = new CodeCompiler(Log)
                .InstantiateClass(virtualPath, name, relativePath, throwOnError);

            // if it supports all our known context properties, attach them
            if (instance is WithContext isShared)
                isShared.InitShared(this);

            return instance;
        }


        #endregion

        /// <inheritdoc />
        public string SharedCodeVirtualRoot { get; set; }
    }
}