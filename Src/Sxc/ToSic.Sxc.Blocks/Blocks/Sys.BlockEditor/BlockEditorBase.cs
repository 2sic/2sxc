using ToSic.Eav.Data.Sys.Entities;
using ToSic.Sxc.Blocks.Sys.Views;
using ToSic.Sxc.Blocks.Sys.Work;

namespace ToSic.Sxc.Blocks.Sys.BlockEditor;

// todo: create interface
// todo: move some parts out into a BlockManagement
[ShowApiWhenReleased(ShowApiMode.Never)]
public abstract partial class BlockEditorBase : ServiceBase<BlockEditorBase.Dependencies>
{
    #region DI and Construction

    public class Dependencies(
        GenWorkPlus<WorkBlocks> appBlocks,
        GenWorkDb<WorkBlocksMod> workBlocksMod,
        GenWorkDb<WorkEntityPublish> publisher)
        : DependenciesBase(connect: [workBlocksMod, appBlocks, publisher])
    {
        public GenWorkDb<WorkBlocksMod> WorkBlocksMod { get; } = workBlocksMod;
        public GenWorkDb<WorkEntityPublish> Publisher { get; } = publisher;
        public GenWorkPlus<WorkBlocks> AppBlocks { get; } = appBlocks;
    }

    internal BlockEditorBase(Dependencies services, object[] connect) : base(services, "CG.RefMan", connect: connect)
    { }

    internal void Init(IBlock block) => Block = block;

    #endregion

    protected IBlock Block = null!;

    #region methods which are fairly stable / the same across content-block implementations

    [field: AllowNull, MaybeNull]
    protected BlockConfiguration BlockConfiguration => field ??= Block.Configuration;
        
    public Guid? SaveTemplateId(int templateId, bool forceCreateContentGroup)
    {
        var l = Log.Fn<Guid?>($"save template#{templateId}, CG-exists:{BlockConfiguration.Exists} forceCreateCG:{forceCreateContentGroup}");

        // if it exists or has a force-create, then write to the Content-Group, otherwise it's just a preview
        if (BlockConfiguration.Exists || forceCreateContentGroup)
        {
            var existedBeforeSettingTemplate = BlockConfiguration.Exists;
            var contentGroupGuid = Services.WorkBlocksMod
                .New(Block.Context.AppReaderRequired)
                .UpdateOrCreateContentGroup(BlockConfiguration, templateId);

            if (!existedBeforeSettingTemplate) EnsureLinkToContentGroup(contentGroupGuid);

            return l.ReturnAndLog(contentGroupGuid);
        }

        // only set preview / content-group-reference - but must use the guid
        var templateGuid = Block.App.Data.List.One(templateId)!.EntityGuid;
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


        // make sure we really have the draft item an not the live one
        var appReader = Block.Context.AppReaderRequired;
        var publisher = Services.Publisher.New(appReader: appReader);
        var contDraft = contEntity.IsPublished
            ? appReader.GetDraft(contEntity)
            : contEntity;

        if (contDraft != null)
            publisher.Publish(contDraft.RepositoryId);

        // if has presentation entity, publish that too
        if (presEntity != null)
        {
            var presDraft = presEntity.IsPublished
                ? appReader.GetDraft(presEntity)
                : presEntity;
            if (presDraft != null)
                publisher.Publish(presDraft.RepositoryId);
        }

        return l.ReturnTrue();
    }

    #endregion

}