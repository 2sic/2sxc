﻿using System;
using ToSic.Eav.Apps;
using ToSic.Eav.Context;
using ToSic.Eav.Data;
using ToSic.Eav.Documentation;
using ToSic.Eav.Logging;
using ToSic.Eav.Run;
using ToSic.Sxc.Context;

namespace ToSic.Sxc.Adam
{
    /// <summary>
    /// The Manager of ADAM
    /// In charge of managing assets inside this app - finding them, creating them etc.
    /// </summary>
    /// <remarks>
    /// It's abstract, because there will be a typed implementation inheriting this
    /// </remarks>
    public abstract class AdamManager: HasLog, ICompatibilityLevel
    {
        #region Constructor for inheritance

        protected AdamManager(Lazy<AppRuntime> appRuntimeLazy, Lazy<AdamMetadataMaker> metadataMakerLazy, string logName) : base(logName ?? "Adm.Managr")
        {
            _appRuntimeLazy = appRuntimeLazy;
            _metadataMakerLazy = metadataMakerLazy;
        }
        
        public AdamMetadataMaker MetadataMaker => _metadataMakerLazy.Value;
        private readonly Lazy<AdamMetadataMaker> _metadataMakerLazy;

        public AppRuntime AppRuntime => _appRuntimeLazy.Value;
        private readonly Lazy<AppRuntime> _appRuntimeLazy;

        #endregion

        #region Init

        public virtual AdamManager Init(IContextOfApp ctx, int compatibility, ILog parentLog)
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
        
        public IContextOfApp AppContext { get; private set; }

        public ISite Site { get; private set; }
        
        #endregion



        /// <summary>
        /// Path to the app assets
        /// </summary>
        public string Path => _path ?? (_path = Configuration.AppReplacementMap(AppContext.AppState)
                                  .ReplaceInsensitive(Configuration.AdamAppRootFolder));
        private string _path;


        [PrivateApi]
        public int CompatibilityLevel { get; set; }


        #region Properties the base class already provides, but must be implemented at inheritance

        public abstract IFolder Folder(Guid entityGuid, string fieldName);

        public abstract IFolder Folder(IEntity entity, string fieldName);
        
        #endregion
    }
}