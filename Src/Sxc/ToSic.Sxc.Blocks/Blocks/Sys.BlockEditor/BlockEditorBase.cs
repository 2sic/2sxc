using ToSic.Sxc.Blocks.Sys.Views;
using ToSic.Sxc.Blocks.Sys.Work;

namespace ToSic.Sxc.Blocks.Sys.BlockEditor;

// todo: create interface
// todo: move some parts out into a BlockManagement
[ShowApiWhenReleased(ShowApiMode.Never)]
public abstract partial class BlockEditorBase : ServiceBase<BlockEditorBase.Dependencies>
{
    #region DI and Construction

    public record Dependencies(
        LazySvc<AppWorkContextService> AppWorkCtxSvc,
        GenWorkPlus<WorkBlocks> AppBlocks,
        GenWorkDb<WorkBlocksMod> WorkBlocksMod,
        LazySvc<WorkEntityPublish> Publisher
    ) : DependenciesBase(connect: [AppWorkCtxSvc, WorkBlocksMod, AppBlocks, Publisher]);

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

            if (!existedBeforeSettingTemplate)
                EnsureLinkToContentGroup(contentGroupGuid);

            return l.ReturnAndLog(contentGroupGuid);
        }

        // only set preview / content-group-reference - but must use the guid
        var templateGuid = Block.App.Data.List.GetOne(templateId)!.EntityGuid;
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

        var appReader = Block.Context.AppReaderRequired;
        var publishIds = new[] { contEntity, presEntity }
            // make sure we really have the draft item and not the live one
            .Select(e => e?.IsPublished == true ? appReader.GetDraft(e) : e)
            .OfType<IEntity>()
            .Select(e => e.RepositoryId)
            .ToArray();

        // This must happen within the using context, otherwise the appReader will not be able to find the draft entity
        using (Services.AppWorkCtxSvc.Value.WithContext(Block.Context.AppReaderRequired))
            Services.Publisher.Value.Publish(publishIds);

        return l.ReturnTrue();
    }

    #endregion

}