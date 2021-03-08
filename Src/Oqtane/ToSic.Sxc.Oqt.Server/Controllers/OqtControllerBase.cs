using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using Oqtane.Repository;
using System;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using ToSic.Eav;
using ToSic.Eav.Configuration;
using ToSic.Eav.Context;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Code;
using ToSic.Sxc.Context;
using ToSic.Sxc.Oqt.Server.Code;
using ToSic.Sxc.Oqt.Server.Run;
using ToSic.Sxc.WebApi.Adam;
using App = ToSic.Sxc.Apps.App;
using IApp = ToSic.Sxc.Apps.IApp;


namespace ToSic.Sxc.Oqt.Server.Controllers
{
    // TODO: replace oqtane dynamic code implementation with hybrid implementation
    public abstract class OqtControllerBase : OqtStatelessControllerBase, IHasOqtaneDynamicCodeContext
    {
        protected IServiceProvider ServiceProvider;
        private IModuleRepository _moduleRepository;
        private OqtTempInstanceContext _oqtTempInstanceContext;

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            ServiceProvider = context.HttpContext.RequestServices;

            base.OnActionExecuting(context);

            _moduleRepository = ServiceProvider.Build<IModuleRepository>(typeof(IModuleRepository));
            _oqtTempInstanceContext = ServiceProvider.Build<OqtTempInstanceContext>(typeof(OqtTempInstanceContext));
            DynCode = ServiceProvider.Build<OqtaneDynamicCode>().Init(GetBlock(), Log);
            var stxResolver = ServiceProvider.Build<IContextResolver>(typeof(IContextResolver));
            stxResolver.AttachRealBlock(() => GetBlock());
            stxResolver.AttachBlockContext(GetContext);

            // Latter used as path for DynCode.CreateInstance.
            if (context.HttpContext.Items.TryGetValue(CodeCompiler.SharedCodeRootPathKeyInCache, out var createInstancePath))
                CreateInstancePath = createInstancePath as string;
        }

        protected IContextOfSite GetSiteContext()
        {
            return ServiceProvider.Build<IContextOfSite>();
        }

        protected IContextOfApp GetAppContext(int appId)
        {
            // First get a normal basic context which is initialized with site, etc.
            var appContext = ServiceProvider.Build<IContextOfApp>();
            appContext.Init(Log);
            appContext.ResetApp(appId);
            return appContext;
        }

        protected IContextOfBlock GetContext() => GetBlock()?.Context ?? Factory.Resolve(typeof(IContextOfBlock)) as IContextOfBlock;

        protected IBlock GetBlock(bool allowNoContextFound = true) => _block ??= InitializeBlock(allowNoContextFound);
        private IBlock _block;

        private IBlock InitializeBlock(bool allowNoContextFound)
        {
            var wrapLog = Log.Call<IBlock>($"request:..., {nameof(allowNoContextFound)}: {allowNoContextFound}");

            var moduleId = GetTypedHeader(WebApi.WebApiConstants.HeaderInstanceId, -1);
            var contentBlockId = GetTypedHeader(WebApi.WebApiConstants.HeaderContentBlockId, 0); // this can be negative, so use 0
            var pageId = GetTypedHeader(WebApi.WebApiConstants.HeaderPageId, -1);

            if (moduleId == -1 || pageId == -1)
            {
                if (allowNoContextFound) return wrapLog("not found", null);
                throw new Exception("No context found, cannot continue");
            }

            var module = _moduleRepository.GetModule(moduleId);
            var ctx = _oqtTempInstanceContext.CreateContext(pageId, module, Log);
            IBlock block = ServiceProvider.Build<BlockFromModule>().Init(ctx, Log);

            // only if it's negative, do we load the inner block
            if (contentBlockId > 0) return wrapLog("found", block);

            Log.Add($"Inner Content: {contentBlockId}");
            block = ServiceProvider.Build<BlockFromEntity>().Init(block, contentBlockId, Log);
            return wrapLog("found", block);
        }

        private T GetTypedHeader<T>(string headerName, T fallback)
        {
            var valueString = HttpContext.Request.Headers[headerName];
            if (valueString == StringValues.Empty) return fallback;

            try
            {
                return (T)Convert.ChangeType(valueString.ToString(), typeof(T));
            }
            catch
            {
                return fallback;
            }

        }

        /// <summary>
        /// used for API calls to get the current app
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        internal IApp GetApp(int appId) => ServiceProvider.Build<App>().Init(ServiceProvider, appId, Log, GetBlock());

        public string CreateInstancePath { get; set; }

        public OqtaneDynamicCode DynCode { get; set; }

        public dynamic CreateInstance(string virtualPath,
            string dontRelyOnParameterOrder = Eav.Constants.RandomProtectionParameter,
            string name = null,
            string relativePath = null,
            bool throwOnError = true) =>
            DynCode.CreateInstance(virtualPath, dontRelyOnParameterOrder, name, CreateInstancePath, throwOnError);

        #region Adam - Shared Code Across the APIs (prevent duplicate code)

        /// <summary>
        /// See docs of official interface <see cref="IDynamicWebApi"/>
        /// </summary>
        public Sxc.Adam.IFile SaveInAdam(string dontRelyOnParameterOrder = Eav.Constants.RandomProtectionParameter,
            Stream stream = null,
            string fileName = null,
            string contentType = null,
            Guid? guid = null,
            string field = null,
            string subFolder = "")
        {
            Eav.Constants.ProtectAgainstMissingParameterNames(dontRelyOnParameterOrder, "SaveInAdam",
                $"{nameof(stream)},{nameof(fileName)},{nameof(contentType)},{nameof(guid)},{nameof(field)},{nameof(subFolder)} (optional)");

            if (stream == null || fileName == null || contentType == null || guid == null || field == null)
                throw new Exception();

            var feats = new[] { FeatureIds.UseAdamInWebApi, FeatureIds.PublicUpload };
            if (!Eav.Configuration.Features.EnabledOrException(feats, "can't save in ADAM", out var exp))
                throw exp;

            var appId = DynCode?.Block?.AppId ?? DynCode?.App?.AppId ?? throw new Exception("Error, SaveInAdam needs an App-Context to work, but the App is not known.");
            return ServiceProvider.Build<AdamTransUpload<int, int>>(typeof(AdamTransUpload<int, int>))
                .Init(appId, contentType, guid.Value, field, false, Log)
                .UploadOne(stream, fileName, subFolder, true);
        }

        #endregion
    }
}
