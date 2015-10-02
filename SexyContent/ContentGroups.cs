using System;
using System.Collections.Generic;
using System.Linq;
using DotNetNuke.Entities.Modules;
using ToSic.Eav;
using ToSic.Eav.BLL;
using ToSic.Eav.DataSources;

namespace ToSic.SexyContent
{
	public class ContentGroups
	{
		private const string ContentGroupTypeName = "2SexyContent-ContentGroup";
		private const string PreviewTemplateIdString = "ToSIC_SexyContent_PreviewTemplateId";

		private readonly int _zoneId;
		private readonly int _appId;

		public ContentGroups(int zoneId, int appId)
		{
			_zoneId = zoneId;
			_appId = appId;
		}

		private IDataSource ContentGroupSource()
		{
			var dataSource = DataSource.GetInitialDataSource(_zoneId, _appId, false);
			dataSource = DataSource.GetDataSource<EntityTypeFilter>(_zoneId, _appId, dataSource);
			((EntityTypeFilter)dataSource).TypeName = ContentGroupTypeName;
			return dataSource;
		}

		public IEnumerable<ContentGroup> GetContentGroups()
		{
			return ContentGroupSource().List.Select(p => new ContentGroup(p.Value, _zoneId, _appId));
		}

		public ContentGroup GetContentGroup(Guid contentGroupGuid)
		{
			var dataSource = ContentGroupSource();
			// ToDo: Should use an indexed guid source
			return new ContentGroup(dataSource.List.FirstOrDefault(e => e.Value.EntityGuid == contentGroupGuid).Value, _zoneId, _appId);
		}

		public bool IsConfigurationInUse(int templateId, string type)
		{
			var contentGroups = GetContentGroups().Where(p => p.Template != null && p.Template.TemplateId == templateId);
			return contentGroups.Any(p => p[type].Any(c => c != null));
		}

		public Guid CreateContentGroup(int moduleId, int? templateId)
		{
		    var context = EavDataController.Instance(_zoneId, _appId).Entities;//  EavContext.Instance(_zoneId, _appId);
			var contentType = DataSource.GetCache(_zoneId, _appId).GetContentType(ContentGroupTypeName);

			var values = new Dictionary<string, object>
			{
				{"Template", templateId.HasValue ? new [] { templateId.Value } : new int[] {}},
				{"Content", new int[] {}},
				{"Presentation", new int[] {}},
				{"ListContent", new int[] {}},
				{"ListPresentation", new int[] {}}
			};

			var entity = context.AddEntity(contentType.AttributeSetId, values, null, null);

			new ModuleController().UpdateModuleSetting(moduleId, SexyContent.ContentGroupGuidString, entity.EntityGUID.ToString());

			return entity.EntityGUID;
		}


		/// <summary>
		/// Saves a temporary templateId to the module's settings
		/// This templateId will be used until a contentgroup exists
		/// </summary>
		/// <param name="moduleId"></param>
		/// <param name="previewTemplateId"></param>
		public void SetPreviewTemplateId(int moduleId, int previewTemplateId)
		{
            // todo: 2rm - I believe you are accidentally using uncached module settings access - pls check and probably change
            // todo: note: this is done ca. 3x in this class
			var moduleController = new ModuleController();
			var settings = moduleController.GetModuleSettings(moduleId);

			// Do not allow saving the temporary template id if a contentgroup exists for this module
			if(settings[SexyContent.ContentGroupGuidString] != null)
				throw new Exception("Preview template id cannot be set for a module that already has content.");

			var dataSource = DataSource.GetInitialDataSource(_zoneId, _appId);
			var previewTemplateGuid = dataSource.List[previewTemplateId].EntityGuid;

			moduleController.UpdateModuleSetting(moduleId, PreviewTemplateIdString, previewTemplateGuid.ToString());
		}

		public static void DeletePreviewTemplateId(int moduleId)
		{
			var moduleController = new ModuleController();
			moduleController.DeleteModuleSetting(moduleId, PreviewTemplateIdString);
		}

		public void SaveTemplateId(int moduleId, int templateId)
		{
			// Remove the previewTemplateId (because it's not needed as soon Content is inserted)
			DeletePreviewTemplateId(moduleId);


			// Create a new contentgroup if it does not exist
			var contentGroup = GetContentGroupForModule(moduleId);

			if(!contentGroup.Exists)
				CreateContentGroup(moduleId, templateId);
			else
				contentGroup.UpdateTemplate(templateId);
		}

		public ContentGroup GetContentGroupForModule(int moduleId)
		{
			var moduleControl = new ModuleController();
			var settings = moduleControl.GetModuleSettings(moduleId);

			// Return a "faked" ContentGroup if it does not exist yet (with the preview templateId)
			if (settings[SexyContent.ContentGroupGuidString] == null)
			{
				var previewTemplateString = settings[PreviewTemplateIdString];
				return new ContentGroup(previewTemplateString != null ? Guid.Parse(previewTemplateString.ToString()) : new Guid?(), _zoneId, _appId);
			}

			settings = moduleControl.GetModuleSettings(moduleId);
			return GetContentGroup(Guid.Parse(settings[SexyContent.ContentGroupGuidString].ToString()));
		}
	}
}