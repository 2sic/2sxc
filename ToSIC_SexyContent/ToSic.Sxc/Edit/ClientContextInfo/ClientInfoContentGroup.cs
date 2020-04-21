using System;
using ToSic.Sxc.Blocks;

namespace ToSic.SexyContent.Edit.ClientContextInfo
{
    public class ClientInfoContentGroup : ClientInfoEntity
    {
        public bool IsCreated;
        public bool IsList;
        public int TemplateId;
        public int? QueryId;
        public string ContentTypeName;
        public string AppUrl;
        public int? AppSettingsId;
        public int? AppResourcesId;

        public bool IsContent;
        public bool HasContent;
        public bool SupportsAjax;

        public string TemplateEdition;

        public ClientInfoContentGroup(IBlockBuilder blockBuilder, bool isCreated)
        {
            IsCreated = isCreated;
            IsContent = blockBuilder.Block.IsContentApp;

            Id = blockBuilder.Block.Configuration?.ContentGroupId ?? 0;
            Guid = blockBuilder.Block.Configuration?.ContentGroupGuid ?? Guid.Empty;
            AppId = blockBuilder.Block.AppId;// 2019-11-09, Id not nullable any more // ?? 0;
            AppUrl = blockBuilder.App?.Path ?? "" + "/";
            AppSettingsId = (blockBuilder.App?.Settings?.Entity?.Attributes?.Count > 0)
                ? blockBuilder.App?.Settings?.EntityId : null;    // the real id (if entity exists), 0 (if entity missing, but type has fields), or null (if not available)
            AppResourcesId = (blockBuilder.App?.Resources?.Entity?.Attributes?.Count > 0)
                ? blockBuilder.App?.Resources?.EntityId : null;  // the real id (if entity exists), 0 (if entity missing, but type has fields), or null (if not available)

            HasContent = blockBuilder.View != null && (blockBuilder.Block.Configuration?.Exists ?? false);

            ZoneId = blockBuilder.Block.ZoneId; // 2019-11-09, Id not nullable any more // ?? 0;
            TemplateId = blockBuilder.View?.Id ?? 0;
            TemplateEdition = blockBuilder.View?.Edition;
            QueryId = blockBuilder.View?.Query?.Id; // will be null if not defined
            ContentTypeName = blockBuilder.View?.ContentType ?? "";
            IsList = blockBuilder.Block.Configuration?.View?.UseForList ?? false;//  isCreated && ((sxc.BlockConfiguration?.Content?.Count ?? 0) > 1);
            SupportsAjax = blockBuilder.Block.IsContentApp || (blockBuilder.App?.Configuration?.EnableAjax ?? false);
        }
    }
}
