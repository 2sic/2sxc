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

        public ClientInfoContentGroup(ICmsBlock cms, bool isCreated)
        {
            IsCreated = isCreated;
            IsContent = cms.Block.IsContentApp;

            Id = cms.Block.Configuration?.ContentGroupId ?? 0;
            Guid = cms.Block.Configuration?.ContentGroupGuid ?? Guid.Empty;
            AppId = cms.Block.AppId;// 2019-11-09, Id not nullable any more // ?? 0;
            AppUrl = cms.App?.Path ?? "" + "/";
            AppSettingsId = (cms.App?.Settings?.Entity?.Attributes?.Count > 0)
                ? cms.App?.Settings?.EntityId : null;    // the real id (if entity exists), 0 (if entity missing, but type has fields), or null (if not available)
            AppResourcesId = (cms.App?.Resources?.Entity?.Attributes?.Count > 0)
                ? cms.App?.Resources?.EntityId : null;  // the real id (if entity exists), 0 (if entity missing, but type has fields), or null (if not available)

            HasContent = cms.View != null && (cms.Block.Configuration?.Exists ?? false);

            ZoneId = cms.Block.ZoneId; // 2019-11-09, Id not nullable any more // ?? 0;
            TemplateId = cms.View?.Id ?? 0;
            QueryId = cms.View?.Query?.Id; // will be null if not defined
            ContentTypeName = cms.View?.ContentType ?? "";
            IsList = cms.Block.Configuration?.View?.UseForList ?? false;//  isCreated && ((sxc.BlockConfiguration?.Content?.Count ?? 0) > 1);
            SupportsAjax = cms.Block.IsContentApp || (cms.App?.Configuration?.EnableAjax ?? false);
        }
    }
}
