using System;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Parts;
using ToSic.Eav.Context;
using ToSic.Eav.Data;
using ToSic.Lib.Logging;
using ToSic.Eav.Run;
using ToSic.Lib.DI;
using ToSic.Lib.Documentation;
using ToSic.Lib.Services;
using ToSic.Sxc.Data;
using ToSic.Eav.Plumbing;

// ReSharper disable ConvertToNullCoalescingCompoundAssignment

namespace ToSic.Sxc.Adam
{
    /// <summary>
    /// The Manager of ADAM
    /// In charge of managing assets inside this app - finding them, creating them etc.
    /// </summary>
    /// <remarks>
    /// It's abstract, because there will be a typed implementation inheriting this
    /// </remarks>
    public abstract class AdamManager: ServiceBase, ICompatibilityLevel
    {
        private readonly LazySvc<CodeDataFactory> _cdf;

        #region Constructor for inheritance

        protected AdamManager(
            LazySvc<AppRuntime> appRuntimeLazy,
            LazySvc<CodeDataFactory> cdf,
            AdamConfiguration adamConfiguration,
            string logName) : base(logName ?? "Adm.Managr")
        {
            ConnectServices(
                _appRuntimeLazy = appRuntimeLazy,
                _adamConfiguration = adamConfiguration,
                _cdf = cdf.SetInit(asc => asc.SetFallbacks(AppContext?.Site, CompatibilityLevel, this))
            );
        }

        private readonly AdamConfiguration _adamConfiguration;

        public AppRuntime AppRuntime => _appRuntimeLazy.Value;
        private readonly LazySvc<AppRuntime> _appRuntimeLazy;

        #endregion

        #region Init

        public virtual AdamManager Init(IContextOfApp ctx, CodeDataFactory cdf, int compatibility)
        {
            AppContext = ctx;

            var callLog = Log.Fn<AdamManager>();
            Site = AppContext.Site;
            AppRuntime.InitQ(AppContext.AppState);
            CompatibilityLevel = compatibility;
            _asc = cdf;
            return callLog.Return(this, "ready");
        }
        
        public IContextOfApp AppContext { get; private set; }

        public ISite Site { get; private set; }

        internal CodeDataFactory Cdf => _asc ?? (_asc = _cdf.Value);
        private CodeDataFactory _asc;
        #endregion

        #region Static Helpers

        public static int? CheckIdStringForId(string id)
        {
            if (!id.HasValue()) return null;
            var linkParts = new LinkParts(id);
            if (!linkParts.IsMatch || linkParts.Id == 0) return null;
            return linkParts.Id;
        }


        #endregion

        /// <summary>
        /// Path to the app assets
        /// </summary>
        public string Path => _path ?? (_path = _adamConfiguration.PathForApp(AppContext.AppState));
        private string _path;


        [PrivateApi]
        public int CompatibilityLevel { get; set; }


        #region Properties the base class already provides, but must be implemented at inheritance

        public abstract IFolder Folder(Guid entityGuid, string fieldName, IField field = default);


        public abstract IFile File(int id);

        public abstract IFolder Folder(int id);
        #endregion
    }
}