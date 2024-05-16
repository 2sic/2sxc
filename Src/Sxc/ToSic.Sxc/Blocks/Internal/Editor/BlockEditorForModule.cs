using ToSic.Lib.DI;
using ToSic.Sxc.Integration.Modules;

namespace ToSic.Sxc.Blocks.Internal;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class BlockEditorForModule : BlockEditorBase
{
    public BlockEditorForModule(MyServices services,
        LazySvc<IPlatformModuleUpdater> platformModuleUpdater) : base(services)
    {
        ConnectLogs([_platformModuleUpdater = platformModuleUpdater]);
    }

    private readonly LazySvc<IPlatformModuleUpdater> _platformModuleUpdater;

    private IPlatformModuleUpdater PlatformModuleUpdater => _platformModuleUpdater.Value;


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