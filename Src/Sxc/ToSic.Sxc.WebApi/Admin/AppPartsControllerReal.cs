using System;
using ToSic.Eav.Context;
using ToSic.Lib.Logging;
using ToSic.Eav.WebApi.Adam;
using ToSic.Eav.WebApi.Admin;
using ToSic.Eav.WebApi.Dto;
using ToSic.Lib.DI;
using ToSic.Sxc.WebApi.ImportExport;
using ServiceBase = ToSic.Lib.Services.ServiceBase;
#if NETFRAMEWORK
using THttpResponseType = System.Net.Http.HttpResponseMessage;
#else
using THttpResponseType = Microsoft.AspNetCore.Mvc.IActionResult;
#endif

namespace ToSic.Sxc.WebApi.Admin
{
    public class AppPartsControllerReal : ServiceBase, IAppPartsController
    {
        public const string LogSuffix = "AParts";

        public AppPartsControllerReal(
            LazySvc<IContextOfSite> context,
            LazySvc<ExportContent> exportContent,
            Generator<ImportContent> importContent
            ): base("Api.APartsRl")
        {
            ConnectServices(
                _context = context,
                _exportContent = exportContent,
                _importContent = importContent
            );
            
        }
        private readonly LazySvc<IContextOfSite> _context;
        private readonly LazySvc<ExportContent> _exportContent;
        private readonly Generator<ImportContent> _importContent;


        #region Parts Export/Import

        /// <inheritdoc />
        public ExportPartsOverviewDto Get(int zoneId, int appId, string scope) => _exportContent.Value.PreExportSummary(zoneId: zoneId, appId: appId, scope: scope);


        /// <inheritdoc />
        public THttpResponseType Export(int zoneId, int appId, string contentTypeIdsString, string entityIdsString, string templateIdsString)
            => _exportContent.Value.Export(zoneId: zoneId, appId: appId, contentTypeIdsString: contentTypeIdsString, entityIdsString: entityIdsString, templateIdsString: templateIdsString);


        /// <summary>
        /// This method is not implemented for ControllerReal, because ControllerReal implements Import(HttpUploadedFile uploadInfo, int zoneId, int appId)
        /// </summary>
        /// <param name="zoneId"></param>
        /// <param name="appId"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public ImportResultDto Import(int zoneId, int appId)
        {
            throw new NotImplementedException();
        }


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
                return wrapLog.Return(new ImportResultDto(false, "no file uploaded"), "no file uploaded");

            var (fileName, stream) = uploadInfo.GetStream(0);

            var result = _importContent.New()
                .Import(zoneId: zoneId, appId: appId, fileName: fileName, stream: stream, defaultLanguage: _context.Value.Site.DefaultCultureCode);

            return wrapLog.ReturnAsOk(result);
        }

        #endregion
    }
}