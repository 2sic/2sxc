using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Internal.Work;

namespace ToSic.Sxc.Blocks.Internal;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class BlockEditorForEntity : BlockEditorBase
{
    private readonly IAppsCatalog _appsCatalog;
    private readonly GenWorkDb<WorkEntityUpdate> _entityUpdate;

    public BlockEditorForEntity(MyServices services, GenWorkDb<WorkEntityUpdate> entityUpdate, IAppsCatalog appsCatalog)
        : base(services)
    {
        ConnectLogs([
            _entityUpdate = entityUpdate,
            _appsCatalog = appsCatalog,
        ]);
    }

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
            var zoneAppId = _appsCatalog.AppIdentity(appId.Value);
            appName = _appsCatalog.AppNameId(zoneAppId);
        }
        UpdateValue(BlockFromEntity.CbPropertyApp, appName);
    }

    internal override void EnsureLinkToContentGroup(Guid cgGuid)
        => UpdateValue(BlockFromEntity.CbPropertyContentGroup, cgGuid.ToString()); // must pre-convert to string, as it's not a reference to an entity in the same app


    internal override void UpdateTitle(IEntity titleItem)
    {
        if (titleItem?.GetBestTitle() == null) return;
        UpdateValue(BlockFromEntity.CbPropertyTitle, titleItem.GetBestTitle());
    }

    #endregion

    #region private helpers

    private void UpdateValue(string key, object value) 
        => Update(new() { { key, value } });

    private void Update(Dictionary<string, object> newValues)
    {
        var parentBlockAppState = ((IAppWithInternal)((BlockBase)Block).Parent.App).AppReader;
        _entityUpdate.New(parentBlockAppState)
            .UpdateParts(Math.Abs(Block.ContentBlockId), newValues);
    }

    #endregion

}