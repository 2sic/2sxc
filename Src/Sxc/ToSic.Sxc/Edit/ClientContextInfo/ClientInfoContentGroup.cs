using System;
using Newtonsoft.Json;
using ToSic.Sxc.Blocks;
using static Newtonsoft.Json.NullValueHandling;

namespace ToSic.Sxc.Edit.ClientContextInfo
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

        [JsonProperty(NullValueHandling = Ignore)]
        public string TemplateEdition;

        public ClientInfoContentGroup(IBlock block)
        {
            //var block = blockBuilder.Block;
            IsCreated = block.ContentGroupExists;
            IsContent = block.IsContentApp;
            var app = block.App;

            Id = block.Configuration?.Id ?? 0;
            Guid = block.Configuration?.Guid ?? Guid.Empty;
            AppId = block.AppId;
            AppUrl = app?.Path ?? "" + "/";
            AppSettingsId = app?.Settings?.Entity?.Attributes?.Count > 0
                ? app?.Settings?.EntityId : null;    // the real id (if entity exists), 0 (if entity missing, but type has fields), or null (if not available)
            AppResourcesId = app?.Resources?.Entity?.Attributes?.Count > 0
                ? app?.Resources?.EntityId : null;  // the real id (if entity exists), 0 (if entity missing, but type has fields), or null (if not available)

            HasContent = block.View != null && (block.Configuration?.Exists ?? false);

            ZoneId = block.ZoneId; // 2019-11-09, Id not nullable any more // ?? 0;
            TemplateId = block.View?.Id ?? 0;
            TemplateEdition = block.View?.Edition;
            QueryId = block.View?.Query?.Id; // will be null if not defined
            ContentTypeName = block.View?.ContentType ?? "";
            IsList = block.Configuration?.View?.UseForList ?? false;//  isCreated && ((sxc.BlockConfiguration?.Content?.Count ?? 0) > 1);
            SupportsAjax = block.IsContentApp || (block.App?.Configuration?.EnableAjax ?? false);
        }
    }
}
