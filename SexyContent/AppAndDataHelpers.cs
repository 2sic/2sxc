using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using ToSic.Eav;
using ToSic.Eav.Apps;
using ToSic.Eav.DataSources;
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
    public class AppAndDataHelpers : IAppAndDataHelpers
    {
        private readonly SxcInstance _sxcInstance;

        public AppAndDataHelpers(SxcInstance sexy) : this(sexy, sexy.ModuleInfo) {}

        public AppAndDataHelpers(SxcInstance sexy, ModuleInfo module)
        {
            // ModuleInfo module = sexy.ModuleInfo;
            // Init things than require module-info or similar, but not 2sxc
            Dnn = new DnnHelper(module);
            Link = new DnnLinkHelper(Dnn);

            // todo: maybe init App & App.Data, as they don't really require a working 2sxc

            if (sexy == null)
                return;

            ViewDataSource data = sexy.Data;
            _sxcInstance = sexy;
            App = sexy.App;// app;
            Data = sexy.Data;// data;
			Sxc = new SxcHelper(sexy);
            Edit = new InPageEditingHelper(sexy);

            // If PortalSettings is null - for example, while search index runs - HasEditPermission would fail
            // But in search mode, it shouldn't show drafts, so this is ok.
            // Note that app could be null, if a user is in admin-ui of a module which hasn't actually be configured yet
            App?.InitData(PortalSettings.Current != null && sexy.Environment.Permissions.UserMayEditContent, new Environment.Dnn7.PagePublishing().IsVersioningEnabled(module.ModuleID), data.ConfigurationProvider);

            #region Assemble the mapping of the data-stream "default"/Presentation to the List object and the "ListContent" too
	        List = new List<Element>();
            if (data != null && sexy.Template != null)
	        {
		        if (data.Out.ContainsKey("Default"))
		        {
			        var entities = data.List.Select(e => e.Value);
					var elements = entities.Select(GetElementFromEntity).ToList();
					List = elements;

					if (elements.Any())
					{
						Content = elements.First().Content;
						Presentation = elements.First().Presentation;
					}
		        }

		        if (data.Out.ContainsKey(AppConstants.ListContent))
		        {
			        var listEntity = data[AppConstants.ListContent].List.Select(e => e.Value).FirstOrDefault();
					var listElement = listEntity != null ? GetElementFromEntity(listEntity) : null;

					if (listElement != null)
					{
						ListContent = listElement.Content;
						ListPresentation = listElement.Presentation;
					}
		        }

	        }
            #endregion

        }

	    private Element GetElementFromEntity(ToSic.Eav.Interfaces.IEntity e)
	    {
			var el = new Element
			{
				EntityId = e.EntityId,
				Content =AsDynamic(e)
			};

			if (e is EntityInContentGroup)
			{
				var c = ((EntityInContentGroup)e);
				el.GroupId = c.GroupId;
				el.Presentation = c.Presentation == null ? null : AsDynamic(c.Presentation);
				el.SortOrder = c.SortOrder;
			}

		    return el;
	    }

        public App App { get; }
        public ViewDataSource Data { get; }
        public DnnHelper Dnn { get; }
		public SxcHelper Sxc { get; }
        public ILinkHelper Link { get; }


        #region AsDynamic overrides
        /// <summary>
        /// Transform a IEntity to a DynamicEntity as dynamic object
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public dynamic AsDynamic(ToSic.Eav.Interfaces.IEntity entity) => new DynamicEntity(entity, new[] { Thread.CurrentThread.CurrentCulture.Name }, _sxcInstance);
        

        /// <summary>
        /// Makes sure a dynamicEntity could be wrapped in AsDynamic()
        /// </summary>
        /// <param name="dynamicEntity"></param>
        /// <returns></returns>
        public dynamic AsDynamic(dynamic dynamicEntity) => dynamicEntity;

        /// <summary>
        /// Returns the value of a KeyValuePair as DynamicEntity
        /// </summary>
        /// <param name="entityKeyValuePair"></param>
        /// <returns></returns>
        public dynamic AsDynamic(KeyValuePair<int, ToSic.Eav.Interfaces.IEntity> entityKeyValuePair) => AsDynamic(entityKeyValuePair.Value);

        /// <summary>
        /// In case AsDynamic is used with Data["name"]
        /// </summary>
        /// <returns></returns>
        public IEnumerable<dynamic> AsDynamic(IDataStream stream) => AsDynamic(stream.List);

        /// <summary>
        /// In case AsDynamic is used with Data["name"].List
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public IEnumerable<dynamic> AsDynamic(IDictionary<int, ToSic.Eav.Interfaces.IEntity> list) => list.Select(e => AsDynamic(e.Value));

        /// <summary>
        /// Transform a DynamicEntity dynamic object back to a IEntity instance
        /// </summary>
        /// <param name="dynamicEntity"></param>
        /// <returns></returns>
        public ToSic.Eav.Interfaces.IEntity AsEntity(dynamic dynamicEntity) => ((DynamicEntity) dynamicEntity).Entity;

        /// <summary>
        /// Returns a list of DynamicEntities
        /// </summary>
        /// <param name="entities">List of entities</param>
        /// <returns></returns>
        public IEnumerable<dynamic> AsDynamic(IEnumerable<ToSic.Eav.Interfaces.IEntity> entities) => entities.Select(e => AsDynamic(e));
        #endregion

        private IValueCollectionProvider _configurationProvider;
        private IValueCollectionProvider ConfigurationProvider => _configurationProvider ??
                                                                  (_configurationProvider = Data.In["Default"].Source.ConfigurationProvider);

        public IDataSource CreateSource(string typeName = "", IDataSource inSource = null, IValueCollectionProvider configurationProvider = null)
        {
            if (configurationProvider == null)
                configurationProvider = ConfigurationProvider;

            if (inSource != null)
                return DataSource.GetDataSource(typeName, inSource.ZoneId, inSource.AppId, inSource, configurationProvider);

            var initialSource = DataSource.GetInitialDataSource(_sxcInstance.Environment.ZoneMapper.GetZoneId(Dnn.Portal.PortalId) /* ZoneHelpers.GetZoneId(Dnn.Portal.PortalId).Value*/, App.AppId, _sxcInstance.Environment.Permissions.UserMayEditContent/* SecurityHelpers.HasEditPermission(Dnn.Module)*/);
            return typeName != "" ? DataSource.GetDataSource(typeName, initialSource.ZoneId, initialSource.AppId, initialSource, configurationProvider) : initialSource;
        }

        public T CreateSource<T>(IDataSource inSource = null, IValueCollectionProvider configurationProvider = null)
        {
            if (configurationProvider == null)
                configurationProvider = ConfigurationProvider;

            if (inSource != null)
                return DataSource.GetDataSource<T>(inSource.ZoneId, inSource.AppId, inSource, configurationProvider);

            var initialSource = DataSource.GetInitialDataSource(_sxcInstance.Environment.ZoneMapper.GetZoneId(Dnn.Portal.PortalId) /* ZoneHelpers.GetZoneId(Dnn.Portal.PortalId).Value */, App.AppId, _sxcInstance.Environment.Permissions.UserMayEditContent /*SecurityHelpers.HasEditPermission(Dnn.Module)*/);
            return DataSource.GetDataSource<T>(initialSource.ZoneId, initialSource.AppId, initialSource, configurationProvider);
        }

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
            srcDs.In.Add(Constants.DefaultStreamName, inStream);
            return src;
        }

        #region basic properties like Content, Presentation, ListContent, ListPresentation

        public dynamic Content { get; private set; }
		public dynamic Presentation { get; private set; }
		public dynamic ListContent { get; private set; }
		public dynamic ListPresentation { get; private set; }
        [Obsolete("This is an old way used to loop things - shouldn't be used any more - will be removed in 2sxc v10")]
        public List<Element> List { get; private set; }
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
        public AdamNavigator AsAdam(ToSic.Eav.Interfaces.IEntity entity, string fieldName)
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