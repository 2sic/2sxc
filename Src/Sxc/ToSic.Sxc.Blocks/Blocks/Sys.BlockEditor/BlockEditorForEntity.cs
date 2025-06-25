using ToSic.Eav.Apps;
using ToSic.Sxc.Apps.Sys;
using ToSic.Sxc.Blocks.Internal;

namespace ToSic.Sxc.Blocks.Sys.BlockEditor;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class BlockEditorForEntity(
    BlockEditorBase.MyServices services,
    GenWorkDb<WorkEntityUpdate> entityUpdate,
    IAppsCatalog appsCatalog)
    : BlockEditorBase(services, connect: [entityUpdate, appsCatalog])
{
    #region methods which the entity-implementation must customize 

    protected override void SavePreviewTemplateId(Guid templateGuid)
        => Update(new()
        {
            {ViewParts.TemplateContentType, templateGuid.ToString()}
        });


    internal override void SetAppId(int? appId)
    {
        // 2rm 2016-04-05 added resolver for app guid here (discuss w/ 2dm, I'm not sure why the id was saved before)
        var appName = "";
        if (appId.HasValue)
        {
            var zoneAppId = appsCatalog.AppIdentity(appId.Value);
            appName = appsCatalog.AppNameId(zoneAppId);
        }
        UpdateValue(BlockBuildingConstants.CbPropertyApp, appName);
    }

    internal override void EnsureLinkToContentGroup(Guid cgGuid)
        => UpdateValue(BlockBuildingConstants.CbPropertyContentGroup, cgGuid.ToString()); // must pre-convert to string, as it's not a reference to an entity in the same app


    internal override void UpdateTitle(IEntity? titleItem)
    {
        var title = titleItem?.GetBestTitle();
        if (title == null)
            return;
        UpdateValue(BlockBuildingConstants.CbPropertyTitle, title);
    }

    #endregion

    #region private helpers

    private void UpdateValue(string key, object value) 
        => Update(new() { { key, value } });

    private void Update(Dictionary<string, object> newValues)
    {
        var parentBlock = Block.ParentBlockOrNull!; // must exist on an entity-block
        var parentBlockAppState = ((IAppWithInternal)parentBlock.App).AppReader;
        entityUpdate.New(parentBlockAppState)
            .UpdateParts(Math.Abs(Block.ContentBlockId), newValues, new());
    }

    #endregion

}