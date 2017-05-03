using System;
using System.Collections.Generic;
using ToSic.Eav;
using ToSic.Eav.Apps;
using ToSic.Eav.DataSources.Caches;

namespace ToSic.SexyContent.ContentBlocks
{
    internal class EntityContentGroupReferenceManager : ContentGroupReferenceManagerBase
    {
        internal EntityContentGroupReferenceManager(SxcInstance sxc): base(sxc)
        {
        }
        #region methods which the entity-implementation must customize - so it's virtual

        protected override void SavePreviewTemplateId(Guid templateGuid, bool? newTemplateChooserState = null)
        {
            var vals = new Dictionary<string, object>
            {
                {EntityContentBlock.CbPropertyTemplate, templateGuid.ToString()}
            };
            if (newTemplateChooserState.HasValue)
                vals.Add(EntityContentBlock.CbPropertyShowChooser, newTemplateChooserState.Value); // must blank the template
            Update(vals);
        }

        internal override void SetTemplateChooserState(bool state)
            => UpdateValue(EntityContentBlock.CbPropertyShowChooser, state);


        internal override void SetAppId(int? appId)
        {
            // 2rm 2016-04-05 added resolver for app guid here (discuss w/ 2dm, I'm not sure why the id was saved before)
            var appName = "";
            if (appId.HasValue)
            {
                var zoneAppId = ((BaseCache)DataSource.GetCache(0, 0)).GetZoneAppId(null, appId);
                appName = ((BaseCache)DataSource.GetCache(0, 0)).ZoneApps[zoneAppId.Item1].Apps[appId.Value];
            }
            UpdateValue(EntityContentBlock.CbPropertyApp, appName);
        }

        internal override void EnsureLinkToContentGroup(Guid cgGuid)
            => UpdateValue(EntityContentBlock.CbPropertyContentGroup, cgGuid);


        internal override void UpdateTitle(IEntity titleItem)
        {
            if (titleItem?.GetBestTitle() == null) return;

            UpdateValue(EntityContentBlock.CbPropertyTitle, titleItem.GetBestTitle());
        }

        #endregion

        #region private helpers

        private void UpdateValue(string key, object value)
        {
            var vals = new Dictionary<string, object> { { key, value } };
            Update(vals);

        }

        private void Update(Dictionary<string, object> newValues)
        {
            // 2017-04-01 2dm centralizing eav access
            var app = ((ContentBlockBase)SxcContext.ContentBlock).Parent.App;
            new AppManager(app.ZoneId, app.AppId).Entities.Update(Math.Abs(SxcContext.ContentBlock.ContentBlockId), newValues);
            //var cgApp = ((ContentBlockBase)SxcContext.ContentBlock).Parent.App;
            //var eavDc = EavDataController.Instance(cgApp.ZoneId, cgApp.AppId);
            //eavDc.Entities.UpdateEntity(Math.Abs(SxcContext.ContentBlock.ContentBlockId), newValues);

            //((ContentBlockBase)SxcContext.ContentBlock).Parent.App
            //    .Data.Update(Math.Abs(SxcContext.ContentBlock.ContentBlockId), newValues); 
        }

        #endregion

    }

}
