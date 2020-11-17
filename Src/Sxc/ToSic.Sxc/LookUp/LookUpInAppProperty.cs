using ToSic.Eav.Documentation;
using ToSic.Eav.LookUp;
using ToSic.Sxc.Apps;

namespace ToSic.Sxc.LookUp
{
    /// <summary>
    /// Look up things in app-settings, app-resources etc.
    /// </summary>
    [InternalApi_DoNotUse_MayChangeWithoutNotice("this is just fyi")]
    internal class LookUpInAppProperty : LookUpBase
    {
        private readonly IApp _app;

        #region Internal stuff to be able to supply sub-properties
        private ILookUp _settings;

        private ILookUp Settings
        {
            get
            {
                if(_settings == null && _app.Settings != null) 
                    _settings = new LookUpInDynamicEntity("appsettings", _app.Settings);
                return _settings;

            }
        }

        private LookUpInDynamicEntity _resources;
        private ILookUp Resources
        {
            get
            {
                if(_resources == null && _app.Resources != null) 
                    _resources = new LookUpInDynamicEntity("appresources", _app.Resources);
                return _resources;

            }
        }


        #endregion

        /// <summary>
		/// Constructor
		/// </summary>
        public LookUpInAppProperty(string name, IApp app)
        {
            Name = name; 
			_app = app;
        }

        /// <inheritdoc/>
        public override string Get(string key, string strFormat)
        {
            key = key.ToLower();
            if (key == "path")
                return _app.Path;
            if (key == "physicalpath")
                return _app.PhysicalPath;

            var subToken = CheckAndGetSubToken(key);

            if (subToken.HasSubtoken)
            {
                var subProvider =
                    (subToken.Source == "settings")
                        ? Settings
                        : (subToken.Source == "resources") ? Resources : null;
                if (subProvider != null)
                    return subProvider.Get(subToken.Rest, strFormat);
            }

            // Maybe someday: also retrieve metadata like Folder, Name, Version
            // var found = base.Get(key, strFormat, ref notFound);

            return string.Empty;
        }

    }
}