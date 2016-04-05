using System;
using System.Linq;
using ToSic.SexyContent.Internal;

namespace ToSic.SexyContent.ContentBlock
{
    internal class ModuleContentGroupReferenceManager: ContentGroupReferenceManagerBase
    {
        public ModuleContentGroupReferenceManager(SxcInstance sxc) : base(sxc)
        {
        }

        #region methods which the entity-implementation must customize - so it's virtual

        protected override void SavePreviewTemplateId(Guid templateGuid, bool? newTemplateChooserState = null)
        {
            SxcContext.AppContentGroups.SetModulePreviewTemplateId(ModuleId, templateGuid);
            if(newTemplateChooserState.HasValue)
                SetTemplateChooserState(newTemplateChooserState.Value);
        }

        internal override void SetTemplateChooserState(bool state)
            => DnnStuffToRefactor.UpdateModuleSettingForAllLanguages(ModuleId, Settings.SettingsShowTemplateChooser, state.ToString());
        

        internal override void SetAppId(int? appId)
            => AppHelpers.SetAppIdForModule(SxcContext.ModuleInfo, appId);
        

        internal override void EnsureLinkToContentGroup(Guid cgGuid)
            => SxcContext.ContentBlock.App.ContentGroupManager.PersistContentGroupAndBlankTemplateToModule(ModuleId,
                true, cgGuid);

        internal override void UpdateTitle(string newTitle)
        {
            // check the contentGroup as to what should be the module title, then try to set it
            // technically it could have multiple different groups to save in, 
            // ...but for now we'll just update the current modules title
            // note: it also correctly handles published/unpublished, but I'm not sure why :)
            var app = SxcContext.App;
            var modContentGroup = app.ContentGroupManager.GetContentGroupForModule(SxcContext.ModuleInfo.ModuleID);

            var titleItem = modContentGroup.ListContent.FirstOrDefault() ?? modContentGroup.Content.FirstOrDefault();

            if (titleItem?.GetBestValue("EntityTitle") != null)
                SxcContext.ModuleInfo.ModuleTitle = titleItem.GetBestValue("EntityTitle").ToString();

        }


        #endregion

    }

}