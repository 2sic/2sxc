using Microsoft.Extensions.Primitives;
using Oqtane.Repository;
using System;
using System.IO;
using ToSic.Eav.Configuration;
using ToSic.Eav.Context;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Context;
using ToSic.Sxc.Oqt.Server.Code;
using ToSic.Sxc.Oqt.Server.Run;
using ToSic.Sxc.WebApi.Adam;
using App = ToSic.Sxc.Apps.App;
using IApp = ToSic.Sxc.Apps.IApp;


namespace ToSic.Sxc.Oqt.Server.Controllers
{
    // TODO: replace oqtane dynamic code implementation with hybrid implementation
    public abstract class OqtStatefulControllerBase: OqtStatelessControllerBase, IHasOqtaneDynamicCodeContext
    {
        protected readonly IServiceProvider ServiceProvider;
        private readonly OqtState _oqtState;
        private readonly IModuleRepository _moduleRepository;
        private readonly OqtTempInstanceContext _oqtTempInstanceContext;

        protected OqtStatefulControllerBase(StatefulControllerDependencies dependencies) : base()
        {
            ServiceProvider = dependencies.ServiceProvider;
            _oqtState = new OqtState(HttpContext, Log);
            _moduleRepository = dependencies.ModuleRepository;
            _oqtTempInstanceContext = dependencies.OqtTempInstanceContext;

            dependencies.CtxResolver.AttachRealBlock(() => GetBlock());
            dependencies.CtxResolver.AttachBlockContext(GetContext);
        }

        protected IContextOfSite GetSiteContext() => _oqtState.GetSiteContext();

        protected IContextOfApp GetAppContext(int appId) => _oqtState.GetAppContext(appId);

        protected IContextOfBlock GetContext() => _oqtState.GetContext();

        protected IBlock GetBlock(bool allowNoContextFound = true) => _oqtState.GetBlock(allowNoContextFound);

        protected IApp GetApp(int appId) => _oqtState.GetApp(appId);



        public string CreateInstancePath { get; set; }

        public OqtaneDynamicCode DynCode => _dynCode ??= ServiceProvider.Build<OqtaneDynamicCode>().Init(GetBlock(), Log);
        private OqtaneDynamicCode _dynCode;

        protected dynamic CreateInstance(string virtualPath,
            string dontRelyOnParameterOrder = Eav.Constants.RandomProtectionParameter,
            string name = null,
            string relativePath = null,
            bool throwOnError = true) =>
            DynCode.CreateInstance(virtualPath, dontRelyOnParameterOrder, name,
                CreateInstancePath, throwOnError);



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

            var appId = DynCode?.Block?.AppId ?? DynCode?.App?.AppId ?? throw new Exception("Error, SaveInAdam needs an App-OqtState to work, but the App is not known.");
            return ServiceProvider.Build<AdamTransUpload<int, int>>(typeof(AdamTransUpload<int, int>))
                .Init(appId, contentType, guid.Value, field, false, Log)
                .UploadOne(stream, fileName, subFolder, true);
        }

        #endregion
    }
}
