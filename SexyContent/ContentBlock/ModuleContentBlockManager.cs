using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.UI.WebControls;
using DotNetNuke.Services.Exceptions;
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

        protected override ContentGroup ContentGroup
            => _cg ?? (_cg = SxcContext.AppContentGroups.GetContentGroupForModule(ModuleID));

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

        protected override IEnumerable<Template> GetSelectableTemplatesForWebApi()
        {
            return SxcContext.AppTemplates.GetAvailableTemplatesForSelector(ModuleID, SxcContext.AppContentGroups);
        }

        #endregion

    }

}