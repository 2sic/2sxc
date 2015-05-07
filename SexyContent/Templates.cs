using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav;
using ToSic.Eav.DataSources;

namespace ToSic.SexyContent
{
	public class Templates
	{
		private const string TemplateTypeName = "2SexyContent-Template";

		private readonly int _zoneId;
		private readonly int _appId;

		public Templates(int zoneId, int appId)
		{
			_zoneId = zoneId;
			_appId = appId;
		}

		private IDataSource TemplateDataSource()
		{
			var dataSource = DataSource.GetInitialDataSource(_zoneId, _appId, false);
			dataSource = DataSource.GetDataSource<EntityTypeFilter>(_zoneId, _appId, dataSource);
			((EntityTypeFilter)dataSource).TypeName = TemplateTypeName;
			return dataSource;
		}

		public IEnumerable<Template> GetAllTemplates()
		{
			
            return TemplateDataSource().List.Select(p => new Template(p.Value)).OrderBy(p => p.Name);
        }

		public IEnumerable<Template> GetVisibleTemplates()
		{
			return GetAllTemplates().Where(t => !t.IsHidden);
		}

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
			var template = GetTemplate(templateId);
			var eavContext = EavContext.Instance(_zoneId, _appId);
			var canDelete = eavContext.CanDeleteEntity(template.TemplateId);
			if(!canDelete.Item1)
				throw new Exception(canDelete.Item2);
			return eavContext.DeleteEntity(template.TemplateId);
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

			var context = EavContext.Instance(_zoneId, _appId);

			if(templateId.HasValue)
				context.UpdateEntity(templateId.Value, values);
			else
			{
				var contentType = DataSource.GetCache(_zoneId, _appId).GetContentType(TemplateTypeName);
				context.AddEntity(contentType.AttributeSetId, values, null, null);
			}
			
		}

	}
}