using System;
using ToSic.Eav.Apps;
using ToSic.Eav.Documentation;
using ToSic.Eav.Logging;
using ToSic.Eav.Run;
using ToSic.Sxc.Blocks;
using IApp = ToSic.Sxc.Apps.IApp;

namespace ToSic.Sxc.Adam
{
    /// <summary>
    /// The app-context of ADAM
    /// In charge of managing assets inside this app
    /// </summary>
    public abstract class AdamAppContext: HasLog, IContextAdamMaybe, ICompatibilityLevel
    {
        /// <summary>
        /// the app is only used to get folder / guid etc.
        /// don't use it to access data! as the data should never have to be initialized for this to work
        /// always use the AppRuntime instead
        /// </summary>
        private IApp _app;
        public AppRuntime AppRuntime { get; private set; }
        public ITenant Tenant { get; private set; }
        public IBlock Block { get; private set;  }
        
        protected AdamAppContext(string logName) : base(logName ?? "Adm.AppCtx") { }

        public virtual AdamAppContext Init(ITenant tenant, IApp app, IBlock block, int compatibility, ILog parentLog)
        {
            Log.LinkTo(parentLog);
            var callLog = Log.Call();
            Tenant = tenant;
            _app = app;
            Block = block;
            AppRuntime = new AppRuntime(app, block?.EditAllowed ?? false, null);
            CompatibilityLevel = compatibility;
            callLog("ready");
            return this;
        }

        /// <summary>
        /// Path to the app assets
        /// </summary>
        public string Path => _path ?? (_path = Configuration.AppReplacementMap(_app)
                                  .ReplaceInsensitive(Configuration.AdamAppRootFolder));
        private string _path;


        [PrivateApi]
        public int CompatibilityLevel { get; set; }


        #region Properties the base class already provides, but must be implemented at inheritance

        internal abstract IFolder FolderOfField(Guid entityGuid, string fieldName);

        #endregion
    }
}