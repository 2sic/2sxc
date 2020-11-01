using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Apps.Parts;
using ToSic.Eav.Apps.Run;
using ToSic.Eav.Data;
using ToSic.Eav.DataSources;
using ToSic.Sxc.Apps.Blocks;
using ToSic.Sxc.Blocks;

namespace ToSic.Sxc.Apps
{
    public class BlocksRuntime: PartOf<CmsRuntime, BlocksRuntime>
    {
        public const string BlockTypeName = "2SexyContent-ContentGroup";

        internal BlocksRuntime() : base("Sxc.BlkRdr")
        {
        }

        private IDataSource ContentGroupSource()
        {
            if (_contentGroupSource != null) return _contentGroupSource;
            var dataSource = Parent.Data;
            var onlyCGs = Parent.DataSourceFactory.GetDataSource<EntityTypeFilter>(Parent, dataSource);
            onlyCGs.TypeName = BlockTypeName;
            return _contentGroupSource = dataSource;
        }
        private IDataSource _contentGroupSource;

        public List<BlockConfiguration> AllWithView()
        {
            return Entities()
                .Select(b =>
                {
                    var templateGuid = b.Children(ViewParts.ViewFieldInContentBlock)
                        .FirstOrDefault()
                        ?.EntityGuid;
                    return templateGuid != null
                        ? new { Entity = b, ViewGuid = templateGuid }
                        : null;
                })
                .Where(b => b != null)
                .Select(e => new BlockConfiguration(e.Entity, Parent, Log))
                .ToList();
        }

        public IEnumerable<IEntity> Entities() => ContentGroupSource().Immutable;

        public BlockConfiguration GetBlockConfig(Guid contentGroupGuid)
        {
            var wrapLog = Log.Call($"get CG#{contentGroupGuid}");
            // ToDo: Should use an indexed guid source
            var groupEntity = Entities().One(contentGroupGuid);
            var found = groupEntity != null;
            wrapLog(found ? "found" : "missing");
            return found
                ? new BlockConfiguration(groupEntity, Parent, Log)
                : new BlockConfiguration(Guid.Empty, Parent, Log)
                {
                    DataIsMissing = true
                };
        }
        

        internal BlockConfiguration GetOrGeneratePreviewConfig(IBlockIdentifier blockId)
        {
            var wrapLog = Log.Call($"get CG or gen preview for grp#{blockId.Guid}, preview#{blockId.PreviewView}");
            // Return a "faked" ContentGroup if it does not exist yet (with the preview templateId)
            var createTempBlockForPreview = blockId.Guid == Guid.Empty;
            Log.Add($"{nameof(createTempBlockForPreview)}:{createTempBlockForPreview}");
            var result = createTempBlockForPreview
                ? new BlockConfiguration(blockId.PreviewView, Parent, Log)
                : GetBlockConfig(blockId.Guid);
            wrapLog(null);
            return result;
        }

    }
}
