using System;
using System.Linq;
using ToSic.Eav;
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
    public class AdamAppContext: HasLog, IContextAdamMaybe, ICompatibilityLevel
    {
        /// <summary>
        /// the app is only used to get folder / guid etc.
        /// don't use it to access data! as the data should never have to be initialized for this to work
        /// always use the AppRuntime instead
        /// </summary>
        private readonly IApp _app;
        public readonly AppRuntime AppRuntime;
        public readonly ITenant Tenant;
        public readonly IBlock Block;
        
        protected AdamAppContext(ITenant tenant, IApp app, IBlock block, int compatibility, ILog parentLog) : base("Adm.ApCntx", parentLog, "starting")
        {
            Tenant = tenant;
            _app = app;
            Block = block;
            AppRuntime = new AppRuntime(app, block?.EditAllowed ?? false, null);
            CompatibilityLevel = compatibility;
        }

        /// <summary>
        /// Path to the app assets
        /// </summary>
        public string Path => _path ?? (_path = Configuration.AppReplacementMap(_app)
                                  .ReplaceInsensitive(Configuration.AdamAppRootFolder));
        private string _path;



        #region basic, generic folder commands -- all internal







        #endregion

        // todo: #adamId (int)
        public Export Export => new Export(this as AdamAppContext<int, int>);
        [PrivateApi]
        public int CompatibilityLevel { get; }
    }
}