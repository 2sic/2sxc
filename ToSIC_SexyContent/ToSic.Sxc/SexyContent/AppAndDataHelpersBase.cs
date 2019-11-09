using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ToSic.Eav;
using ToSic.Eav.Apps;
using ToSic.Eav.DataSources;
using ToSic.Eav.Documentation;
using ToSic.Eav.Logging;
using ToSic.Eav.LookUp;
using ToSic.Sxc.Adam;
using ToSic.SexyContent.DataSources;
using ToSic.SexyContent.EAVExtensions;
using ToSic.Sxc;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Code;
using ToSic.Sxc.Edit.InPageEditingSystem;
using IApp = ToSic.Sxc.Apps.IApp;
using IEntity = ToSic.Eav.Data.IEntity;
using IFolder = ToSic.Sxc.Adam.IFolder;

// ReSharper disable once CheckNamespace - probably in use publicly somewhere, but unsure; otherwise move some day
namespace ToSic.SexyContent
{
    public abstract class AppAndDataHelpersBase : HasLog, IDynamicCode
    {
        protected readonly /*SxcInstance*/ICmsBlock CmsInstance;

        private readonly ITenant _tenant;
        protected AppAndDataHelpersBase(/*SxcInstance*/ICmsBlock cmsInstance, ITenant tenant, ILog parentLog): base("Sxc.AppHlp", parentLog ?? cmsInstance?.Log)
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

        internal void LateAttachApp(IApp app) => App = app;


        /// <summary>
        /// The current app containing the data and read/write commands
        /// </summary>
        public IApp App { get; private set; }

        /// <summary>
        /// The view data
        /// </summary>
        public IBlockDataSource Data { get; }

		public SxcHelper Sxc { get; }

        /// <summary>
        /// Link helper object to create the correct links
        /// </summary>
        public ILinkHelper Link { get; protected set; }


        #region AsDynamic Implementations
        /// <inheritdoc />
        /// <summary>
        /// Transform a IEntity to a DynamicEntity as dynamic object
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public dynamic AsDynamic(IEntity entity) => new DynamicEntity(entity, new[] { Thread.CurrentThread.CurrentCulture.Name }, CmsInstance);

        [PrivateApi]
        [Obsolete("for compatibility only, avoid using this and cast your entities to ToSic.Eav.Data.IEntity")]
        public dynamic AsDynamic(Eav.Interfaces.IEntity entity) => AsDynamic(entity as IEntity);


        /// <inheritdoc />
        /// <summary>
        /// Makes sure a dynamicEntity could be wrapped in AsDynamic()
        /// </summary>
        /// <param name="dynamicEntity"></param>
        /// <returns></returns>
        public dynamic AsDynamic(dynamic dynamicEntity) => dynamicEntity;

        /// <inheritdoc />
        /// <summary>
        /// Returns the value of a KeyValuePair as DynamicEntity
        /// </summary>
        /// <param name="entityKeyValuePair"></param>
        /// <returns></returns>
        public dynamic AsDynamic(KeyValuePair<int, IEntity> entityKeyValuePair) => AsDynamic(entityKeyValuePair.Value);

        [PrivateApi]
        [Obsolete("for compatibility only, avoid using this and cast your entities to ToSic.Eav.Data.IEntity")]
        public dynamic AsDynamic(KeyValuePair<int, Eav.Interfaces.IEntity> entityKeyValuePair) => AsDynamic(entityKeyValuePair.Value);


        /// <inheritdoc />
        /// <summary>
        /// In case AsDynamic is used with Data["name"]
        /// </summary>
        /// <returns></returns>
        [Obsolete("2019-03-07 2dm: probably not needed any more, as 2sxc 9.40.01 adds the IEnumerable to the IDatastream")]
        public IEnumerable<dynamic> AsDynamic(IDataStream stream) => AsDynamic(stream.List);

        /// <inheritdoc />
        /// <summary>
        /// Transform a DynamicEntity dynamic object back to a IEntity instance
        /// </summary>
        /// <param name="dynamicEntity"></param>
        /// <returns></returns>
        public IEntity AsEntity(dynamic dynamicEntity) => ((IDynamicEntity) dynamicEntity).Entity;

        /// <inheritdoc />
        /// <summary>
        /// Returns a list of DynamicEntities
        /// </summary>
        /// <param name="entities">List of entities</param>
        /// <returns></returns>
        public IEnumerable<dynamic> AsDynamic(IEnumerable<IEntity> entities) => entities.Select(e => AsDynamic(e));


        [PrivateApi]
        [Obsolete("for compatibility only, avoid using this and cast your entities to ToSic.Eav.Data.IEntity")]
        public IEnumerable<dynamic> AsDynamic(IEnumerable<Eav.Interfaces.IEntity> entities) => entities.Select(e => AsDynamic(e));

        #endregion

        #region DataSource and ConfigurationProvider (for DS) section
        private ITokenListFiller _configurationProvider;

        private ITokenListFiller ConfigurationProvider
            => _configurationProvider ??
               (_configurationProvider = Data.In[Eav.Constants.DefaultStreamName].Source.ConfigurationProvider);

        /// <summary>
        /// Create a data-source by type-name. Note that it's better to use the typed version
        /// CreateSource T instead
        /// </summary>
        /// <param name="typeName"></param>
        /// <param name="inSource"></param>
        /// <param name="configurationProvider"></param>
        /// <returns></returns>
        public IDataSource CreateSource(string typeName = "", IDataSource inSource = null, ITokenListFiller configurationProvider = null)
        {
            if (configurationProvider == null)
                configurationProvider = ConfigurationProvider;

            if (inSource != null)
                return DataSource.GetDataSource(typeName, inSource.ZoneId, inSource.AppId, inSource, configurationProvider);

            var userMayEdit = CmsInstance.UserMayEdit;

            var initialSource = DataSource.GetInitialDataSource(CmsInstance.Environment.ZoneMapper.GetZoneId(_tenant.Id), App.AppId,
                userMayEdit, ConfigurationProvider as TokenListFiller);
            return typeName != "" ? DataSource.GetDataSource(typeName, initialSource.ZoneId, initialSource.AppId, initialSource, configurationProvider) : initialSource;
        }

        /// <summary>
        /// Create a data-source in code of the expected type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="inSource"></param>
        /// <param name="configurationProvider"></param>
        /// <returns></returns>
        public T CreateSource<T>(IDataSource inSource = null, ITokenListFiller configurationProvider = null) where T : IDataSource
        {
            if (configurationProvider == null)
                configurationProvider = ConfigurationProvider;

            if (inSource != null)
                return DataSource.GetDataSource<T>(inSource.ZoneId, inSource.AppId, inSource, configurationProvider, Log);

            var userMayEdit = CmsInstance.UserMayEdit;

            var initialSource = DataSource.GetInitialDataSource(CmsInstance.Environment.ZoneMapper.GetZoneId(_tenant.Id), App.AppId,
                userMayEdit, ConfigurationProvider as TokenListFiller);
            return DataSource.GetDataSource<T>(initialSource.ZoneId, initialSource.AppId, initialSource, configurationProvider, Log);
        }

        /// <inheritdoc />
        /// <summary>
        /// Create a source with initial stream to attach...
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="inStream"></param>
        /// <returns></returns>
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

        /// <summary>
        /// content item of the current view
        /// </summary>
        public dynamic Content {
            get
            {
                if(_content == null) TryToBuildContentAndList();
                return _content;
            } 
        }
        private dynamic _content;

        
        /// <summary>
        /// List item of the current view
        /// </summary>
		public dynamic Header {
            get
            {
                if(_header == null) TryToBuildHeaderObject();
                return _header;
            } 
        }
        private dynamic _header;

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

                if (e is EntityInContentGroup c)
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

        /// <summary>
        /// Provides an Adam instance for this item and field
        /// </summary>
        /// <param name="entity">The entity, often Content or similar</param>
        /// <param name="fieldName">The field name, like "Gallery" or "Pics"</param>
        /// <returns>An Adam object for navigating the assets</returns>
        public IFolder AsAdam(IDynamicEntity entity, string fieldName)
            => AsAdam(AsEntity(entity), fieldName);


        /// <summary>
        /// Provides an Adam instance for this item and field
        /// </summary>
        /// <param name="entity">The entity, often Content or similar</param>
        /// <param name="fieldName">The field name, like "Gallery" or "Pics"</param>
        /// <returns>An Adam object for navigating the assets</returns>
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

        /// <summary>
        /// Helper commands to enable in-page editing functionality
        /// Use it to check if edit is enabled, generate context-json infos and provide toolbar buttons
        /// </summary>
        public IInPageEditingSystem Edit { get; }
        #endregion

        #region SharedCode Compiler
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

        public string SharedCodeVirtualRoot { get; set; }
    }
}