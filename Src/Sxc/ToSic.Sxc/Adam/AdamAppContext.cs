using System;
using ToSic.Eav.Apps;
using ToSic.Eav.Context;
using ToSic.Eav.Documentation;
using ToSic.Eav.Logging;
using ToSic.Eav.Run;
using ToSic.Sxc.Context;

namespace ToSic.Sxc.Adam
{
    /// <summary>
    /// The app-context of ADAM
    /// In charge of managing assets inside this app
    /// </summary>
    public abstract class AdamAppContext: HasLog, IContextAdamMaybe, ICompatibilityLevel
    {
        public AdamMetadataMaker MetadataMaker => _metadataMakerLazy.Value;
        private readonly Lazy<AdamMetadataMaker> _metadataMakerLazy;

        public AppRuntime AppRuntime => _appRuntime.Value;
        private readonly Lazy<AppRuntime> _appRuntime;


        public IContextOfApp AppContext { get; private set; }

        public ISite Site { get; private set; }
        
        protected AdamAppContext(Lazy<AppRuntime> appRuntime, Lazy<AdamMetadataMaker> metadataMakerLazy, string logName) : base(logName ?? "Adm.AppCtx")
        {
            _appRuntime = appRuntime;
            _metadataMakerLazy = metadataMakerLazy;
        }

        public virtual AdamAppContext Init(IContextOfApp ctx, int compatibility, ILog parentLog)
        {
            Log.LinkTo(parentLog);
            AppContext = ctx;

            var callLog = Log.Call();
            Site = AppContext.Site;
            AppRuntime.Init(AppContext.AppState, AppContext.UserMayEdit, null);
            CompatibilityLevel = compatibility;
            callLog("ready");
            return this;
        }

        /// <summary>
        /// Path to the app assets
        /// </summary>
        public string Path => _path ?? (_path = Configuration.AppReplacementMap(AppContext.AppState)
                                  .ReplaceInsensitive(Configuration.AdamAppRootFolder));
        private string _path;


        [PrivateApi]
        public int CompatibilityLevel { get; set; }


        #region Properties the base class already provides, but must be implemented at inheritance

        internal abstract IFolder FolderOfField(Guid entityGuid, string fieldName);

        #endregion
    }
}