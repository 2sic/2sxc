using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav;
using ToSic.Eav.Apps;
using ToSic.Eav.Data.Query;
using ToSic.Eav.DataSources;
using ToSic.Eav.Logging;
using ToSic.Sxc.Apps.Blocks;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Interfaces;

namespace ToSic.Sxc.Apps
{
	public class BlocksManager: HasLog
	{
		private const string ContentGroupTypeName = "2SexyContent-ContentGroup";

		private readonly int _zoneId;
		private readonly int _appId;
        private readonly bool _showDrafts;
        private readonly bool _enableVersioning;

        public BlocksManager(int zoneId, int appId, bool showDrafts, bool enableVersioning, ILog parentLog)
            : base("CG.Manage", parentLog, "constructor", nameof(BlocksManager))
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

		public IEnumerable<BlockConfiguration> GetContentGroups() 
            => ContentGroupSource().List.Select(p => new BlockConfiguration(p, _zoneId, _appId, _showDrafts, _enableVersioning, Log));

	    public BlockConfiguration GetBlockConfig(Guid contentGroupGuid)
		{
		    Log.Add($"get CG#{contentGroupGuid}");
			var dataSource = ContentGroupSource();
			// ToDo: Should use an indexed guid source
		    var groupEntity = dataSource.List.One(contentGroupGuid);
		    return groupEntity != null 
                ? new BlockConfiguration(groupEntity, _zoneId, _appId, _showDrafts, _enableVersioning, Log) 
                : new BlockConfiguration(Guid.Empty, _zoneId, _appId, _showDrafts, _enableVersioning, Log)
                {
                    DataIsMissing = true
                };
		}

	    /// <summary>
	    /// Saves a temporary templateId to the module's settings
	    /// This templateId will be used until a contentgroup exists
	    /// </summary>
	    public static void SetPreviewTemplate(int instanceId, Guid previewTemplateGuid) 
            => Factory.Resolve<IMapAppToInstance>().SetPreviewTemplate(instanceId, previewTemplateGuid);

	    public static void ClearPreviewTemplate(int instanceId) 
            => Factory.Resolve<IMapAppToInstance>().ClearPreviewTemplate(instanceId);

	    public Guid UpdateOrCreateContentGroup(BlockConfiguration blockConfiguration, int templateId)
		{
		    var appMan = new AppManager(_zoneId, _appId, Log);

		    if (!blockConfiguration.Exists)
		    {
		        Log.Add($"doesn't exist, will creat new CG with template#{templateId}");
		        return appMan.Entities.Create(ContentGroupTypeName, new Dictionary<string, object>
		        {
		            {"Template", new List<int> {templateId}},
		            {ViewParts.Content, new List<int>()},
		            {ViewParts.Presentation, new List<int>()},
		            {ViewParts.ListContent, new List<int>()},
		            {ViewParts.ListPresentation, new List<int>()}
		        }).Item2; // new guid
		    }
		    else
		    {
		        Log.Add($"exists, create for group#{blockConfiguration.ContentGroupGuid} with template#{templateId}");
		        appMan.Entities.UpdateParts(blockConfiguration._contentGroupEntity.EntityId,
		            new Dictionary<string, object> {{"Template", new List<int?> {templateId}}});

		        return blockConfiguration.ContentGroupGuid; // guid didn't change
		    }
		}

	    public BlockConfiguration GetInstanceContentGroup(int instanceId, int? pageId)
	        => Factory.Resolve<IMapAppToInstance>().GetInstanceContentGroup(this, Log, instanceId, pageId);

        internal BlockConfiguration GetContentGroupOrGeneratePreview(Guid groupGuid, Guid previewTemplateGuid)
	    {
	        Log.Add($"get CG or gen preview for grp#{groupGuid}, preview#{previewTemplateGuid}");
	        // Return a "faked" ContentGroup if it does not exist yet (with the preview templateId)
	        return groupGuid == Guid.Empty 
                ? new BlockConfiguration(previewTemplateGuid, _zoneId, _appId, _showDrafts, _enableVersioning, Log)
                : GetBlockConfig(groupGuid);
	    }

	}
}