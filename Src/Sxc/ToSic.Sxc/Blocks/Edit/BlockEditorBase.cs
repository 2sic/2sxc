using System;
using ToSic.Eav.Apps;
using ToSic.Eav.Data;
using ToSic.Eav.Logging;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Apps.Blocks;

namespace ToSic.Sxc.Blocks.Edit
{
    // todo: create interface
    // todo: move some parts out into a BlockManagement
    public abstract partial class BlockEditorBase : HasLog
    {
        #region DI and Construction
        internal BlockEditorBase(BlockEditorBaseDependencies dependencies) : base("CG.RefMan")
        {
            Dependencies = dependencies;
            Dependencies.CmsRuntime.SetInit(r => r.Init(Block?.App, true, Log));
            Dependencies.CmsManager.SetInit(r => r.Init(Block?.App, Log));
            Dependencies.AppManager.SetInit(r => r.Init(Block?.App, Log));
        }
        public BlockEditorBaseDependencies Dependencies { get; }

        internal BlockEditorBase Init(IBlock block)
        {
            Log.LinkTo(block.Log);
            Block = block;
            return this;
        }

        #endregion

        private CmsManager CmsManager => Dependencies.CmsManager.Ready;
        private AppManager AppManager => Dependencies.AppManager.Ready;

        protected IBlock Block;

        private BlockConfiguration _cGroup;
        
        #region methods which are fairly stable / the same across content-block implementations

        protected BlockConfiguration BlockConfiguration => _cGroup ?? (_cGroup = Block.Configuration);
        
        public Guid? SaveTemplateId(int templateId, bool forceCreateContentGroup)
        {
            Guid? result;
            Log.A($"save template#{templateId}, CG-exists:{BlockConfiguration.Exists} forceCreateCG:{forceCreateContentGroup}");

            // if it exists or has a force-create, then write to the Content-Group, otherwise it's just a preview
            if (BlockConfiguration.Exists || forceCreateContentGroup)
            {
                var existedBeforeSettingTemplate = BlockConfiguration.Exists;
                var contentGroupGuid = CmsManager.Blocks.UpdateOrCreateContentGroup(BlockConfiguration, templateId);

                if (!existedBeforeSettingTemplate) EnsureLinkToContentGroup(contentGroupGuid);

                result = contentGroupGuid;
            }
            else
            {
                // only set preview / content-group-reference - but must use the guid
                var dataSource = Block.App.Data;
                var templateGuid = dataSource.List.One(templateId).EntityGuid;
                SavePreviewTemplateId(templateGuid);
                result = null; // send null back
            }

            return result;
        }

        public bool Publish(string part, int index)
        {
            Log.A($"publish part{part}, order:{index}");
            var contentGroup = BlockConfiguration;
            var contEntity = contentGroup[part][index];
            var presKey = part.ToLowerInvariant() == ViewParts.ContentLower 
                ? ViewParts.PresentationLower 
                : ViewParts.ListPresentationLower;
            var presEntity = contentGroup[presKey][index];

            var hasPresentation = presEntity != null;

            // make sure we really have the draft item an not the live one
            var contDraft = contEntity.IsPublished ? contEntity.GetDraft() : contEntity;
            AppManager.Entities.Publish(contDraft.RepositoryId);

            if (hasPresentation)
            {
                var presDraft = presEntity.IsPublished ? presEntity.GetDraft() : presEntity;
                AppManager.Entities.Publish(presDraft.RepositoryId);
            }

            return true;
        }

        private AppManager BlockAppManager => Dependencies.AppManager.Ready;

        #endregion

    }
}