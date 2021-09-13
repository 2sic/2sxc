using System;
using System.Collections.Generic;
using System.Net;
using ToSic.Eav.Apps.Security;
using ToSic.Eav.Context;
using ToSic.Eav.DataFormats.EavLight;
using ToSic.Eav.ImportExport;
using ToSic.Eav.Plumbing;
using ToSic.Eav.Security.Permissions;
using ToSic.Eav.WebApi.Errors;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Context;
using ToSic.Sxc.Data;
using ToSic.Sxc.LookUp;


namespace ToSic.Sxc.WebApi.App
{
    public class AppQuery: WebApiBackendBase<AppQuery>
    {
        private const string AllStreams = "*";

        #region Constructor / DI

        public AppQuery(IServiceProvider serviceProvider, IContextResolver ctxResolver, IConvertToEavLight dataToFormatLight) : base(serviceProvider, "Sxc.ApiApQ")
        {
            _ctxResolver = ctxResolver;
            _dataToFormatLight = dataToFormatLight;
        }
        
        private readonly IContextResolver _ctxResolver;
        private readonly IConvertToEavLight _dataToFormatLight;

        #endregion

        #region In-Container-Context Queries

        public IDictionary<string, IEnumerable<EavLightEntity>> Query(int? appId, string name, bool includeGuid, string stream, AppQueryParameters more)
        {
            var wrapLog = Log.Call($"'{name}', inclGuid: {includeGuid}, stream: {stream}");

            var appCtx = appId != null ? _ctxResolver.BlockOrApp(appId.Value) : _ctxResolver.BlockRequired();

            // If the appId wasn't specified or == to the Block-AppId, then also include block info to enable more data-sources like CmsBlock
            var maybeBlock = appId == null || appId == appCtx.AppState.AppId ? _ctxResolver.RealBlockOrNull() : null;

            // If no app available from context, check if an app-id was supplied in url
            // Note that it may only be an app from the current portal
            // and security checks will run internally
            var app = ServiceProvider.Build<Apps.App>().Init(ServiceProvider, appCtx.AppState.AppId, Log, maybeBlock, appCtx.UserMayEdit);

            var result = BuildQueryAndRun(app, name, stream, includeGuid, appCtx,  appCtx.UserMayEdit, more);
            wrapLog(null);
            return result;
        }

        #endregion

        #region Public Queries


        public IDictionary<string, IEnumerable<EavLightEntity>> PublicQuery(string appPath, string name, string stream, AppQueryParameters more)
        {
            var wrapLog = Log.Call($"path:{appPath}, name:{name}, stream: {stream}");
            if (string.IsNullOrEmpty(name))
                throw HttpException.MissingParam(nameof(name));

            var appCtx = _ctxResolver.AppOrBlock(appPath);

            var queryApp = ServiceProvider.Build<Apps.App>().Init(appCtx.AppState,
                ServiceProvider.Build<AppConfigDelegate>().Init(Log).Build(appCtx.UserMayEdit), Log);

            // now just run the default query check and serializer
            var result = BuildQueryAndRun(queryApp, name, stream, false, appCtx, appCtx.UserMayEdit, more);
            wrapLog(null);
            return result;
        }


        #endregion


        private IDictionary<string, IEnumerable<EavLightEntity>> BuildQueryAndRun(
                IApp app, 
                string name, 
                string stream, 
                bool includeGuid, 
                IContextOfSite context, 
                bool userMayEdit,
                AppQueryParameters more)
        {
            var wrapLog = Log.Call($"name:{name}, stream:{stream}, withModule:{(context as IContextOfBlock)?.Module.Id}");
            var query = app.GetQuery(name);

            if (query == null)
            {
                var msg = $"query '{name}' not found";
                wrapLog(msg);
                throw new HttpExceptionAbstraction(HttpStatusCode.NotFound, msg, "query not found");
            }

            var permissionChecker = context.ServiceProvider.Build<AppPermissionCheck>()
                .ForItem(context, app, query.Definition.Entity, Log);
            var readExplicitlyAllowed = permissionChecker.UserMay(GrantSets.ReadSomething);

            var isAdmin = context.User.IsAdmin;

            // Only return query if permissions ok
            if (!(readExplicitlyAllowed || isAdmin))
            {
                var msg = $"Request not allowed. User does not have read permissions for query '{name}'";
                wrapLog(msg);
                throw new HttpExceptionAbstraction(HttpStatusCode.Unauthorized, msg, "Request not allowed");
            }

            //var serializer = new DataToDictionary(userMayEdit) { WithGuid = includeGuid };
            var serializer = _dataToFormatLight;
            if (serializer is ConvertToJsonLightWithCmsInfo serializerWithEdit) serializerWithEdit.WithEdit = userMayEdit;
            serializer.WithGuid = includeGuid;
            if (stream == AllStreams) stream = null;
            var result = serializer.Convert(query, stream?.Split(','), more?.Guids);
            wrapLog(null);
            return result;
        }
    }
}
