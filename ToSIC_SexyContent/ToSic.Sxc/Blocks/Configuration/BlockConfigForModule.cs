using System;
using ToSic.Eav;
using ToSic.Eav.Data;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Interfaces;

namespace ToSic.Sxc.Blocks
{
    internal class BlockConfigForModule: BlockConfigBase
    {
        public BlockConfigForModule(ICmsBlock cms) : base(cms)
        {
        }

        #region methods which the entity-implementation must customize - so it's virtual

        protected override void SavePreviewTemplateId(Guid templateGuid)
            => Factory.Resolve<IMapAppToInstance>().SetPreviewTemplate(ModuleId, templateGuid);
            //=> BlocksManager.SetPreviewTemplate(ModuleId, templateGuid);

        internal override void SetAppId(int? appId)
            => Factory.Resolve<IMapAppToInstance>().SetAppIdForInstance(CmsContext.EnvInstance, CmsContext.Environment, appId, Log);
        

        internal override void EnsureLinkToContentGroup(Guid cgGuid)
            => Factory.Resolve<IMapAppToInstance>().SetContentGroup(ModuleId, true, cgGuid);

        internal override void UpdateTitle(IEntity titleItem)
        {
            Log.Add("update title");
            Factory.Resolve<IMapAppToInstance>().UpdateTitle(CmsContext, titleItem);
        }

        

        #endregion

    }

}