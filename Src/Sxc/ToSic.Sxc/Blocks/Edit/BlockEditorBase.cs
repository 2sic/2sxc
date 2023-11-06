using System;
using ToSic.Eav.Apps.Work;
using ToSic.Eav.Data;
using ToSic.Lib.DI;
using ToSic.Lib.Logging;
using ToSic.Lib.Services;
using ToSic.Sxc.Apps.Blocks;
using ToSic.Sxc.Apps.CmsSys;
using ToSic.Sxc.Apps.Work;

namespace ToSic.Sxc.Blocks.Edit
{
    // todo: create interface
    // todo: move some parts out into a BlockManagement
    public abstract partial class BlockEditorBase : ServiceBase<BlockEditorBase.MyServices>
    {
        #region DI and Construction

        public class MyServices : MyServicesBase
        {
            public AppWorkUnitWithDb<WorkBlocksMod> WorkBlocksMod { get; }
            public LazySvc<AppWorkUnit<WorkEntityPublish, IAppWorkCtxWithDb>> Publisher { get; }
            public LazySvc<AppWork> AppSys { get; }
            public LazySvc<AppBlocks> AppBlocks { get; }
            //public LazySvc<CmsManager> CmsManager { get; }
            //public LazySvc<AppManager> AppManager { get; }
            public Generator<BlockEditorForModule> BlkEdtForMod { get; }
            public Generator<BlockEditorForEntity> BlkEdtForEnt { get; }

            public MyServices(
                LazySvc<AppWork> appSys,
                LazySvc<AppBlocks> appBlocks,
                AppWorkUnitWithDb<WorkBlocksMod> workBlocksMod,
                //LazySvc<CmsManager> cmsManager,
                //LazySvc<AppManager> appManager,
                LazySvc<AppWorkUnit<WorkEntityPublish, IAppWorkCtxWithDb>> publisher,
                Generator<BlockEditorForModule> blkEdtForMod,
                Generator<BlockEditorForEntity> blkEdtForEnt)
            {
                ConnectServices(
                    //CmsManager = cmsManager,
                    WorkBlocksMod = workBlocksMod,
                    //AppManager = appManager,
                    BlkEdtForMod = blkEdtForMod,
                    BlkEdtForEnt = blkEdtForEnt,
                    AppSys = appSys,
                    AppBlocks = appBlocks,
                    Publisher = publisher
                );
            }
        }

        internal BlockEditorBase(MyServices services) : base(services, "CG.RefMan")
        {
            //Services.CmsManager.SetInit(r => r.Init(Block?.App));
            //Services.AppManager.SetInit(r => r.Init(Block?.App));
        }

        internal void Init(IBlock block) => Block = block;

        #endregion

        //private CmsManager CmsManager => Services.CmsManager.Value;
        //private AppManager AppManager => Services.AppManager.Value;

        protected IBlock Block;

        private BlockConfiguration _cGroup;
        
        #region methods which are fairly stable / the same across content-block implementations

        protected BlockConfiguration BlockConfiguration => _cGroup ?? (_cGroup = Block.Configuration);
        
        public Guid? SaveTemplateId(int templateId, bool forceCreateContentGroup)
        {
            var l = Log.Fn<Guid?>($"save template#{templateId}, CG-exists:{BlockConfiguration.Exists} forceCreateCG:{forceCreateContentGroup}");

            // if it exists or has a force-create, then write to the Content-Group, otherwise it's just a preview
            if (BlockConfiguration.Exists || forceCreateContentGroup)
            {
                var existedBeforeSettingTemplate = BlockConfiguration.Exists;
                var contentGroupGuid = Services.WorkBlocksMod.New(Block.Context.AppState)/* CmsManager.Blocks*/.UpdateOrCreateContentGroup(BlockConfiguration, templateId);

                if (!existedBeforeSettingTemplate) EnsureLinkToContentGroup(contentGroupGuid);

                return l.ReturnAndLog(contentGroupGuid);
            }

            // only set preview / content-group-reference - but must use the guid
            var dataSource = Block.App.Data;
            var templateGuid = dataSource.List.One(templateId).EntityGuid;
            SavePreviewTemplateId(templateGuid);
            return l.Return(null, "only set preview, return null");
        }

        public bool Publish(string part, int index)
        {
            var l = Log.Fn<bool>($"publish part{part}, order:{index}");
            var contentGroup = BlockConfiguration;
            var contEntity = contentGroup[part][index];
            var presKey = part.ToLowerInvariant() == ViewParts.ContentLower 
                ? ViewParts.PresentationLower 
                : ViewParts.ListPresentationLower;
            var presEntity = contentGroup[presKey][index];

            var hasPresentation = presEntity != null;

            // make sure we really have the draft item an not the live one
            var appState = Block.Context.AppState;
            var publisher = Services.Publisher.Value.New(state: appState);
            var contDraft = contEntity.IsPublished ? appState.GetDraft(contEntity) : contEntity;
            publisher.Publish(contDraft.RepositoryId);
            // AppManager.Entities.Publish(contDraft.RepositoryId);
            
            if (hasPresentation)
            {
                var presDraft = presEntity.IsPublished ? appState.GetDraft(presEntity) : presEntity;
                publisher.Publish(presDraft.RepositoryId);
                //AppManager.Entities.Publish(presDraft.RepositoryId);
            }

            return l.ReturnTrue();
        }

        #endregion

    }
}