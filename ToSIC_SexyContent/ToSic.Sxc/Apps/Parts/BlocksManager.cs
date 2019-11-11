using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Logging;
using ToSic.Sxc.Apps.Blocks;
using ToSic.Sxc.Blocks;

namespace ToSic.Sxc.Apps
{
	public partial class BlocksManager: CmsManagerBase
	{
        public BlocksManager(CmsManager cms, ILog parentLog)
            : base(cms, parentLog, "CG.Manage")
        {
        }

		public IEnumerable<BlockConfiguration> GetContentGroups() 
            => CmsManager.Read.Blocks.ContentGroupSource().List
                .Select(p => new BlockConfiguration(p, CmsManager.ZoneId, CmsManager.AppId, CmsManager.ShowDrafts, CmsManager.EnablePublishing, Log));


	    public Guid UpdateOrCreateContentGroup(BlockConfiguration blockConfiguration, int templateId)
		{
		    var appMan = CmsManager;

		    if (!blockConfiguration.Exists)
		    {
		        Log.Add($"doesn't exist, will creat new CG with template#{templateId}");
		        return appMan.Entities.Create(BlocksRuntime.BlockTypeName, new Dictionary<string, object>
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
		        appMan.Entities.UpdateParts(blockConfiguration.Entity.EntityId,
		            new Dictionary<string, object> {{"Template", new List<int?> {templateId}}});

		        return blockConfiguration.ContentGroupGuid; // guid didn't change
		    }
		}


	}
}