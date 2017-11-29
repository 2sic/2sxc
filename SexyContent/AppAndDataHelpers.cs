using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using ToSic.Eav;
using ToSic.Eav.Apps;
using ToSic.Eav.DataSources;
using ToSic.Eav.Logging;
using ToSic.Eav.Logging.Simple;
using ToSic.Eav.ValueProvider;
using ToSic.SexyContent.Adam;
using ToSic.SexyContent.DataSources;
using ToSic.SexyContent.DnnWebForms.Helpers;
using ToSic.SexyContent.EAVExtensions;
using ToSic.SexyContent.Edit.InPageEditingSystem;
using ToSic.SexyContent.Interfaces;
using ToSic.SexyContent.Razor.Helpers;

namespace ToSic.SexyContent
{
    public class AppAndDataHelpers : HasLog, IAppAndDataHelpers
    {
        private readonly SxcInstance _sxcInstance;

        public AppAndDataHelpers(SxcInstance sexy) : this(sexy, sexy.ModuleInfo, null) {}

        public AppAndDataHelpers(SxcInstance sexy, ModuleInfo module, Log parentLog): base("Sxc.AppHlp", parentLog ?? sexy?.Log)
        {
            // Init things than require module-info or similar, but not 2sxc
            Dnn = new DnnHelper(module);
            Link = new DnnLinkHelper(Dnn);

            if (sexy == null)
                return;

            _sxcInstance = sexy;
            App = sexy.App;
            Data = sexy.Data;
			Sxc = new SxcHelper(sexy);
            Edit = new InPageEditingHelper(sexy);

            // If PortalSettings is null - for example, while search index runs - HasEditPermission would fail
            // But in search mode, it shouldn't show drafts, so this is ok.
            // Note that app could be null, if a user is in admin-ui of a module which hasn't actually be configured yet
            App?.InitData(PortalSettings.Current != null && sexy.Environment.Permissions.UserMayEditContent,
                PortalSettings.Current != null && sexy.Environment.PagePublishing.IsEnabled(module.ModuleID), 
                Data.ConfigurationProvider);
        }



        /// <summary>
        /// The current app containing the data and read/write commands
        /// </summary>
        public App App { get; }

        /// <summary>
        /// The view data
        /// </summary>
        public ViewDataSource Data { get; }

        /// <summary>
        /// Dnn context with module, page, portal etc.
        /// </summary>
        public DnnHelper Dnn { get; }
		public SxcHelper Sxc { get; }

        /// <summary>
        /// Link helper object to create the correct links
        /// </summary>
        public ILinkHelper Link { get; }


        #region AsDynamic overrides
        /// <inheritdoc />
        /// <summary>
        /// Transform a IEntity to a DynamicEntity as dynamic object
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public dynamic AsDynamic(Eav.Interfaces.IEntity entity) => new DynamicEntity(entity, new[] { Thread.CurrentThread.CurrentCulture.Name }, _sxcInstance);
        

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
        public dynamic AsDynamic(KeyValuePair<int, Eav.Interfaces.IEntity> entityKeyValuePair) => AsDynamic(entityKeyValuePair.Value);

        /// <inheritdoc />
        /// <summary>
        /// In case AsDynamic is used with Data["name"]
        /// </summary>
        /// <returns></returns>
        public IEnumerable<dynamic> AsDynamic(IDataStream stream) => AsDynamic(stream.List);

        /// <inheritdoc />
        /// <summary>
        /// Transform a DynamicEntity dynamic object back to a IEntity instance
        /// </summary>
        /// <param name="dynamicEntity"></param>
        /// <returns></returns>
        public Eav.Interfaces.IEntity AsEntity(dynamic dynamicEntity) => ((DynamicEntity) dynamicEntity).Entity;

        /// <inheritdoc />
        /// <summary>
        /// Returns a list of DynamicEntities
        /// </summary>
        /// <param name="entities">List of entities</param>
        /// <returns></returns>
        public IEnumerable<dynamic> AsDynamic(IEnumerable<Eav.Interfaces.IEntity> entities) => entities.Select(e => AsDynamic(e));
        #endregion

        #region DataSource and ConfigurationProvider (for DS) section
        private IValueCollectionProvider _configurationProvider;
        private IValueCollectionProvider ConfigurationProvider => _configurationProvider ??
                                                                  (_configurationProvider = Data.In[Eav.Constants.DefaultStreamName].Source.ConfigurationProvider);

        /// <summary>
        /// Create a data-source by type-name. Note that it's better to use the typed version
        /// CreateSource T instead
        /// </summary>
        /// <param name="typeName"></param>
        /// <param name="inSource"></param>
        /// <param name="configurationProvider"></param>
        /// <returns></returns>
        public IDataSource CreateSource(string typeName = "", IDataSource inSource = null, IValueCollectionProvider configurationProvider = null)
        {
            if (configurationProvider == null)
                configurationProvider = ConfigurationProvider;

            if (inSource != null)
                return DataSource.GetDataSource(typeName, inSource.ZoneId, inSource.AppId, inSource, configurationProvider);

            var initialSource = DataSource.GetInitialDataSource(_sxcInstance.Environment.ZoneMapper.GetZoneId(Dnn.Portal.PortalId), App.AppId, _sxcInstance.Environment.Permissions.UserMayEditContent, ConfigurationProvider as ValueCollectionProvider);
            return typeName != "" ? DataSource.GetDataSource(typeName, initialSource.ZoneId, initialSource.AppId, initialSource, configurationProvider) : initialSource;
        }

        /// <summary>
        /// Create a data-source in code of the expected type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="inSource"></param>
        /// <param name="configurationProvider"></param>
        /// <returns></returns>
        public T CreateSource<T>(IDataSource inSource = null, IValueCollectionProvider configurationProvider = null)
        {
            if (configurationProvider == null)
                configurationProvider = ConfigurationProvider;

            if (inSource != null)
                return DataSource.GetDataSource<T>(inSource.ZoneId, inSource.AppId, inSource, configurationProvider, Log);

            var initialSource = DataSource.GetInitialDataSource(_sxcInstance.Environment.ZoneMapper.GetZoneId(Dnn.Portal.PortalId), App.AppId, _sxcInstance.Environment.Permissions.UserMayEditContent, ConfigurationProvider as ValueCollectionProvider);
            return DataSource.GetDataSource<T>(initialSource.ZoneId, initialSource.AppId, initialSource, configurationProvider, Log);
        }

        /// <inheritdoc />
        /// <summary>
        /// Create a source with initial stream to attach...
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="inStream"></param>
        /// <returns></returns>
        public T CreateSource<T>(IDataStream inStream)
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
		public dynamic ListContent {
            get
            {
                if(_listContent == null) TryToBuildListContentObject();
                return _listContent;
            } 
        }

        private dynamic _listContent;

        /// <remarks>
        /// This must be lazyl-loaded, otherwise initializing the AppAndDataHelper will break when the Data-object fails 
        /// - this would break API even though the List etc. are never accessed
        /// </remarks>
        private void TryToBuildListContentObject()
        {
            Log.Add("try to build ListContent (header) object");
            if (Data == null || _sxcInstance.Template == null) return;
            if (!Data.Out.ContainsKey(AppConstants.ListContent)) return;

            var listEntity = Data[AppConstants.ListContent].List.FirstOrDefault();
            _listContent = listEntity == null ? null : AsDynamic(listEntity);
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
        /// This must be lazyl-loaded, otherwise initializing the AppAndDataHelper will break when the Data-object fails 
        /// - this would break API even though the List etc. are never accessed
        /// </remarks>
        private void TryToBuildContentAndList()
        {
            Log.Add("try to build List and Content objects");
            _list = new List<Element>();

            if (Data == null || _sxcInstance.Template == null) return;
            if (!Data.Out.ContainsKey(Eav.Constants.DefaultStreamName)) return;

            var entities = Data.List.ToList();
            if (entities.Any()) _content = AsDynamic(entities.First());

            _list = entities.Select(GetElementFromEntity).ToList();

            Element GetElementFromEntity(Eav.Interfaces.IEntity e)
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
        public AdamNavigator AsAdam(DynamicEntity entity, string fieldName)
            => AsAdam(AsEntity(entity), fieldName);


        /// <summary>
        /// Provides an Adam instance for this item and field
        /// </summary>
        /// <param name="entity">The entity, often Content or similar</param>
        /// <param name="fieldName">The field name, like "Gallery" or "Pics"</param>
        /// <returns>An Adam object for navigating the assets</returns>
        public AdamNavigator AsAdam(Eav.Interfaces.IEntity entity, string fieldName)
            => new AdamNavigator(_sxcInstance, App, Dnn.Portal, entity.EntityGuid, fieldName, false);
        #endregion


        #region Edit

        /// <summary>
        /// Helper commands to enable in-page editing functionality
        /// Use it to check if edit is enabled, generate context-json infos and provide toolbar buttons
        /// </summary>
        public IInPageEditingSystem Edit { get; private set; }
        #endregion
    }
}