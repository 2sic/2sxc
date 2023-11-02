using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using ToSic.Eav.Apps.Run;
using ToSic.Eav.Apps.Work;
using ToSic.Eav.Context;
using ToSic.Eav.Data;
using ToSic.Eav.DataSource.Query;
using ToSic.Lib.DI;
using ToSic.Lib.Logging;
using ToSic.Lib.Services;
using ToSic.Sxc.Apps.Blocks;
using ToSic.Sxc.Blocks;

namespace ToSic.Sxc.Apps.CmsSys
{
    public class AppBlocks: ServiceBase
    {
        public const string BlockTypeName = "2SexyContent-ContentGroup";

        private readonly AppWork _appWork;
        private readonly LazySvc<QueryDefinitionBuilder> _qDefBuilder;
        private readonly IZoneCultureResolver _cultureResolver;

        public AppBlocks(IZoneCultureResolver cultureResolver, LazySvc<QueryDefinitionBuilder> qDefBuilder, AppWork appWork) : base("SxS.Blocks")
        {
            ConnectServices(
                _cultureResolver = cultureResolver,
                _qDefBuilder = qDefBuilder,
                _appWork = appWork
            );
        }

        // ReSharper disable once ConvertToNullCoalescingCompoundAssignment
        private IImmutableList<IEntity> ContentGroups(IAppWorkCtxPlus appCtx) => _appWork.Entities.Get(appCtx, BlockTypeName).ToImmutableList();

        public List<BlockConfiguration> AllWithView(IAppWorkCtxPlus appCtx)
        {
            return ContentGroups(appCtx)
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
                .Select(e => new BlockConfiguration(e.Entity, appCtx, null, _qDefBuilder, _cultureResolver.CurrentCultureCode, Log))
                .ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Will always return an object, even if the group doesn't exist yet. The .Entity would be null then</returns>
        public BlockConfiguration GetBlockConfig(IAppWorkCtxPlus appCtx, Guid contentGroupGuid)
        {
            var l = Log.Fn<BlockConfiguration>($"get CG#{contentGroupGuid}");
            var groupEntity = ContentGroups(appCtx).One(contentGroupGuid);
            var found = groupEntity != null;
            return l.Return(found
                    ? new BlockConfiguration(groupEntity, appCtx, null, _qDefBuilder, _cultureResolver.CurrentCultureCode, Log)
                        .WarnIfMissingData()
                    : new BlockConfiguration(null, appCtx, null, _qDefBuilder, _cultureResolver.CurrentCultureCode, Log)
                    {
                        DataIsMissing = true
                    },
                found ? "found" : "missing");
        }


        internal BlockConfiguration GetOrGeneratePreviewConfig(IAppWorkCtxPlus appCtx, IBlockIdentifier blockId)
        {
            var l = Log.Fn<BlockConfiguration>($"grp#{blockId.Guid}, preview#{blockId.PreviewView}");
            // Return a "faked" ContentGroup if it does not exist yet (with the preview templateId)
            var createTempBlockForPreview = blockId.Guid == Guid.Empty;
            l.A($"{nameof(createTempBlockForPreview)}:{createTempBlockForPreview}");
            var result = createTempBlockForPreview
                ? new BlockConfiguration(null, appCtx, appCtx.Data.List.One(blockId.PreviewView), _qDefBuilder, _cultureResolver.CurrentCultureCode, Log)
                : GetBlockConfig(appCtx, blockId.Guid);
            result.BlockIdentifierOrNull = blockId;
            return l.Return(result);
        }

    }
}
