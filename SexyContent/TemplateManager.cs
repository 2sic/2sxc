using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Ui;
using ToSic.Eav.DataSources;
using ToSic.Eav.Serializers;
using ToSic.Eav.WebApi;
using ToSic.SexyContent;
using ToSic.SexyContent.Internal;

// mostly because of content-groups, cannot refactor out yet :(

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
        

        internal IEnumerable<TemplateUiInfo> GetCompatibleTemplates(SexyContent.App app, ContentGroup contentGroup)
	    {
	        IEnumerable<Template> availableTemplates;
	        var items = contentGroup.Content;

            // if any items were already initialized...
	        if (items.Any(e => e != null))
	            availableTemplates = GetFullyCompatibleTemplates(contentGroup);

            // if it's only nulls, and only one (no list yet)
	        else if (items.Count <= 1)
	            availableTemplates = GetAllTemplates(); 

            // if it's a list of nulls, only allow lists
	        else
	            availableTemplates = GetAllTemplates().Where(p => p.UseForList);

	        return availableTemplates.Select(t => new TemplateUiInfo
	        {
	            TemplateId = t.TemplateId,
	            Name = t.Name,
	            ContentTypeStaticName = t.ContentTypeStaticName,
	            IsHidden = t.IsHidden,
	            Thumbnail = TemplateHelpers.GetTemplateThumbnail(app, t.Location, t.Path)
	        });
	    }


        /// <summary>
        /// Get templates which match the signature of possible content-items, presentation etc. of the current template
        /// </summary>
        /// <param name="contentGroup"></param>
        /// <returns></returns>
	    private IEnumerable<Template> GetFullyCompatibleTemplates(ContentGroup contentGroup)
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
        public IEnumerable<ContentTypeUiInfo> GetContentTypesWithStatus()
        {
            var templates = GetAllTemplates().ToList();
            var visible = templates.Where(t => !t.IsHidden).ToList();
            var mdCache = TemplateDataSource().Cache;
            var ctc = new ContentTypeController();
            var serializer = new Serializer();

            return new AppRuntime(ZoneId, AppId).ContentTypes.FromScope(Settings.AttributeSetScope) 
                .Where(ct => templates.Any(t => t.ContentTypeStaticName == ct.StaticName)) // must exist in at least 1 template
                .OrderBy(ct => ct.Name)
                .Select(ct =>
                {
                    var metadata = ctc.GetMetadata(ct, mdCache);
                    return new ContentTypeUiInfo {
                        StaticName = ct.StaticName,
                        Name = ct.Name,
                        IsHidden = visible.All(t => t.ContentTypeStaticName != ct.StaticName),   // must check if *any* template is visible, otherise tell the UI that it's hidden
                        Thumbnail = metadata?.GetBestValue(AppConstants.TemplateIcon, true)?.ToString(),
                        Metadata = serializer.Prepare(metadata)
                    };
                });
        }



    }
}