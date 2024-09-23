using ToSic.Eav.Apps.Internal.Work;
using ToSic.Lib.DI;
using ToSic.Lib.Services;
using ToSic.Sxc.Apps.Internal;
using ToSic.Sxc.Apps.Internal.Work;

namespace ToSic.Sxc.Blocks.Internal;

// todo: create interface
// todo: move some parts out into a BlockManagement
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public abstract partial class BlockEditorBase : ServiceBase<BlockEditorBase.MyServices>
{
    #region DI and Construction

    public class MyServices : MyServicesBase
    {
        public GenWorkDb<WorkBlocksMod> WorkBlocksMod { get; }
        public GenWorkDb<WorkEntityPublish> Publisher { get; }
        public GenWorkPlus<WorkBlocks> AppBlocks { get; }
        public Generator<BlockEditorForModule> BlkEdtForMod { get; }
        public Generator<BlockEditorForEntity> BlkEdtForEnt { get; }

        public MyServices(
            GenWorkPlus<WorkBlocks> appBlocks,
            GenWorkDb<WorkBlocksMod> workBlocksMod,
            GenWorkDb<WorkEntityPublish> publisher,
            Generator<BlockEditorForModule> blkEdtForMod,
            Generator<BlockEditorForEntity> blkEdtForEnt)
        {
            ConnectLogs([
                WorkBlocksMod = workBlocksMod,
                BlkEdtForMod = blkEdtForMod,
                BlkEdtForEnt = blkEdtForEnt,
                AppBlocks = appBlocks,
                Publisher = publisher
            ]);
        }
    }

    internal BlockEditorBase(MyServices services) : base(services, "CG.RefMan")
    {
    }

    internal void Init(IBlock block) => Block = block;

    #endregion

    protected IBlock Block;

    private BlockConfiguration _cGroup;
        
    #region methods which are fairly stable / the same across content-block implementations

    protected BlockConfiguration BlockConfiguration => _cGroup ??= Block.Configuration;
        
    public Guid? SaveTemplateId(int templateId, bool forceCreateContentGroup)
    {
        var l = Log.Fn<Guid?>($"save template#{templateId}, CG-exists:{BlockConfiguration.Exists} forceCreateCG:{forceCreateContentGroup}");

        // if it exists or has a force-create, then write to the Content-Group, otherwise it's just a preview
        if (BlockConfiguration.Exists || forceCreateContentGroup)
        {
            var existedBeforeSettingTemplate = BlockConfiguration.Exists;
            var contentGroupGuid = Services.WorkBlocksMod.New(Block.Context.AppReader).UpdateOrCreateContentGroup(BlockConfiguration, templateId);

            if (!existedBeforeSettingTemplate) EnsureLinkToContentGroup(contentGroupGuid);

            return l.ReturnAndLog(contentGroupGuid);
        }

        // only set preview / content-group-reference - but must use the guid
        var templateGuid = Block.App.Data.List.One(templateId).EntityGuid;
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
        var appState = Block.Context.AppReader;
        var publisher = Services.Publisher.New(appState: appState);
        var contDraft = contEntity.IsPublished ? appState.GetDraft(contEntity) : contEntity;
        publisher.Publish(contDraft.RepositoryId);

        if (hasPresentation)
        {
            var presDraft = presEntity.IsPublished ? appState.GetDraft(presEntity) : presEntity;
            publisher.Publish(presDraft.RepositoryId);
        }

        return l.ReturnTrue();
    }

    #endregion

}