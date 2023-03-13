using System.Collections.Generic;
using System.Net;
using ToSic.Eav.Apps.Security;
using ToSic.Eav.Context;
using ToSic.Eav.DataFormats.EavLight;
using ToSic.Lib.Logging;
using ToSic.Eav.Security.Permissions;
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
        private readonly Generator<AppConfigDelegate> _appConfigDelegate;
        private readonly Generator<Apps.App> _app;
        public const string LogSuffix = "AppQry";

        private const string AllStreams = "*";

        #region Constructor / DI

        public AppQueryControllerReal(Sxc.Context.IContextResolver ctxResolver, 
            IConvertToEavLight dataConverter, 
            Generator<AppPermissionCheck> appPermissionCheck,
            Generator<AppConfigDelegate> appConfigDelegate,
            Generator<Apps.App> app) : base("Sxc.ApiApQ")
        {
            ConnectServices(
                _ctxResolver = ctxResolver,
                _dataConverter = dataConverter,
                _appPermissionCheck = appPermissionCheck,
                _appConfigDelegate = appConfigDelegate,
                _app = app
            );
        }
        
        private readonly Sxc.Context.IContextResolver _ctxResolver;
        private readonly IConvertToEavLight _dataConverter;
        private readonly Generator<AppPermissionCheck> _appPermissionCheck;

        #endregion

        #region In-Container-Context Queries

        public IDictionary<string, IEnumerable<EavLightEntity>> Query(string name, int? appId, string stream = null,
            bool includeGuid = false)
            => QueryPost(name, null, appId, stream, includeGuid);

        public IDictionary<string, IEnumerable<EavLightEntity>> QueryPost(string name, QueryParameters more,
            int? appId, string stream = null,
            bool includeGuid = false) => Log.Func($"'{name}', inclGuid: {includeGuid}, stream: {stream}", l =>
        {
            var appCtx = appId != null ? _ctxResolver.GetBlockOrSetApp(appId.Value) : _ctxResolver.BlockContextRequired();

            // If the appId wasn't specified or == to the Block-AppId, then also include block info to enable more data-sources like CmsBlock
            var maybeBlock = appId == null || appId == appCtx.AppState.AppId ? _ctxResolver.BlockOrNull() : null;

            // If no app available from context, check if an app-id was supplied in url
            // Note that it may only be an app from the current portal
            // and security checks will run internally
            var app = _app.New().Init(appCtx.AppState.AppId, maybeBlock);

            var result = BuildQueryAndRun(app, name, stream, includeGuid, appCtx, more);
            return result;
        });

        #endregion

        #region Public Queries

        public IDictionary<string, IEnumerable<EavLightEntity>> PublicQuery(string appPath, string name, string stream)
            => PublicQueryPost(appPath, name, null, stream);


        public IDictionary<string, IEnumerable<EavLightEntity>> PublicQueryPost(string appPath, string name,
            QueryParameters more, string stream) => Log.Func($"path:{appPath}, name:{name}, stream: {stream}", l =>
        {
            if (string.IsNullOrEmpty(name))
                throw HttpException.MissingParam(nameof(name));

            var appCtx = _ctxResolver.SetAppOrGetBlock(appPath);
            
            var queryApp = _app.New().Init(appCtx.AppState, _appConfigDelegate.New().Build(/*appCtx.UserMayEdit*/));

            // now just run the default query check and serializer
            var result = BuildQueryAndRun(queryApp, name, stream, false, appCtx, /*appCtx.UserMayEdit,*/ more);
            return result;
        });


        #endregion


        private IDictionary<string, IEnumerable<EavLightEntity>> BuildQueryAndRun(
            IApp app,
            string name,
            string stream,
            bool includeGuid,
            IContextOfApp context,
            //bool userMayEdit,
            QueryParameters more
        ) => Log.Func($"name:{name}, stream:{stream}, withModule:{(context as IContextOfBlock)?.Module.Id}", l =>
        {
            var query = app.GetQuery(name);

            if (query == null)
            {
                var msg = $"query '{name}' not found";
                throw l.Ex(new HttpExceptionAbstraction(HttpStatusCode.NotFound, msg, "query not found"));
            }

            var permissionChecker = _appPermissionCheck.New()
                .ForItem(context, app, query.Definition.Entity);
            var readExplicitlyAllowed = permissionChecker.UserMay(GrantSets.ReadSomething);

            var isAdmin = context.User.IsContentAdmin;

            // Only return query if permissions ok
            if (!(readExplicitlyAllowed || isAdmin))
            {
                var msg = $"Request not allowed. User does not have read permissions for query '{name}'";
                throw l.Ex(new HttpExceptionAbstraction(HttpStatusCode.Unauthorized, msg, "Request not allowed"));
            }

            _dataConverter.WithGuid = includeGuid;
            if (_dataConverter is ConvertToEavLightWithCmsInfo serializerWithEdit)
                serializerWithEdit.WithEdit = context.UserMayEdit; // userMayEdit;
            if (stream == AllStreams) stream = null;
            var result = _dataConverter.Convert(query, stream?.Split(','), more?.Guids);
            return result;
        });
    }
}
