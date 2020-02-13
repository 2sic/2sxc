using System;
using System.Collections.Generic;
using ToSic.Eav;
using ToSic.Eav.Apps;
using ToSic.Eav.Data;

namespace ToSic.Sxc.Blocks
{
    internal class BlockEditorForEntity : BlockEditorBase
    {
        internal BlockEditorForEntity(IBlockBuilder blockBuilder): base(blockBuilder) { }

        #region methods which the entity-implementation must customize 

        protected override void SavePreviewTemplateId(Guid templateGuid)
            => Update(new Dictionary<string, object>
            {
                {BlockFromEntity.CbPropertyTemplate, templateGuid.ToString()}
            });


        internal override void SetAppId(int? appId)
        {
            // 2rm 2016-04-05 added resolver for app guid here (discuss w/ 2dm, I'm not sure why the id was saved before)
            var appName = "";
            if (appId.HasValue)
            {
                var cache = /*Factory.GetAppsCache*/Eav.Apps.State.Cache;
                var zoneAppId = cache.GetIdentity(null, appId);
                appName = cache.Zones[zoneAppId.ZoneId].Apps[appId.Value];
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
        {
            var app = ((BlockBase)BlockBuilder.Block).Parent.App;
            new AppManager(app, Log)
                .Entities.UpdateParts(Math.Abs(BlockBuilder.Block.ContentBlockId), newValues);
        }

        #endregion

    }

}
