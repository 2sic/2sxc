using System;
using System.Collections.Generic;
using ToSic.Eav.Context;
using ToSic.Eav.Persistence.Logging;
using ToSic.Eav.WebApi.Adam;
using ToSic.Eav.WebApi.Assets;
using ToSic.Eav.WebApi.Context;
using ToSic.Eav.WebApi.Dto;
using ToSic.Lib.DI;
using ToSic.Lib.Logging;
using ToSic.Sxc.Apps.Internal;
using ToSic.Sxc.Backend.Usage;
using ToSic.Sxc.Backend.Views;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Blocks.Internal;
using ServiceBase = ToSic.Lib.Services.ServiceBase;
#if NETFRAMEWORK
using THttpResponseType = System.Net.Http.HttpResponseMessage;
#else
using THttpResponseType = Microsoft.AspNetCore.Mvc.IActionResult;
#endif

namespace ToSic.Sxc.Backend.Admin;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class ViewControllerReal : ServiceBase, IViewController
{
    public const string LogSuffix = "View";

    public ViewControllerReal(
        LazySvc<IContextOfSite> context,
        LazySvc<ViewsBackend> viewsBackend, 
        LazySvc<ViewsExportImport> viewExportImport, 
        LazySvc<UsageBackend> usageBackend, 
        LazySvc<PolymorphismBackend> polymorphismBackend
    ) : base("Api.ViewRl")
    {
        ConnectServices(
            _context = context,
            _polymorphismBackend = polymorphismBackend,
            _usageBackend = usageBackend,
            _viewsBackend = viewsBackend,
            _viewExportImport = viewExportImport
        );
    }
    private readonly LazySvc<IContextOfSite> _context;
    private readonly LazySvc<PolymorphismBackend> _polymorphismBackend;
    private readonly LazySvc<UsageBackend> _usageBackend;
    private readonly LazySvc<ViewsBackend> _viewsBackend;
    private readonly LazySvc<ViewsExportImport> _viewExportImport;

    /// <inheritdoc />
    public IEnumerable<ViewDetailsDto> All(int appId) => _viewsBackend.Value.GetAll(appId);

    /// <inheritdoc />
    public PolymorphismDto Polymorphism(int appId) => _polymorphismBackend.Value.Polymorphism(appId);

    /// <inheritdoc />
    public bool Delete(int appId, int id) => _viewsBackend.Value.Delete(appId, id);

    /// <inheritdoc />
    public THttpResponseType Json(int appId, int viewId) => _viewExportImport.Value.DownloadViewAsJson(appId, viewId);

    /// <summary>
    /// This method is not implemented for ControllerReal, because ControllerReal implements Import(HttpUploadedFile uploadInfo, int zoneId, int appId)
    /// </summary>
    /// <param name="zoneId"></param>
    /// <param name="appId"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public ImportResultDto Import(int zoneId, int appId) => throw new NotImplementedException();

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
        var wrapLog = Log.Fn<ImportResultDto>();
            
        if (!uploadInfo.HasFiles())
            return wrapLog.Return(new ImportResultDto(false, "no file uploaded", Message.MessageTypes.Error), "no file uploaded");

        var streams = new List<FileUploadDto>();
        for (var i = 0; i < uploadInfo.Count; i++)
        {
            var (fileName, stream) = uploadInfo.GetStream(i);
            streams.Add(new FileUploadDto {Name = fileName, Stream = stream});
        }
        var result = _viewExportImport.Value.ImportView(zoneId, appId, streams, _context.Value.Site.DefaultCultureCode);

        return wrapLog.ReturnAsOk(result);
    }

    /// <inheritdoc />
    public IEnumerable<ViewDto> Usage(int appId, Guid guid)
    {
        if (FinalBuilder == null)
        {
            Log.A("Error, FinalBuilder implementation is not set.");
            throw new ArgumentException("FinalBuilder implementation is not set.");
        }
        return _usageBackend.Value.ViewUsage(appId, guid, FinalBuilder);
    }
    public ViewControllerReal UsagePreparations(Func<List<IView>, List<BlockConfiguration>, IEnumerable<ViewDto>> finalBuilder)
    {
        FinalBuilder = finalBuilder;
        return this;
    }
    private Func<List<IView>, List<BlockConfiguration>, IEnumerable<ViewDto>> FinalBuilder { get; set; }

    /// <summary>
    /// Helper method to get SiteId for ControllerReal proxy class.
    /// </summary>
    public int SiteId => _context.Value.Site.Id;
}