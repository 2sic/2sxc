using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using ToSic.Eav.Apps.Parts;
using ToSic.Eav.Apps.Run;
using ToSic.Eav.Context;
using ToSic.Eav.Data;
using ToSic.Eav.Logging;
using ToSic.Sxc.Apps.Blocks;
using ToSic.Sxc.Blocks;
// ReSharper disable ConvertToNullCoalescingCompoundAssignment

namespace ToSic.Sxc.Apps
{
    public class BlocksRuntime: PartOf<CmsRuntime, BlocksRuntime>
    {
        private readonly IZoneCultureResolver _cultureResolver;
        public const string BlockTypeName = "2SexyContent-ContentGroup";

        public BlocksRuntime(IZoneCultureResolver cultureResolver) : base("Sxc.BlkRdr")
        {
            _cultureResolver = cultureResolver;
        }

        //private IDataSource ContentGroupSource()
        //{
        //    if (_contentGroupSource != null) return _contentGroupSource;
        //    var dataSource = Parent.Data;
        //    var onlyCGs = Parent.DataSourceFactory.GetDataSource<EntityTypeFilter>(Parent, dataSource);
        //    onlyCGs.TypeName = BlockTypeName;
        //    return _contentGroupSource = dataSource;
        //}
        //private IDataSource _contentGroupSource;

        public List<BlockConfiguration> AllWithView()
        {
            return ContentGroups()
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
                .Select(e => new BlockConfiguration(e.Entity, Parent, _cultureResolver.CurrentCultureCode, Log))
                .ToList();
        }

        public IImmutableList<IEntity> ContentGroups() => _contentGroups ?? (_contentGroups = Parent.Entities.Get(BlockTypeName).ToImmutableArray());
        private IImmutableList<IEntity> _contentGroups;

        public BlockConfiguration GetBlockConfig(Guid contentGroupGuid)
        {
            var wrapLog = Log.Call($"get CG#{contentGroupGuid}");
            var groupEntity = ContentGroups().One(contentGroupGuid);
            var found = groupEntity != null;
            wrapLog(found ? "found" : "missing");
            return found
                ? new BlockConfiguration(groupEntity, Parent, _cultureResolver.CurrentCultureCode, Log).WarnIfMissingData()
                : new BlockConfiguration(null, Parent, _cultureResolver.CurrentCultureCode, Log)
                {
                    PreviewTemplateId = Guid.Empty,
                    DataIsMissing = true
                };
        }
        

        internal BlockConfiguration GetOrGeneratePreviewConfig(IBlockIdentifier blockId)
        {
            var wrapLog = Log.Call($"get CG or gen preview for grp#{blockId.Guid}, preview#{blockId.PreviewView}");
            // Return a "faked" ContentGroup if it does not exist yet (with the preview templateId)
            var createTempBlockForPreview = blockId.Guid == Guid.Empty;
            Log.A($"{nameof(createTempBlockForPreview)}:{createTempBlockForPreview}");
            var result = createTempBlockForPreview
                ? new BlockConfiguration(null, Parent, _cultureResolver.CurrentCultureCode, Log) { PreviewTemplateId = blockId.PreviewView }
                : GetBlockConfig(blockId.Guid);
            result.BlockIdentifierOrNull = blockId;
            wrapLog(null);
            return result;
        }

    }
}
