using ToSic.Sxc.Integration.Modules;

namespace ToSic.Sxc.Blocks.Sys.BlockEditor;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class BlockEditorForModule(
    BlockEditorBase.MyServices services,
    LazySvc<IPlatformModuleUpdater> platformModuleUpdater)
    : BlockEditorBase(services, connect: [platformModuleUpdater])
{
    private IPlatformModuleUpdater PlatformModuleUpdater => platformModuleUpdater.Value;


    protected override void SavePreviewTemplateId(Guid templateGuid)
        => PlatformModuleUpdater.SetPreview(Block.Context.Module.Id, templateGuid);


    internal override void SetAppId(int? appId)
        => PlatformModuleUpdater.SetAppId(Block.Context.Module, appId);

    internal override void EnsureLinkToContentGroup(Guid cgGuid)
        => PlatformModuleUpdater.SetContentGroup(Block.Context.Module.Id, true, cgGuid);

    internal override void UpdateTitle(IEntity titleItem)
    {
        Log.A("update title");
        PlatformModuleUpdater.UpdateTitle(Block, titleItem);
    }

}