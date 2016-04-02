using System;
using System.Collections.Generic;
using System.ComponentModel;
using ToSic.Eav.Api.Api01;
using ToSic.SexyContent.Internal;

namespace ToSic.SexyContent.ContentBlock
{
    internal class EntityContentBlockManager: ContentBlockManagerBase
    {
        internal EntityContentBlockManager(SxcInstance sxc)
        {
            SxcContext = sxc;
            ModuleID = SxcContext.ModuleInfo.ModuleID;
        }
        #region methods which the entity-implementation must customize - so it's virtual

        protected override void SavePreviewTemplateId(int templateId)
        {
            UpdateValue(EntityContentBlock.CbPropertyTemplate, templateId);
        }

        internal override void SetTemplateChooserState(bool state)
        {
            UpdateValue(EntityContentBlock.CbPropertyShowChooser, state);
            // todo: update the value in the contentblock entity
            //var vals = new Dictionary<string, object>();
            //vals.Add(EntityContentBlock.CbPropertyShowChooser, state);
            //Update(vals);
            //((ContentBlockBase)SxcContext.ContentBlock).Parent.App.Data.Update(Math.Abs(SxcContext.ContentBlock.ContentBlockId), vals);
        }

        internal override void SetAppId(int? appId)
        {
            UpdateValue(EntityContentBlock.CbPropertyApp, appId ?? 0);
            // AppHelpers.SetAppIdForModule(SxcContext.ModuleInfo, appId);
        }
        #endregion

        #region private helpers

        private void UpdateValue(string key, object value)
        {
            var vals = new Dictionary<string, object>();
            vals.Add(key, value);
            Update(vals);

        }

        private void Update(Dictionary<string, object> newValues)
        {
            ((ContentBlockBase)SxcContext.ContentBlock).Parent.App.Data.Update(Math.Abs(SxcContext.ContentBlock.ContentBlockId), newValues); 
        }

        #endregion

    }

}