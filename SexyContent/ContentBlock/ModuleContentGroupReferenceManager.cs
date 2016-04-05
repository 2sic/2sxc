using System;
using ToSic.SexyContent.Internal;

namespace ToSic.SexyContent.ContentBlock
{
    internal class ModuleContentGroupReferenceManager: ContentGroupReferenceManagerBase
    {
        internal ModuleContentGroupReferenceManager(SxcInstance sxc)
        {
            SxcContext = sxc;
            ModuleId = SxcContext.ModuleInfo.ModuleID;
        }
        #region methods which the entity-implementation must customize - so it's virtual

        protected override void SavePreviewTemplateId(Guid templateGuid, bool? newTemplateChooserState = null)
        {
            SxcContext.AppContentGroups.SetModulePreviewTemplateId(ModuleId, templateGuid);
            if(newTemplateChooserState.HasValue)
                SetTemplateChooserState(newTemplateChooserState.Value);
        }

        internal override void SetTemplateChooserState(bool state)
        {
            DnnStuffToRefactor.UpdateModuleSettingForAllLanguages(ModuleId, Settings.SettingsShowTemplateChooser, state.ToString());
        }

        internal override void SetAppId(int? appId)
        {
            AppHelpers.SetAppIdForModule(SxcContext.ModuleInfo, appId);
        }

        internal override void EnsureLinkToContentGroup(Guid cgGuid)
        {
            SxcContext.ContentBlock.App.ContentGroupManager.PersistContentGroupAndBlankTemplateToModule(ModuleId,
                true, cgGuid);
        }


        #endregion

    }

}