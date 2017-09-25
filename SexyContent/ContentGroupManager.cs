using System;
using System.Collections.Generic;
using System.Linq;
using DotNetNuke.Entities.Modules;
using ToSic.Eav;
using ToSic.Eav.Apps;
using ToSic.Eav.DataSources;
using ToSic.Eav.Logging;
using ToSic.Eav.Logging.Simple;
using ToSic.SexyContent.Internal;

namespace ToSic.SexyContent
{
	public class ContentGroupManager: HasLog
	{
		private const string ContentGroupTypeName = "2SexyContent-ContentGroup";

		private readonly int _zoneId;
		private readonly int _appId;
        private readonly bool _showDrafts;
        private readonly bool _enableVersioning;

        public ContentGroupManager(int zoneId, int appId, bool showDrafts, bool enableVersioning, Log parentLog): base("CGMang", parentLog)
		{
			_zoneId = zoneId;
			_appId = appId;
            _showDrafts = showDrafts;
            _enableVersioning = enableVersioning;
		}

		private IDataSource ContentGroupSource()
		{
			var dataSource = DataSource.GetInitialDataSource(_zoneId, _appId, _showDrafts);
			dataSource = DataSource.GetDataSource<EntityTypeFilter>(_zoneId, _appId, dataSource);
			((EntityTypeFilter)dataSource).TypeName = ContentGroupTypeName;
			return dataSource;
		}

		public IEnumerable<ContentGroup> GetContentGroups()
		{
			return ContentGroupSource().List.Select(p => new ContentGroup(p.Value, _zoneId, _appId, _showDrafts, _enableVersioning, Log));
		}

		public ContentGroup GetContentGroup(Guid contentGroupGuid)
		{
		    Log.Add($"get CG#{contentGroupGuid}");
			var dataSource = ContentGroupSource();
			// ToDo: Should use an indexed guid source
		    var groupEntity = dataSource.List.FirstOrDefault(e => e.Value.EntityGuid == contentGroupGuid).Value;
		    return groupEntity != null 
                ? new ContentGroup(groupEntity, _zoneId, _appId, _showDrafts, _enableVersioning, Log) 
                : new ContentGroup(Guid.Empty, _zoneId, _appId, _showDrafts, _enableVersioning, Log) {DataIsMissing = true};
		}

		public bool IsConfigurationInUse(int templateId, string type)
		{
			var contentGroups = GetContentGroups().Where(p => p.Template != null && p.Template.TemplateId == templateId);
			return contentGroups.Any(p => p[type].Any(c => c != null));
		}

		public Guid CreateNewContentGroup(int? templateId)
		{
			var values = new Dictionary<string, object>
			{
				{"Template", templateId.HasValue ? new List<int> { templateId.Value } : new List<int>()},
				{ AppConstants.Content, new List<int>()},
				{ AppConstants.Presentation, new List<int>()},
				{AppConstants.ListContent, new List<int>()},
				{AppConstants.ListPresentation, new List<int>()}
			};

            return new AppManager(_zoneId, _appId).Entities.Create(ContentGroupTypeName, values).Item2;
		}


	    /// <summary>
	    /// Saves a temporary templateId to the module's settings
	    /// This templateId will be used until a contentgroup exists
	    /// </summary>
	    /// <param name="moduleId"></param>
	    /// <param name="previewTemplateGuid"></param>
	    public void SetModulePreviewTemplateId(int moduleId, Guid previewTemplateGuid)
		{
            // todo: 2rm - I believe you are accidentally using uncached module settings access - pls check and probably change
            // todo: note: this is done ca. 3x in this class
			var moduleController = new ModuleController();
			var settings = moduleController.GetModuleSettings(moduleId);

			// Do not allow saving the temporary template id if a contentgroup exists for this module
			if(settings[Settings.ContentGroupGuidString] != null)
				throw new Exception("Preview template id cannot be set for a module that already has content.");

			//var dataSource = DataSource.GetInitialDataSource(_zoneId, _appId);
			//var previewTemplateGuid = dataSource.List[previewTemplateId].EntityGuid;

            //moduleController.UpdateModuleSetting(moduleId, PreviewTemplateIdString, previewTemplateGuid.ToString());
            DnnStuffToRefactor.UpdateModuleSettingForAllLanguages(moduleId, Settings.PreviewTemplateIdString, previewTemplateGuid.ToString());
        }

		public static void DeletePreviewTemplateId(int moduleId)
		{
            DnnStuffToRefactor.UpdateModuleSettingForAllLanguages(moduleId, Settings.PreviewTemplateIdString, null);
		}

		public Guid UpdateOrCreateContentGroup(ContentGroup contentGroup, int templateId)
		{
		    if (!contentGroup.Exists)
		        return CreateNewContentGroup(templateId);
		    
		    contentGroup.UpdateTemplate(templateId);
		    return contentGroup.ContentGroupGuid;
		}

	    internal void PersistContentGroupAndBlankTemplateToModule(int moduleId, bool wasCreated, Guid guid)
	    {
            // Remove the previewTemplateId (because it's not needed as soon Content is inserted)
	        DeletePreviewTemplateId(moduleId);
	        // Update contentGroup Guid for this module
	        if (wasCreated)
	            DnnStuffToRefactor.UpdateModuleSettingForAllLanguages(moduleId, Settings.ContentGroupGuidString,
	                guid.ToString());
	    }

	    // todo: this doesn't look right, will have to mostly move to the new content-block
		public Tuple<Guid,Guid> GetContentGroupForModule(int moduleId, int tabId)
		{
		    Log.Add($"find content-group for mid#{moduleId} and tab#{tabId}");
			var settings = ModuleController.Instance.GetModule(moduleId, tabId, false).ModuleSettings;
			//var settings = moduleControl.GetModule(moduleId,).ModuleSettings;
		    var maybeGuid = settings[Settings.ContentGroupGuidString];
		    Guid.TryParse(maybeGuid?.ToString(), out var groupGuid);
            var previewTemplateString = settings[Settings.PreviewTemplateIdString]?.ToString();

		    var templateGuid = !string.IsNullOrEmpty(previewTemplateString)
		        ? Guid.Parse(previewTemplateString)
		        : new Guid();

		    return Tuple.Create(groupGuid, templateGuid);
            //return GetContentGroupOrGeneratePreview(groupGuid, templateGuid);
		}

	    internal ContentGroup GetContentGroupOrGeneratePreview(Guid groupGuid, Guid previewTemplateGuid)
	    {
	        Log.Add($"get CG or gen preview for grp#{groupGuid}, preview#{previewTemplateGuid}");
	        // Return a "faked" ContentGroup if it does not exist yet (with the preview templateId)
	        return groupGuid == Guid.Empty 
                ? new ContentGroup(previewTemplateGuid, _zoneId, _appId, _showDrafts, _enableVersioning, Log)
                : GetContentGroup(groupGuid);
	    }

	}
}