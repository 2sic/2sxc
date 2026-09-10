using ToSic.Eav.WebApi.Sys.Cms;
using ToSic.Sxc.Backend.InPage;
using Microsoft.Extensions.Logging;

namespace ToSic.Sxc.Backend.Cms;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class EditControllerReal(
    LazySvc<EditLoadBackend> loadBackend,
    LazySvc<EditSaveBackend> saveBackendLazy,
    LazySvc<HyperlinkBackend> linkBackendLazy,
    LazySvc<AppViewPickerBackend> appViewPickerBackendLazy,
    ILogStoreLive store,
    ILoggerFactory loggerFactory)
    : ServiceBase("Api.EditRl", connect: store.Mode == LogStoreMode.ILogger
        ? [saveBackendLazy, linkBackendLazy, appViewPickerBackendLazy]
        : [loadBackend, saveBackendLazy, linkBackendLazy, appViewPickerBackendLazy]),
        IEditController
{
    public const string LogSuffix = "Edit";
    private readonly ILogger _logger = loggerFactory.CreateLogger(MicrosoftLoggerEventSink.Category);

    public async Task<EditLoadDto> Load(List<ItemIdentifier> items, int appId)
    {
        var l = Log.Fn<EditLoadDto>($"appId:{appId}, items:{items?.Count}");
        using var invocation = _logger.BeginInvocation(l);
        try
        {
            var result = await loadBackend.Value.Load(appId, items!);
            return l.Return(result);
        }
        catch (Exception ex)
        {
            l.Done(ex);
            throw;
        }
    }

    public async Task<Dictionary<Guid, int>> Save(EditSaveDto package, int appId, bool partOfPage)
        => await saveBackendLazy.Value.Save(appId, package, partOfPage);


    public LinkInfoDto LinkInfo(string link, int appId, string? contentType = default, Guid guid = default, string? field = default)
        => linkBackendLazy.Value.LookupHyperlink(appId, link, contentType, guid, field);

    // TODO: we will need to make simpler implementation
    public bool Publish(int id)
        => appViewPickerBackendLazy.Value.Publish(id);
}
