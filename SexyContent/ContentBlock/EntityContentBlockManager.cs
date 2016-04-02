using System.Collections.Generic;
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
            SxcContext.AppContentGroups.SetPreviewTemplateId(ModuleID, templateId);
        }

        internal override void SetTemplateChooserState(bool state)
        {
            DnnStuffToRefactor.UpdateModuleSettingForAllLanguages(ModuleID, Settings.SettingsShowTemplateChooser, state.ToString());
        }

        internal override void SetAppId(int? appId)
        {
            AppHelpers.SetAppIdForModule(SxcContext.ModuleInfo, appId);
        }
        #endregion

    }

}