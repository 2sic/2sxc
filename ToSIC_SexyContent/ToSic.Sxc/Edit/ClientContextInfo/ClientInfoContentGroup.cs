using System;

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

        public ClientInfoContentGroup(SxcInstance sxc, bool isCreated)
        {
            IsCreated = isCreated;
            IsContent = sxc.IsContentApp;

            Id = sxc.ContentGroup?.ContentGroupId ?? 0;
            Guid = sxc.ContentGroup?.ContentGroupGuid ?? Guid.Empty;
            AppId = sxc.AppId ?? 0;
            AppUrl = sxc.App?.Path ?? "" + "/";
            AppSettingsId = (sxc.App?.Settings?.Entity?.Attributes?.Count > 0)
                ? sxc.App?.Settings?.EntityId : null;    // the real id (if entity exists), 0 (if entity missing, but type has fields), or null (if not available)
            AppResourcesId = (sxc.App?.Resources?.Entity?.Attributes?.Count > 0)
                ? sxc.App?.Resources?.EntityId : null;  // the real id (if entity exists), 0 (if entity missing, but type has fields), or null (if not available)

            HasContent = sxc.Template != null && (sxc.ContentGroup?.Exists ?? false);

            ZoneId = sxc.ZoneId ?? 0;
            TemplateId = sxc.Template?.TemplateId ?? 0;
            QueryId = sxc.Template?.Query?.EntityId; // will be null if not defined
            ContentTypeName = sxc.Template?.ContentTypeStaticName ?? "";
            IsList = sxc.ContentGroup?.Template?.UseForList ?? false;//  isCreated && ((sxc.ContentGroup?.Content?.Count ?? 0) > 1);
            SupportsAjax = sxc.IsContentApp || sxc.App?.Configuration?.SupportsAjaxReload ?? false;
        }
    }
}
