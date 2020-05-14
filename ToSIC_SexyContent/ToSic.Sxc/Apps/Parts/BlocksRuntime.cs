using System;
using System.Collections.Generic;
using ToSic.Eav;
using ToSic.Eav.Data;
using ToSic.Eav.DataSources;
using ToSic.Eav.Logging;
using ToSic.Sxc.Apps.Blocks;
using ToSic.Sxc.Interfaces;

namespace ToSic.Sxc.Apps
{
    public class BlocksRuntime: CmsRuntimeBase
    {
        public const string BlockTypeName = "2SexyContent-ContentGroup";

        internal BlocksRuntime(CmsRuntime cmsRuntime, ILog parentLog) : base(cmsRuntime, parentLog, "Sxc.BlkRdr")
        {
        }

        internal IDataSource ContentGroupSource()
        {
            var dataSource = CmsRuntime.Data;
            var onlyCGs = CmsRuntime.DataSourceFactory.GetDataSource<EntityTypeFilter>(CmsRuntime, dataSource);
            onlyCGs.TypeName = BlockTypeName;
            return dataSource;
        }

        public IEnumerable<IEntity> ContentBlockEntities() => ContentGroupSource().List;

        public BlockConfiguration GetBlockConfig(Guid contentGroupGuid)
        {
            var wrapLog = Log.Call($"get CG#{contentGroupGuid}");
            var dataSource = ContentGroupSource();
            // ToDo: Should use an indexed guid source
            var groupEntity = dataSource.List.One(contentGroupGuid);
            var found = groupEntity != null;
            wrapLog(found ? "found" : "missing");
            return found
                ? new BlockConfiguration(groupEntity, CmsRuntime, Log)
                : new BlockConfiguration(Guid.Empty, CmsRuntime, Log)
                {
                    DataIsMissing = true
                };
        }


        public BlockConfiguration GetInstanceContentGroup(int instanceId, int? pageId)
            => Factory.Resolve<IMapAppToInstance>().GetInstanceContentGroup(this, Log, instanceId, pageId);

        internal BlockConfiguration GetContentGroupOrGeneratePreview(Guid groupGuid, Guid previewTemplateGuid)
        {
            var wrapLog = Log.Call($"get CG or gen preview for grp#{groupGuid}, preview#{previewTemplateGuid}");
            // Return a "faked" ContentGroup if it does not exist yet (with the preview templateId)
            var createFake = groupGuid == Guid.Empty;
            Log.Add($"{nameof(createFake)}:{createFake}");
            var result = createFake
                ? new BlockConfiguration(previewTemplateGuid, CmsRuntime, Log)
                : GetBlockConfig(groupGuid);
            wrapLog(null);
            return result;
        }

    }
}
