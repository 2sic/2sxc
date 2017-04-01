using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav;
using ToSic.Eav.DataSources;
using ToSic.Eav.DataSources.Caches;
using ToSic.Eav.Serializers;
using ToSic.Eav.WebApi;
using ToSic.SexyContent.Internal;

namespace ToSic.SexyContent
{
	public class TemplateManager
	{
		private const string TemplateTypeName = "2SexyContent-Template";

		private readonly int _zoneId;
		private readonly int _appId;

		public TemplateManager(int zoneId, int appId)
		{
			_zoneId = zoneId;
			_appId = appId;
		}

	    private IDataSource _templateDs;
		private IDataSource TemplateDataSource()
		{
            if(_templateDs!= null)return _templateDs;
		    // ReSharper disable once RedundantArgumentDefaultValue
			var dataSource = DataSource.GetInitialDataSource(_zoneId, _appId, false);
			dataSource = DataSource.GetDataSource<EntityTypeFilter>(_zoneId, _appId, dataSource);
			((EntityTypeFilter)dataSource).TypeName = TemplateTypeName;
		    _templateDs = dataSource;
			return dataSource;
		}

		public IEnumerable<Template> GetAllTemplates() 
            => TemplateDataSource().List.Select(p => new Template(p.Value)).OrderBy(p => p.Name);

		public Template GetTemplate(int templateId)
		{
			var dataSource = TemplateDataSource();
			dataSource = DataSource.GetDataSource<EntityIdFilter>(_zoneId, _appId, dataSource);
			((EntityIdFilter)dataSource).EntityIds = templateId.ToString();
			var templateEntity = dataSource.List.FirstOrDefault().Value;

			if(templateEntity == null)
				throw new Exception("The template with id " + templateId + " does not exist.");

			return new Template(templateEntity);
		}

        public bool DeleteTemplate(int templateId)
		{
            // really get template first, to be sure it is a template
			var template = GetTemplate(templateId);
            // 2017-04-01 2dm centralizing eav access
		    return new EavBridge(_zoneId, _appId).EntityDelete(template.TemplateId);
		    //         var eavContext = EavDataController.Instance(_zoneId, _appId).Entities; //EavContext.Instance(_zoneId, _appId);
		    //var canDelete = eavContext.CanDeleteEntity(template.TemplateId);
		    //if(!canDelete.Item1)
		    //	throw new Exception(canDelete.Item2);
		    //return eavContext.DeleteEntity(template.TemplateId);
		}

		/// <summary>
		/// Adds or updates a template - will create a new template if templateId is not specified
		/// </summary>
		public void UpdateTemplate(int? templateId, string name, string path, string contentTypeStaticName,
			int? contentDemoEntity, string presentationTypeStaticName, int? presentationDemoEntity,
			string listContentTypeStaticName, int? listContentDemoEntity, string listPresentationTypeStaticName,
			int? listPresentationDemoEntity, string templateType, bool isHidden, string location, bool useForList,
			bool publishData, string streamsToPublish, int? pipelineEntity, string viewNameInUrl)
		{
			var values = new Dictionary<string,object>
			{
				{ "Name", name },
				{ "Path", path },
				{ "ContentTypeStaticName", contentTypeStaticName },
				{ "ContentDemoEntity", contentDemoEntity.HasValue ? new[] { contentDemoEntity.Value } : new int[]{} },
				{ "PresentationTypeStaticName", presentationTypeStaticName },
				{ "PresentationDemoEntity", presentationDemoEntity.HasValue ? new[] { presentationDemoEntity.Value } : new int[]{} },
				{ "ListContentTypeStaticName", listContentTypeStaticName },
				{ "ListContentDemoEntity", listContentDemoEntity.HasValue ? new[] { listContentDemoEntity.Value } : new int[]{} },
				{ "ListPresentationTypeStaticName", listPresentationTypeStaticName },
				{ "ListPresentationDemoEntity", listPresentationDemoEntity.HasValue ? new[] { listPresentationDemoEntity.Value } : new int[]{} },
				{ "Type", templateType },
				{ "IsHidden", isHidden },
				{ "Location", location },
				{ "UseForList", useForList },
				{ "PublishData", publishData },
				{ "StreamsToPublish", streamsToPublish },
				{ "Pipeline", pipelineEntity.HasValue ? new[] { pipelineEntity } : new int?[]{} },
				{ "ViewNameInUrl", viewNameInUrl }
			};

            // 2017-04-01 2dm centralizing eav access code
            var bridge = new EavBridge(_zoneId, _appId);
            //var context = EavDataController.Instance(_zoneId, _appId).Entities;// EavContext.Instance(_zoneId, _appId);

			if(templateId.HasValue)
                bridge.EntityUpdate(templateId.Value, values);
				// context.UpdateEntity(templateId.Value, values);
			else
                bridge.EntityCreate(TemplateTypeName, values);
			//{
			//	var contentType = DataSource.GetCache(_zoneId, _appId).GetContentType(TemplateTypeName);
			//	context.AddEntity(contentType.AttributeSetId, values, null, null);
			//}
			
		}


	    /// <summary>
	    /// Returns all templates that should be available in the template selector
	    /// </summary>
	    /// <returns></returns>
	    public IEnumerable<Template> GetAvailableTemplatesForSelector(int modId, ContentGroupManager cgContentGroups)
        {
            // IEnumerable<Template> availableTemplates;
            var contentGroup = cgContentGroups.GetContentGroupForModule(modId);
            return GetAvailableTemplates(contentGroup);
        }

        // 2016-09-08 2dm - changed to deliver hidden as well, because of https://github.com/2sic/2sxc/issues/831
        internal IEnumerable<Template> GetAvailableTemplates(ContentGroup contentGroup)
	    {
	        IEnumerable<Template> availableTemplates;
	        var items = contentGroup.Content;

	        if (items.Any(e => e != null))
	            availableTemplates = GetCompatibleTemplates(contentGroup); //.Where(p => !p.IsHidden);
	        else if (items.Count <= 1)
	            availableTemplates = GetAllTemplates(); // GetVisibleTemplates();
	        else
	            availableTemplates = GetAllTemplates() /* GetVisibleTemplates() */.Where(p => p.UseForList);

	        return availableTemplates;
	    }

	    private IEnumerable<Template> GetCompatibleTemplates(ContentGroup contentGroup)
        {
            var isList = contentGroup.Content.Count > 1;

            var compatibleTemplates = GetAllTemplates().Where(t => t.UseForList || !isList);
            compatibleTemplates = compatibleTemplates
                .Where(t => contentGroup.Content.All(c => c == null) || contentGroup.Content.First(e => e != null).Type.StaticName == t.ContentTypeStaticName)
                .Where(t => contentGroup.Presentation.All(c => c == null) || contentGroup.Presentation.First(e => e != null).Type.StaticName == t.PresentationTypeStaticName)
                .Where(t => contentGroup.ListContent.All(c => c == null) || contentGroup.ListContent.First(e => e != null).Type.StaticName == t.ListContentTypeStaticName)
                .Where(t => contentGroup.ListPresentation.All(c => c == null) || contentGroup.ListPresentation.First(e => e != null).Type.StaticName == t.ListPresentationTypeStaticName);

            return compatibleTemplates;
        }

        // todo: check if this call could be replaced with the normal ContentTypeController.Get to prevent redundant code
        public IEnumerable<object> GetContentTypesWithStatus()
        {
            // 2016-09-08 2dm - changed to use all templates, because of https://github.com/2sic/2sxc/issues/831
            var availableTemplates = GetAllTemplates().ToList();// GetVisibleTemplates();
            var visTemplates = availableTemplates.Where(t => !t.IsHidden).ToList();
            var mdCache = TemplateDataSource().Cache;
            var ctc = new ContentTypeController();
            var ser = new Serializer();

            return GetAvailableContentTypes(Settings.AttributeSetScope)
                .Where(p => availableTemplates.Any(t => t.ContentTypeStaticName == p.StaticName)) // must exist in at least 1 template
                .OrderBy(p => p.Name)
                .Select(p => new
                {
                    p.StaticName,
                    p.Name,
                    IsHidden = !(visTemplates.Any(t => t.ContentTypeStaticName == p.StaticName)), // must check if *any* template is visible, otherise tell the UI that it's hidden
                    Metadata = ser.Prepare(ctc.GetMetadata(p, mdCache))
                });
        }



        public IEnumerable<IContentType> GetAvailableContentTypes(string scope, bool includeAttributeTypes = false)
        {
            return GetAvailableContentTypes(includeAttributeTypes).Where(p => p.Scope == scope);
        }

        public IEnumerable<IContentType> GetAvailableContentTypes(bool includeAttributeTypes = false)
        {
            var contentTypes = ((BaseCache)DataSource.GetCache(_zoneId, _appId)).GetContentTypes();
            return contentTypes.Select(c => c.Value).Where(c => includeAttributeTypes || !c.Name.StartsWith("@")).OrderBy(c => c.Name);
        }



    }
}