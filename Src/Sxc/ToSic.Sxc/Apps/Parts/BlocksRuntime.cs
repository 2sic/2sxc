using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using ToSic.Eav.Apps.Parts;
using ToSic.Eav.Apps.Run;
using ToSic.Eav.Context;
using ToSic.Eav.Data;
using ToSic.Lib.Logging;
using ToSic.Sxc.Apps.Blocks;
using ToSic.Sxc.Blocks;
// ReSharper disable ConvertToNullCoalescingCompoundAssignment

namespace ToSic.Sxc.Apps
{
    public class BlocksRuntime: PartOf<CmsRuntime>
    {
        private readonly IZoneCultureResolver _cultureResolver;
        public const string BlockTypeName = "2SexyContent-ContentGroup";

        public BlocksRuntime(IZoneCultureResolver cultureResolver) : base("Sxc.BlkRdr") 
            => ConnectServices(_cultureResolver = cultureResolver);

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

        public IImmutableList<IEntity> ContentGroups() => _contentGroups ?? (_contentGroups = Parent.Entities.Get(BlockTypeName).ToImmutableList());
        private IImmutableList<IEntity> _contentGroups;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contentGroupGuid"></param>
        /// <returns>Will always return an object, even if the group doesn't exist yet. The .Entity would be null then</returns>
        public BlockConfiguration GetBlockConfig(Guid contentGroupGuid) => Log.Func($"get CG#{contentGroupGuid}", () =>
        {
            var groupEntity = ContentGroups().One(contentGroupGuid);
            var found = groupEntity != null;
            return (found
                    ? new BlockConfiguration(groupEntity, Parent, _cultureResolver.CurrentCultureCode, Log)
                        .WarnIfMissingData()
                    : new BlockConfiguration(null, Parent, _cultureResolver.CurrentCultureCode, Log)
                    {
                        PreviewTemplateId = Guid.Empty,
                        DataIsMissing = true
                    },
                found ? "found" : "missing");
        });
        

        internal BlockConfiguration GetOrGeneratePreviewConfig(IBlockIdentifier blockId
        ) => Log.Func($"get CG or gen preview for grp#{blockId.Guid}, preview#{blockId.PreviewView}", l =>
        {
            // Return a "faked" ContentGroup if it does not exist yet (with the preview templateId)
            var createTempBlockForPreview = blockId.Guid == Guid.Empty;
            l.A($"{nameof(createTempBlockForPreview)}:{createTempBlockForPreview}");
            var result = createTempBlockForPreview
                ? new BlockConfiguration(null, Parent, _cultureResolver.CurrentCultureCode, Log) { PreviewTemplateId = blockId.PreviewView }
                : GetBlockConfig(blockId.Guid);
            result.BlockIdentifierOrNull = blockId;
            return result;
        });

    }
}
