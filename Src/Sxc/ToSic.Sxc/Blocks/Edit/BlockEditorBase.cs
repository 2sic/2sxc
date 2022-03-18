using System;
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
            Dependencies.CmsManager.SetInit(m => m.Init(Block?.App, Log));
        }
        public BlockEditorBaseDependencies Dependencies { get; }
        private CmsManager CmsManager => Dependencies.CmsManager.Ready;

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

        public bool Publish(string part, int sortOrder)
        {
            Log.Add($"publish part{part}, order:{sortOrder}");
            var contentGroup = BlockConfiguration;
            var contEntity = contentGroup[part][sortOrder];
            var presKey = part.ToLowerInvariant() == ViewParts.ContentLower 
                ? ViewParts.PresentationLower 
                : ViewParts.ListPresentationLower;
            var presEntity = contentGroup[presKey][sortOrder];

            var hasPresentation = presEntity != null;

            // make sure we really have the draft item an not the live one
            var contDraft = contEntity.IsPublished ? contEntity.GetDraft() : contEntity;
            CmsManager.Entities.Publish(contDraft.RepositoryId);

            if (hasPresentation)
            {
                var presDraft = presEntity.IsPublished ? presEntity.GetDraft() : presEntity;
                CmsManager.Entities.Publish(presDraft.RepositoryId);
            }

            return true;
        }


        #endregion

    }
}