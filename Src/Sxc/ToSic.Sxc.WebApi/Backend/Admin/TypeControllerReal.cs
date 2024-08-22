using ToSic.Eav.Data.Shared;
using ToSic.Eav.Persistence.Logging;
using ToSic.Eav.WebApi;
using ToSic.Eav.WebApi.Adam;
using ToSic.Eav.WebApi.Admin;
using ToSic.Eav.WebApi.Assets;
using ToSic.Eav.WebApi.ImportExport;
using ToSic.Sxc.Backend.ImportExport;
using ServiceBase = ToSic.Lib.Services.ServiceBase;
#if NETFRAMEWORK
using THttpResponseType = System.Net.Http.HttpResponseMessage;
#else
using THttpResponseType = Microsoft.AspNetCore.Mvc.IActionResult;
#endif

namespace ToSic.Sxc.Backend.Admin;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class TypeControllerReal(
    LazySvc<IContextOfSite> context,
    LazySvc<ContentTypeDtoService> ctApiLazy,
    LazySvc<ContentExportApi> contentExportLazy,
    GenWorkDb<WorkContentTypesMod> typeMod,
    LazySvc<IUser> userLazy,
    IAppReaderFactory appReaders,
    Generator<ImportContent> importContent)
    : ServiceBase("Api.TypesRl",
        connect: [appReaders, context, ctApiLazy, contentExportLazy, userLazy, typeMod, importContent]), ITypeController
{
    public const string LogSuffix = "Types";


    public IEnumerable<ContentTypeDto> List(int appId, string scope = null, bool withStatistics = false)
    {
        var l = Log.Fn<IEnumerable<ContentTypeDto>>($"{appId}, scope:{scope}, stats:{withStatistics}");
        var list = ctApiLazy.Value.List(appId, scope, withStatistics);
        return l.Return(list);
    }

    /// <summary>
    /// Used to be GET ContentTypes/Scopes
    /// </summary>
    public ScopesDto Scopes(int appId)
    {
        var l = Log.Fn<ScopesDto>($"{appId}");
        var reader = appReaders.Get(appId);
        var dic = reader.ContentTypes.GetAllScopesWithLabels();
        var infos = dic
            .Select(pair =>
            {
                var typesInScope = reader.ContentTypes.OfScope(pair.Key).ToList();
                var count = typesInScope.Count;
                var withAncestor = typesInScope.Where(ct => ct.HasAncestor()).ToList();
                return new ScopeDetailsDto
                {
                    Name = pair.Key,
                    Label = pair.Value ?? pair.Key,
                    TypesTotal = count,
                    TypesInherited = withAncestor.Count,
                    TypesOfApp = count - withAncestor.Count
                };
            })
            .ToList();
        return l.Return(new() { Old = dic, Scopes = infos });
    }

    /// <summary>
    /// Used to be GET ContentTypes/Scopes
    /// </summary>
    public ContentTypeDto Get(int appId, string contentTypeId, string scope = null)
        => ctApiLazy.Value.GetSingle(appId, contentTypeId, scope);


    public bool Delete(int appId, string staticName)
        => typeMod.New(appId).Delete(staticName);


    // 2019-11-15 2dm special change: item to be Dictionary<string, object> because in DNN 9.4
    // it causes problems when a content-type has additional metadata, where a value then is a deeper object
    // in the future, the JS front-end should send something clearer and not the whole object
    public bool Save(int appId, Dictionary<string, object> item)
    {
        var l = Log.Fn<bool>();
            
        if (item == null) return l.ReturnFalse("item was null, will cancel");

        var dic = item.ToDictionary(i => i.Key, i => i.Value?.ToString());
        var result = typeMod.New(appId).AddOrUpdate(dic["StaticName"], dic["Scope"], dic["Name"], null, false);
            
        return l.ReturnAndLog(result);
    }

    /// <summary>
    /// Used to be GET ContentType/CreateGhost
    /// </summary>
    /// <param name="appId"></param>
    /// <param name="sourceStaticName"></param>
    /// <returns></returns>
    public bool AddGhost(int appId, string sourceStaticName)
        => typeMod.New(appId).CreateGhost(sourceStaticName);


    public void SetTitle(int appId, int contentTypeId, int attributeId)
        => typeMod.New(appId).SetTitle(contentTypeId, attributeId);

    /// <summary>
    /// Used to be GET ContentExport/DownloadTypeAsJson
    /// </summary>
    public THttpResponseType Json(int appId, string name)
        => contentExportLazy.Value.Init(appId).DownloadTypeAsJson(userLazy.Value, name);

    /// <summary>
    /// This method is not implemented for ControllerReal, because ControllerReal implements Import(HttpUploadedFile uploadInfo, int zoneId, int appId)
    /// </summary>
    /// <param name="zoneId"></param>
    /// <param name="appId"></param>
    /// <returns></returns>
    /// <exception cref="NotSupportedException"></exception>
    public ImportResultDto Import(int zoneId, int appId)
        => throw new NotSupportedException("This is not supported on ControllerReal, use overload.");

    /// <summary>
    /// This implementation is special ControllerReal, instead of ImportResultDto Import(int zoneId, int appId) that is not implemented.
    /// </summary>
    /// <param name="uploadInfo"></param>
    /// <param name="zoneId"></param>
    /// <param name="appId"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public ImportResultDto Import(HttpUploadedFile uploadInfo, int zoneId, int appId)
    {
        var l = Log.Fn<ImportResultDto>();

        if (!uploadInfo.HasFiles())
            return l.Return(new(false, "no file uploaded", Message.MessageTypes.Error), "no file uploaded");

        var streams = new List<FileUploadDto>();
        for (var i = 0; i < uploadInfo.Count; i++)
        {
            var (fileName, stream) = uploadInfo.GetStream(i);
            streams.Add(new() { Name = fileName, Stream = stream });
        }
        var result = importContent.New()
            .ImportJsonFiles(zoneId, appId, streams, context.Value.Site.DefaultCultureCode);

        return l.ReturnAsOk(result);
    }

    /// <summary>
    /// Used to be GET ContentExport/DownloadTypeAsJson
    /// </summary>

    public THttpResponseType JsonBundleExport(int appId, Guid exportConfiguration, int indentation)
        => contentExportLazy.Value.Init(appId).JsonBundleExport(userLazy.Value, exportConfiguration, indentation);
}