using System;
using System.Collections.Generic;
using ToSic.Eav.BLL;

namespace ToSic.SexyContent.ContentBlock
{
    internal class EntityContentGroupReferenceManager: ContentGroupReferenceManagerBase
    {
        public EntityContentGroupReferenceManager(SxcInstance sxc) : base(sxc)
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
            => UpdateValue(EntityContentBlock.CbPropertyApp, appId ?? 0);
        

        internal override void EnsureLinkToContentGroup(Guid cgGuid)
            => UpdateValue(EntityContentBlock.CbPropertyContentGroup, cgGuid);
        

        internal override void UpdateTitle(string newTitle)
        {
            throw new Exception("not working");
        }

        #endregion

            #region private helpers

        private void UpdateValue(string key, object value)
        {
            var vals = new Dictionary<string, object> {{key, value}};
            Update(vals);

        }

        private void Update(Dictionary<string, object> newValues)
        {
            var cgApp = ((ContentBlockBase) SxcContext.ContentBlock).Parent.App;
            var eavDc = EavDataController.Instance(cgApp.ZoneId, cgApp.AppId);

            eavDc.Entities.UpdateEntity(Math.Abs( SxcContext.ContentBlock.ContentBlockId), newValues);

            //((ContentBlockBase)SxcContext.ContentBlock).Parent.App
            //    .Data.Update(Math.Abs(SxcContext.ContentBlock.ContentBlockId), newValues); 
        }

        #endregion

    }

}