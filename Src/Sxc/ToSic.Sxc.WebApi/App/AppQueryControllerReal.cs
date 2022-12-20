using System;
using System.Collections.Generic;
using System.Net;
using ToSic.Eav.Apps.Security;
using ToSic.Eav.Context;
using ToSic.Eav.DataFormats.EavLight;
using ToSic.Lib.Logging;
using ToSic.Eav.Security.Permissions;
using ToSic.Eav.WebApi;
using ToSic.Eav.WebApi.Admin.App;
using ToSic.Eav.WebApi.Admin.Query;
using ToSic.Eav.WebApi.Errors;
using ToSic.Lib.Services;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Context;
using ToSic.Sxc.Data;
using ToSic.Sxc.LookUp;
using ToSic.Lib.DI;

namespace ToSic.Sxc.WebApi.App
{
    /// <summary>
    /// In charge of delivering Pipeline-Queries on the fly
    /// They will only be delivered if the security is confirmed - it must be publicly available
    /// </summary>
    public class AppQueryControllerReal: ServiceBase , IAppQueryController
    {
        private readonly GeneratorLog<AppConfigDelegate> _appConfigDelegate;
        private readonly GeneratorLog<Apps.App> _app;
        public const string LogSuffix = "AppQry";

        private const string AllStreams = "*";

        #region Constructor / DI

        public AppQueryControllerReal(IContextResolver ctxResolver, 
            IConvertToEavLight dataToFormatLight, 
            GeneratorLog<AppPermissionCheck> appPermissionCheck,
            GeneratorLog<AppConfigDelegate> appConfigDelegate,
            GeneratorLog<Apps.App> app) : base("Sxc.ApiApQ")
        {
            ConnectServices(
                _ctxResolver = ctxResolver,
                _dataToFormatLight = dataToFormatLight,
                _appPermissionCheck = appPermissionCheck,
                _appConfigDelegate = appConfigDelegate,
                _app = app);
        }
        
        private readonly IContextResolver _ctxResolver;
        private readonly IConvertToEavLight _dataToFormatLight;
        private readonly GeneratorLog<AppPermissionCheck> _appPermissionCheck;

        #endregion

        #region In-Container-Context Queries

        public IDictionary<string, IEnumerable<EavLightEntity>> Query(string name, int? appId, string stream = null,
            bool includeGuid = false)
            => QueryPost(name, null, appId, stream, includeGuid);

        public IDictionary<string, IEnumerable<EavLightEntity>> QueryPost(string name, QueryParameters more,
            int? appId, string stream = null,
            bool includeGuid = false)
        {
            var wrapLog = Log.Fn<IDictionary<string, IEnumerable<EavLightEntity>>>($"'{name}', inclGuid: {includeGuid}, stream: {stream}");

            var appCtx = appId != null ? _ctxResolver.BlockOrApp(appId.Value) : _ctxResolver.BlockRequired();

            // If the appId wasn't specified or == to the Block-AppId, then also include block info to enable more data-sources like CmsBlock
            var maybeBlock = appId == null || appId == appCtx.AppState.AppId ? _ctxResolver.RealBlockOrNull() : null;

            // If no app available from context, check if an app-id was supplied in url
            // Note that it may only be an app from the current portal
            // and security checks will run internally
            var app = _app.New().Init(appCtx.AppState.AppId, Log, maybeBlock, appCtx.UserMayEdit);

            var result = BuildQueryAndRun(app, name, stream, includeGuid, appCtx,  appCtx.UserMayEdit, more);
            return wrapLog.Return(result);
        }

        #endregion

        #region Public Queries

        public IDictionary<string, IEnumerable<EavLightEntity>> PublicQuery(string appPath, string name, string stream)
            => PublicQueryPost(appPath, name, null, stream);


        public IDictionary<string, IEnumerable<EavLightEntity>> PublicQueryPost(string appPath, string name, QueryParameters more, string stream)
        {
            var wrapLog = Log.Fn<IDictionary<string, IEnumerable<EavLightEntity>>>($"path:{appPath}, name:{name}, stream: {stream}");
            if (string.IsNullOrEmpty(name))
                throw HttpException.MissingParam(nameof(name));

            var appCtx = _ctxResolver.AppOrBlock(appPath);
            
            var queryApp = _app.New().Init(appCtx.AppState,
                _appConfigDelegate.New().Build(appCtx.UserMayEdit), Log);

            // now just run the default query check and serializer
            var result = BuildQueryAndRun(queryApp, name, stream, false, appCtx, appCtx.UserMayEdit, more);
            return wrapLog.Return(result);
        }


        #endregion


        private IDictionary<string, IEnumerable<EavLightEntity>> BuildQueryAndRun(
                IApp app, 
                string name, 
                string stream, 
                bool includeGuid, 
                IContextOfSite context, 
                bool userMayEdit,
                QueryParameters more)
        {
            var wrapLog = Log.Fn<IDictionary<string, IEnumerable<EavLightEntity>>>($"name:{name}, stream:{stream}, withModule:{(context as IContextOfBlock)?.Module.Id}");
            var query = app.GetQuery(name);

            if (query == null)
            {
                var msg = $"query '{name}' not found";
                wrapLog.ReturnNull(msg);
                throw new HttpExceptionAbstraction(HttpStatusCode.NotFound, msg, "query not found");
            }

            var permissionChecker = _appPermissionCheck.New()
                .ForItem(context, app, query.Definition.Entity, Log);
            var readExplicitlyAllowed = permissionChecker.UserMay(GrantSets.ReadSomething);

            var isAdmin = context.User.IsContentAdmin;

            // Only return query if permissions ok
            if (!(readExplicitlyAllowed || isAdmin))
            {
                var msg = $"Request not allowed. User does not have read permissions for query '{name}'";
                wrapLog.ReturnNull(msg);
                throw new HttpExceptionAbstraction(HttpStatusCode.Unauthorized, msg, "Request not allowed");
            }

            //var serializer = new DataToDictionary(userMayEdit) { WithGuid = includeGuid };
            var serializer = _dataToFormatLight;
            if (serializer is ConvertToEavLightWithCmsInfo serializerWithEdit) serializerWithEdit.WithEdit = userMayEdit;
            serializer.WithGuid = includeGuid;
            if (stream == AllStreams) stream = null;
            var result = serializer.Convert(query, stream?.Split(','), more?.Guids);
            return wrapLog.Return(result);
        }
    }
}
