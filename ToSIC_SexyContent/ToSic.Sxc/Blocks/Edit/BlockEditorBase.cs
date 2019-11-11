using System;
using System.Web.Http;
using ToSic.Eav.Apps;
using ToSic.Eav.Data;
using ToSic.Eav.Data.Query;
using ToSic.Eav.Logging;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Apps.Blocks;

namespace ToSic.Sxc.Blocks
{

    // todo: create interface
    // todo: move some parts out into a BlockManagement
    public abstract partial class BlockEditorBase : HasLog
    {
        protected ICmsBlock CmsContext;
        protected int ModuleId;

        private BlockConfiguration _cGroup;

        internal BlockEditorBase(ICmsBlock cms): base("CG.RefMan", cms.Log)
        {
            CmsContext = cms;
            ModuleId = CmsContext.EnvInstance.Id;
        }


        #region methods which the entity-implementation must customize - so it's virtual

        protected abstract void SavePreviewTemplateId(Guid templateGuid);


        internal abstract void SetAppId(int? appId);

        internal abstract void EnsureLinkToContentGroup(Guid cgGuid);
        internal abstract void UpdateTitle(IEntity titleItem);
        #endregion

        #region methods which are fairly stable / the same across content-block implementations

        protected BlockConfiguration BlockConfiguration
            => _cGroup ?? (_cGroup = CmsContext.Block.Configuration);


        public Guid? SaveTemplateId(int templateId, bool forceCreateContentGroup)
        {
            Guid? result;
            Log.Add($"save template#{templateId}, CG-exists:{BlockConfiguration.Exists} forceCreateCG:{forceCreateContentGroup}");

            // if it exists or has a force-create, then write to the Content-Group, otherwise it's just a preview
            if (BlockConfiguration.Exists || forceCreateContentGroup)
            {
                var existedBeforeSettingTemplate = BlockConfiguration.Exists;

                var app = CmsContext.Block.App;
                var cms = new CmsManager(app, Log);

                var contentGroupGuid = cms.Blocks // CmsContext.Block.App.BlocksManager
                    .UpdateOrCreateContentGroup(BlockConfiguration, templateId);

                if (!existedBeforeSettingTemplate)
                    EnsureLinkToContentGroup(contentGroupGuid);

                result = contentGroupGuid;
            }
            else
            {
                // only set preview / content-group-reference - but must use the guid
                var dataSource = CmsContext.App.Data;
                var templateGuid = dataSource.List.One(templateId).EntityGuid;
                SavePreviewTemplateId(templateGuid);
                result = null; // send null back
            }

            return result;
        }

        public bool Publish(string part, int sortOrder)
        {
            Log.Add($"publish part{part}, order:{sortOrder}");
            var contentGroup = BlockConfiguration;
            var contEntity = contentGroup[part][sortOrder];
            var presKey = part.ToLower() == ViewParts.ContentLower ? ViewParts.PresentationLower : "listpresentation";
            var presEntity = contentGroup[presKey][sortOrder];

            var hasPresentation = presEntity != null;

            var appMan = new AppManager(CmsContext.App.ZoneId, CmsContext.App.AppId);

            // make sure we really have the draft item an not the live one
            var contDraft = contEntity.IsPublished ? contEntity.GetDraft() : contEntity;
            appMan.Entities.Publish(contDraft.RepositoryId);

            if (hasPresentation)
            {
                var presDraft = presEntity.IsPublished ? presEntity.GetDraft() : presEntity;
                appMan.Entities.Publish(presDraft.RepositoryId);
            }

            return true;
        }


        #endregion



    }
}