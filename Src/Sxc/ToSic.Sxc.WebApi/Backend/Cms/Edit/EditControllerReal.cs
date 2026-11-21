using ToSic.Eav.Ui.Edit.Sys;
using ToSic.Eav.Ui.Edit.Sys.Load.Links;
using ToSic.Sys.HookUp;

namespace ToSic.Sxc.Backend.Cms;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class EditControllerReal(LazySvc<EditLoadBackend> load, LazySvc<EditSaveBackend> save, LazySvc<ILinkResolver> links, LazySvc<IContextCompleteService> ctxService, LazySvc<PublishWebApi> publish)
    : ServiceBase("Api.EditRl", connect: [load, save, links, ctxService, publish])
{
    public const string LogSuffix = "Edit";

    public Task<EditLoadDto> Load(List<ItemIdentifier> items, int appId)
        => load.Value.Load(appId, items);

    public Task<Dictionary<Guid, int>> Save(EditSaveDto package, int appId, bool partOfPage)
        => save.Value.Save(appId, package, partOfPage);


    public async Task<LinkInfoDto> LinkInfo(string link, int appId, string? contentType = default, Guid guid = default, string? field = default)
        => (await links.Value.Handle(new(), new ILinkResolver.Parameters(link, contentType, guid, field).ToPackage())).Data;

    // TODO: we will need to make simpler implementation
    public Task<bool> Publish(int id) => publish.Value.Publish(ctxService.Value.BlockRequired(), id);
}
