using System;
using ToSic.SexyContent.Internal;

namespace ToSic.SexyContent.ContentBlocks
{
    internal class ModuleContentGroupReferenceManager: ContentGroupReferenceManagerBase
    {
        public ModuleContentGroupReferenceManager(SxcInstance sxc) : base(sxc)
        {
        }

        #region methods which the entity-implementation must customize - so it's virtual

        protected override void SavePreviewTemplateId(Guid templateGuid) 
            => SxcContext.App.ContentGroupManager.SetModulePreviewTemplateId(ModuleId, templateGuid);

        internal override void SetAppId(int? appId)
            => AppHelpers.SetAppIdForModule(SxcContext.InstanceInfo, SxcContext.Environment, appId, Log);
        

        internal override void EnsureLinkToContentGroup(Guid cgGuid)
            => SxcContext.ContentBlock.App.ContentGroupManager.PersistContentGroupAndBlankTemplateToModule(ModuleId,
                true, cgGuid);

        internal override void UpdateTitle(Eav.Interfaces.IEntity titleItem)
        {
            Log.Add("update title");

            var languages = SxcContext.Environment.ZoneMapper.CulturesWithState(SxcContext.InstanceInfo.TennantId,
                SxcContext.ZoneId.Value);

            // Find Module for default language
            var moduleController = new DotNetNuke.Entities.Modules.ModuleController();
            var originalModule = moduleController.GetModule(SxcContext.InstanceInfo.Id);

            foreach (var dimension in languages)
            {
                if (!originalModule.IsDefaultLanguage)
                    originalModule = originalModule.DefaultLanguageModule;

                try // this can sometimes fail, like if the first item is null - https://github.com/2sic/2sxc/issues/817
                {
                    // Break if default language module is null
                    if (originalModule == null)
                        return;

                    // Get Title value of Entitiy in current language
                    var titleValue = titleItem.Title[dimension.Key].ToString();

                    // Find module for given Culture
                    var moduleByCulture = moduleController.GetModuleByCulture(originalModule.ModuleID,
                        originalModule.TabID, SxcContext.InstanceInfo.TennantId,
                        DotNetNuke.Services.Localization.LocaleController.Instance.GetLocale(dimension.Key));

                    // Break if no title module found
                    if (moduleByCulture == null || titleValue == null)
                        return;

                    moduleByCulture.ModuleTitle = titleValue;
                    moduleController.UpdateModule(moduleByCulture);
                }
                catch
                {
                    // ignored
                }
            }
        }

        

        #endregion

    }

}