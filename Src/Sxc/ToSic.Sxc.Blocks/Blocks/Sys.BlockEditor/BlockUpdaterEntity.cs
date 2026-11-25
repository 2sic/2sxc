using ToSic.Eav.Apps;
using ToSic.Sxc.Blocks.Sys.Views;
using ToSic.Sxc.Context;
using ToSic.Sxc.Integration.Modules;

namespace ToSic.Sxc.Blocks.Sys.BlockEditor;

/// <summary>
/// Data updater for entity-blocks.
/// Matches the API of <see cref="IPlatformModuleUpdater"/> but will store the changes into the target entity.
/// </summary>
/// <param name="entityUpdate"></param>
/// <param name="appsCatalog"></param>
[ShowApiWhenReleased(ShowApiMode.Never)]
public class BlockUpdaterEntity(AppWorkQuick<WorkEntityUpdate> entityUpdate, IAppsCatalog appsCatalog)
    : ServiceWithSetup<IBlock>("Sxc.BlUpEnt", connect: [entityUpdate, appsCatalog]), IPlatformModuleUpdater
{

    public void SetAppId(IModule instance, int? appId)
    {
        var appName = appId.HasValue
            ? appsCatalog.AppNameId(appsCatalog.AppIdentity(appId.Value))
            : "";
        UpdateValue(BlockBuildingConstants.CbPropertyApp, appName);
    }

    public void SetPreview(int instanceId, Guid previewView)
        => UpdateValue(ViewParts.TemplateContentType, previewView.ToString());

    public void SetContentGroup(int instanceId, bool blockExists, Guid guid)
        => UpdateValue(BlockBuildingConstants.CbPropertyContentGroup, guid.ToString()); // must pre-convert to string, as it's not a reference to an entity in the same app

    public void UpdateTitle(IBlock block, IEntity titleItem)
    {
        var title = titleItem?.GetBestTitle();
        if (title == null)
            return;
        UpdateValue(BlockBuildingConstants.CbPropertyTitle, title);
    }

    private void UpdateValue(string key, string value)
        => Update(MyOptions, new() { { key, value } });

    private void Update(IBlock block, Dictionary<string, object> newValues)
    {
        // The parent block must always exist on an entity block
        var parentBlock = block.ParentBlockOrNull
            ?? throw new InvalidOperationException("Parent block must exist on an entity block");
        entityUpdate.New(parentBlock.App.AppReader)
            .UpdateParts(Math.Abs(block.ContentBlockId), newValues, new());
    }

}