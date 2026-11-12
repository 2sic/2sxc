using ToSic.Eav.Ui.Edit.Sys;
using ToSic.Eav.WebApi.Sys.Cms;

namespace ToSic.Sxc.Backend.Cms;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class EditControllerReal(LazySvc<EditLoadBackend> load, LazySvc<EditSaveBackend> save, LazySvc<HyperlinkBackend> links, LazySvc<PublishWebApi> publish)
    : ServiceBase("Api.EditRl", connect: [load, save, links, publish]), IEditController
{
    public const string LogSuffix = "Edit";

    public Task<EditLoadDto> Load(List<ItemIdentifier> items, int appId)
        => load.Value.Load(appId, items);

    public Task<Dictionary<Guid, int>> Save(EditSaveDto package, int appId, bool partOfPage)
        => save.Value.Save(appId, package, partOfPage);


    public LinkInfoDto LinkInfo(string link, int appId, string? contentType = default, Guid guid = default, string? field = default)
        => links.Value.LookupHyperlink(appId, link, contentType, guid, field);

    // TODO: we will need to make simpler implementation
    public Task<bool> Publish(int id) => publish.Value.Publish(id);
}
