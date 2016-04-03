using System;
using System.Collections.Generic;
using ToSic.SexyContent.Internal;

namespace ToSic.SexyContent.ContentBlock
{
    internal class ModuleContentBlockManager: ContentBlockManagerBase
    {
        internal ModuleContentBlockManager(SxcInstance sxc)
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

        internal override Guid? SaveTemplateIdInContentGroup(bool isNew, Guid cgGuid)
        {
            //bool willCreate = !ContentGroup.Exists;
            //var cgm = SxcContext.ContentBlock.App.ContentGroupManager;
            //var cgGuid = cgm.SaveTemplateToContentGroup(ModuleID,
            //    ContentGroup,
            //    templateId);

            SxcContext.ContentBlock.App.ContentGroupManager.PersistContentGroupAndBlankTemplateToModule(ModuleID,
                isNew, cgGuid);

            return cgGuid;
        }


        #endregion

    }

}