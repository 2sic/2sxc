using System;
using System.Collections.Generic;
using System.ComponentModel;
using ToSic.Eav.Api.Api01;
using ToSic.SexyContent.Internal;

namespace ToSic.SexyContent.ContentBlock
{
    internal class EntityContentGroupReferenceManager: ContentGroupReferenceManagerBase
    {
        internal EntityContentGroupReferenceManager(SxcInstance sxc)
        {
            SxcContext = sxc;
            ModuleID = SxcContext.ModuleInfo.ModuleID;
        }
        #region methods which the entity-implementation must customize - so it's virtual

        protected override void SavePreviewTemplateId(Guid templateGuid, bool? newTemplateChooserState = null)
        {
            var vals = new Dictionary<string, object>();
            vals.Add(EntityContentBlock.CbPropertyTemplate, templateGuid.ToString());
            if (newTemplateChooserState.HasValue)
                vals.Add(EntityContentBlock.CbPropertyShowChooser, newTemplateChooserState.Value); // must blank the template
            Update(vals);
        }

        internal override void SetTemplateChooserState(bool state)
        {
            UpdateValue(EntityContentBlock.CbPropertyShowChooser, state);
        }

        internal override void SetAppId(int? appId)
        {
            UpdateValue(EntityContentBlock.CbPropertyApp, appId ?? 0);
        }

        internal override void EnsureLinkToContentGroup(Guid cgGuid)
        {
            // link to the CG
            UpdateValue(EntityContentBlock.CbPropertyContentGroup, cgGuid);
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