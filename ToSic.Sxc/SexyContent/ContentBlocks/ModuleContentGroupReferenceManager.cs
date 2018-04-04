using System;
using ToSic.Eav;
using ToSic.SexyContent.Interfaces;

namespace ToSic.SexyContent.ContentBlocks
{
    internal class ModuleContentGroupReferenceManager: ContentGroupReferenceManagerBase
    {
        public ModuleContentGroupReferenceManager(SxcInstance sxc) : base(sxc)
        {
        }

        #region methods which the entity-implementation must customize - so it's virtual

        protected override void SavePreviewTemplateId(Guid templateGuid) 
            => ContentGroupManager.SetPreviewTemplate(ModuleId, templateGuid);

        internal override void SetAppId(int? appId)
            => Factory.Resolve<IMapAppToInstance>().SetAppIdForInstance(SxcContext.EnvInstance, SxcContext.Environment, appId, Log);
        

        internal override void EnsureLinkToContentGroup(Guid cgGuid)
            => Factory.Resolve<IMapAppToInstance>().SetContentGroupAndBlankTemplate(ModuleId, true, cgGuid);

        internal override void UpdateTitle(Eav.Interfaces.IEntity titleItem)
        {
            Log.Add("update title");
            Factory.Resolve<IMapAppToInstance>().UpdateTitle(SxcContext, titleItem);
        }

        

        #endregion

    }

}