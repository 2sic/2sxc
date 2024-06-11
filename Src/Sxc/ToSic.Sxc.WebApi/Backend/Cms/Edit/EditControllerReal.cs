using ToSic.Eav.WebApi.Cms;
using ToSic.Eav.WebApi.Formats;
using ToSic.Sxc.Backend.InPage;

namespace ToSic.Sxc.Backend.Cms;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class EditControllerReal(
    LazySvc<EditLoadBackend> loadBackend,
    LazySvc<EditSaveBackend> saveBackendLazy,
    LazySvc<HyperlinkBackend<int, int>> linkBackendLazy,
    LazySvc<AppViewPickerBackend> appViewPickerBackendLazy)
    : ServiceBase("Api.EditRl", connect: [loadBackend, saveBackendLazy, linkBackendLazy, appViewPickerBackendLazy]),
        IEditController
{
    public const string LogSuffix = "Edit";

    public EditDto Load(List<ItemIdentifier> items, int appId) => loadBackend.Value.Load(appId, items);

    public Dictionary<Guid, int> Save(EditDto package, int appId, bool partOfPage)
        => saveBackendLazy.Value.Init(appId).Save(package, partOfPage);


    public LinkInfoDto LinkInfo(string link, int appId, string contentType = default, Guid guid = default, string field = default)
        => linkBackendLazy.Value.LookupHyperlink(appId, link, contentType, guid, field);

    // TODO: we will need to make simpler implementation
    public bool Publish(int id)
        => appViewPickerBackendLazy.Value.Publish(id);
}