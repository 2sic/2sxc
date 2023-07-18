using System.IO;
using System;
using System.Net.Http;
using System.Web.Http.Controllers;
using ToSic.Eav.Code.InfoSystem;
using ToSic.Eav.Generics;
using ToSic.Eav.Plumbing;
using ToSic.Lib.Logging;
using ToSic.Lib.Services;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Code;
using ToSic.Sxc.Code.CodeHelpers;
using ToSic.Sxc.Context;
using ToSic.Sxc.WebApi.Adam;
using ToSic.Sxc.Dnn;
using ToSic.Sxc.Dnn.Code;
using DotNetNuke.Web.Api;
using ToSic.Eav.Run;

namespace ToSic.Sxc.WebApi
{
    internal class DynamicApiCodeHelpers: CodeHelper
    {

        public DynamicApiCodeHelpers(DnnApiController owner, DnnWebApiHelper sysHlp)
        {
            _owner = owner;
            this.LinkLog((owner as IHasLog)?.Log);
            SysHlp = sysHlp;
        }

        private readonly DnnApiController _owner;
        private readonly DnnWebApiHelper SysHlp;

        #region Init

        /// <summary>
        /// This will make sure that any services requiring the context can get it.
        /// It must usually be called from the base class which expects to use this.
        /// </summary>
        /// <param name="request"></param>
        public void InitializeBlockContext(HttpRequestMessage request)
        {
            if (_blockContextInitialized) return;
            _blockContextInitialized = true;
            SharedContextResolver = SysHlp.GetService<IContextResolver>();
            SharedContextResolver.AttachBlock(SysHlp.GetBlockAndContext(request));
        }

        private bool _blockContextInitialized;


        public (IDynamicCodeRoot Root, string Path) Initialize(HttpControllerContext controllerContext)
        {
            var request = controllerContext.Request;
            InitializeBlockContext(request);

            // Note that the CmsBlock is created by the BaseClass, if it's detectable. Otherwise it's null
            var block = SysHlp.GetBlockAndContext(request)?.LoadBlock();
            Log.A($"HasBlock: {block != null}");
            var compatibilityLevel = (_owner as ICompatibilityLevel)?.CompatibilityLevel ?? Constants.CompatibilityLevel10;

            var services = SysHlp.GetService<DynamicApiServices>().ConnectServices(Log);
            var codeRoot = services.CodeRootFactory
                .BuildDynamicCodeRoot(this)
                .InitDynCodeRoot(block, Log, compatibilityLevel);

            SysHlp.ConnectToRoot(codeRoot);

            AdamCode = codeRoot.GetService<AdamCode>();

            // In case SxcBlock was null, there is no instance, but we may still need the app
            if (codeRoot.App == null)
            {
                Log.A("DynCode.App is null");
                var app = TryToAttachAppFromUrlParams(services, request);
                if (app != null)
                    codeRoot.AttachApp(app);
            }

            var reqProperties = request.Properties;

            // must run this after creating AppAndDataHelpers
            reqProperties.Add(DnnConstants.DnnContextKey, (codeRoot as IHasDnn)?.Dnn);

            /*if (*/
            reqProperties.TryGetTyped(CodeCompiler.SharedCodeRootPathKeyInCache, out string path);
            /*) CreateInstancePath = path; */

            // 16.02 - try to log more details about the current API call
            var currentPath = reqProperties.TryGetTyped(CodeCompiler.SharedCodeRootFullPathKeyInCache, out string p2) ? p2.AfterLast("/") : null;
            SysHlp.WebApiLogging?.AddLogSpecs(block, codeRoot.App, currentPath, SysHlp.GetService<CodeInfosInScope>());


            return (codeRoot, path);
        }


        private IApp TryToAttachAppFromUrlParams(DynamicApiServices services, HttpRequestMessage request)
        {
            var l = Log.Fn<IApp>();
            try
            {
                var routeAppPath = services.AppFolderUtilities.GetAppFolder(request, false);
                var appState = SharedContextResolver.SetAppOrNull(routeAppPath)?.AppState;

                if (appState != default)
                {
                    var siteCtx = SharedContextResolver.Site();
                    // Look up if page publishing is enabled - if module context is not available, always false
                    l.A($"AppId: {appState.AppId}");
                    var app = services.AppOverrideLazy.Value
                        .PreInit(siteCtx.Site)
                        .Init(appState, services.AppConfigDelegateLazy.Value.Build());
                    //_DynCodeRoot.AttachApp(app);
                    return l.Return(app, $"found #{app.AppId}");
                }
            }
            catch
            {
                l.ReturnNull("error, ignore");
                /* ignore */
            }

            return l.ReturnNull("no app detected");
        }

        #endregion

        public IContextResolver SharedContextResolver;

        #region Adam

        public AdamCode AdamCode { get; private set; }

        public Sxc.Adam.IFile SaveInAdam(string noParamOrder = Eav.Parameters.Protector,
            Stream stream = null,
            string fileName = null,
            string contentType = null,
            Guid? guid = null,
            string field = null,
            string subFolder = "") =>
            AdamCode.SaveInAdam(
                stream: stream,
                fileName: fileName,
                contentType: contentType,
                guid: guid,
                field: field,
                subFolder: subFolder);

        #endregion

    }
}
