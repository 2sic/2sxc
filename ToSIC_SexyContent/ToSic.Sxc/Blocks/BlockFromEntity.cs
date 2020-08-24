using System;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Run;
using ToSic.Eav.Data;
using ToSic.Eav.Logging;

namespace ToSic.Sxc.Blocks
{
    internal sealed class BlockFromEntity: BlockBase
    {
        internal const string CbPropertyApp = "App";
        internal const string CbPropertyTitle = "Title";
        internal const string CbPropertyContentGroup = "ContentGroup";
        
        #region Constructor and DI

        public BlockFromEntity() : base("CB.Ent") { }

        public BlockFromEntity Init(IBlock parent, IEntity blockEntity, ILog parentLog)
        {
            Init(parent.Context, parent, parentLog);
            return CompleteInit(parent, blockEntity);
        }

        public BlockFromEntity Init(IBlock parent, int contentBlockId, ILog parentLog)
        {
            Init(parent.Context, parent, parentLog);
            var wrapLog = Log.Call<BlockFromEntity>($"{nameof(contentBlockId)}:{contentBlockId}");
            var blockEntity = GetBlockEntity(parent, contentBlockId);
            return wrapLog("ok", CompleteInit(parent, blockEntity));
        }

        private BlockFromEntity CompleteInit(IBlock parent, IEntity blockEntity)
        {
            var wrapLog = Log.Call<BlockFromEntity>();
            Parent = parent;
            var blockId = LoadBlockDefinition(parent.ZoneId, blockEntity, Log);

            // Must override previous AppId, as that was of the container-block
            // but the current instance can be of another block
            AppId = blockId.AppId;

            CompleteInit<BlockFromEntity>(parent.BlockBuilder, blockId, -blockEntity.EntityId);
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
            var appName = blockDefinition.GetBestValue(CbPropertyApp)?.ToString() ?? "";
            IsContentApp = appName == Eav.Constants.DefaultAppName;
            var temp = blockDefinition.GetBestValue(CbPropertyContentGroup)?.ToString() ?? "";
            Guid.TryParse(temp, out var contentGroupGuid);

            temp = blockDefinition.GetBestValue(ViewParts.TemplateContentType)?.ToString() ?? "";
            Guid.TryParse(temp, out var previewTemplateGuid);

            var appId = new ZoneRuntime(zoneId, log).FindAppId(appName);
            return new BlockIdentifier(zoneId, appId, contentGroupGuid, previewTemplateGuid);
        }
        #endregion

    }
}