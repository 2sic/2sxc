using System.Collections.Generic;
using System.Linq;
using System.Threading;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using ToSic.Eav;
using ToSic.Eav.DataSources;
using ToSic.Eav.ValueProvider;
using ToSic.SexyContent.DataSources;
using ToSic.SexyContent.EAVExtensions;
using ToSic.SexyContent.Razor.Helpers;

namespace ToSic.SexyContent
{
    public class AppAndDataHelpers : IAppAndDataHelpers
    {
        private readonly SexyContent _sexy;

        public AppAndDataHelpers(SexyContent sexy, ModuleInfo module, ViewDataSource data, App app)
        {
            _sexy = sexy;
            App = app;
            Data = data;
            Dnn = new DnnHelper(module);
	        List = new List<Element>();

	        if (data != null)
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

		        if (data.Out.ContainsKey("ListContent"))
		        {
			        var listEntity = data["ListContent"].List.Select(e => e.Value).FirstOrDefault();
					var listElement = listEntity != null ? GetElementFromEntity(listEntity) : null;

					if (listElement != null)
					{
						ListContent = listElement.Content;
						ListPresentation = listElement.Presentation;
					}
		        }

	        }

	        // If PortalSettings is null - for example, while search index runs - HasEditPermission would fail
            // But in search mode, it shouldn't show drafts, so this is ok.
            App.InitData(PortalSettings.Current != null && SexyContent.HasEditPermission(module));
        }

	    private Element GetElementFromEntity(IEntity e)
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

        public App App { get; private set; }
        public ViewDataSource Data { get; private set; }
        public DnnHelper Dnn { get; private set; }

        /// <summary>
        /// Transform a IEntity to a DynamicEntity as dynamic object
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public dynamic AsDynamic(IEntity entity)
        {
            return new DynamicEntity(entity, new[] { Thread.CurrentThread.CurrentCulture.Name }, _sexy);
        }

        /// <summary>
        /// Makes sure a dynamicEntity could be wrapped in AsDynamic()
        /// </summary>
        /// <param name="dynamicEntity"></param>
        /// <returns></returns>
        public dynamic AsDynamic(dynamic dynamicEntity)
        {
            return dynamicEntity;
        }

        /// <summary>
        /// Returns the value of a KeyValuePair as DynamicEntity
        /// </summary>
        /// <param name="entityKeyValuePair"></param>
        /// <returns></returns>
        public dynamic AsDynamic(KeyValuePair<int, IEntity> entityKeyValuePair)
        {
            return AsDynamic(entityKeyValuePair.Value);
        }

        /// <summary>
        /// In case AsDynamic is used with Data["name"]
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public IEnumerable<dynamic> AsDynamic(IDataStream stream)
        {
            return AsDynamic(stream.List);
        }

        /// <summary>
        /// In case AsDynamic is used with Data["name"].List
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public IEnumerable<dynamic> AsDynamic(IDictionary<int, IEntity> list)
        {
            return list.Select(e => AsDynamic(e.Value));
        }

        /// <summary>
        /// Transform a DynamicEntity dynamic object back to a IEntity instance
        /// </summary>
        /// <param name="dynamicEntity"></param>
        /// <returns></returns>
        public IEntity AsEntity(dynamic dynamicEntity)
        {
            return ((DynamicEntity)dynamicEntity).Entity;
        }

        /// <summary>
        /// Returns a list of DynamicEntities
        /// </summary>
        /// <param name="entities">List of entities</param>
        /// <returns></returns>
        public IEnumerable<dynamic> AsDynamic(IEnumerable<IEntity> entities)
        {
            return entities.Select(e => AsDynamic(e));
        }


        private IValueCollectionProvider _configurationProvider;
        private IValueCollectionProvider ConfigurationProvider
        {
            get
            {
                if (_configurationProvider == null)
                {
                    _configurationProvider = Data.In["Default"].Source.ConfigurationProvider;
                }
                return _configurationProvider;
            }
        }

        public IDataSource CreateSource(string typeName = "", IDataSource inSource = null, IValueCollectionProvider configurationProvider = null)
        {
            if (configurationProvider == null)
                configurationProvider = ConfigurationProvider;

            if (inSource != null)
                return DataSource.GetDataSource(typeName, inSource.ZoneId, inSource.AppId, inSource, configurationProvider);

            var initialSource = SexyContent.GetInitialDataSource(SexyContent.GetZoneID(Dnn.Portal.PortalId).Value, App.AppId, SexyContent.HasEditPermission(Dnn.Module));
            return typeName != "" ? DataSource.GetDataSource(typeName, initialSource.ZoneId, initialSource.AppId, initialSource, configurationProvider) : initialSource;
        }

        public T CreateSource<T>(IDataSource inSource = null, IValueCollectionProvider configurationProvider = null)
        {
            if (configurationProvider == null)
                configurationProvider = ConfigurationProvider;

            if (inSource != null)
                return DataSource.GetDataSource<T>(inSource.ZoneId, inSource.AppId, inSource, configurationProvider);

            var initialSource = SexyContent.GetInitialDataSource(SexyContent.GetZoneID(Dnn.Portal.PortalId).Value, App.AppId, SexyContent.HasEditPermission(Dnn.Module));
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
            srcDs.In.Add(DataSource.DefaultStreamName, inStream);
            return src;
        }

		public dynamic Content { get; set; }
		public dynamic Presentation { get; set; }
		public dynamic ListContent { get; set; }
		public dynamic ListPresentation { get; set; }
		public List<Element> List { get; set; }

    }
}