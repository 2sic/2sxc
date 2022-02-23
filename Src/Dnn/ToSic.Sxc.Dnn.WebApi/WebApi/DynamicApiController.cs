using System;
using System.IO;
using System.Net.Http;
using System.Web.Http.Controllers;
using ToSic.Eav.Documentation;
using ToSic.Sxc.Code;
using ToSic.Sxc.Dnn;
using ToSic.Sxc.Dnn.Code;
using ToSic.Sxc.Dnn.Run;
using ToSic.Sxc.Dnn.WebApi.Logging;
using ToSic.Sxc.Dnn.WebApiRouting;
using ToSic.Sxc.WebApi.Adam;

namespace ToSic.Sxc.WebApi
{
    /// <inheritdoc cref="SxcApiControllerBase" />
    /// <summary>
    /// This is the foundation for both the old SxcApiController and the new Dnn.ApiController.
    /// incl. the current App, DNN, Data, etc.
    /// For others, please use the SxiApiControllerBase, which doesn't have all that, and is usually then
    /// safer because it can't accidentally mix the App with a different appId in the params
    /// </summary>
    [PrivateApi("This is an internal base class used for the App ApiControllers. Make sure the implementations don't break")]
    [DnnLogExceptions]
    public abstract class DynamicApiController : SxcApiControllerBase, ICreateInstance, IHasDynamicCodeRoot
    {
        protected override string HistoryLogName => "Api.DynApi";

        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            var block = GetBlock();
            Log.Add($"HasBlock: {block != null}");
            // Note that the CmsBlock is created by the BaseClass, if it's detectable. Otherwise it's null
            // if it's null, use the log of this object
            var compatibilityLevel = this is IDynamicCode12 ? Constants.CompatibilityLevel12 : Constants.CompatibilityLevel10;
            _DynCodeRoot = GetService<DnnDynamicCodeRoot>().Init(block, Log, compatibilityLevel);
            _AdamCode = GetService<AdamCode>().Init(_DynCodeRoot, Log);

            // In case SxcBlock was null, there is no instance, but we may still need the app
            if (_DynCodeRoot.App == null)
            {
                Log.Add("DynCode.App is null");
                TryToAttachAppFromUrlParams();
            }

            // must run this after creating AppAndDataHelpers
            controllerContext.Request.Properties.Add(DnnConstants.DnnContextKey, Dnn); 

            if(controllerContext.Request.Properties.TryGetValue(CodeCompiler.SharedCodeRootPathKeyInCache, out var value))
                CreateInstancePath = value as string;
        }

        [PrivateApi]
        public IDynamicCodeRoot _DynCodeRoot { get; private set; }

        // ReSharper disable once InconsistentNaming
        [PrivateApi]
        public AdamCode _AdamCode { get; private set; }

        public IDnnContext Dnn => (_DynCodeRoot as DnnDynamicCodeRoot)?.Dnn;

        private void TryToAttachAppFromUrlParams()
        {
            var wrapLog = Log.Call();
            var found = false;
            try
            {
                var routeAppPath = Route.AppPathOrNull(Request.GetRouteData());
                var appId = SharedContextResolver.AppOrNull(routeAppPath)?.AppState.AppId ?? Eav.Constants.NullId;

                if (appId != Eav.Constants.NullId)
                {
                    // Look up if page publishing is enabled - if module context is not available, always false
                    Log.Add($"AppId: {appId}");
                    var app = Factory.App(appId, false, parentLog: Log);
                    _DynCodeRoot.AttachApp(app);
                    found = true;
                }
            } catch { /* ignore */ }

            wrapLog(found.ToString());
        }


        #region Adam - Shared Code Across the APIs
        /// <summary>
        /// See docs of official interface <see cref="IDynamicWebApi"/>
        /// </summary>
        public Sxc.Adam.IFile SaveInAdam(string noParamOrder = Eav.Parameters.Protector, 
            Stream stream = null, 
            string fileName = null, 
            string contentType = null, 
            Guid? guid = null, 
            string field = null,
            string subFolder = "") =>
            _AdamCode.SaveInAdam(
                stream: stream,
                fileName: fileName,
                contentType: contentType,
                guid: guid,
                field: field,
                subFolder: subFolder);

        #endregion

        public string CreateInstancePath { get; set; }

        public dynamic CreateInstance(string virtualPath, 
            string noParamOrder = Eav.Parameters.Protector,
            string name = null, 
            string relativePath = null, 
            bool throwOnError = true) =>
            _DynCodeRoot.CreateInstance(virtualPath, noParamOrder, name,
                CreateInstancePath, throwOnError);
    }
}
