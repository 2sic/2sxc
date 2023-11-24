using System;
using System.Collections.Generic;
using ToSic.Eav.Apps;
using ToSic.Eav.Data;
using ToSic.Eav.Apps.Work;

namespace ToSic.Sxc.Blocks.Edit;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class BlockEditorForEntity : BlockEditorBase
{
    private readonly GenWorkDb<WorkEntityUpdate> _entityUpdate;

    public BlockEditorForEntity(MyServices services, IAppStates appStates, GenWorkDb<WorkEntityUpdate> entityUpdate) 
        : base(services)
    {
        ConnectServices(
            _entityUpdate = entityUpdate,
            _appStates = appStates
        );
    }

    #region methods which the entity-implementation must customize 

    protected override void SavePreviewTemplateId(Guid templateGuid)
        => Update(new Dictionary<string, object>
        {
            {ViewParts.TemplateContentType, templateGuid.ToString()}
        });


    internal override void SetAppId(int? appId)
    {
        // 2rm 2016-04-05 added resolver for app guid here (discuss w/ 2dm, I'm not sure why the id was saved before)
        var appName = "";
        if (appId.HasValue)
        {
            var zoneAppId = _appStates.IdentityOfApp(appId.Value);
            appName = _appStates.AppIdentifier(zoneAppId.ZoneId, zoneAppId.AppId);
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
        => Update(new Dictionary<string, object> { { key, value } });

    private void Update(Dictionary<string, object> newValues)
    {
        var parentAppState = ((BlockBase)Block).Parent.App.AppState;
        _entityUpdate.New(parentAppState)
            .UpdateParts(Math.Abs(Block.ContentBlockId), newValues);
    }

    private readonly IAppStates _appStates;

    #endregion

}