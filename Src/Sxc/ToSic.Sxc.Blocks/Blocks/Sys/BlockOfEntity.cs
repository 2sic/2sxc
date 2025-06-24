using ToSic.Eav.Apps.Sys;
using ToSic.Eav.Cms.Internal;
using ToSic.Eav.Data.Entities.Sys.Lists;
using ToSic.Lib.Services;
using ToSic.Sxc.Context.Internal;

namespace ToSic.Sxc.Blocks.Internal;

[ShowApiWhenReleased(ShowApiMode.Never)]
public sealed class BlockOfEntity(BlockGeneratorHelpers helpers, LazySvc<AppFinder> appFinderLazy)
    : ServiceBase("CB.Ent", connect: [appFinderLazy, helpers])
{
    #region Init

    public IBlock GetBlockOfEntity(IBlock parentBlock, IEntity? blockEntity, int contentBlockId = -1)
    {
        var l = Log.Fn<BlockSpecs>($"{nameof(contentBlockId)}:{contentBlockId}; {nameof(blockEntity)}:{blockEntity?.EntityId}", timer: true);

        // Get the content-block definition if we only have the ID
        // The block ID. Is usually negative to mark inner-content-blocks
        blockEntity ??= parentBlock.App.Data.List.One(Math.Abs(contentBlockId))!;

        var (isContentApp, blockId) = LoadBlockDefinition(appFinderLazy, parentBlock.ZoneId, blockEntity);
        var ctx = (IContextOfBlock)parentBlock.Context.Clone(Log);
        ctx.ResetApp(blockId);

        // Must override previous AppId, as that was of the container-block
        // but the current instance can be of another block
        var specs = new BlockSpecs
        {
            Context = ctx,
            AppId = blockId.AppId,
            ZoneId = parentBlock.ZoneId,
            IsContentApp = isContentApp,
            IsInnerBlock = true,
        };

        specs = helpers.CompleteInit(specs, parentBlock, blockId, -blockEntity.EntityId);

        return l.Return(specs);
    }

    #endregion


    #region ContentBlock Definition Entity

    /// <summary>
    /// Get all the specs for this content-block from the entity
    /// </summary>
    /// <returns></returns>
    private static (bool IsContentApp, IBlockIdentifier BlockIdentifier) LoadBlockDefinition(LazySvc<AppFinder> appFinderLazy, int zoneId, IEntity blockDefinition)
    {
        var appNameId = blockDefinition.Get(BlockBuildingConstants.CbPropertyApp, fallback: "");
        var isContentApp = appNameId == KnownAppsConstants.DefaultAppGuid;

        var temp = blockDefinition.Get(BlockBuildingConstants.CbPropertyContentGroup, fallback: "");
#pragma warning disable CA1806
        Guid.TryParse(temp, out var contentGroupGuid);

        temp = blockDefinition.Get(ViewParts.TemplateContentType, fallback: "");
        Guid.TryParse(temp, out var previewTemplateGuid);
#pragma warning restore CA1806

        var appId = appFinderLazy.Value.FindAppId(zoneId, appNameId);
        var identifier = new BlockIdentifier(zoneId, appId, appNameId, contentGroupGuid, previewTemplateGuid);
        return (isContentApp, identifier);
    }
    #endregion

}