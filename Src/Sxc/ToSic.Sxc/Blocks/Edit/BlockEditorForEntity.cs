using System;
using System.Collections.Generic;
using ToSic.Eav.Apps;
using ToSic.Eav.Data;
using ToSic.Lib.DI;
using ToSic.Sxc.Apps;
using ToSic.Lib.Logging;

namespace ToSic.Sxc.Blocks.Edit
{
    public class BlockEditorForEntity : BlockEditorBase
    {
        public BlockEditorForEntity(BlockEditorBaseServices services, LazySvc<CmsManager> parentCmsManager, IAppStates appStates) 
            : base(services)
        {
            ConnectServices(
                _parentCmsManager = parentCmsManager.SetInit(p => p.Init(((BlockBase)Block).Parent.App)),
                _appStates = appStates
            );
        }

        #region methods which the entity-implementation must customize 

        protected override void SavePreviewTemplateId(Guid templateGuid)
            => Update(new Dictionary<string, object>
            {
                {ViewParts.TemplateContentType, templateGuid.ToString()}
            });


        internal override void SetAppId(int? appId)
        {
            // 2rm 2016-04-05 added resolver for app guid here (discuss w/ 2dm, I'm not sure why the id was saved before)
            var appName = "";
            if (appId.HasValue)
            {
                var zoneAppId = _appStates.IdentityOfApp(appId.Value);
                appName = _appStates.AppIdentifier(zoneAppId.ZoneId, zoneAppId.AppId);
            }
            UpdateValue(BlockFromEntity.CbPropertyApp, appName);
        }

        internal override void EnsureLinkToContentGroup(Guid cgGuid)
            => UpdateValue(BlockFromEntity.CbPropertyContentGroup, cgGuid.ToString()); // must pre-convert to string, as it's not a reference to an entity in the same app


        internal override void UpdateTitle(IEntity titleItem)
        {
            if (titleItem?.GetBestTitle() == null) return;
            UpdateValue(BlockFromEntity.CbPropertyTitle, titleItem.GetBestTitle());
        }

        #endregion

        #region private helpers

        private void UpdateValue(string key, object value) 
            => Update(new Dictionary<string, object> { { key, value } });

        private void Update(Dictionary<string, object> newValues) 
            => ParentBlockAppManager().Entities.UpdateParts(Math.Abs(Block.ContentBlockId), newValues);

        protected AppManager ParentBlockAppManager() => _parentCmsManager.Value;
        private readonly LazySvc<CmsManager> _parentCmsManager;
        private readonly IAppStates _appStates;

        #endregion

    }

}
