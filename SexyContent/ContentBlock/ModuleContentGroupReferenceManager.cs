using System;
using System.Linq;
using ToSic.Eav;
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

        internal override void UpdateTitle(IEntity titleItem)
        {
            // todo: this should probably do the more complex stuff
            // to ensure that it happens on all versions of this module (all languages)
            // used to work once...
            if (titleItem?.GetBestValue("EntityTitle") != null)
                SxcContext.ModuleInfo.ModuleTitle = titleItem.GetBestValue("EntityTitle").ToString();
        }

        

        #endregion

    }

}