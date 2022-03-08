using System;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Run;
using ToSic.Eav.Data;
using ToSic.Eav.Logging;
using ToSic.Sxc.Context;
using ToSic.Sxc.DataSources;

namespace ToSic.Sxc.Blocks
{
    public sealed class BlockFromEntity: BlockBase
    {
        internal const string CbPropertyApp = "App";
        internal const string CbPropertyTitle = "Title";
        internal const string CbPropertyContentGroup = "ContentGroup";

        /// <summary>
        /// This is the entity that was used to configure the block
        /// We need it for later operations, like mentioning what index it was on in a list
        /// </summary>
        public IEntity Entity;
        
        #region Constructor and DI

        public BlockFromEntity(Dependencies dependencies, Lazy<AppFinder> appFinderLazy) : base(dependencies, "CB.Ent")
        {
            _appFinderLazy = appFinderLazy;
        }
        private readonly Lazy<AppFinder> _appFinderLazy;

        public BlockFromEntity Init(IBlock parent, IEntity blockEntity, ILog parentLog)
        {
            var ctx = parent.Context.Clone(Log) as IContextOfBlock;
            Init(ctx, parent, parentLog);
            var wrapLog = Log.Call<BlockFromEntity>($"{nameof(blockEntity)}:{blockEntity.EntityId}", useTimer: true);
            return wrapLog(null, CompleteInit(parent, blockEntity));
        }

        public BlockFromEntity Init(IBlock parent, int contentBlockId, ILog parentLog)
        {
            var ctx = parent.Context.Clone(Log) as IContextOfBlock;
            Init(ctx, parent, parentLog);
            var wrapLog = Log.Call<BlockFromEntity>($"{nameof(contentBlockId)}:{contentBlockId}");
            var blockEntity = GetBlockEntity(parent, contentBlockId);
            return wrapLog(null, CompleteInit(parent, blockEntity));
        }

        private BlockFromEntity CompleteInit(IBlock parent, IEntity blockEntity)
        {
            var wrapLog = Log.Call<BlockFromEntity>();
            Entity = blockEntity;
            Parent = parent;
            var blockId = LoadBlockDefinition(parent.ZoneId, blockEntity, Log);
            Context.ResetApp(blockId);

            // Must override previous AppId, as that was of the container-block
            // but the current instance can be of another block
            AppId = blockId.AppId;

            CompleteInit(parent.BlockBuilder, blockId, -blockEntity.EntityId);
            return wrapLog("ok", this);
        }
        #endregion



        /// <summary>
        /// Get the content-block definition if we only have the ID
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="contentBlockId"></param>
        /// <returns></returns>
        private static IEntity GetBlockEntity(IBlock parent, int contentBlockId)
        {
            // for various reasons this can be introduced as a negative value, make sure we neutralize that
            contentBlockId = Math.Abs(contentBlockId); 
            return parent.App.Data.List.One(contentBlockId);
        }

        #region ContentBlock Definition Entity

        /// <summary>
        /// Get all the specs for this content-block from the entity
        /// </summary>
        /// <returns></returns>
        private IBlockIdentifier LoadBlockDefinition(int zoneId, IEntity blockDefinition, ILog log)
        {
            var appName = blockDefinition.Value<string>(CbPropertyApp) ?? "";
            IsContentApp = appName == Eav.Constants.DefaultAppGuid;
            var temp = blockDefinition.Value<string>(CbPropertyContentGroup) ?? "";
            Guid.TryParse(temp, out var contentGroupGuid);

            temp = blockDefinition.Value<string>(ViewParts.TemplateContentType) ?? "";
            Guid.TryParse(temp, out var previewTemplateGuid);

            var appId = _appFinderLazy.Value.Init(log)/* new ZoneRuntime().Init(zoneId, log)*/.FindAppId(zoneId, appName);
            return new BlockIdentifier(zoneId, appId, contentGroupGuid, previewTemplateGuid);
        }
        #endregion

    }
}