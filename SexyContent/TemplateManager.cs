using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Apps;
using ToSic.Eav.DataSources;
using ToSic.Eav.Serializers;
using ToSic.Eav.WebApi;
using ToSic.SexyContent; // mostly because of content-groups

namespace ToSic.Eav.AppEngine
{
	public class TemplateManager
	{
		public readonly int ZoneId;
		public readonly int AppId;

		public TemplateManager(int zoneId, int appId)
		{
			ZoneId = zoneId;
			AppId = appId;
		}

	    private IDataSource _templateDs;
		private IDataSource TemplateDataSource()
		{
            if(_templateDs!= null)return _templateDs;
		    // ReSharper disable once RedundantArgumentDefaultValue
			var dataSource = DataSource.GetInitialDataSource(ZoneId, AppId, false);
			dataSource = DataSource.GetDataSource<EntityTypeFilter>(ZoneId, AppId, dataSource);
		    ((EntityTypeFilter) dataSource).TypeName = Apps.Configuration.TemplateContentType;// TemplateTypeName;
		    _templateDs = dataSource;
			return dataSource;
		}

		public IEnumerable<Template> GetAllTemplates() 
            => TemplateDataSource().List.Select(p => new Template(p.Value)).OrderBy(p => p.Name);

		public Template GetTemplate(int templateId)
		{
			var dataSource = TemplateDataSource();
			dataSource = DataSource.GetDataSource<EntityIdFilter>(ZoneId, AppId, dataSource);
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
            return new AppManager(ZoneId, AppId).Entities.Delete(template.TemplateId);
		}
        


	    /// <summary>
	    /// Returns all templates that should be available in the template selector
	    /// </summary>
	    /// <returns></returns>
	    public IEnumerable<Template> GetAvailableTemplatesForSelector(int modId, ContentGroupManager cgContentGroups)
        {
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

            return State.ContentTypes(ZoneId, AppId, Settings.AttributeSetScope) //  GetAvailableContentTypes(Settings.AttributeSetScope)
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



    }
}