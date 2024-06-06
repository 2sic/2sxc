using ToSic.Eav.Apps.Internal;
using ToSic.Eav.Cms.Internal;
using ToSic.Lib.DI;
using ToSic.Sxc.Context.Internal;

namespace ToSic.Sxc.Blocks.Internal;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public sealed class BlockFromEntity(BlockBase.MyServices services, LazySvc<AppFinder> appFinderLazy)
    : BlockBase(services, "CB.Ent", connect: [appFinderLazy])
{
    internal const string CbPropertyApp = "App";
    internal const string CbPropertyTitle = Attributes.TitleNiceName;
    internal const string CbPropertyContentGroup = "ContentGroup";

    /// <summary>
    /// This is the entity that was used to configure the block
    /// We need it for later operations, like mentioning what index it was on in a list
    /// </summary>
    public IEntity Entity;
        
    #region Init

    public BlockFromEntity Init(IBlock parent, IEntity blockEntity, int contentBlockId = -1)
    {
        var l = Log.Fn<BlockFromEntity>($"{nameof(contentBlockId)}:{contentBlockId}; {nameof(blockEntity)}:{blockEntity?.EntityId}", timer: true);
        var ctx = parent.Context.Clone(Log) as IContextOfBlock;
        Init(ctx, parent);
        blockEntity ??= GetBlockEntity(parent, contentBlockId);

        Entity = blockEntity;
        Parent = parent;

        var blockId = LoadBlockDefinition(parent.ZoneId, blockEntity, Log);
        Context.ResetApp(blockId);

        // Must override previous AppId, as that was of the container-block
        // but the current instance can be of another block
        AppId = blockId.AppId;

        CompleteInit(parent.BlockBuilder, blockId, -blockEntity.EntityId);

        return l.Return(this);
    }

    #endregion



    /// <summary>
    /// Get the content-block definition if we only have the ID
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="contentBlockId">The block ID. Can sometimes be negative to mark inner-content-blocks</param>
    /// <returns></returns>
    private static IEntity GetBlockEntity(IBlock parent, int contentBlockId) 
        => parent.App.Data.List.One(Math.Abs(contentBlockId));

    #region ContentBlock Definition Entity

    /// <summary>
    /// Get all the specs for this content-block from the entity
    /// </summary>
    /// <returns></returns>
    private IBlockIdentifier LoadBlockDefinition(int zoneId, IEntity blockDefinition, ILog log)
    {
        var appNameId = blockDefinition.Get(CbPropertyApp, fallback: "");
        IsContentApp = appNameId == Eav.Constants.DefaultAppGuid;
        var temp = blockDefinition.Get(CbPropertyContentGroup, fallback: "");
        Guid.TryParse(temp, out var contentGroupGuid);

        temp = blockDefinition.Get(ViewParts.TemplateContentType, fallback: "");
        Guid.TryParse(temp, out var previewTemplateGuid);

        var appId = appFinderLazy.Value.FindAppId(zoneId, appNameId);
        return new BlockIdentifier(zoneId, appId, appNameId, contentGroupGuid, previewTemplateGuid);
    }
    #endregion

}