using System;
using System.Net.Http;
using ToSic.Eav.Context;
using ToSic.Eav.Logging;
using ToSic.Eav.Plumbing;
using ToSic.Eav.WebApi.Admin;
using ToSic.Eav.WebApi.Dto;
using ToSic.Sxc.WebApi.Adam;
using ToSic.Sxc.WebApi.ImportExport;

namespace ToSic.Sxc.WebApi.Admin
{
    public class AppPartsControllerReal : HasLog<AppPartsControllerReal>, IAppPartsController
    {
        public const string LogSuffix = "AParts";

        public AppPartsControllerReal(
            LazyInitLog<IContextOfSite> context,
            LazyInitLog<ExportContent> exportContent,
            Generator<ImportContent> importContent, 
            Lazy<IUser> user
            ): base("Api.APartsRl")
        {
            _context = context.SetLog(Log);
            _exportContent = exportContent.SetLog(Log);
            _importContent = importContent;
            _user = user;
        }
        private readonly LazyInitLog<IContextOfSite> _context;
        private readonly LazyInitLog<ExportContent> _exportContent;
        private readonly Generator<ImportContent> _importContent;
        private readonly Lazy<IUser> _user;


        #region Parts Export/Import

        /// <inheritdoc />
        public ExportPartsOverviewDto Get(int zoneId, int appId, string scope) => _exportContent.Ready.PreExportSummary(appId, zoneId, scope);


        /// <inheritdoc />
        public HttpResponseMessage Export(int zoneId, int appId, string contentTypeIdsString, string entityIdsString, string templateIdsString)
            => _exportContent.Ready.Export(appId, zoneId, contentTypeIdsString, entityIdsString, templateIdsString);


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
            var wrapLog = Log.Call<ImportResultDto>();

            if (!uploadInfo.HasFiles()) 
                return new ImportResultDto(false, "no files uploaded");

            var (fileName, stream) = uploadInfo.GetStream(0);

            var result = _importContent.New.Init(_user.Value, Log)
                .Import(zoneId, appId, fileName, stream, _context.Ready.Site.DefaultCultureCode);

            return wrapLog("ok", result);
        }

        #endregion
    }
}