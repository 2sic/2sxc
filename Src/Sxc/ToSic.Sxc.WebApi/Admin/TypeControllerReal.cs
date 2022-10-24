using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Context;
using ToSic.Eav.DI;
using ToSic.Lib.Logging;
using ToSic.Eav.Persistence.Logging;
using ToSic.Eav.WebApi;
using ToSic.Eav.WebApi.Adam;
using ToSic.Eav.WebApi.Admin;
using ToSic.Eav.WebApi.Assets;
using ToSic.Eav.WebApi.Dto;
using ToSic.Eav.WebApi.ImportExport;
using ToSic.Sxc.WebApi.ImportExport;

namespace ToSic.Sxc.WebApi.Admin
{
    public class TypeControllerReal<THttpResponseType> : HasLog<TypeControllerReal<THttpResponseType>>, ITypeController<THttpResponseType>
    {
        public const string LogSuffix = "Types";

        public TypeControllerReal(
            LazyInitLog<IContextOfSite> context,
            Lazy<ContentTypeApi> ctApiLazy, 
            Lazy<ContentExportApi<THttpResponseType>> contentExportLazy, 
            Lazy<IUser> userLazy,
            Generator<ImportContent> importContent
            ) : base("Api.TypesRl")
        {
            _context = context.SetLog(Log);
            _ctApiLazy = ctApiLazy;
            _contentExportLazy = contentExportLazy;
            _userLazy = userLazy;
            _importContent = importContent;
        }

        private readonly LazyInitLog<IContextOfSite> _context;
        private readonly Lazy<ContentTypeApi> _ctApiLazy;
        private readonly Lazy<ContentExportApi<THttpResponseType>> _contentExportLazy;
        private readonly Lazy<IUser> _userLazy;
        private readonly Generator<ImportContent> _importContent;


        public IEnumerable<ContentTypeDto> List(int appId, string scope = null, bool withStatistics = false)
            => _ctApiLazy.Value.Init(appId, Log).Get(scope, withStatistics);

        /// <summary>
        /// Used to be GET ContentTypes/Scopes
        /// </summary>
        public IDictionary<string, string> Scopes(int appId) => _ctApiLazy.Value.Init(appId, Log).Scopes();

        /// <summary>
        /// Used to be GET ContentTypes/Scopes
        /// </summary>
        public ContentTypeDto Get(int appId, string contentTypeId, string scope = null) => _ctApiLazy.Value.Init(appId, Log).GetSingle(contentTypeId, scope);


        public bool Delete(int appId, string staticName) => _ctApiLazy.Value.Init(appId, Log).Delete(staticName);


        // 2019-11-15 2dm special change: item to be Dictionary<string, object> because in DNN 9.4
        // it causes problems when a content-type has metadata, where a value then is a deeper object
        // in future, the JS front-end should send something clearer and not the whole object
        public bool Save(int appId, Dictionary<string, object> item)
        {
            var cleanList = item.ToDictionary(i => i.Key, i => i.Value?.ToString());
            return _ctApiLazy.Value.Init(appId, Log).Save(cleanList);
        }

        /// <summary>
        /// Used to be GET ContentType/CreateGhost
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="sourceStaticName"></param>
        /// <returns></returns>

        public bool AddGhost(int appId, string sourceStaticName) => _ctApiLazy.Value.Init(appId, Log).CreateGhost(sourceStaticName);


        public void SetTitle(int appId, int contentTypeId, int attributeId)
            => _ctApiLazy.Value.Init(appId, Log).SetTitle(contentTypeId, attributeId);

        /// <summary>
        /// Used to be GET ContentExport/DownloadTypeAsJson
        /// </summary>

        public THttpResponseType Json(int appId, string name)
            => _contentExportLazy.Value.Init(appId, Log).DownloadTypeAsJson(_userLazy.Value, name);

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
                streams.Add(new FileUploadDto { Name = fileName, Stream = stream });
            }
            var result = _importContent.New.Init(_userLazy.Value, Log)
                .ImportContentType(zoneId, appId, streams, _context.Ready.Site.DefaultCultureCode);

            return wrapLog.ReturnAsOk(result);
        }
    }
}