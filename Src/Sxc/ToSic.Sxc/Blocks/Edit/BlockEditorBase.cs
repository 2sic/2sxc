using System;
using ToSic.Eav.Apps;
using ToSic.Eav.Data;
using ToSic.Eav.Logging;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Apps.Blocks;
using IApp = ToSic.Eav.Apps.IApp;

namespace ToSic.Sxc.Blocks.Edit
{
    // todo: create interface
    // todo: move some parts out into a BlockManagement
    public abstract partial class BlockEditorBase : HasLog
    {
        #region DI and Construction

        private readonly Lazy<CmsRuntime> _lazyCmsRuntime;
        private readonly Lazy<CmsManager> _cmsManagerLazy;
        protected CmsManager CmsManager => _cmsManager ?? (_cmsManager = _cmsManagerLazy.Value.Init(Block?.App, Log));
        private CmsManager _cmsManager;

        internal BlockEditorBase(Lazy<CmsRuntime> lazyCmsRuntime, Lazy<CmsManager> cmsManagerLazy) : base("CG.RefMan")
        {
            _lazyCmsRuntime = lazyCmsRuntime;
            _cmsManagerLazy = cmsManagerLazy;
        }

        internal BlockEditorBase Init(IBlock block)
        {
            Log.LinkTo(block.Log);
            Block = block;
            return this;
        }

        #endregion

        protected IBlock Block;

        private BlockConfiguration _cGroup;


        
        #region methods which are fairly stable / the same across content-block implementations

        protected BlockConfiguration BlockConfiguration => _cGroup ?? (_cGroup = Block.Configuration);
        
        public Guid? SaveTemplateId(int templateId, bool forceCreateContentGroup)
        {
            Guid? result;
            Log.Add($"save template#{templateId}, CG-exists:{BlockConfiguration.Exists} forceCreateCG:{forceCreateContentGroup}");

            // if it exists or has a force-create, then write to the Content-Group, otherwise it's just a preview
            if (BlockConfiguration.Exists || forceCreateContentGroup)
            {
                var existedBeforeSettingTemplate = BlockConfiguration.Exists;

                //var app = Block.App;
                //var cms = new CmsManager().Init(app, Log);

                var contentGroupGuid = CmsManager.Blocks.UpdateOrCreateContentGroup(BlockConfiguration, templateId);

                if (!existedBeforeSettingTemplate) EnsureLinkToContentGroup(contentGroupGuid);

                result = contentGroupGuid;
            }
            else
            {
                // only set preview / content-group-reference - but must use the guid
                var dataSource = Block.App.Data;
                var templateGuid = dataSource.Immutable.One(templateId).EntityGuid;
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
            var presKey = part.ToLower() == ViewParts.ContentLower 
                ? ViewParts.PresentationLower 
                : ViewParts.ListPresentationLower;
            var presEntity = contentGroup[presKey][sortOrder];

            var hasPresentation = presEntity != null;

            var appMan = GetAppManagerOrReuse(Block.App);// new AppManager(Block.App, Log);

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

        protected AppManager GetAppManagerOrReuse(IApp appIdentity) =>
            _appManager ?? (_appManager = _cmsManagerLazy.Value.Init(appIdentity, Log));
        private AppManager _appManager;


        #endregion

    }
}